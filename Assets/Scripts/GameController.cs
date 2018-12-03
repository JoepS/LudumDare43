using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameController : MonoBehaviour {

    public static GameController instance;

    public bool Paused = false;
    public bool GameOver = false;

    public bool DoNothing { get { return (Paused || GameOver); } }

    [SerializeField] SpriteRenderer _fadeSprite;

    [SerializeField] WallHealth _wallHealth;
    public WallHealth WallHealth { get { return _wallHealth; } }

    PlayerMovement _player;

    float _fadeSpeed = 0.2f;
    float _lerpSpeed = 10;

    [SerializeField]  int _difficulty = 1;
    public int Difficulty { get { return _difficulty; } }

    [SerializeField] StatisticsController _statisticsController;
    public StatisticsController Statistics { get { return _statisticsController; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        string save = SaveState.Load();
        if (!save.Equals(""))
        {
            string[] split = save.Split(';');
            _difficulty = int.Parse(split[2]);
        }
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        if (Paused)
            ResumeGame();
        if (GameOver)
        {
            GameOver = false;
            ResumeGame();
        }
        float deltatime = Time.deltaTime;
        while(_fadeSprite.color.a > 0)
        {
            if (Time.timeScale == 0)
                deltatime = Time.unscaledDeltaTime;
            else
                deltatime = Time.deltaTime;
            Color c = _fadeSprite.color;
            c.a = Mathf.Lerp(c.a, c.a-_fadeSpeed, deltatime * _lerpSpeed);
            _fadeSprite.color = c;
            yield return new WaitForSecondsRealtime(deltatime);
        }
    }

    IEnumerator FadeOut(SceneNames sceneToLoad)
    {
        float deltatime = Time.deltaTime;
        while (_fadeSprite.color.a < 1)
        {
            if (Time.timeScale == 0)
                deltatime = Time.unscaledDeltaTime;
            else
                deltatime = Time.deltaTime;
            Color c = _fadeSprite.color;
            c.a = Mathf.Lerp(c.a, c.a + _fadeSpeed, deltatime * _lerpSpeed);
            _fadeSprite.color = c;
            yield return new WaitForSecondsRealtime(deltatime);
        }
        SceneManager.LoadScene(sceneToLoad.ToString());
        StartCoroutine(FadeIn());
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        _fadeSprite.transform.position = Camera.main.transform.position + new Vector3(0, 0, 10);
        if (Input.GetButtonDown("Cancel") && LoadedScene() != SceneNames.StartMenuScene.ToString())
        {
            if (!Paused)
                PauseGame();
            else
                ResumeGame();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            DroppableFactory.instance.CreateDroppable(10, 10, ItemType.Health, new Vector3(0, 5, 0));
        }
    }
    float timer = 10;
    float timerReset = 10;
    private void FixedUpdate()
    {
        if (timer <= 0)
        {
            Save();
            timer = timerReset;
        }
        timer -= Time.fixedDeltaTime;
    }

    public void PauseGame()
    {
        Save();
        Paused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Paused = false;
        Time.timeScale = 1;
    }

    public void GameIsOver()
    {
        GameOver = true;
        Time.timeScale = 0;
    }

    public void ShakeCamera(float duration, float magnitude, Vector2 range)
    {
        Camera.main.GetComponent<CameraShaker>().ShakeCamera(duration, magnitude, range);
    }

    public void LoadScene(SceneNames name, bool clearSave)
    {
        if (clearSave)
        {
            SaveState.ClearSave();
            _difficulty = 1;
        }
        else
            Save();

        StopAllCoroutines();
        StartCoroutine(FadeOut(name));
    }

    public void LoadScene(SceneNames name)
    {
        Save();
        StopAllCoroutines();
        StartCoroutine(FadeOut(name));
    }

    public string LoadedScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void Save()
    {
        _player = GameObject.FindObjectOfType<PlayerMovement>();
        if (_player != null)
        {
            SaveState.Save(_player, VillagerGenerator.Villagers);
        }
    }

    public void GivePlayerExperience(float amount)
    {
        _player = GameObject.FindObjectOfType<PlayerMovement>();
        _player.GetStats().AddExperience(amount);
    }
    
    public void CheckDifficultyIncrease()
    {
        float maxRange = 10;
        if (_player.GetStats().Level - _difficulty > 0)
            maxRange -= 4;
        if (_statisticsController.GetValue(StatisticsFollowed.Sacrifices) % 10 == 0)
            maxRange -= 4;
        if (VillagerGenerator.VillagerCount() % 5 == 0)
            maxRange -= 1;
        if (_statisticsController.GetValue(StatisticsFollowed.EnemiesKilled) % 5 == 0)
            maxRange -= 1;

        int difficultyIncreaseChance = Random.Range(0, 10);
        if (difficultyIncreaseChance >= maxRange)
        {
            _difficulty++;
        }
    }
}

public enum SceneNames
{
    StartMenuScene,
    VillageScene,
    BattleScene
}

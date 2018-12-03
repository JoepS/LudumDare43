using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillageSceneUiController : MonoBehaviour {

    [SerializeField] Image _playerHealthBar;
    [SerializeField] Image _playerMagicBar;
    [SerializeField] Image _playerExperienceBar;
    [SerializeField] Text _playerExperienceBarText;
    [SerializeField] Image _wallHealthBar;
    [SerializeField] Text _wallHealthText;
    [SerializeField] Text _villagerAmountText;

    [SerializeField] Text _levelText;

    [SerializeField] PlayerMovement _player;

    [SerializeField] List<StatPanel> _statPanels;

    [SerializeField] CanvasGroup _pausePanel;
    [SerializeField] CanvasGroup _uiPanel;
    [SerializeField] CanvasGroup _shopPanel;
    [SerializeField] CanvasGroup _gameOverPanel;
    [SerializeField] CanvasGroup _tutorialPanel;

    bool _overrideUi = false;
	// Use this for initialization
	void Start () {
		if(!PlayerPrefs.HasKey("TutorialSeen"))
        {
            _tutorialPanel.alpha = 1;
            _tutorialPanel.blocksRaycasts = true;
            GameController.instance.PauseGame();
        }
        else
        {
            _tutorialPanel.alpha = 0;
            _tutorialPanel.blocksRaycasts = false;

        }
	}
	
	// Update is called once per frame
	void Update () {
        UpdateUI();
        UpdatePaused();
	}

    void UpdatePaused()
    {
        _pausePanel.alpha = GameController.instance.Paused && !_overrideUi ? 1 : 0;
        _pausePanel.blocksRaycasts = GameController.instance.Paused && !_overrideUi;
        _gameOverPanel.alpha = GameController.instance.GameOver && !_overrideUi ? 1 : 0;
        _gameOverPanel.blocksRaycasts = GameController.instance.GameOver && !_overrideUi;
    }

    void UpdateUI()
    {
        float healthPercentage = _player.GetStats().HitPoints / _player.GetStats().MaxHitPoints;
        _playerHealthBar.fillAmount = healthPercentage;
        float magicPercentage = _player.GetStats().MagicPoints / _player.GetStats().MaxMagicPoints;
        _playerMagicBar.fillAmount = magicPercentage;
        float maxExperience = (_player.GetStats().Level + 1) * 10;
        float experiencePercentage = _player.GetStats().Experience / maxExperience;
        _playerExperienceBar.fillAmount = experiencePercentage;
        _playerExperienceBarText.text = Mathf.RoundToInt(_player.GetStats().Experience) + " / " + maxExperience;

        float wallHealthPercentage = GameController.instance.WallHealth.HitPoints / GameController.instance.WallHealth.MaxHitPoints;
        _wallHealthBar.fillAmount = wallHealthPercentage;
        _wallHealthText.text = "Wall Health: " + GameController.instance.WallHealth.HitPoints + " / " + GameController.instance.WallHealth.MaxHitPoints;

        _levelText.text = "" +_player.GetStats().Level;

        _villagerAmountText.text = "" + VillagerGenerator.VillagerCount();

        foreach(StatPanel sp in _statPanels)
        {
            float statValue = _player.GetStats().GetStat(sp.StatName);
            sp.SetStats(statValue);
        }
    }

    public void OnResumeButtonClick()
    {
        GameController.instance.ResumeGame();
    }

    public void OnRestartButtonClick()
    {
        _uiPanel.alpha = 0;
        _gameOverPanel.alpha = 0;
        _overrideUi = true;
        GameController.instance.LoadScene(SceneNames.VillageScene, true);
    }

    public void OnMenuButtonClick()
    {
        _uiPanel.alpha = 0;
        _pausePanel.alpha = 0;
        _overrideUi = true;
        GameController.instance.LoadScene(SceneNames.StartMenuScene);
    }

    public void OnCloseTutorialClick()
    {
        _tutorialPanel.alpha = 0;
        _tutorialPanel.blocksRaycasts = false;
        PlayerPrefs.SetInt("TutorialSeen", 1);
        GameController.instance.ResumeGame();
    }
}

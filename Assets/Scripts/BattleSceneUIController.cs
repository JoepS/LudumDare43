using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BattleSceneUIController : MonoBehaviour {

    [SerializeField] Image _healthBar;
    [SerializeField] Image _magicBar;
    [SerializeField] Image _experienceBar;
    [SerializeField] Text _experienceText;
    [SerializeField] Text _villagerAmountText;

    [SerializeField] Image _wallHealth;
    [SerializeField] Text _wallHealthText;

    [SerializeField] PlayerMovement _player;

    [SerializeField] CanvasGroup _pausePanel;
    [SerializeField] CanvasGroup _uiPanel;
    [SerializeField] CanvasGroup _gameOverPanel;

    bool _overrideUi = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        UpdateUi();
        UpdatePaused();
    }

    void UpdatePaused()
    {
        _pausePanel.alpha = GameController.instance.Paused && !_overrideUi ? 1 : 0;
        _pausePanel.blocksRaycasts = GameController.instance.Paused && !_overrideUi;
        _gameOverPanel.alpha = GameController.instance.GameOver && !_overrideUi ? 1 : 0;
        _gameOverPanel.blocksRaycasts = GameController.instance.GameOver && !_overrideUi;
    }

    void UpdateUi()
    {
        float healthPercentage = _player.GetStats().HitPoints / _player.GetStats().MaxHitPoints;
        _healthBar.fillAmount = healthPercentage;
        float magicPercentage = _player.GetStats().MagicPoints / _player.GetStats().MaxMagicPoints;
        _magicBar.fillAmount = magicPercentage;
        float maxExperience = (_player.GetStats().Level + 1) * 10;
        float experiencePercentage = _player.GetStats().Experience / maxExperience;
        _experienceBar.fillAmount = experiencePercentage;
        _experienceText.text = Mathf.RoundToInt(_player.GetStats().Experience) + " / " + maxExperience;
        _villagerAmountText.text = "" + VillagerGenerator.VillagerCount();

        float wallHealthPercentage = GameController.instance.WallHealth.HitPoints / GameController.instance.WallHealth.MaxHitPoints;
        _wallHealth.fillAmount = wallHealthPercentage;
        _wallHealthText.text = "Wall Health: " + GameController.instance.WallHealth.HitPoints + " / " + GameController.instance.WallHealth.MaxHitPoints;
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
}

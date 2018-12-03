using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour {

    [SerializeField] CanvasGroup _startMenuCanvasGroup;
    [SerializeField] CanvasGroup _controllsCanvasGroup;
    [SerializeField] CanvasGroup _creditsCanvasGroup;
    [SerializeField] Button _clearSaveButton;

    float _fadeSpeed = 0.15f;
    
	// Use this for initialization
	void Start () {
        if (SaveState.Load().Equals(""))
            _clearSaveButton.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnStartGameButtonClick()
    {
        GameController.instance.LoadScene(SceneNames.VillageScene);
    }

    public void OnCreditsButtonClick()
    {
        StartCoroutine(Fade(_startMenuCanvasGroup, _creditsCanvasGroup, _fadeSpeed));
    }

    public void OnBackButtonClick()
    {
        StartCoroutine(Fade(_creditsCanvasGroup, _startMenuCanvasGroup, _fadeSpeed));
    }

    public void OnControllsButtonClick()
    {
        StartCoroutine(Fade(_startMenuCanvasGroup, _controllsCanvasGroup, _fadeSpeed));
    }

    public void OnBackButtonClickControlls()
    {
        StartCoroutine(Fade(_controllsCanvasGroup, _startMenuCanvasGroup, _fadeSpeed));
    }

    public void OnClearSaveButtonClick()
    {
        _clearSaveButton.gameObject.SetActive(false);
        SaveState.ClearSave();
    }

    IEnumerator Fade(CanvasGroup a, CanvasGroup b, float speed)
    {
        a.blocksRaycasts = false;
        while(b.alpha < 1)
        {
            b.alpha += speed;
            a.alpha -= speed;
            yield return new WaitForSeconds(speed);
        }
        b.alpha = 1;
        a.alpha = 0;
        b.blocksRaycasts = true;
    }
}

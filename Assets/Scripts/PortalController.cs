using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour {
    [SerializeField] SceneNames _sceneToLoad;
    [SerializeField] Vector3 _locationToGoTo;

    [SerializeField] AudioSource _portalSource;
	
    public void ActivatePortal(GameObject hit)
    {
        _portalSource.Play();
        if(_sceneToLoad.ToString() != GameController.instance.LoadedScene())
        {
            GameController.instance.LoadScene(_sceneToLoad);
        }
        else
        {
            hit.transform.localPosition = _locationToGoTo;
        }
    }
}

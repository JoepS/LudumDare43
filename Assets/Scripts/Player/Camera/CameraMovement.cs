using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    [SerializeField] GameObject _target;
    [SerializeField] int _cameraSpeed;
    [SerializeField] Vector2 _offset;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Movement();
	}

    void Movement()
    {
        Vector3 pos = this.transform.localPosition;
        Vector3 target = _target.transform.localPosition;
        target.z = pos.z;
        target.x -= _offset.x;
        target.y -= _offset.y;

        pos = Vector3.Lerp(pos, target, Time.deltaTime * _cameraSpeed);
        this.transform.localPosition = pos;
    }
}

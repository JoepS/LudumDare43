using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAdio : MonoBehaviour {

    AudioSource source;

    public void OnEnable()
    {
        source = this.GetComponent<AudioSource>();
        Play();
    }

    public void Play()
    {
        source.Play();
    }
}

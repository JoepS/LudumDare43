using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

    Animator _animator;

    private void Awake()
    {
        _animator = this.GetComponent<Animator>();
    }

    // Use this for initialization
    void Start () {
		
	}

    public void SetApplyRootMotion(bool value)
    {
        _animator.applyRootMotion = value;
    }

    public void ActivateTrigger(string triggerName)
    {
        _animator.SetTrigger(triggerName);
    }

    public bool DoneAnimating(string animationName)
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }
	
}

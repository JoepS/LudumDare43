using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttackOrb : MonoBehaviour {

    [SerializeField] AudioSource _magicAttackSource;

    bool _moving = false;
    public bool Moving { get { return _moving; } }

    GameObject _originalParent;

    Vector3 _originalPos;

    float timer = 0;

	void Update()
    {
        if (_moving || !GameController.instance.DoNothing)
        {
            this.transform.position = this.transform.position + ((-this.transform.right) * 5 * Time.deltaTime);
            if (timer < 0)
                this.Reset();
            timer -= Time.deltaTime;
        }
    }

    public void Activate(float angle)
    {
        _originalParent = this.transform.parent.gameObject;
        _originalPos = this.transform.localPosition;
        this.transform.SetParent(null);
        this.transform.eulerAngles = new Vector3(0, angle, 0);
        this.gameObject.SetActive(true);
        timer = 5;
        _moving = true;
        _magicAttackSource.Play();
    }

    public void Reset()
    {
        this.transform.SetParent(_originalParent.transform);
        this.transform.localPosition = _originalPos;
        this._moving = false;
        this.gameObject.SetActive(false);
    }

    public PlayerMovement GetPlayer()
    {
        return this._originalParent.transform.root.GetComponent<PlayerMovement>();
    }
}

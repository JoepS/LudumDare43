using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : IEntity {

    float _movementTimer = 0;
    float _rotationTimer = 0;

    bool _isBeingCarried = false;
    bool _isBeingSacrificed = false;

    public bool IsBeingSacrificed { get { return _isBeingSacrificed; } }

    SpriteRenderer _sprite;

    AnimationController _animator;

    protected Reproducable _reproducable;
    public Reproducable reproducable { get { return _reproducable; } }

    [SerializeField] AudioSource _sacrificeSource;
    
    void Awake()
    {
        _reproducable = this.GetComponent<Reproducable>();
        movementState = MovementState.Moving;
        _animator = this.GetComponent<AnimationController>();
        _sprite = this.GetComponentInChildren<SpriteRenderer>();
        _stats.Start();
        _rigidbody = this.GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        //Movement();
	}

    public override void Kill()
    {
        _reproducable.SetActive(false);
        StartCoroutine(Sacrifice());
    }

    private void OnAnimatorMove()
    {
       // if(!_isBeingSacrificed)
            Movement();
    }

    IEnumerator Sacrifice()
    {
        _sacrificeSource.Play();
        this.transform.rotation = Quaternion.identity;
        _animator.SetApplyRootMotion(false);
        _animator.ActivateTrigger("Sacrifice");
        
        _isBeingCarried = false;
        _isBeingSacrificed = true;

        while (this.transform.localScale.x > 0)
        {
            yield return new WaitForSeconds(0.1f);
        }
        VillagerGenerator.RemoveVillager(this);
        GameController.instance.Statistics.AddValue(StatisticsFollowed.Sacrifices, 1);
        GameController.instance.CheckDifficultyIncrease();
        Destroy(this.gameObject);
    }

    public override void MagicAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void MeleAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void Movement()
    {
        if (!_isBeingCarried && !_isBeingSacrificed)
        {
            _reproducable.SetActive(true);
            if (movementState == MovementState.Moving)
            {
                Vector3 force = this.transform.right;
                force *= _movementSpeed;
                _rigidbody.AddForce(force);
            }
            else if (movementState == MovementState.Waiting)
            {
                _rotationTimer -= Time.deltaTime;
                if (_rotationTimer <= 0)
                {
                    Vector3 rot = this.transform.localEulerAngles;
                    if (Mathf.RoundToInt(rot.y) == 0)
                    {
                        this.transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else if (Mathf.RoundToInt(rot.y) == 180)
                    {
                        this.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    _rotationTimer = Random.Range(0.5f, _movementTimer + 0.5f);
                }

            }

            _movementTimer -= Time.deltaTime;

            if (_movementTimer <= 0)
            {
                if (movementState == MovementState.Waiting)
                    movementState = MovementState.Moving;
                else if (movementState == MovementState.Moving)
                    movementState = MovementState.Waiting;

                _movementTimer = Random.Range(2f, 10f);
            }
        }
        else if (_isBeingCarried)
        {
            _reproducable.SetActive(false);
        }
        else if (_isBeingSacrificed)
        {
            _reproducable.SetActive(false);
            _rigidbody.constraints = RigidbodyConstraints2D.None;
            _rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
        }
    }

    public void SetBeingCarried(bool value)
    {
        _isBeingCarried = value;
        if (value)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 90);
            _sprite.sortingOrder = 1;
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            _sprite.sortingOrder = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_isBeingCarried && collision.gameObject.tag.Equals("SacrificialAltair")){
            this.transform.localPosition = collision.transform.position;
            Kill();
        }
    }

    public string ToJson()
    {
        string json = "";

        string repStats = _reproducable.ToJson();
        json += JsonUtility.ToJson(this.GetStats()) + " / " + repStats + ";";

        return json;
    }

    public void SetParent(Villager v)
    {
        _reproducable.SetParentGameObject(v.gameObject);
        _reproducable.AddToKids(this.gameObject);
    }
}

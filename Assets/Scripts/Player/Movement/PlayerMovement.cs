using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : IEntity {

    [Header("Pickup")]
    [SerializeField] int _pickupRange;

    ContactFilter2D _jumpContactFilter;

    const float _meleAttackWaitTime = 1;
    float _meleAttackTimer = 0;

    const float _magicAttackWaitTime = 2;
    float _magicAttackTimer = 0;

    AnimationController _animationController;

    bool _carryingVillager = false;
    Villager _currentCarryingVillager;

    [SerializeField] Vector3 _carryPosition;

    [SerializeField] GameObject _sword;

    SpriteRenderer[] _spriteRenderers;

    BoxCollider2D _collider;
    [Header("Invincibility")]
    [SerializeField] float _invincibilityTime;

    [SerializeField] MagicAttackOrb _magicAttackOrb;

    private void Awake()
    {
        string save = SaveState.Load();
        if (!save.Equals(""))
            _stats = JsonUtility.FromJson<EntityStats>(save.Split(';')[0]);
        else
            _stats.Start();
        _collider = this.GetComponent<BoxCollider2D>();
        _jumpContactFilter = new ContactFilter2D();
        _jumpContactFilter.useLayerMask = true;
        _jumpContactFilter.SetLayerMask(LayerMask.GetMask("Floor", "Enemy"));
        _jumpContactFilter.maxNormalAngle = 135;
        _jumpContactFilter.minNormalAngle = 45;
        _jumpContactFilter.useNormalAngle = true;
        _rigidbody = this.GetComponent<Rigidbody2D>();
        _animationController = this.GetComponent<AnimationController>();
        _spriteRenderers = this.GetComponentsInChildren<SpriteRenderer>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!GameController.instance.DoNothing)
        {
            Movement();
            Fighting();
            CarryVillager();
        }
	}

    public void CarryVillager()
    {
        if(!_carryingVillager && Input.GetButtonDown("Interact"))
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

            Collider2D hitCollider = Physics2D.OverlapCircle(worldMousePos, 1, 1 << LayerMask.NameToLayer("Villager"));
            if (hitCollider != null)
            {
                if (Vector3.Distance(hitCollider.transform.localPosition, this.transform.localPosition) < _pickupRange)
                {
                    _currentCarryingVillager = hitCollider.gameObject.GetComponent<Villager>();
                    _currentCarryingVillager.SetBeingCarried(true);
                    _currentCarryingVillager.transform.localPosition = this.transform.localPosition + _carryPosition;
                    _carryingVillager = true;
                    _sword.SetActive(false);

                    Vector2 colliderSize = _collider.size;
                    colliderSize.y += 0.64f;
                    _collider.size = colliderSize;
                    Vector2 colliderOffset = _collider.offset;
                    colliderOffset.y = 0.32f;
                    _collider.offset = colliderOffset;
                }
            }
        }
        else if (_carryingVillager && _currentCarryingVillager != null)
        {
            if (!_currentCarryingVillager.IsBeingSacrificed)
            {
                _currentCarryingVillager.transform.localPosition = this.transform.localPosition + _carryPosition;
                if (Input.GetButtonDown("Interact"))
                {
                    _carryingVillager = false;
                    _currentCarryingVillager.SetBeingCarried(false);
                    _sword.SetActive(true);

                    Vector2 colliderSize = _collider.size;
                    colliderSize.y -= 0.64f;
                    _collider.size = colliderSize;
                    Vector2 colliderOffset = _collider.offset;
                    colliderOffset.y = 0;
                    _collider.offset = colliderOffset;
                }
            }
            else
            {
                _stats.AddStats(_currentCarryingVillager.GetStats());
                _currentCarryingVillager = null;
                _carryingVillager = false;
                Vector2 colliderSize = _collider.size;
                colliderSize.y -= 0.64f;
                _collider.size = colliderSize;
                Vector2 colliderOffset = _collider.offset;
                colliderOffset.y = 0;
                _collider.offset = colliderOffset;
            }

        }
    }

    protected override void Movement()
    { 
        float horizontal = Input.GetAxis("Horizontal");
        float rotation = 1;
        if (this.transform.localEulerAngles.y != 0)
            rotation = -1;
        Vector2 movementForce = this.transform.right * rotation;
        movementForce *= horizontal;
        movementForce *= _movementSpeed;
        if (movementForce.x != 0 && !_walkingSource.isPlaying && _rigidbody.IsTouching(_jumpContactFilter))
            _walkingSource.Play();
        _rigidbody.AddForce(movementForce);
        
        if (_rigidbody.IsTouching(_jumpContactFilter)){
            float jump = Input.GetAxis("Jump");
            Vector2 jumpForce = this.transform.up;
            jumpForce *= jump;
            jumpForce *= _jumpSpeed;
            if (jumpForce.y > 0)
                _jumpSource.Play();
            _rigidbody.AddForce(jumpForce, ForceMode2D.Impulse);
        }


        Vector2 mousePos = Input.mousePosition;
        Vector3 rot = this.transform.localEulerAngles;
        if(mousePos.x > Screen.width / 2)
        {
            rot.y = 180;
        }
        else if(mousePos.x < Screen.width / 2)
        {
            rot.y = 0;
        }
        this.transform.localEulerAngles = rot;
    }

    void Fighting()
    {
        if (Input.GetButtonDown("MeleAttack") && !_animationController.DoneAnimating("SwordAttack"))
        {
            MeleAttack();
        }
        else if(Input.GetButtonDown("MagicAttack") && _magicAttackTimer <= 0 && _stats.RemoveMagicPoints(1))
        {
            MagicAttack();
        }

        if (_meleAttackTimer > 0)
        {
            _meleAttackTimer -= Time.deltaTime;
        }
        if (_magicAttackTimer > 0)
        {
            _magicAttackTimer -= Time.deltaTime;
        }
    }

    public override void MeleAttack()
    {
        if (_sword.activeSelf)
        {
            _meleAttackTimer = _meleAttackWaitTime;
            _animationController.ActivateTrigger("MeleAttack");
            _sword.GetComponent<AudioSource>().Play();
        }
    }

    public override void MagicAttack()
    {
        if (_sword.activeSelf)
        {
            if (_magicAttackOrb.Moving)
                _magicAttackOrb.Reset();
            _animationController.ActivateTrigger("MagicAttack");
            _magicAttackTimer = _magicAttackWaitTime;
            _magicAttackOrb.Activate(this.transform.localEulerAngles.y);
        }
    }

    public override void Kill()
    {
        this.gameObject.SetActive(false);
        GameController.instance.GameIsOver();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.gameObject.tag.Equals("Portal"))
        {
            collision.gameObject.GetComponent<PortalController>().ActivatePortal(this.gameObject);
            this.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool dead = false;
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            float hitPoints = collision.gameObject.transform.root.GetComponent<IEntity>().GetMagicHitPoints();
            dead = _stats.RemoveHitPoints(hitPoints);
            ContactPoint2D[] cp = new ContactPoint2D[1];
            collision.GetContacts(cp);
            GetHit(cp[0].normal, hitPoints);
            StartCoroutine(InvincibilityFrames());
        }

        if (dead)
            Kill();

    }


    IEnumerator InvincibilityFrames()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        float timer = _invincibilityTime;
        while (timer > 0)
        {
            foreach (SpriteRenderer sp in _spriteRenderers)
            {
                Color c = sp.color;
                if (c.a == 0.5f)
                    c.a = 1;
                else
                    c.a = 0.5f;
                sp.color = c;
            }
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
        }
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        foreach (SpriteRenderer sp in _spriteRenderers)
        {
            Color c = sp.color;
            c.a = 1;
            sp.color = c;
        }
    }
}

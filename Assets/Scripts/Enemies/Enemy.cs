using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : IEntity {
    [SerializeField] int _targetFollowDistance;

    [SerializeField] GameObject _playerTarget;
    [SerializeField] GameObject _wallTarget;

    ContactFilter2D _jumpContactFilter;

    [SerializeField] GameObject _healthBar;
    float _startHealthBarWidth;

    Vector3 _dir;

    EnemySpawner _enemySpawner;

    private void Awake()
    {
        if (_healthBar != null)
            _startHealthBarWidth = _healthBar.GetComponent<SpriteRenderer>().size.x;
        _jumpContactFilter = new ContactFilter2D();
        _jumpContactFilter.useLayerMask = true;
        _jumpContactFilter.SetLayerMask(LayerMask.GetMask("Floor", "Enemy"));
        _jumpContactFilter.maxNormalAngle = 135;
        _jumpContactFilter.minNormalAngle = 45;
        _jumpContactFilter.useNormalAngle = true;
        _rigidbody = this.GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start () {
        _stats.Start();
	}
	
	// Update is called once per frame
	void Update () {
        if(_rigidbody != null)
            Movement();
	}

    void UpdateUi()
    {
        float percentage = _stats.HitPoints / _stats.MaxHitPoints;
        Vector2 size = _healthBar.GetComponent<SpriteRenderer>().size;
        size.x = _startHealthBarWidth * percentage;
        _healthBar.GetComponent<SpriteRenderer>().size = size;
    }

    protected override void Movement()
    {
        Vector3 pos = this.transform.localPosition;
        GameObject target;
        if (Vector3.Distance(pos, _playerTarget.transform.localPosition) < _targetFollowDistance)
        {
            target = _playerTarget;
            if (_playerTarget.transform.localPosition.y - pos.y > 0.5f && _rigidbody.IsTouching(_jumpContactFilter))
            {
                Vector3 jumpForce = this.transform.up;
                _jumpSource.Play();
                jumpForce *= _jumpSpeed;
                _rigidbody.AddForce(jumpForce, ForceMode2D.Impulse);
            }
        }
        else
        {
            target = _wallTarget;
        }

        Vector3 targetPos = target.transform.localPosition;
        

        targetPos.y = pos.y;

        _dir = (targetPos - pos).normalized;
        _rigidbody.AddForce(_dir * _movementSpeed);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Wall"))
        {
            float hitPoints = this.GetAttackHitPoints();
            GameController.instance.WallHealth.RemoveHitPoints(hitPoints);
            ContactPoint2D[] cp = new ContactPoint2D[1];
            collision.GetContacts(cp);
            GetHit(cp[0].normal, hitPoints);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool dead = false;
        if (collision.tag.Equals("PlayerMeleAttack"))
        {
            float hitPoints = collision.gameObject.transform.root.GetComponent<PlayerMovement>().GetAttackHitPoints();
            dead = _stats.RemoveHitPoints(hitPoints);
            GetHit(-_dir.normalized, hitPoints);
            UpdateUi();
        }
        else if (collision.tag.Equals("PlayerMagicAttack"))
        {
            float hitPoints = collision.gameObject.GetComponent<MagicAttackOrb>().GetPlayer().GetMagicHitPoints();
            collision.gameObject.GetComponent<MagicAttackOrb>().Reset();
            dead = _stats.RemoveHitPoints(hitPoints);
            GetHit(-_dir.normalized, hitPoints);
            UpdateUi();
        }

        if (dead)
            Kill();
    }

    public override void Kill()
    {
        _enemySpawner.RemoveFromEnemiesList(this.gameObject);
        int totalOrbs = 5;
        int amountOfHealthOrbs = Random.Range(0, 5);
        int amountOfMagicOrb = Random.Range(0, totalOrbs - amountOfHealthOrbs);
        for (int i = 0; i < amountOfHealthOrbs; i++)
            DroppableFactory.instance.CreateDroppable(Random.Range(1, 5), 10, ItemType.Health, this.transform.localPosition).Drop();
        for (int i = 0; i < amountOfMagicOrb; i++)
            DroppableFactory.instance.CreateDroppable(Random.Range(1, 5), 10, ItemType.Magic, this.transform.localPosition).Drop();
        float experience = GetStats().Experience;
        int amountOfExperienceOrbs = Mathf.RoundToInt(experience / 5);
        if (amountOfExperienceOrbs == 0)
            amountOfExperienceOrbs = 1;

        for (int i = 0; i < amountOfExperienceOrbs; i++) {
            float droppingExperience = 5;
            if (droppingExperience > experience)
                droppingExperience = experience;
            DroppableFactory.instance.CreateDroppable(droppingExperience, 10, ItemType.Experience, this.transform.localPosition).Drop();
            experience -= droppingExperience;
        }

        float goldDropped = Random.Range(5f * GetStats().Level, 10f * GetStats().Level);
        int amountOfGoldOrbs = Mathf.RoundToInt(goldDropped / 10);
        if (amountOfGoldOrbs == 0)
            amountOfGoldOrbs = 1;
        for (int i = 0; i < amountOfGoldOrbs; i++)
        {
            float droppingGold = 10;
            if (droppingGold > goldDropped)
                droppingGold = goldDropped;
            DroppableFactory.instance.CreateDroppable(droppingGold, 10, ItemType.Gold, this.transform.localPosition).Drop();
            goldDropped -= droppingGold;
        }

        GameController.instance.Statistics.AddValue(StatisticsFollowed.EnemiesKilled, 1);

        Destroy(this.gameObject);
    }

    public void SetEnemySpawner(EnemySpawner value)
    {
        _enemySpawner = value;
    }

    public override void MeleAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void MagicAttack()
    {
        throw new System.NotImplementedException();
    }

    public void SetPlayerAndWallValues(GameObject player, GameObject wall)
    {
        _playerTarget = player;
        _wallTarget = wall;
    }
}

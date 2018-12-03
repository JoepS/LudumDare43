using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IEntity : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] protected float _movementSpeed;
    [SerializeField] protected float _jumpSpeed;

    [SerializeField] float _knockBackStrength = 1;
    [Header("Stats")]
    [SerializeField] protected EntityStats _stats;
    
    
    protected MovementState movementState { get; set; }

    float _defaultMeleAttackStrenght = 1;
    float _defaultMagicAttackStrength = 1;

    protected Rigidbody2D _rigidbody;

    abstract protected void Movement();
    abstract public void MeleAttack();
    abstract public void MagicAttack();

    [SerializeField] protected AudioSource _gettingHitSource;
    [SerializeField] protected AudioSource _jumpSource;
    [SerializeField] protected AudioSource _walkingSource;

    public float GetAttackHitPoints()
    {
        float strength = _stats.Strength;
        float luck = _stats.Luck;

        float minHitPoints = (_defaultMeleAttackStrenght * strength) / 2;
        float maxHitPoints = (_defaultMeleAttackStrenght * strength) * 2;

        float hitPoints = Mathf.Lerp(minHitPoints, maxHitPoints, Random.Range(0, Time.deltaTime * luck));

        return hitPoints;
    }
    public float GetMagicHitPoints()
    {
        float intelligence = _stats.Intelligence;
        float luck = _stats.Luck;

        float minHitPoints = (_defaultMagicAttackStrength * intelligence) / 2f;
        float maxHitPoints = (_defaultMagicAttackStrength * intelligence) * 2f;

        float hitPoints = Mathf.Lerp(minHitPoints, maxHitPoints, Random.Range(0, Time.deltaTime * luck));

        return hitPoints;
    }

    public EntityStats GetStats()
    {
        return _stats;
    }

    protected void GetHit(Vector3 hitDirection, float hitIntensity)
    {
        float magnitude = hitIntensity / GetStats().HitPoints;
        if (GetStats().HitPoints - hitIntensity < 0)
            magnitude = 0.25f;
        GameController.instance.ShakeCamera(0.25f, magnitude, new Vector2(0.15f, 0.15f));
        Vector3 force = hitDirection;
        force *= _knockBackStrength;
        _gettingHitSource.Play();
        _rigidbody.AddForce(force, ForceMode2D.Impulse);
    }

    abstract public void Kill();

    public void SetStats(EntityStats stats)
    {
        _stats = stats;
    }
}

public enum MovementState
{
    Waiting,
    Moving,
    Attacking
}

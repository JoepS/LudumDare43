using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EntityStats {
    [SerializeField] int _id;
    public int Id { get { return _id; } }


    [Header("Level")]
    [SerializeField] int _level;
    public float Level { get { return _level; } }
    [SerializeField] float _experience;
    public float Experience { get { return _experience; } }

    [Header("Hitpoints")]
    [SerializeField] float _maxHitPoints;
    public float MaxHitPoints { get { return _maxHitPoints; } set { _maxHitPoints = value; } }
    [SerializeField] float _hitPoints;
    public float HitPoints { get { return _hitPoints; } }
    [Header("Magicpoints")]
    [SerializeField] float _maxMagicPoints;
    public float MaxMagicPoints { get { return _maxMagicPoints; } set { _maxMagicPoints = value; } }
    [SerializeField] float _magicPoints;
    public float MagicPoints { get { return _magicPoints; } }

    [Header("Gold")]
    [SerializeField] float _gold;
    public float Gold { get { return _gold; } }

    [Header("Stats")]
    [SerializeField]
    float _strength;
    public float Strength { get { return _strength; } }
    [SerializeField]
    float _dexterity;
    public float Dexterity { get { return _dexterity; } }
    [SerializeField]
    float _intelligence;
    public float Intelligence { get { return _intelligence; } }
    [SerializeField]
    float _luck;
    public float Luck { get { return _luck; } }

    public EntityStats(int id)
    {
        _id = id;
    }

    public void Start()
    {
        _hitPoints = _maxHitPoints;
        _magicPoints = _maxMagicPoints;
    }
    
    public void UpdateStats(float strength, float dexterity, float intelligence, float luck)
    {
        _strength = strength;
        _dexterity = dexterity;
        _intelligence = intelligence;
        _luck = luck;
    }

    public void AddHitPoints(float amount)
    {
        _hitPoints += amount;
        if(_hitPoints > _maxHitPoints)
        {
            _hitPoints = _maxHitPoints;
        }
    }

    /// <summary>
    /// Returns true if no hit points are left;
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool RemoveHitPoints(float amount)
    {
        _hitPoints -= amount;
        if(_hitPoints <= 0)
        {
            return true;
        }
        return false;
    }

    public void AddMagicPoints(float amount)
    {
        _magicPoints += amount;
        if(_magicPoints > _maxMagicPoints)
        {
            _magicPoints = _maxMagicPoints;
        }
    }
    /// <summary>
    /// Returns false when the amount cant be removed from magic points
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool RemoveMagicPoints(float amount)
    {
        if (_magicPoints >= amount)
        {
            _magicPoints -= amount;
            return true;
        }
        return false;
    }

    public void AddStats(EntityStats statsToAdd)
    {
        _strength += statsToAdd.Strength;
        _intelligence += statsToAdd.Intelligence;
        _luck += statsToAdd.Luck;
    }


    public override string ToString()
    {
        return _maxHitPoints + " / " + _maxMagicPoints + " / " + _strength + " / " + _dexterity + " / " + _intelligence + " / " + _luck;
    }

    public float GetStat(string name)
    {
        switch (name)
        {
            case "strength":
                return _strength;
            case "dexterity":
                return _dexterity;
            case "intelligence":
                return _intelligence;
            case "luck":
                return _luck;
            default:
                return -1;
        }
    }

    public void SetLevel(int value)
    {
        _level = value;
    }

    public void SetExperience(float value)
    {
        _experience = value;
    }

    public void AddExperience(float value)
    {
        if(_experience + value >= (_level + 1) * 10)
        {
            float remainingExperience = (_experience + value) - ((_level+1) * 10);
            if (remainingExperience < 0)
                remainingExperience = 0;
            LevelUp();
            value = remainingExperience;
        }
        _experience += value;
    }

    public void LevelUp()
    {
        _experience = 0;
        _level++;
        _maxHitPoints = 10 + (_level * 10);
        _hitPoints = _maxHitPoints;
        _maxMagicPoints = 1 + (_level * 2);
        _magicPoints = _maxMagicPoints;

        GameController.instance.CheckDifficultyIncrease();
    }

    public void AddGold(float amount)
    {
        _gold += amount;
    }

    public bool RemoveGold(float amount)
    {
        if (_gold - amount <= 0)
            return false;
        _gold -= amount;
        return true;
    }

    public void AddDroppable(float amount, ItemType type)
    {
        switch (type)
        {
            case ItemType.Health:
                AddHitPoints(amount);
                break;
            case ItemType.Magic:
                AddMagicPoints(amount);
                break;
            case ItemType.Experience:
                AddExperience(amount);
                break;
            case ItemType.Gold:
                AddGold(amount);
                break;
        }
    }


}

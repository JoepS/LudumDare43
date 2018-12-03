using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHealth : MonoBehaviour {

    [SerializeField] float _hitpoints;
    public float HitPoints { get { return _hitpoints; } }
    [SerializeField] float _maxHitPoints;
    public float MaxHitPoints { get { return _maxHitPoints; } }

    bool SetupDone = false;

    public void Setup()
    {
        if (SetupDone)
            return;
        string save = SaveState.Load();
        if (!save.Equals(""))
        {
            string[] split = save.Split(';');
            WallStats wallStats = JsonUtility.FromJson<WallStats>(split[1]);
            _hitpoints = wallStats._wallHealth;
            _maxHitPoints = wallStats._wallHealthMax;

        }
        else
        {
            _hitpoints = _maxHitPoints;
        }
        SetupDone = true;
    }

    public void Reset()
    {
        SetupDone = false;
        Setup();
    }

    public void RemoveHitPoints(float hitPoints)
    {
        _hitpoints -= hitPoints;
        if (_hitpoints <= 0)
            GameController.instance.GameIsOver();
    }

    public void AddHitPoints(float amount)
    {
        _hitpoints += amount;
        if (_hitpoints > _maxHitPoints)
            _hitpoints = MaxHitPoints;
    }

    public string ToJson()
    {
        WallStats ws = new WallStats();
        ws._wallHealth = _hitpoints;
        ws._wallHealthMax = _maxHitPoints;
        return JsonUtility.ToJson(ws);
    }

    
}


[System.Serializable]
public class WallStats
{
    public float _wallHealth;
    public float _wallHealthMax;
}
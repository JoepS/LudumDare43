using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatisticsController : MonoBehaviour {

    List<Tupler<StatisticsFollowed, float>> _statistics;

    private void Awake()
    {
        _statistics = new List<Tupler<StatisticsFollowed, float>>();
    }

    // Use this for initialization
    void Start () {

	}

    public void AddValue(StatisticsFollowed statistic, float value)
    {
        Tupler<StatisticsFollowed, float> tupler = _statistics.Where(x => x.key == statistic).FirstOrDefault();
        if(tupler == null)
        {
            tupler = new Tupler<StatisticsFollowed, float>(statistic);
            _statistics.Add(tupler);
        }
        tupler.value += value;
    }

    public float GetValue(StatisticsFollowed statistic)
    {
        Tupler<StatisticsFollowed, float> tupler = _statistics.Where(x => x.key == statistic).FirstOrDefault();
        if (tupler == null)
        {
            tupler = new Tupler<StatisticsFollowed, float>(statistic);
            _statistics.Add(tupler);
        }
        return tupler.value;
    }
	
    public void Reset()
    {
        _statistics = new List<Tupler<StatisticsFollowed, float>>();
    }

	// Update is called once per frame
	void Update () {
	}
}

public class Tupler<T, T2>
{
    public Tupler(T key)
    {
        this.key = key;
        this.value = default(T2);
    }

    public T key;
    public T2 value;

    public override string ToString()
    {
        return key + " / " + value;
    }
}

public enum StatisticsFollowed
{
    Sacrifices,
    EnemiesKilled
}

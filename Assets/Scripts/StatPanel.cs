using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatPanel : MonoBehaviour {

    [SerializeField] string _statName;
    public string StatName { get { return _statName; } }
    [SerializeField] Text _statValue;
	
    public void SetStats(float value)
    {
        _statValue.text = "" + Mathf.RoundToInt(value);
    }
}

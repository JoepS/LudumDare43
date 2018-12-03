using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Reproducable : MonoBehaviour {

    [SerializeField] Reproducable _parent;
    List<GameObject> _kids;
    public float _reproduceTime = 10;
    float _maxOffspring = 3;

    GameObject _parentGameObject;

    private void Awake()
    {
        _kids = new List<GameObject>();
    }

    void Start()
    {
        StartCoroutine(Reproducing());
    }

    void SetParent(Reproducable parent)
    {
        _parent = parent;
    }

    public void SetStats(List<float> list)
    {
        _maxOffspring = list[0];
        _reproduceTime = list[1];
    }

    public void SetParentGameObject(GameObject parent)
    {
        _parentGameObject = parent;
    }

    public void SetActive(bool value)
    {
        if (value == this.enabled)
            return;
        this.enabled = value;
        if (!value)
        {
            StopAllCoroutines();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(Reproducing());
        }
    }

    void Reproduce()
    {
        if (VillagerGenerator.CanReproduce())
        {
            GameObject child = GameObject.Instantiate(this.gameObject, _parentGameObject.transform);
            child.name = "Villager " + VillagerGenerator.TotalVillagersGenerated;
            _kids.Add(child);
            Villager childVillager = child.GetComponent<Villager>();
            childVillager.SetStats(VillagerGenerator.GenerateStats());
            Reproducable childReproducable = child.GetComponent<Reproducable>();
            childReproducable.SetParentGameObject(_parentGameObject);
            childReproducable.SetStats(VillagerGenerator.GenerateReproducableStats());
            childReproducable.SetParent(this);
            VillagerGenerator.AddVillager(childVillager);
        }
    }

    void RemoveChild(GameObject child)
    {
        _kids.Remove(child);
    }

    private void OnDestroy()
    {
        if(_parent != null)
            _parent.RemoveChild(this.gameObject);
        if (_kids != null)
        {
            foreach (GameObject go in _kids)
            {
                go.GetComponent<Reproducable>().SetParent(null);
            }
        }
    }

    public List<float> GetStats()
    {
        List<float> list = new List<float>();
        list.Add(_maxOffspring);
        list.Add(_reproduceTime);
        return list;
    }

    float timer;
    IEnumerator Reproducing()
    {
        timer = _reproduceTime;
        while (true)
        {
            if (_kids.Count < _maxOffspring && timer <= 0)
            {
                Reproduce();
                timer = _reproduceTime * (_kids.Count + 1) * VillagerGenerator.TotalVillagersGenerated;
            }
            else
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    timer = 0;
                }
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public string ToJson()
    {
        string json = "";

        ListFloat repStats = new ListFloat();
        repStats.list = this.GetStats();

        int parentId = -1;
        if (_parent != null)
            parentId = _parent.GetComponent<Villager>().GetStats().Id;
        json = JsonUtility.ToJson(repStats) + " / {\"_parent\": " + parentId + " }";

        return json;
    }

    public void AddToKids(GameObject go)
    {
        _kids.Add(go);
    }

}

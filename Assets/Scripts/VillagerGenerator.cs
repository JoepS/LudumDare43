using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VillagerGenerator {

    static int _totalAmountOfVillagersGenerated = 0;
    public static int TotalVillagersGenerated { get { return _totalAmountOfVillagersGenerated; } }

    static List<Villager> _villagers;
    public static List<Villager> Villagers { get { return _villagers; } }

    const int _MaxVillagers = 25;

	public static EntityStats GenerateStats()
    {
        int difficulty = GameController.instance.Difficulty;
        EntityStats stats = new EntityStats(TotalVillagersGenerated);
        float rand = Random.Range(0f, 1f);
        stats.MaxHitPoints = (Mathf.Lerp(5, 10 * difficulty, rand));
        stats.MaxMagicPoints = (Mathf.Lerp(1, difficulty, Random.Range(0f, 1f)));

        stats.UpdateStats(Random.Range(1f, 5f * difficulty), Random.Range(1f, 5f * difficulty), Random.Range(1f, 5f * difficulty), Random.Range(1f, 5f * difficulty));
        return stats;
    }

    public static bool CanReproduce()
    {
        bool value = true;

        if(_villagers.Count + 1 > _MaxVillagers)
        {
            value = false;
        }

        return value;
    }

    public static void Reset()
    {
        _villagers = new List<Villager>();
        _totalAmountOfVillagersGenerated = 0;
    }

    public static void RemoveVillager(Villager villager)
    {
        _villagers.Remove(villager);
    }

    public static int VillagerCount()
    {
        if (_villagers == null)
            _villagers = new List<Villager>();
        return _villagers.Count;
    }

    public static bool ContainsVillager(Villager villager)
    {
        if (_villagers == null)
        {
            _villagers = new List<Villager>();
        }
        return _villagers.Contains(villager);
    }

    public static void AddVillager(Villager villager)
    {
        if (_villagers == null)
            _villagers = new List<Villager>();
        _villagers.Add(villager);
        _totalAmountOfVillagersGenerated++;
    }

    public static List<float> GenerateReproducableStats()
    {
        List<float> list = new List<float>();

        //Max Amount of kids:
        list.Add(Random.Range(1, 3));
        //Reproducition time;
        list.Add(Random.Range(5 * GameController.instance.Difficulty, 10 * GameController.instance.Difficulty));

        return list;
    }
}

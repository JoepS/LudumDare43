using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VillageSceneController : MonoBehaviour {

    [SerializeField] GameObject _villagerPrefab;
    [SerializeField] GameObject _villagersParent;

	// Use this for initialization
	void Start () {

        GameController.instance.WallHealth.Setup();

        string save = SaveState.Load();
        if (!save.Equals(""))
        {
            VillagerGenerator.Reset();
            string[] stats = save.Split(';');
            stats[0] = "";
            stats[1] = "";
            stats[2] = "";
            foreach (string s in stats)
            {
                if (!s.Equals("") && !string.IsNullOrEmpty(s))
                {
                    string[] villagerStats = s.Split('/');
                    ListFloat repStats = JsonUtility.FromJson<ListFloat>(villagerStats[1]);
                    CreateVillager(JsonUtility.FromJson<EntityStats>(villagerStats[0]), repStats.list);
                }
            }
            foreach(string s in stats)
            {
                if (!s.Equals("") && !string.IsNullOrEmpty(s))
                {
                    string[] villagerStats = s.Split('/');
                    EntityStats villagerEntityStats = JsonUtility.FromJson<EntityStats>(villagerStats[0]);
                    int parentid = int.Parse(villagerStats[2].Split(':')[1].Replace(" }", ""));
                    if (parentid == -1)
                        continue;
                    Villager villager = VillagerGenerator.Villagers.Where(x => x.GetStats().Id == villagerEntityStats.Id).FirstOrDefault();
                    if(villager != null)
                    {
                        Villager parentVillager = VillagerGenerator.Villagers.Where(x => x.GetStats().Id == parentid).FirstOrDefault();
                        if(parentVillager != null)
                        {
                            villager.SetParent(parentVillager);
                        }
                    }
                }
            }


        }
        else if (VillagerGenerator.VillagerCount() <= 0)
            CreateVillager();

        StartCoroutine(SimulateWallAttack());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateVillager()
    {
        CreateVillager(VillagerGenerator.GenerateStats(), VillagerGenerator.GenerateReproducableStats());
    }

    public void CreateVillager(EntityStats stats, List<float> reproducableStats)
    {
        GameObject go = GameObject.Instantiate(_villagerPrefab, _villagersParent.transform);
        go.transform.localPosition = new Vector3(0, 3, 0);
        go.name = "Villager " + VillagerGenerator.TotalVillagersGenerated;
        Villager villager = go.GetComponent<Villager>();
        villager.SetStats(stats);
        VillagerGenerator.AddVillager(villager);
        Reproducable childReproducable = go.GetComponent<Reproducable>();
        childReproducable.SetParentGameObject(_villagersParent);
        childReproducable.SetStats(reproducableStats);
    }

    IEnumerator SimulateWallAttack()
    {
        GameObject go = new GameObject();
        go.transform.SetParent(this.transform);
        IEntity statsAttacking = go.AddComponent<Enemy>();
        statsAttacking.SetStats(EnemySpawner.GenerateStats(-1));
        float difficulty = GameController.instance.Difficulty;
        while (true)
        {
            float hitPoints = statsAttacking.GetAttackHitPoints();
            GameController.instance.WallHealth.RemoveHitPoints(hitPoints);
            yield return new WaitForSeconds(Random.Range(2, 10));
            if(difficulty != GameController.instance.Difficulty)
            {
                Destroy(statsAttacking);
                statsAttacking = go.AddComponent<Enemy>();
                statsAttacking.SetStats(EnemySpawner.GenerateStats(-1));
            }
        }
    }
}

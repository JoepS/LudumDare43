using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {


    public int Difficulty { get; set; }

    [SerializeField] List<GameObject> _enemyPrefabs;
    [SerializeField] PlayerMovement _player;
    [SerializeField] GameObject _wall;

    public static List<GameObject> _enemies;

	// Use this for initialization
	void Start () {
        if (_enemies == null)
            _enemies = new List<GameObject>();
        StartCoroutine(SpawnEnemies());	
	}
	
	// Update is called once per frame
	void Update () {
	}

    IEnumerator SpawnEnemies()
    {
        float difficulty = GameController.instance.Difficulty;

        while (true)
        {
            bool spawnedEnemy = false;
            if (_enemies.Count < 5 * difficulty && _enemies.Count < 25)
            {
                SpawnEnemy();
                spawnedEnemy = true;
            }

            float minWait = 5f / difficulty;
            float maxWait = 20f;
            float waitTime = spawnedEnemy ? Mathf.Lerp(minWait, maxWait, Random.Range(0, 1 -Time.deltaTime * difficulty)) : 1;
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void RemoveFromEnemiesList(GameObject go)
    {
        _enemies.Remove(go);
    }

    public void SpawnEnemy()
    {
        GameController.instance.CheckDifficultyIncrease();
        GameObject prefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Count)];
        GameObject go = GameObject.Instantiate(prefab);
        go.transform.localPosition = this.transform.position;
        Enemy enemy = go.GetComponent<Enemy>();
        enemy.SetPlayerAndWallValues(_player.gameObject, _wall);
        enemy.SetStats(GenerateStats());
        enemy.SetEnemySpawner(this);
        _enemies.Add(go);
    }

    public static EntityStats GenerateStats()
    {
        return GenerateStats(_enemies.Count);
    }
    
    public static EntityStats GenerateStats(int id)
    {
        EntityStats stats = new EntityStats(id);
        float rand = Random.Range(0f, 1f);

        stats.SetLevel(Random.Range(1, GameController.instance.Difficulty+1));
        stats.SetExperience(stats.Level * Random.Range(1f, 5f));
        stats.MaxHitPoints = (Mathf.Lerp(5 * stats.Level, 10 * stats.Level, rand));
        stats.MaxMagicPoints = (Mathf.Lerp(1, stats.Level, Random.Range(0f, 1f)));

        stats.UpdateStats(Random.Range(1f, 5f * stats.Level), Random.Range(1f, 5f * stats.Level), Random.Range(1f, 5f * stats.Level), Random.Range(1f, 5f * stats.Level));

        return stats;

    }
}

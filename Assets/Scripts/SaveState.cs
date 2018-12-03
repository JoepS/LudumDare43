using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

public static class SaveState
{
    const string EncryptionKey = "abcdefghijklmnop";
    const string EncryptionIv = "ponmlkjihgfedcba";

    static List<Villager> _villagers;
    static PlayerMovement _player;

    const string HashKey = "SaveKey";

    const string PlayerPrefsKey = "Savestate";

    public static void Save(PlayerMovement player, List<Villager> villagers)
    {
        _player = player;
        _villagers = villagers;
        string stringToHash = JsonUtility.ToJson(_player.GetStats()) + ";";
        stringToHash += GameController.instance.WallHealth.ToJson() + ";";
        stringToHash += GameController.instance.Difficulty + ";";
        foreach(Villager v in _villagers)
        {
            string repStats = v.reproducable.ToJson();
            stringToHash += JsonUtility.ToJson(v.GetStats()) + " / " + repStats + ";";
        }
        PlayerPrefs.SetString(PlayerPrefsKey, stringToHash);
    }

    public static string Load()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string hash = PlayerPrefs.GetString(PlayerPrefsKey);
            if (hash.Equals(""))
                return "";
            return hash;
        }
        return "";
    }

    public static void ClearSave()
    {
        PlayerPrefs.SetString(PlayerPrefsKey, "");
        VillagerGenerator.Reset();
        GameController.instance.Statistics.Reset();
        GameController.instance.WallHealth.Reset();
    }
}

[System.Serializable]
public class ListFloat
{
    public List<float> list;
}

[System.Serializable]
public class ListEntityStats
{
    public List<EntityStats> list;
}
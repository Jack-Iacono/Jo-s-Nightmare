using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareSpawnData
{
    public enum nightmareType{ SPIDER, SKELETON, BEAR };

    public GameObject nightmareObject;
    public float spawnTime;
    public bool isSpawned;
    public nightmareType type;

    public NightmareSpawnData(GameObject n, float s, bool b, nightmareType t)
    {
        nightmareObject = n;
        spawnTime = s;
        isSpawned = b;
        type = t;
    }
}

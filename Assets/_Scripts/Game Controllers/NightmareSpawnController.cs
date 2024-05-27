using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareSpawnController : MonoBehaviour
{
    private const string nightmareIndexText = "\n0: Spider\n1: Bear\n2: Skeleton";
    public static NightmareSpawnController Instance;

    [Header("NavAgent Locations")]
    public GameObject bedGameObject;
    public Transform bedPosition;

    [Header("Nightmare Skeleton")]
    public GameObject skeletonObject;
    public Transform skeletonSpawn;
    public Transform[] skeletonPatrolPoints;

    [Header("Spawning Variables")]
    [Tooltip("Time from beginning of night in seconds" + nightmareIndexText)]
    //public List<float> spiderSpawnDelays = new List<float>();
    //public List<float> bearSpawnDelays = new List<float>();
    public List<float> skeletonSpawnDelays = new List<float>();

    private List<NightmareSpawnData> nightmares = new List<NightmareSpawnData>();

    public static float nightLength = 240;
    public static float currentTime { get; private set; } = 0.0f;
    private static bool allSpawned = false;

    private string blockedNightmare;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        GameController.UnregisterNightmares();

        currentTime = 0.0f;

        //Creates the nightmares and stores their data
        for (int i = 0; i < skeletonSpawnDelays.Count; i++)
        {
            skeletonSpawnDelays[i] = RandomizeSpawnTimer(skeletonSpawnDelays[i]);
            nightmares.Add(new NightmareSpawnData(Instantiate(skeletonObject), skeletonSpawnDelays[i], false, NightmareSpawnData.nightmareType.SKELETON));
        }

        for(int i = 0; i < nightmares.Count; i++)
        {
            nightmares[i].nightmareObject.SendMessage("Initialize");
            nightmares[i].nightmareObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.isPaused)
        {
            if(GameController.currentPhase == GameController.NightPhase.ATTACK)
            {
                if (!allSpawned)
                {
                    bool check = true;

                    for (int i = 0; i < nightmares.Count; i++)
                    {
                        if (nightmares[i].spawnTime <= currentTime && !nightmares[i].isSpawned)
                        {
                            SpawnNightmare(i);
                        }

                        if (!nightmares[i].isSpawned)
                        {
                            check = false;
                        }
                    }

                    if (check)
                        allSpawned = true;
                }

                currentTime += Time.deltaTime;

                if (currentTime >= nightLength)
                {
                    EndNight();
                }

            }
        }
    }

    public void BeginNight()
    {
        currentTime = 0;

        if (CheckPaths())
        {
            GameController.SetPhase(GameController.NightPhase.ATTACK);
        }
        else
        {
            InterfaceController.Instance.HUDMessage("The Nightmares Can't Reach the Bedroom! (Try moving some furniture)");
        }
    }
    public void EndNight()
    {
        for(int i = 0; i < nightmares.Count; i++)
        {
            nightmares[i].nightmareObject.SetActive(false);
        }

        //nightmares.Clear();

        GameController.SetPhase(GameController.NightPhase.WIN);
    }
    public bool CheckPaths()
    {
        bool check = true;

        for(int i = 0; i < nightmares.Count; i++)
        {
            nightmares[i].nightmareObject.SetActive(true);
            if (!nightmares[i].nightmareObject.GetComponent<Nightmare>().CheckPath())
            {
                // Will notify if the spider specifically has the issue
                if (nightmares[i].GetType() == typeof(NightmareSpiderController).GetType())
                {
                    InterfaceController.Instance.HUDMessage("The spiders can't find their nests");
                }

                check = false;
            }
            nightmares[i].nightmareObject.SetActive(false);
        }
        return check;
    }

    private float RandomizeSpawnTimer(float f)
    {
        return Mathf.Abs(Random.Range(f - 5, f + 5));
    }

    #region Spawning Methods
    public void SpawnNightmare(int i)
    {
        nightmares[i].nightmareObject.SetActive(true);
        nightmares[i].nightmareObject.SendMessage("Spawn");
        nightmares[i].isSpawned = true;
    }
    #endregion

    #region Get Methods

    public static string GetTimeRemaining()
    {
        return MyFunctions.ConvertToTime(nightLength - currentTime);
    }
    public static float GetTimeRemainingPercent()
    {
        return (nightLength - currentTime) / nightLength;
    }

    #endregion

}

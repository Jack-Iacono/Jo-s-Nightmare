using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class ValueStoreController
{
    #region Achievement Stuff

    public static int totalNightmareStalls { get; private set; } = 0;
    public static int totalNightmareSkeletonHeadHits { get; private set; } = 0;
    public static int totalNightmareBearHoneyPotDistracts { get; private set; } = 0;
    public static int totalNightmareSpiderSwats { get; private set; } = 0;

    public static int totalNightsCompleted { get; private set; } = 0;

    // Lore Related Variables
    public const int LORE_PAGES = 5;
    private static bool[] pagesCollected = new bool[LORE_PAGES];

    #endregion

    public static KeyBindData keyBinds = new KeyBindData();

    public static void SlapSkeletonHead()
    {
        totalNightmareSkeletonHeadHits++;
        totalNightmareStalls++;
    }
    public static void SwatSpider()
    {
        totalNightmareSpiderSwats++;
        totalNightmareStalls++;
    }
    public static void HoneyPotDistract()
    {
        totalNightmareBearHoneyPotDistracts++;
        totalNightmareStalls++;
    }

    public static void NightFinish()
    {
        totalNightsCompleted++;
    }

    public static void CollectPage(int i)
    {
        if(i < pagesCollected.Length)
        {
            pagesCollected[i] = true;
        }

        Debug.Log(pagesCollected[i]);
    }
    public static bool[] LoadPages()
    {
        //Put loading data for pages here
        // TEMPORARY, will reset to 0 on every load
        if(pagesCollected.Length != LORE_PAGES)
            pagesCollected = new bool[LORE_PAGES];

        return pagesCollected;
    }
}

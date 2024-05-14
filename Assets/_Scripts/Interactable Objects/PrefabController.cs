using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabController : MonoBehaviour
{
    public enum PrefabType { POPUP };

    public GameObject popupPrefab;
    private static GameObject popupInstance;
}

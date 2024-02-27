using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{
    public static InterfaceController Instance { get; private set; }

    public enum Screen { HUD, PAUSE, NIGHT_WIN, NIGHT_LOSE, LORE };
    private Screen currentScreen = Screen.HUD;

    //Creates a list of screens for as many values as there are in the enum Screen
    [Tooltip("0: HUD\n1: PAUSE\n2: Win\n3: Lose\n4: Lore")]
    public GameObject[] screens = new GameObject[Enum.GetNames(typeof(Screen)).Length];

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        SetupScreen(Screen.HUD);
        GameController.RegisterObserver(this);
    }

    #region Screen Setup Methods
    public void SetupCurrentScreen()
    {
        for(int i = 0; i  < screens.Length; i++)
        {
            if (i == (int)currentScreen)
                screens[i].SetActive(true);
            else
                screens[i].SetActive(false);
        }

        screens[(int)currentScreen].SendMessage("SetupScreen", SendMessageOptions.DontRequireReceiver);
    }
    public void SetupScreen(int s)
    {
        currentScreen = (Screen)s;
        SetupCurrentScreen();
    }
    public void SetupScreen(Screen s)
    {
        currentScreen = s;
        SetupCurrentScreen();
    }
    #endregion

    public void ChangePhase(GameController.NightPhase phase)
    {
        screens[(int)Screen.HUD].SendMessage("ChangePhase", phase, SendMessageOptions.DontRequireReceiver);
    }

    #region HUD Methods

    public void HUDToolTip(string type)
    {
        screens[(int)Screen.HUD].SendMessage("ToolTip", type, SendMessageOptions.DontRequireReceiver);
    }
    public void HUDMessage(string str)
    {
        screens[(int)Screen.HUD].SendMessage("Message", str, SendMessageOptions.DontRequireReceiver);
    }
    public void HUDLight()
    {
        screens[(int)Screen.HUD].SendMessage("Light", SendMessageOptions.DontRequireReceiver);
    }
    public void HUDBand()
    {
        screens[(int)Screen.HUD].SendMessage("Band", SendMessageOptions.DontRequireReceiver);
    }
    public void HUDObjective(List<string> o)
    {
        screens[(int)Screen.HUD].SendMessage("Objectives", o, SendMessageOptions.DontRequireReceiver);
    }
    

    #endregion
}

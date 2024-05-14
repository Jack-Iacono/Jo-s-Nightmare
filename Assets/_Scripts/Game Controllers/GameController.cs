using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    // Method names
    private const string PAUSE_METHOD_NAME = "GamePause";

    public enum NightPhase { PREGAME, SETUP, ATTACK, WIN, LOSE };
    public static NightPhase currentPhase { get; private set; } = NightPhase.PREGAME;
    public const int MAX_NIGHT_LIGHTS = 3;
    public static int currentNightLights = MAX_NIGHT_LIGHTS;
    public static bool isPaused { get; private set; } = false;

    private static List<NightmareController> nightmares = new List<NightmareController>();
    private static List<MonoBehaviour> phaseObservers = new List<MonoBehaviour>();

    private void Start()
    {
        Instance = this;

        currentNightLights = MAX_NIGHT_LIGHTS;

        if(!TutorialScreenController.tutorialActive)
        {
            currentPhase = NightPhase.SETUP;
            SetPause(true);
        }
        else
        {
            currentPhase = NightPhase.PREGAME;
            SetPause(true);
        }
    }

    private void Update()
    {
        if
            (
                (currentPhase == NightPhase.SETUP || currentPhase == NightPhase.ATTACK) && 
                !TutorialScreenController.tutorialActive && 
                Input.GetKeyDown(ValueStoreController.keyBinds.keyPause)
            )
        {
            SetPauseMenu(!isPaused);
        }
    }

    public static void DestroyLight()
    {
        SoundManager.PlaySound(SoundManager.shatter);

        if (currentNightLights > 1)
        {
            currentNightLights--;
            InterfaceController.Instance.HUDLight();
        }
        else
        {
            SetPhase(NightPhase.LOSE);
            InterfaceController.Instance.HUDLight();
        }
    }

    public static void SetPause(bool b)
    {
        isPaused = b;
        BroadcastToNightmares(PAUSE_METHOD_NAME);

        if (isPaused)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

        NotifyGamePause();
    }
    public static void SetPauseMenu(bool b)
    {
        SetPause(b);

        if (isPaused)
        {
            InterfaceController.Instance.SetupScreen(InterfaceController.Screen.PAUSE);
        }
        else
        {
            InterfaceController.Instance.SetupScreen(InterfaceController.Screen.HUD);
        }
    }

    public static void SetPhase(NightPhase phase)
    {
        currentPhase = phase;

        NotifyPhaseChange();

        //Specific Setups for each phase
        switch (phase)
        {
            case NightPhase.ATTACK:
                InterfaceController.Instance.HUDObjective(new List<string> { "Watch out for the nightmares" });
                break;
            case NightPhase.SETUP:
                InterfaceController.Instance.HUDObjective(new List<string> { "Move Furniture", "Hit the Alarm (start attack)" });
                break;
            case NightPhase.WIN:
                InterfaceController.Instance.SetupScreen(InterfaceController.Screen.NIGHT_WIN);
                SetPause(true);

                // Clear out the nightmares from this night, not needed anymore
                nightmares.Clear();
                break;
            case NightPhase.LOSE:
                InterfaceController.Instance.SetupScreen(InterfaceController.Screen.NIGHT_LOSE);
                SetPause(true);

                // Clear out the nightmares from this night, not needed anymore
                nightmares.Clear();
                break;
        }
    }
    public static void EndTutorial()
    {
        SetPhase(NightPhase.SETUP);
        SetPause(false);
    }

    #region Subject Methods
    public static void ResetObservers()
    {
        Debug.Log("test");
        phaseObservers.Clear();
        nightmares.Clear();
    }

    public static void BroadcastToNightmares(string method)
    {
        foreach(NightmareController n in nightmares)
        {
            if(n)
            {
                n.SendMessage(method, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    public static void RegisterNightmare(NightmareController n)
    {
        nightmares.Add(n);
    }
    public static void UnregisterNightmares() 
    {
        nightmares.Clear();
    }

    #region Notifications

    public static void NotifyPhaseChange()
    {
        foreach(MonoBehaviour m in phaseObservers)
        {
            m.SendMessage("ChangePhase", currentPhase, SendMessageOptions.DontRequireReceiver);
        }
    }
    public static void NotifyGamePause()
    {
        foreach (MonoBehaviour m in phaseObservers)
        {
            m.SendMessage("GamePause", isPaused, SendMessageOptions.DontRequireReceiver);
        }
    }

    #endregion
    public static void RegisterObserver(MonoBehaviour m)
    {
        phaseObservers.Add(m);
    }
    public static void UnregisterObersver(MonoBehaviour m)
    {
        try
        {
            phaseObservers.Remove(m);
        }
        catch
        {
            Debug.Log("Invalid Observer");
        }
    }

    #endregion
}

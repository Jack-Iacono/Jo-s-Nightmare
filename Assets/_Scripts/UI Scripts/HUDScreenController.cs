using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDScreenController : MonoBehaviour
{
    [Header("Timer Variables")]
    public Image timerHolder;
    public Image timerFill;
    public Animator timerAnimator;

    [Space]
    public TMP_Text messageText;
    public CanvasGroup messageArea;
    public float messageFadeSpeed = 2;
    public float messageFadeTime = 4;
    private List<string> messageBuffer = new List<string>();
    private bool messageFade = false;
    private Coroutine messageRoutine;
    
    [Space]
    public GameObject lightArea;
    public GameObject lightImage;
    public Sprite lightOn;
    public Sprite lightOff;
    public Animator lightAnim;
    private List<GameObject> lights = new List<GameObject>();
    
    [Space]
    public TMP_Text bandText;

    [Space]
    public TMP_Text objectiveText;

    [Space]
    public GameObject phaseIndicator;
    public Sprite setupPhaseIcon;
    public Sprite attackPhaseIcon;
    public Animator phaseAnimation;

    [Space]
    public GameObject toolTipArea;
    public Sprite furnitureToolTip;

    // Start is called before the first frame update
    void Start()
    {
        messageArea.alpha = 0;

        SetupScreen();
    }
    private void Update()
    {
        if (messageFade)
        {
            messageArea.alpha -= Time.deltaTime * messageFadeSpeed;
            if (messageArea.alpha <= 0)
            {
                messageFade = false;
                messageBuffer.Clear();
            }
        }

        timerFill.fillAmount = NightSpawnController.GetTimeRemainingPercent();
    }

    public void SetupScreen()
    {
        Light();
        Band();
        ToolTip("empty");
    }

    public void ToolTip(string type)
    {
        switch (type) 
        {
            case "furniture":
                toolTipArea.GetComponent<Image>().color = MyColors.white;
                toolTipArea.GetComponent<Image>().sprite = furnitureToolTip;
                break;
            case "empty":
                toolTipArea.GetComponent<Image>().color = MyColors.clear;
                break;
            default:
                toolTipArea.GetComponent<Image>().sprite = null;
                Debug.Log("Invlaid Type");
                break;
        }

    }
    public void Message(string str)
    {
        messageBuffer.Add(str);
        messageFade = false;
        DisplayMessages();

        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(MessageFadeWait());
    }
    public void Light()
    {
        for (int i = 0; i < lights.Count; i++)
        {
            Destroy(lights[i]);
        }

        lights.Clear();

        for (int i = 0; i < GameController.MAX_NIGHT_LIGHTS; i++)
        {
            lights.Add(Instantiate(lightImage, lightArea.transform));

            RectTransform lightRect = lights[i].GetComponent<RectTransform>();
            lightRect.anchoredPosition = new Vector2(i * 130 + 65, 0);

            if (i < GameController.currentNightLights)
                lights[i].GetComponent<Image>().sprite = lightOn;
            else
                lights[i].GetComponent<Image>().sprite = lightOff;
        }

        lightAnim.SetTrigger("shake");
    }
    public void Band()
    {
        try
        {
            bandText.text = PlayerController.playerInstances[0].rubberBands.ToString();
        }
        catch
        {
            // Defaults to the max amount
            bandText.text = PlayerController.RUBBER_BAND_MAX.ToString();
        }
    }
    public void Objectives(List<string> o)
    {
        string currentObjectives = "";

        foreach(string s in o)
        {
            if (currentObjectives == "")
                currentObjectives += s;
            else
                currentObjectives += "\n\n" + s;
        }

        objectiveText.text = currentObjectives;
    }
    public void ChangePhase(GameController.NightPhase phase)
    {
        switch (phase)
        {
            case GameController.NightPhase.SETUP:
                phaseIndicator.GetComponent<Image>().sprite = setupPhaseIcon;
                phaseAnimation.SetTrigger("Start");

                timerHolder.GetComponent<CanvasGroup>().alpha = 0;
                break;
            case GameController.NightPhase.ATTACK:
                phaseIndicator.GetComponent<Image>().sprite = attackPhaseIcon;
                phaseAnimation.SetTrigger("Start");

                timerAnimator.SetTrigger("Enter");
                timerHolder.GetComponent<CanvasGroup>().alpha = 1;
                break;
            case GameController.NightPhase.PREGAME:
                timerHolder.GetComponent<CanvasGroup>().alpha = 0;
                break;
        }
    }

    public void DisplayMessages()
    {
        messageArea.alpha = 1;
        messageText.text = "";

        for (int i = 0; i < messageBuffer.Count; i++)
        {
            if (messageText.text == "")
                messageText.text += messageBuffer[i];
            else
                messageText.text += "\n" + messageBuffer[i];
        }

    }
    IEnumerator MessageFadeWait()
    {
        yield return new WaitForSeconds(messageFadeTime);

        messageFade = true;
    }
}

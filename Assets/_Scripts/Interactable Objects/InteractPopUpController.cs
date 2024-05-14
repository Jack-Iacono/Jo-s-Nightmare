using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractPopUpController : MonoBehaviour
{
    public static InteractPopUpController Instance { get; private set; }

    private static Sprite bgSpriteDefault;
    private static bool isActive = false;
    private static GameObject currentTarget;
    private static TMP_Text popupText;
    private static Image popupBackground;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
            Instance = this;

        popupText = GetComponentInChildren<TMP_Text>();

        // Gets the image for the background and the sprite currently set, this is now the default background
        popupBackground = GetComponentInChildren<Image>();
        bgSpriteDefault = popupBackground.sprite;

        EndPopup(null);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.isPaused)
        {
            if (isActive)
            {
                // Places the popup above the selected object and rotates to face the camera
                transform.position = currentTarget.transform.position + Vector3.up;
                transform.LookAt(PlayerController.playerInstances[0].camCont.transform.position);
            }
            else
            {
                transform.position = Vector3.one * -10;
            }
        }
    }

    public static void StartPopup(GameObject target, Sprite bg, string message)
    {
        popupBackground.sprite = bg;
        popupText.text = message;

        StartPopup(target);
    }
    public static void StartPopup(GameObject target, Sprite bg)
    {
        popupBackground.sprite = bg;
        popupText.text = "";

        StartPopup(target);
    }
    public static void StartPopup(GameObject target, string message)
    {
        popupBackground.sprite = bgSpriteDefault;
        popupText.text = message;

        StartPopup(target);
    }
    public static void StartPopup(GameObject target)
    {
        isActive = true;
        currentTarget = target;

        Instance.gameObject.SetActive(true);
    }

    public static void EndPopup(GameObject target)
    {
        if(target == currentTarget)
        {
            isActive = false;
            currentTarget = null;

            Instance.gameObject.SetActive(false);
        }
    }

}

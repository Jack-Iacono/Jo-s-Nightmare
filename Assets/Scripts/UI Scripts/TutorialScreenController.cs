using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreenController : MonoBehaviour
{
    public GameObject tutorialScreen;

    public List<Sprite> tutorialPages = new List<Sprite>();
    public Sprite emptyPage;

    private int currentScreen = 0;
    private int screenCount;

    public GameObject leftPage;
    public GameObject rightPage;
    public GameObject leftPageButton;
    public GameObject rightPageButton;

    public GameObject finishButton;

    public static bool tutorialActive = false;

    private Animator anim;

    private void Start()
    {
        tutorialActive = false;
        tutorialScreen.SetActive(false);

        anim = GetComponent<Animator>();

        BeginTutorial();
    }

    public void BeginTutorial()
    {
        tutorialActive = true;
        tutorialScreen.SetActive(true);

        rightPageButton.SetActive(false);
        leftPageButton.SetActive(false);

        currentScreen = 0;

        screenCount = Mathf.CeilToInt((float)tutorialPages.Count / 2);

        anim.SetTrigger("Enter");
        StartCoroutine(Wait(1.5f));

        GameController.SetPause(true);

        SetupScreen();
    }
    public void EndTutorial()
    {
        anim.SetTrigger("Exit");
        StartCoroutine(WaitDisable(1.5f));
    }

    public void NextPage()
    {
        // Already accounts for the empty page
        if(currentScreen < screenCount - 1)
        {
            currentScreen++;
        }

        SetupScreen();
    }
    public void PreviousPage()
    {
        if(currentScreen > 0)
        {
            currentScreen--;
        }

        SetupScreen();
    }

    private void SetupScreen()
    {
        leftPage.GetComponent<Image>().sprite = tutorialPages[currentScreen * 2];

        if (currentScreen * 2 + 1 < tutorialPages.Count)
            rightPage.GetComponent<Image>().sprite = tutorialPages[currentScreen * 2 + 1];
        else
            rightPage.GetComponent<Image>().sprite = emptyPage;

        if (currentScreen == 0)
        {
            leftPageButton.SetActive(false);
            leftPageButton.GetComponent<Image>().color = Color.white;
        } 
        else
            leftPageButton.SetActive(true);

        if(currentScreen * 2 + 1 >= tutorialPages.Count - 1)
        {
            rightPageButton.SetActive(false);
            rightPageButton.GetComponent<Image>().color = Color.white;
        }
        else
            rightPageButton.SetActive(true);

        if (currentScreen == 0)
            finishButton.SetActive(false);
        else
            finishButton.SetActive(true);
            
    }
    private void EnableItems(bool b)
    {
        rightPageButton.SetActive(b);
        leftPageButton.SetActive(b);
    }

    public void ButtonHover(Image image)
    {
        image.color = MyColors.tutorialButtonHover;
    }
    public void ButtonUnhover(Image image)
    {
        image.color = MyColors.white;
    }

    IEnumerator Wait(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
    IEnumerator WaitDisable(float delay)
    {
        EnableItems(false);
        yield return new WaitForSeconds(delay);
        EnableItems(true);

        tutorialActive = false;
        tutorialScreen.SetActive(false);

        GameController.EndTutorial();
    }
    
}

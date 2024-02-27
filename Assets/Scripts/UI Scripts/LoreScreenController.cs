using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LoreScreenController : MonoBehaviour
{
    public static LoreScreenController Instance { get; private set; }

    public Sprite[] lorePages = new Sprite[ValueStoreController.LORE_PAGES];
    public Sprite emptyPageLeft;
    public Sprite emptyPageRight;

    private int currentScreen = 0;
    private int screenCount = Mathf.CeilToInt(ValueStoreController.LORE_PAGES / 2);

    public GameObject leftPage;
    public GameObject rightPage;
    public GameObject leftPageButton;
    public GameObject rightPageButton;

    public TMP_Text leftPageNumber;
    public TMP_Text rightPageNumber;

    private void Start()
    {
        Instance = this;

        SetupScreen();
    }

    public void SetupScreen()
    {
        currentScreen = 0;
        SetPages();
    }

    public void NextPage()
    {
        // Already accounts for the empty page
        if (currentScreen < screenCount)
        {
            currentScreen++;
        }

        SetPages();
    }
    public void PreviousPage()
    {
        if (currentScreen > 0)
        {
            currentScreen--;
        }

        SetPages();
    }

    private void SetPages()
    {
        bool[] pagesCollected = ValueStoreController.LoadPages();

        if (pagesCollected[currentScreen * 2])
            leftPage.GetComponent<Image>().sprite = lorePages[currentScreen * 2];
        else
            leftPage.GetComponent<Image>().sprite = emptyPageLeft;

        leftPageNumber.text = (currentScreen * 2 + 1).ToString();

        if (currentScreen * 2 + 1 < pagesCollected.Length)
        {
            if (pagesCollected[currentScreen * 2 + 1])
                rightPage.GetComponent<Image>().sprite = lorePages[currentScreen * 2 + 1];
            else
                rightPage.GetComponent<Image>().sprite = emptyPageRight;

            rightPageNumber.text = (currentScreen * 2 + 2).ToString();
        }
        else
        {
            rightPage.GetComponent<Image>().sprite = emptyPageRight;
            rightPageNumber.text = "";
        }

        if (currentScreen == 0)
        {
            leftPageButton.SetActive(false);
            leftPageButton.GetComponent<Image>().color = Color.white;
        }
        else
            leftPageButton.SetActive(true);

        if (currentScreen * 2 + 1 >= pagesCollected.Length - 1)
        {
            rightPageButton.SetActive(false);
            rightPageButton.GetComponent<Image>().color = Color.white;
        }
        else
            rightPageButton.SetActive(true);

        
    }

    public void ButtonHover(Image image)
    {
        image.color = MyColors.loreButtonHover;
    }
    public void ButtonUnhover(Image image)
    {
        image.color = MyColors.white;
    }
}

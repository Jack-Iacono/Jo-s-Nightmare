using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Sprite normalSprite;
    public Sprite pressedSprite;

    public void PressButton()
    {
        GetComponent<Image>().sprite = pressedSprite;
    }
    public void ReleaseButton()
    {
        GetComponent<Image>().sprite = normalSprite;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HoneyPotController : InteractableController
{
    public static List<HoneyPotController> activeHoneyPots = new List<HoneyPotController>();
    private bool isActive = false;

    public float expireTime = 10;
    private float currentExpireTimer = -1;

    public override void StartOverride()
    {
        base.StartOverride();

        if (activeHoneyPots.Count != 0)
            activeHoneyPots.Clear();
    }
    public override void ExtraUpdate()
    {
        if (isActive)
        {
            if (currentExpireTimer > 0)
            {
                currentExpireTimer -= Time.deltaTime;
            }
            else if (currentExpireTimer != -1)
            {
                currentExpireTimer = -1;
                isActive = false;
                activeHoneyPots.Remove(this);
                gameObject.SetActive(false);
            }
        }

        if (!isActive && activeHoneyPots.Count > 0 && activeHoneyPots[0] == this)
        {
            isActive = true;
        }
    }

    public void HoneyDestroy()
    {
        currentExpireTimer = 0;
        InteractPopUpController.EndPopup(gameObject);
    }
    public override void ObjectHit()
    {
        if (currentExpireTimer == -1)
        {
            currentExpireTimer = expireTime;

            activeHoneyPots.Add(this);

            SoundManager.PlaySound(SoundManager.bonk);
            SoundManager.PlaySound(SoundManager.bee);

            InteractPopUpController.EndPopup(gameObject);
        }
    }

    public override void InRangeAction()
    {
        if(currentExpireTimer == -1)
            InteractPopUpController.StartPopup(gameObject, "Hit");
    }
    public override void OutRangeAction()
    {
        InteractPopUpController.EndPopup(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LorePageCollectable : InteractableController
{
    public int lorePageIndex = -1;


    public override void Interact()
    {
        ValueStoreController.CollectPage(lorePageIndex);
        Destroy(gameObject);
    }

    public override void InRangeAction()
    {
        InteractPopUpController.StartPopup(gameObject, ValueStoreController.keyBinds.keyInteract.ToString());
    }
    public override void OutRangeAction()
    {
        InteractPopUpController.EndPopup(gameObject);
    }
}

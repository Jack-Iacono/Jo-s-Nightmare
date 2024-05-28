using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmClockController : InteractableController
{
    private ParticleSystem particle;

    public override void StartOverride()
    {
        base.StartOverride();

        particle = GetComponent<ParticleSystem>();
        particle.Play();
    }
    public override void Interact()
    {
        NightmareSpawnController.Instance.BeginNight();

        particle.Stop();

        base.Interact();

        OutRangeAction();
    }
    public override void InRangeAction()
    {
        InteractPopUpController.StartPopup(gameObject, ValueStoreController.keyBinds.keyInteract.ToString());
    }
    public override void OutRangeAction()
    {
        InteractPopUpController.EndPopup(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            NightmareSpawnController.Instance.BeginNight();
        }
    }
}

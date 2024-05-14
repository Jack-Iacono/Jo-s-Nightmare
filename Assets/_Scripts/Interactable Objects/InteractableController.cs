using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableController : MonoBehaviour
{
    public static InteractableController Instance { get; private set; }

    public Collider sightOverrideCollider = null;
    private Collider sightCollider;
    public float interactRange = 10;
    public Vector3 originOffset;

    [Space]
    public List<GameController.NightPhase> activePhases = new List<GameController.NightPhase>() { GameController.NightPhase.SETUP, GameController.NightPhase.ATTACK };

    public bool canHit;
    public bool canInteract;
    public bool canShoot;

    private void Start()
    {
        if (sightOverrideCollider != null)
            sightCollider = sightOverrideCollider;
        else
            sightCollider = GetComponent<Collider>();

        StartOverride();
    }
    public virtual void StartOverride()
    {
        //Do Nothing yet
    }

    private void Update()
    {
        if (!GameController.isPaused)
        {
            if (activePhases.Contains(GameController.currentPhase))
            {
                if (Instance == null || Instance == this)
                {
                    if (CameraController.camCont.GetCameraSight(sightCollider, interactRange))
                    {
                        if (Instance == null)
                        {
                            InRangeAction();
                        }

                        if (canInteract && Input.GetKeyDown(ValueStoreController.keyBinds.keyInteract))
                        {
                            Interact();
                        }

                        Instance = this;
                    }
                    else
                    {
                        if (Instance == this)
                        {
                            OutRangeAction();
                        } 

                        Instance = null;
                    }
                }
            }
            else
            {
                // If this instance is still being held, relenquish it cuz it ain't possible to use rn
                if(Instance == this)
                {
                    Instance = null;
                }
            }

            ExtraUpdate();
        }
    }
    public virtual void ExtraUpdate()
    {
        //Does Nothing for now
    }

    public virtual void Interact()
    {
        Instance = null;
    }

    public void PlayerDamageSwing()
    {
        if (canHit && activePhases.Contains(GameController.currentPhase))
            ObjectHit();
    }
    public void PlayerDamageShoot()
    {
        if (canShoot && activePhases.Contains(GameController.currentPhase))
            ObjectHit();
    }
    public virtual void ObjectHit()
    {
        //Do Nothing
    }

    public void SetInsance(InteractableController cont)
    {
        Instance = cont;
    }

    public virtual void InRangeAction()
    {
        //Do Nothing
    }
    public virtual void OutRangeAction()
    {
        //Do Nothing
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntFurnitureController : InteractableController
{
    [Header("Furniture Specific Variables")]
    public MeshRenderer meshOverride;
    public MeshRenderer meshRend { get; private set; }

    public Sprite invIcon;
    public List<FurnitureMoveController.PlacementType> placementTypes;

    private Material normalMaterial;

    public static int normalLayer;
    public static int moveLayer;

    public bool useGravity = false;

    public Rigidbody rb { get; private set; }

    public override void StartOverride()
    {
        normalLayer = gameObject.layer;
        moveLayer = LayerMask.NameToLayer("MovingFurniture");

        if (meshOverride != null)
            meshRend = meshOverride;
        else
            meshRend = GetComponentInChildren<MeshRenderer>();

        normalMaterial = meshRend.material;

        rb = GetComponent<Rigidbody>();

        if (useGravity)
            rb.useGravity = true;
    }

    public override void Interact()
    {
        if(FurnitureMoveController.Instance.heldFurniture == null)
        {
            PickupFurniture();
        }
    }

    public override void InRangeAction()
    {
        InteractPopUpController.StartPopup(gameObject, ValueStoreController.keyBinds.keyInteract.ToString());
    }
    public override void OutRangeAction()
    {
        InteractPopUpController.EndPopup(gameObject);
    }

    public void PlaceFurniture(Vector3 pos, Vector3 rot)
    {
        transform.position = pos;
        transform.rotation = Quaternion.Euler(rot);
        PlaceFurniture();
    }
    public void PlaceFurniture()
    {
        gameObject.layer = normalLayer;

        enabled = true;
        GetComponent<Collider>().isTrigger = false;

        if (meshOverride != null)
            meshRend.material = normalMaterial;
        else
            meshRend.material = normalMaterial;

        if (useGravity)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }   

        SetInsance(null);
    }
    public void PickupFurniture()
    {
        gameObject.layer = moveLayer;

        enabled = false;
        GetComponent<Collider>().isTrigger = true;

        if(useGravity)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            FindNearestAngle();
        } 

        FurnitureMoveController.Instance.PickUpFurniture(this);
    }

    private void FindNearestAngle()
    {
        float rotY = transform.rotation.eulerAngles.y;

        transform.rotation = Quaternion.Euler(0,(int)(rotY / 45) * 45, 0);
    }
}

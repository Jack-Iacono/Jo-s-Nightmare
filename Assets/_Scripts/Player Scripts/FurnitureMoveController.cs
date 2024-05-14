using System;
using UnityEngine;

public class FurnitureMoveController : MonoBehaviour
{
    public static FurnitureMoveController Instance { get; private set; }

    [NonSerialized]
    public IntFurnitureController heldFurniture = null;
    private Vector3 heldFurnPosStore = Vector3.zero;
    private Vector3 heldFurnRotStore = Vector3.zero;

    [Header("Materials")]
    [Tooltip("The material an object will have when it CAN be placed")]
    public Material placeClearMaterial;
    [Tooltip("The material an object will have when it CANNOT be placed")]
    public Material placeBlockMaterial;

    [Header("Placement Variables")]
    [Tooltip("All layers that objects can be placed on")]
    public LayerMask placementLayers;
    public LayerMask playerLayers;
    [Tooltip("The distance from the camera that an object can be placed")]
    public float placementRange;

    private bool clearPlacement = false;
    //This exists to prevent automatic placing after picking up (provides a 1 frame delay)
    private bool placeBuffer = false;

    //Keycodes for playing
    private KeyCode keyRotate = KeyCode.R;
    private KeyCode keyPlace = KeyCode.E;
    private KeyCode keyDrop = KeyCode.T;

    public enum PlacementType { WALL, CEILING, FLOOR, INVALID };

    private void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (heldFurniture != null)
        {
            Ray ray = CameraController.camCont.GetFurnitureMoveRay();
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, placementRange, placementLayers))
            {
                PlacementType placementType;

                // dictate what kind of surface the object is being placed on
                switch (Mathf.RoundToInt(hit.normal.y))
                {
                    case > 0:
                        placementType = PlacementType.FLOOR;
                        break;
                    case 0:
                        placementType = PlacementType.WALL;
                        break;
                    case < 0:
                        placementType = PlacementType.CEILING;
                        break;
                }

                float wallAngleY = Mathf.Atan2(hit.normal.x, hit.normal.z) * Mathf.Rad2Deg;

                Vector3 colliderSize = heldFurniture.GetComponent<BoxCollider>().size;

                // Decide how far off the wall the item should be, based on rotation
                float colliderOffset = Mathf.Abs(Mathf.Cos((heldFurniture.transform.rotation.eulerAngles.y - wallAngleY) * Mathf.Deg2Rad)) * colliderSize.z + Mathf.Abs(Mathf.Sin((heldFurniture.transform.rotation.eulerAngles.y - wallAngleY) * Mathf.Deg2Rad)) * colliderSize.x;

                Vector3 position =  hit.point + new Vector3
                    (
                        hit.normal.x * (colliderOffset / 2),
                        hit.normal.y * (colliderSize.y / 2),
                        hit.normal.z * (colliderOffset / 2)
                    );

                // Actually Moving and Checking the Validity of the Position
                heldFurniture.transform.position = position;

                // Final check if the object is in a wall or on the player, if so, it is invalid
                if
                (
                    !heldFurniture.placementTypes.Contains(placementType) || 
                    Physics.CheckBox(heldFurniture.transform.position, heldFurniture.GetComponent<BoxCollider>().size * 0.499f, heldFurniture.transform.rotation, placementLayers | playerLayers)
                )
                {
                    heldFurniture.meshRend.material = placeBlockMaterial;

                    clearPlacement = false;
                }
                else
                {
                    heldFurniture.meshRend.material = placeClearMaterial;

                    clearPlacement = true;
                }

                if (Input.GetKeyDown(keyPlace) && clearPlacement && !placeBuffer)
                    PlaceFurniture();
                if (Input.GetKeyDown(keyRotate))
                    heldFurniture.transform.Rotate(new Vector3(0, 45, 0));
                if (Input.GetKeyDown(keyDrop))
                    DropFurniture();

                if (placeBuffer)
                    placeBuffer = false;
            }
        }
    }

    public void PickUpFurniture(IntFurnitureController furnObj)
    {
        heldFurniture = furnObj;
        heldFurniture.meshRend.material = placeClearMaterial;
        clearPlacement = false;

        heldFurnPosStore = furnObj.transform.position;
        heldFurnRotStore = furnObj.transform.rotation.eulerAngles;

        placeBuffer = true;

        // Adds the tooltip to the hud
        InterfaceController.Instance.HUDToolTip("furniture");

        // Stops the raycasts from hittin the object
        heldFurniture.gameObject.layer = LayerMask.NameToLayer("Default");
    }
    public void PlaceFurniture()
    {
        // Stops the raycasts from hittin the object
        heldFurniture.gameObject.layer = LayerMask.NameToLayer("Interactables");

        InteractPopUpController.EndPopup(heldFurniture.gameObject);

        heldFurniture.GetComponent<IntFurnitureController>().PlaceFurniture();
        heldFurniture = null;

        // Adds the tooltip to the hud
        InterfaceController.Instance.HUDToolTip("empty");
    }
    public void DropFurniture()
    {
        // Stops the raycasts from hittin the object
        heldFurniture.gameObject.layer = LayerMask.NameToLayer("Interactables");

        InteractPopUpController.EndPopup(heldFurniture.gameObject);

        heldFurniture.GetComponent<IntFurnitureController>().PlaceFurniture(heldFurnPosStore, heldFurnRotStore);
        heldFurniture = null;

        // Adds the tooltip to the hud
        InterfaceController.Instance.HUDToolTip("empty");
    }
}

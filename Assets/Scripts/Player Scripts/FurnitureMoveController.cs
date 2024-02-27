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
            //Ray ray = CameraController.camCont.GetCameraSightRay();
            Ray ray = CameraController.camCont.GetFurnitureMoveRay();
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, placementRange, placementLayers))
            {
                RaycastHit xAngleHit;
                RaycastHit yAngleHit;

                PlacementType placementType;

                float wallAngleX = 0;
                float wallAngleY = 0;

                Vector3 perpVec = Vector3.zero;
                
                // the points i need
                Debug.DrawRay(ray.origin, hit.point - ray.origin, Color.green, 0.5f);

                Vector3 colliderSize = heldFurniture.GetComponent<BoxCollider>().size;
                float colliderOffset = 0;

                Ray rayTop = new Ray(ray.origin + 0.01f * Vector3.up, ray.direction);
                Ray rayBottom = new Ray(ray.origin - 0.01f * Vector3.up, ray.direction);
                Ray rayRight = new Ray(ray.origin + 0.01f * transform.right, ray.direction); 

                float topDist = hit.distance + 0.25f;

                // Calculate the angle that the hit object is
                if(Physics.Raycast(rayTop, out xAngleHit, topDist, placementLayers))
                {
                    float xDif = xAngleHit.point.x - hit.point.x;
                    float zDif = xAngleHit.point.z - hit.point.z;

                    float hDist = Mathf.Sqrt((xDif * xDif) + (zDif * zDif));
                    float yDist = xAngleHit.point.y - hit.point.y;

                    wallAngleX = Mathf.Round(Mathf.Atan2(yDist, hDist) * Mathf.Rad2Deg);
                }
                else
                {
                    // Will only run if the top cast doesn't hit anything, just a failsafe
                    Physics.Raycast(rayBottom, out xAngleHit, topDist, placementLayers);

                    float xDif = hit.point.x - xAngleHit.point.x;
                    float zDif = hit.point.z - xAngleHit.point.z;

                    float hDist = Mathf.Sqrt((xDif * xDif) + (zDif * zDif));
                    float yDist = hit.point.y - xAngleHit.point.y;

                    wallAngleX = Mathf.Round(Mathf.Atan2(yDist, hDist) * Mathf.Rad2Deg);
                }

                // Calculate the Y-Angle of the Wall in relation to the world
                if(Physics.Raycast(rayRight, out yAngleHit, topDist, placementLayers))
                {
                    Vector2 vec = new Vector2(yAngleHit.point.x, yAngleHit.point.z) - new Vector2(hit.point.x, hit.point.z);
                    perpVec = new Vector3(vec.y, 0, -vec.x).normalized;

                    float playerRot = PlayerController.playerInstances[0].transform.rotation.eulerAngles.x * Mathf.Deg2Rad;
                    float xDif = yAngleHit.point.x - hit.point.x;
                    float zDif = yAngleHit.point.z - hit.point.z;

                    float xDist = Mathf.Cos(playerRot) * xDif + Mathf.Sin(playerRot) * -zDif;
                    float zDist = Mathf.Cos(playerRot) * -zDif + Mathf.Sin(playerRot) * xDif;

                    wallAngleY = Mathf.Round(Mathf.Atan2(zDist, xDist) * Mathf.Rad2Deg);
                }

                if (wallAngleX == 0 && CameraController.camCont.gameObject.transform.rotation.eulerAngles.x > 180)
                {
                    wallAngleX = 180;
                }

                // Decide how far off the wall the item should be, based on rotation
                colliderOffset = Mathf.Abs(Mathf.Cos((heldFurniture.transform.rotation.eulerAngles.y - wallAngleY) * Mathf.Deg2Rad)) * colliderSize.z + Mathf.Abs(Mathf.Sin((heldFurniture.transform.rotation.eulerAngles.y - wallAngleY) * Mathf.Deg2Rad)) * colliderSize.x;

                // dictate what kind of surface the object is being placed on
                switch (wallAngleX)
                {
                    case <= 45:
                        placementType = PlacementType.FLOOR;
                        break;
                    case <= 135:
                        placementType = PlacementType.WALL;
                        break;
                    case <= 180:
                        placementType = PlacementType.CEILING;
                        break;
                    default:
                        placementType = PlacementType.INVALID;
                        break;
                }

                Vector3 position = hit.point;
                position += 
                    (Mathf.Sin(wallAngleX * Mathf.Deg2Rad)  * perpVec * (colliderOffset / 2)) + 
                    (Mathf.Cos(wallAngleX * Mathf.Deg2Rad) * Vector3.up * (heldFurniture.GetComponent<BoxCollider>().size.y / 2));

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

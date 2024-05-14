using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("GameObjects")]
    public PlayerController playerCont;
    public Camera cam;

    //Static instance of this camera
    public static CameraController camCont;

    public GameObject playerModel;

    [Header("Characteristics")]
    [Tooltip("The sensistivity of the mouse moving the camera")]
    public float sensitivity = 100;
    [Tooltip("The radius that the camera will attempt to be placed at")]
    public float targetRadius = 5;
    [Tooltip("The Maximum Radius the camera can be from it's origin")]
    public float maxRadius = 5;
    [Tooltip("The Minimum Radius the camera can be from it's origin")]
    public float minRadius = 2.1f;

    private float currentRadius;

    [Header("Layer Masks")]
    public LayerMask collideLayers;
    public LayerMask interactLayers;

    private float xRotation = 0;
    private float yRotation = 0;

    public Vector3 targetOffset = new Vector3(0, 0, 0);

    // TESING ONLY, DELETE LATER
    private Vector3 gizmoOrigin = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        camCont = this;

        currentRadius = targetRadius;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.isPaused)
        {
            MoveCamera();
        }
    }

    #region Camera Movement Methods

    private void MoveCamera()
    {
        //Taking in the input from the mouse
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //Gets the real rotation of the camera
        xRotation = xRotation - mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Getting the horizontal rotations from mouse inputs
        yRotation = yRotation + mouseX;

        //Moves the camera around the player
        transform.localRotation = Quaternion.Euler(Mathf.Clamp(xRotation, -90, 90), 0f, 0f);

        //Rotates the player to always be facing the direction of the camera
        playerCont.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    #region Third Person Cam Methods

    private void MoveCameraThirdPerson()
    {
        //Taking in the input from the mouse
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //Gets the real rotation of the camera
        xRotation = xRotation - mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (currentRadius < 0.5f)
            playerModel.GetComponent<SkinnedMeshRenderer>().enabled = false;
        else
            playerModel.GetComponent<SkinnedMeshRenderer>().enabled = true;

        //Getting the horizontal rotations from mouse inputs
        yRotation = yRotation + mouseX;

        //Moves the camera around the player
        transform.localRotation = Quaternion.Euler(Mathf.Clamp(xRotation, -90, 90), 0f, 0f);
        // Smooths the camera movement
        transform.position = Vector3.Slerp(transform.position, CheckCameraPosition(), 15 * Time.deltaTime);

        //Rotates the player to always be facing the direction of the camera
        playerCont.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);

        targetRadius = Mathf.Clamp(targetRadius - Input.GetAxis("Mouse ScrollWheel") * 5, minRadius, maxRadius);
    }
    private Vector3 CheckCameraPosition()
    {
        Vector3 offset =
                (targetOffset.x * playerCont.transform.right) +
                (targetOffset.y * playerCont.transform.up) +
                (targetOffset.z * playerCont.transform.forward)
            ;
        Vector3 orig = playerCont.transform.position + Vector3.up;
        Vector3 dest = -transform.forward;

        Ray offsetRay = new Ray(orig, offset);

        RaycastHit hit;

        Vector3 colSize = GetComponent<Collider>().transform.localScale;

        //Checks the camera offset from origin
        if (Physics.BoxCast(orig, colSize / 2, offset, out hit, Quaternion.Euler(Vector3.zero), 1, collideLayers))
        {
            //Gets the new offset position and sets it as te origin.
            orig = offsetRay.GetPoint(hit.distance);
        }
        else
            orig = orig + offset;

        Ray ray = new Ray(orig, dest);

        // Checks to see if the location is already overlapping with something
        if
        (
            Physics.CheckBox(orig, colSize, transform.rotation, collideLayers) &&
            Physics.Raycast(orig, dest + cam.transform.right * 0.3f, targetRadius, collideLayers)
        )
        {
            // Brings camera back to origin
            currentRadius = 0;
        }
        else if (Physics.BoxCast(orig, colSize / 2, dest, out hit, transform.rotation, targetRadius, collideLayers))
        {
            //Checks the camera movement away from the origin + offset
            currentRadius = hit.distance;
        }
        else
        {
            // Defaults to max target radius
            currentRadius = targetRadius;
        }

        gizmoOrigin = orig;

        return ray.GetPoint(currentRadius);
    }

    #endregion

    #endregion

    #region Get Methods
    public bool GetCameraSight(Collider col, float dist)
    {
        float distance = Vector3.SqrMagnitude(col.transform.position - (transform.position - transform.up * -0.15f));

        if (distance <= dist * dist)
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            float range = 50;

            if (Physics.Raycast(ray, out hit, range, collideLayers))
            {
                if (hit.collider == col)
                {
                    return true;
                }
            }
        }

        return false;
    }
    public Ray GetCameraSightRay()
    {
        if(currentRadius < 0.5f)
            return new Ray(new Ray(cam.transform.position, cam.transform.forward).GetPoint(0.5f), cam.transform.forward);
        else
            return new Ray(cam.transform.position, cam.transform.forward);
    }
    public Ray GetFurnitureMoveRay()
    {
        return new Ray(cam.transform.position, cam.transform.forward);
    }
    public Ray GetCameraSightRayOrigin()
    {
        Vector3 offset =
                (targetOffset.x * playerCont.transform.right) +
                (targetOffset.y * playerCont.transform.up) +
                (targetOffset.z * playerCont.transform.forward)
            ;

        return new Ray(playerCont.transform.position + offset, cam.transform.forward);
    }
    #endregion
}

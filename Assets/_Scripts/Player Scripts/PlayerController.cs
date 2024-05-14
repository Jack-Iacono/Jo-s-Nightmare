using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static List<PlayerController> playerInstances = new List<PlayerController>();

    public Animator anim;

    public CameraController camCont { get; private set; }

    [Header("Game Control Variables")]
    public Vector3 spawnLocation = Vector3.zero;
    public MeshRenderer meshOverride;
    public MeshRenderer meshRend { get; private set; }

    [Header("Movement Variables")]
    [SerializeField]
    private float moveSpeed = 10;
    [SerializeField]
    private float jumpHeight = 10;
    [SerializeField]
    [Tooltip("Negative values will pull player downward, Positive value will push them up")]
    private float gravity = -0.98f;

    [Header("Acceleration Variables", order = 2)]
    [SerializeField]
    private float groundAcceleration = 1;
    [SerializeField]
    private float airAcceleration = 1;
    [SerializeField]
    private float groundDeceleration = 1;
    [SerializeField]
    private float airDeceleration = 1;

    /*
    [Header("Movement Contstraints", order = 2)]
    [SerializeField]
    private float maxNormalSpeed = 10;
    [SerializeField]
    private float maxCapSpeed = 100;
    */

    [Header("Interaction Variables")]
    public LayerMask environmentLayers;

    private Vector3 currentInput = Vector3.zero;
    [SerializeField]
    private Vector3 currentMove = Vector3.zero;

    private Vector3 previousFramePosition = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    public const int RUBBER_BAND_MAX = 5;
    public int rubberBands { get; private set; } = RUBBER_BAND_MAX;

    #region Modifiers

    [Header("Player Modifiers")]
    public float bearStunTime;
    private float currentBearStunTime;
    public float bearStunSpeed;
    private float bearStunMod = 1;

    #endregion

    private CharacterController charCont;

    // Start is called before the first frame update
    void Start()
    {
        charCont = GetComponent<CharacterController>();
        camCont = GetComponentInChildren<CameraController>();

        spawnLocation = transform.position;

        if (meshOverride != null)
            meshRend = meshOverride;
        else
            meshRend = GetComponent<MeshRenderer>();

        playerInstances.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.isPaused)
        {
            GetInput();
            CalculateNormalPlayerMove();
            MovePlayer();
        }
    }
    private void FixedUpdate()
    {
        if (!GameController.isPaused)
        {
            velocity = (transform.position - previousFramePosition) / Time.fixedDeltaTime;
            previousFramePosition = transform.position;
        }
    }

    private void GetInput()
    {
        currentInput = new Vector3
            (
                Input.GetAxis("Horizontal"),
                Input.GetButtonDown("Jump") ? 1 : 0,
                Input.GetAxis("Vertical")
            );
    }
    private void CalculateNormalPlayerMove()
    {
        float moveX = currentInput.x * transform.right.x * moveSpeed + currentInput.z * transform.forward.x * moveSpeed;
        float moveZ = currentInput.x * transform.right.z * moveSpeed + currentInput.z * transform.forward.z * moveSpeed;

        if (charCont.isGrounded)
        {
            if (currentInput.y != 0)
            {
                anim.SetTrigger("jump");
                currentMove.y = jumpHeight;
            }

            float accelX = moveX == 0 ? groundDeceleration : groundAcceleration;
            float accelZ = moveZ == 0 ? groundDeceleration : groundAcceleration;

            currentMove.x = Mathf.MoveTowards(currentMove.x, moveX, accelX * Time.deltaTime);
            currentMove.z = Mathf.MoveTowards(currentMove.z, moveZ, accelZ * Time.deltaTime);
        }
        else
        {
            // Sets into fall if hitting a ceiling
            if (Physics.Raycast(transform.position, Vector3.up, 1.1f, environmentLayers) && currentMove.y > 0)
                currentMove.y = 0;

            currentMove.y -= gravity * -2 * Time.deltaTime;

            float accelX = moveX == 0 ? airDeceleration : airAcceleration;
            float accelZ = moveZ == 0 ? airDeceleration : airAcceleration;

            currentMove.x = Mathf.MoveTowards(currentMove.x, moveX, accelX * Time.deltaTime);
            currentMove.z = Mathf.MoveTowards(currentMove.z, moveZ, accelZ * Time.deltaTime);
        }

        // checks if the player has moved, for animator
        if (currentMove.x != 0 || currentMove.z != 0)
            anim.SetBool("isWalking", true);
        else
            anim.SetBool("isWalking", false);

        anim.SetFloat("speed", currentMove.z);
    }
    private void MovePlayer()
    {
        charCont.Move(currentMove * Time.deltaTime);
    }

    public void BearAttack()
    {
        //Take an attack from the bear
        StartCoroutine(BearAttackStun());
    }
    IEnumerator BearAttackStun()
    {
        bearStunMod = bearStunSpeed;

        yield return new WaitForSeconds(bearStunTime);

        bearStunMod = 1;
    }

    private void OnDestroy()
    {
        //Takes itself out of the player array
        playerInstances.Remove(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "death_barrier")
        {
            charCont.enabled = false;
            transform.position = spawnLocation;
            charCont.enabled = true;
        }
    }
}

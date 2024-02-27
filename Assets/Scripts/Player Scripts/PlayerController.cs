using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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

    [Header("Player Movement")]
    public float moveSpeed = 10;
    public float jumpHeight = 3;
    public float jumpBoost = 3;
    public float grav = -9.8f;
    public float accel = 10;
    public float decel = 10;

    private Vector3 currentMove = Vector3.zero;
    private Vector2 mStore = Vector2.zero;

    [Header("Player Attacks")]
    public LayerMask attackLayers;
    public float attackRange = 5;
    public Vector3 attackArea = Vector3.zero;

    public const float ATTACK_COOLDOWN = 0.5f;
    private float currentAttackCoolDown = ATTACK_COOLDOWN;

    [Space]
    public ParticleSystem shootParticle;
    public LayerMask shootCollideLayers;
    public float shootDistance;
    public const float SHOOT_COOLDOWN = 0.5f;
    private float currentShootCoolDown = SHOOT_COOLDOWN;
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
            Move();

            if (Input.GetKeyDown(ValueStoreController.keyBinds.keyHit) && currentAttackCoolDown >= ATTACK_COOLDOWN)
            {
                Swing();
                currentAttackCoolDown = 0;
            }
                
            if (Input.GetKeyDown(ValueStoreController.keyBinds.keyShoot) && currentShootCoolDown >= SHOOT_COOLDOWN)
            {
                Shoot();
                currentShootCoolDown = 0;
            }

            if (currentShootCoolDown < SHOOT_COOLDOWN)
                currentShootCoolDown += Time.deltaTime;
            if(currentAttackCoolDown < ATTACK_COOLDOWN)
                currentAttackCoolDown += Time.deltaTime;

            if (Input.GetKeyDown(ValueStoreController.keyBinds.keyDance))
                anim.SetTrigger("dance");
        }
    }

    private void Move()
    {
        //Gets the angle offset for third person movement
        float yOffset = transform.rotation.eulerAngles.y;

        //Gets the input for the X and Z axes
        int inputX = GetInputInt(ValueStoreController.keyBinds.keyRight) - GetInputInt(ValueStoreController.keyBinds.keyLeft);
        float inputZ = GetInputInt(ValueStoreController.keyBinds.keyForward) - GetInputInt(ValueStoreController.keyBinds.keyBackward);

        if (Input.GetKeyDown(ValueStoreController.keyBinds.keyForward))
            inputX = 1;

        if (inputX != 0)
            mStore.x = Mathf.MoveTowards(mStore.x, inputX, accel * Time.deltaTime);
        else
            mStore.x = Mathf.MoveTowards(mStore.x, inputX, decel * Time.deltaTime);

        if (inputZ != 0)
            mStore.y = Mathf.MoveTowards(mStore.y, inputZ, accel * Time.deltaTime);
        else
            mStore.y = Mathf.MoveTowards(mStore.y, inputZ, decel * Time.deltaTime);

        //Gets input for Jumping
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        //Converts the X and Z inputs into movement relative to the direction the player is facing
        currentMove.x = ((Mathf.Cos(-yOffset * Mathf.Deg2Rad) * mStore.x) + (Mathf.Sin(yOffset * Mathf.Deg2Rad) * mStore.y)) * moveSpeed * bearStunMod;
        currentMove.z = ((Mathf.Cos(yOffset * Mathf.Deg2Rad) * mStore.y) + (Mathf.Sin(-yOffset * Mathf.Deg2Rad) * mStore.x)) * moveSpeed * bearStunMod;

        //Checks Gravity
        if (!charCont.isGrounded)
        {
            if (currentMove.y > 0)
                currentMove.y += grav * Time.deltaTime;
            else
                currentMove.y += grav * 2 * Time.deltaTime;

            mStore.x = Mathf.MoveTowards(mStore.x, 0, decel * 0.5f * Time.deltaTime);
            mStore.y = Mathf.MoveTowards(mStore.y, 0, decel * 0.5f * Time.deltaTime);
        }

        // checks if the player has moved, for animator
        if (currentMove.x != 0 || currentMove.z != 0)
            anim.SetBool("isWalking", true);
        else
            anim.SetBool("isWalking", false);

        anim.SetFloat("speed", inputZ);

        //Moves the player
        charCont.Move(currentMove * Time.deltaTime);
    }
    private int GetInputInt(KeyCode key)
    {
        if (Input.GetKey(key))
            return 1;

        return 0;
    }
    private void Jump()
    {
        if (charCont.isGrounded)
        {
            anim.SetTrigger("jump");

            currentMove.y = jumpHeight;
            mStore.x *= jumpBoost;
            mStore.y *= jumpBoost;
        }
    }

    private void Swing()
    {
        anim.SetTrigger("swing");

        Ray ray = camCont.GetCameraSightRayOrigin();

        if (Physics.CheckBox(ray.GetPoint(attackRange / 2), attackArea, transform.rotation, attackLayers))
        {
            Collider[] col = Physics.OverlapBox(ray.GetPoint(attackRange / 2), attackArea, transform.rotation, attackLayers);
            foreach(Collider c in col)
            {
                c.SendMessage("PlayerDamageSwing", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    private void Shoot()
    {
        if(rubberBands > 0)
        {
            anim.SetTrigger("shoot");

            Ray ray = camCont.GetCameraSightRay();
            RaycastHit hit;

            shootParticle.Play();
            SoundManager.PlaySound(SoundManager.pop);

            if (Physics.Raycast(ray, out hit, shootDistance, shootCollideLayers))
            {
                if (MyFunctions.LayermaskContains(attackLayers, hit.collider.gameObject.layer))
                {
                    hit.collider.gameObject.SendMessage("PlayerDamageShoot", SendMessageOptions.DontRequireReceiver);
                }
            }

            if(GameController.currentPhase == GameController.NightPhase.ATTACK)
                rubberBands--;

            InterfaceController.Instance.HUDBand();
        }
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

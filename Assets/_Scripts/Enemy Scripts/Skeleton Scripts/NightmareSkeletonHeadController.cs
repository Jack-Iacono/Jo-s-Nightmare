using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NightmareSkeletonHeadController : MonoBehaviour
{
    private NightmareSkeletonController body;
    private float hopSpeed = 2f;
    private float currentHopTimer = -1;

    public Rigidbody rb { get; private set; }

    private Vector3 velStore = Vector3.zero;

    private bool isMoving;

    public void Initialize(NightmareSkeletonController b)
    {
        body = b;
        rb = GetComponent<Rigidbody>();
    }
    public void PlayerDamageSwing()
    {
        rb.AddForce(PlayerController.playerInstances[0].transform.forward * 10 + Vector3.up, ForceMode.Impulse);
        SoundManager.PlayRandomSound(SoundManager.skeletonWalking);
    }

    public void StopHead()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        rb.angularVelocity = Vector3.zero;

        isMoving = false;
    }
    public void StartHead()
    {
        gameObject.SetActive(true);
        rb.isKinematic = false;
        rb.AddForce(PlayerController.playerInstances[0].transform.forward * 10 + Vector3.up, ForceMode.Impulse);

        isMoving = true;
    }

    private void Update()
    {
        if (!GameController.isPaused)
        {
            if (isMoving)
            {
                //REstores Velocity after pause
                if (velStore != Vector3.zero)
                {
                    rb.velocity = velStore;
                    velStore = Vector3.zero;
                    rb.useGravity = true;
                }

                if (currentHopTimer > 0)
                {
                    currentHopTimer -= Time.deltaTime;
                }
                else
                {
                    rb.AddForce
                        (
                            (body.transform.position - transform.position).normalized * 2 + Vector3.up * 2,
                            ForceMode.Impulse
                        );
                    currentHopTimer = hopSpeed;
                }
            }
        }
        else
        {
            if (velStore == Vector3.zero)
            {
                velStore = rb.velocity;
                rb.velocity = Vector3.zero;
                rb.useGravity = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "death_barrier")
            transform.position = body.transform.position + body.transform.up * 2;
    }
}
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

    private void Initialize(NightmareSkeletonController b)
    {
        body = b;
        rb = GetComponent<Rigidbody>();
    }
    public void PlayerDamageSwing()
    {
        rb.AddForce(body.playerCont.transform.forward * 10 + Vector3.up, ForceMode.Impulse);
        SoundManager.PlayRandomSound(SoundManager.skeletonWalking);
    }
    public void Stop()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (!GameController.isPaused)
        {
            //REstores Velocity after pause
            if(velStore != Vector3.zero)
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

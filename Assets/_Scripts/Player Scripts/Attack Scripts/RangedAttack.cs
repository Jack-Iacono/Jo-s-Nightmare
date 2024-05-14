using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Attack/Ranged", order = 1)]
public class RangedAttack : Attack
{
    [Header("Ranged Attack Attributes")]
    public LayerMask collideLayers;
    public PooledObject shootParticle;

    public override void BeginAttack()
    {
        if (currentUseCount > 0)
        {
            base.BeginAttack();

            Ray ray = controller.playerCont.camCont.GetCameraSightRay();
            RaycastHit hit;

            //shootParticle.Play();
            GameObject particle = ObjectPool.Instance.GetPooledObject(shootParticle.obj.name);
            particle.transform.position = ray.origin;
            particle.transform.rotation = controller.playerCont.camCont.transform.rotation;
            particle.SetActive(true);
            particle.GetComponent<ParticleSystem>().Play();

            SoundManager.PlaySound(SoundManager.pop);

            if (Physics.Raycast(ray, out hit, attackRange, collideLayers))
            {
                if (MyFunctions.LayermaskContains(attackLayers, hit.collider.gameObject.layer))
                {
                    hit.collider.gameObject.SendMessage("PlayerDamageShoot", SendMessageOptions.DontRequireReceiver);
                }
            }

            if (GameController.currentPhase == GameController.NightPhase.ATTACK)
                currentUseCount--;

            InterfaceController.Instance.HUDBand();
        }
        else
        {
            EndAttack();
        }
    }
}

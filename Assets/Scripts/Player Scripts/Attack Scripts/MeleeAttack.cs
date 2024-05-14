using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Attack/Melee", order = 1)]
public class MeleeAttack : Attack
{
    [Header("Melee Attack Attributes")]
    public Vector3 attackArea;

    public override void BeginAttack()
    {
        base.BeginAttack();

        Ray ray = controller.playerCont.camCont.GetCameraSightRayOrigin();

        if (Physics.CheckBox(ray.GetPoint(attackRange / 2), attackArea, controller.transform.rotation, attackLayers))
        {
            Collider[] col = Physics.OverlapBox(ray.GetPoint(attackRange / 2), attackArea, controller.transform.rotation, attackLayers);
            foreach (Collider c in col)
            {
                c.SendMessage("PlayerDamageSwing", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}

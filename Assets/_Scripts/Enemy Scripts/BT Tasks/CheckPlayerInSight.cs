using BehaviorTree;
using UnityEngine;

public class CheckPlayerInSight : Node
{
    private Transform player;
    private Nightmare user;

    public CheckPlayerInSight(Nightmare user, Transform player)
    {
        this.player = player;
        this.user = user;
    }

    public override Status Check()
    {
        // Check if the player is close enough to the user
        if (Vector3.Distance(player.position, user.transform.position) <= user.fovRange)
        {
            RaycastHit hit;
            Ray ray = new Ray(user.transform.position, (player.position - user.transform.position).normalized);

            // Check if the player is within the vision arc
            if (Vector3.Dot(user.transform.forward, ray.direction) >= 0.8)
            {
                // Check if the player is behind any walls / obstructions
                if (Physics.Raycast(ray.origin, ray.direction, out hit, user.fovRange))
                {
                    if (hit.collider.tag == "Player")
                    {
                        parent.SetData("playerKnownPosition", player.position);

                        status = Status.SUCCESS;
                        return status;
                    }
                }
            }
        }

        // Check if there is still a known position
        if(GetData("playerKnownPosition") != null)
        {
            status = Status.SUCCESS;
            return status;
        }

        // If the enemy can't see the player and there is no known last position, then it is  a failure
        status = Status.FAILURE;
        return status;
    }

}

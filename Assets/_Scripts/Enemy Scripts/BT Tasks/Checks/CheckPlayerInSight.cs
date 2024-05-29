using BehaviorTree;
using UnityEngine;

public class CheckPlayerInSight : Node
{
    private Transform player;
    private Nightmare user;

    private float playerSightTime = 1f;
    private float playerSightTimer;

    private float fovRange;
    private float sightAngle;

    private const string SIGHT_KEY = "playerSightBuffer";

    public CheckPlayerInSight(Nightmare user, Transform player, float fovRange, float sightAngle)
    {
        this.player = player;
        this.user = user;

        this.fovRange = fovRange;
        this.sightAngle = sightAngle;

        playerSightTimer = playerSightTime;
    }

    public override Status Check()
    {
        if (GetData(SIGHT_KEY) == null)
            parent.parent.SetData(SIGHT_KEY, false);
            

        bool playerSightBuffer = (bool)GetData(SIGHT_KEY);

        // Decrement the timer for the player sight buffer
        if (playerSightBuffer)
        {
            playerSightTimer -= Time.deltaTime;

            // If the timer ends, stop the detection of the player
            if (playerSightTimer <= 0)
                SetPlayerSightBuffer(false);
        }

        // Check if the player is close enough to the user
        if (Vector3.Distance(player.position, user.transform.position) <= fovRange)
        {
            RaycastHit hit;
            Ray ray = new Ray(user.transform.position, (player.position - user.transform.position).normalized);

            // Check if the player is within the vision arc
            if (Vector3.Dot(user.transform.forward, ray.direction) >= sightAngle)
            {
                // Check if the player is behind any walls / obstructions
                if (Physics.Raycast(ray.origin, ray.direction, out hit, fovRange))
                {
                    if (hit.collider.tag == "Player")
                    {
                        SetPlayerSightBuffer(true);
                        playerSightTimer = playerSightTime;

                        SetPlayerPosition();

                        status = Status.SUCCESS;
                        return status;
                    }
                }
            }
        }

        // Check if the skeleton can still see player due to the buffer
        if (playerSightBuffer)
        {
            SetPlayerPosition();

            status = Status.SUCCESS;
            return status;
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

    public void SetPlayerPosition()
    {
        parent.parent.SetData("playerKnownPosition", player.position);
    }
    public void SetPlayerSightBuffer(bool b)
    {
        parent.parent.SetData(SIGHT_KEY, b);
    }

}

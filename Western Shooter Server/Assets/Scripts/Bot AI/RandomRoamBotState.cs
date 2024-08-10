using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomRoamBotState : BaseBotState
{
    float sincePathUpdate = 0f;
    float pathUpdateDelay = 5f;

    private List<Vector3> pathCorners = new List<Vector3>();

    private Vector3 currentCorner;

    float targetLookDelay = 1f;
    float sinceLookedForTarget = 0f;

    public RandomRoamBotState(BotStateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public override void EnterState()
    {
        UpdatePath();
    }

    public override void ExitState()
    {
        currentCorner = Vector3.zero;
        pathCorners.Clear();
    }

    public override void OnTick()
    {
        sincePathUpdate += TickManager.Singleton.TimeBetweenTicks;

        if (sincePathUpdate > pathUpdateDelay)
        {
            UpdatePath();
        }else
        {
            Pathfind();
            Debug.DrawLine(currentCorner, currentCorner + Vector3.up, Color.blue, 0.1f);
        }

        sinceLookedForTarget += TickManager.Singleton.TimeBetweenTicks;

        if (sinceLookedForTarget > targetLookDelay)
        {
            Player shootingTarget = FindShootingTarget();
            if (shootingTarget != null)
            {
                stateMachine.shootingState.target = shootingTarget;
                stateMachine.SwitchToState(stateMachine.shootingState);
            }
            sinceLookedForTarget = 0f;
        }
    }

    private Player FindShootingTarget()
    {
        Vector3 currentPos = stateMachine.transform.position;
        foreach (Player player in GameManager.Singleton.players.Values)
        {
            if (Physics.Linecast(currentPos, player.self.transform.position, out RaycastHit hit))
            {
                if (Vector3.Distance(hit.point, currentPos) <= 0.1f) continue;
                if (hit.collider.CompareTag("Player"))
                {
                    Vector3 direction = (hit.point - currentPos).normalized;
                    float angle = Vector3.Angle(direction, Quaternion.Euler(stateMachine.look) * Vector3.forward);
                    if (angle <= 85)
                    {
                        return player;
                    }
                }
            }
        }

        return null;
    }

    private void UpdatePath()
    {
        pathCorners.Clear();
        currentCorner = Vector3.zero;
        NavMeshPath path = new NavMeshPath();
        Vector3 currentPosition = stateMachine.transform.position;
        bool success = NavMesh.CalculatePath(currentPosition, currentPosition + new Vector3(Random.Range(-20f, 20f), 1f, Random.Range(-20f, 20f)), NavMesh.AllAreas, path);
        if (!success) return;
        pathCorners.AddRange(path.corners);

        sincePathUpdate = 0f;
    }

    private void Pathfind()
    {
        CheckForNextCorner();
        if (currentCorner != Vector3.zero)
        {
            Vector3 position = stateMachine.transform.position;
            Vector3 lookRot = Quaternion.LookRotation((currentCorner - position).normalized).eulerAngles;

            stateMachine.Look(lookRot);
            stateMachine.inputs[0] = true;
        }
    }

    private void CheckForNextCorner()
    {
        if (pathCorners.Count <= 0) return;

        Vector3 pos1 = stateMachine.transform.position;
        Vector3 pos2 = currentCorner;

        pos1.y = 0f;
        pos2.y = 0f;

        if (currentCorner == Vector3.zero || Vector3.Distance(pos1, pos2) <= 0.25f)
        {
            pathCorners.RemoveAt(0);
            if (pathCorners.Count > 0)
            {
                currentCorner = pathCorners[0];
            }else
            {
                currentCorner = Vector3.zero;
            }
        }
    }
}

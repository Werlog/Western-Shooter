using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomRoamBotState : BaseBotState
{
    float sincePathUpdate = 0f;
    float pathUpdateDelay = 5f;

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
        stateMachine.currentCorner = Vector3.zero;
        stateMachine.pathCorners.Clear();
    }

    public override void OnAttractAttention(AttentionAttractEventArgs e)
    {
        if (Vector3.Distance(stateMachine.transform.position, e.position) > 30f) return;
        if (Random.Range(0, 101) < 20)
        {
            Debug.Log($"{stateMachine.player.Username} found the sound at {e.position} very interesting");
            stateMachine.chasingState.target = e.position + new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
            stateMachine.SwitchToState(stateMachine.chasingState);
        }
    }

    public override void OnDamaged(PlayerDamagedEventArgs e)
    {
        if (e.damagedPlayer.Health > 0)
            stateMachine.LookForTargetAndShoot(150);
    }

    public override void OnTick()
    {
        sincePathUpdate += TickManager.Singleton.TimeBetweenTicks;

        if (sincePathUpdate > pathUpdateDelay)
        {
            UpdatePath();
        }else
        {
            stateMachine.Pathfind();
            if (stateMachine.currentCorner == Vector3.zero)
            {
                UpdatePath();
            }
            Debug.DrawLine(stateMachine.currentCorner, stateMachine.currentCorner + Vector3.up, Color.blue, 0.1f);
        }

        sinceLookedForTarget += TickManager.Singleton.TimeBetweenTicks;

        if (sinceLookedForTarget > targetLookDelay)
        {
            Player shootingTarget = stateMachine.FindShootingTarget();
            if (shootingTarget != null)
            {
                stateMachine.shootingState.target = shootingTarget;
                stateMachine.SwitchToState(stateMachine.shootingState);
            }
            sinceLookedForTarget = 0f;
        }
    }

    private void UpdatePath()
    {
        bool success = stateMachine.PathTo(stateMachine.transform.position + new Vector3(Random.Range(-20f, 20f), 1f, Random.Range(-20f, 20f)));
        if (!success) return;

        sincePathUpdate = 0f;
    }
}

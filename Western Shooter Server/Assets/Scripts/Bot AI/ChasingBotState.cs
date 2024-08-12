using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingBotState : BaseBotState
{
    public Vector3 target;

    private float lookForTargetDelay = 0.5f;

    private float sinceLookedForTarget = 0f;

    public ChasingBotState(BotStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        if (target == Vector3.zero)
        {
            stateMachine.SwitchToState(stateMachine.roamState);
            return;
        }

        stateMachine.PathTo(target);
    }

    public override void ExitState()
    {
        
    }

    public override void OnAttractAttention(AttentionAttractEventArgs e)
    {
        
    }

    public override void OnDamaged(PlayerDamagedEventArgs e)
    {
        if (e.damagedPlayer.Health > 0)
            stateMachine.LookForTargetAndShoot(150);
    }

    public override void OnTick()
    {
        stateMachine.Pathfind();

        sinceLookedForTarget += TickManager.Singleton.TimeBetweenTicks;

        if (sinceLookedForTarget >= lookForTargetDelay)
        {
            Player targetPlayer = stateMachine.FindShootingTarget(95);
            if (targetPlayer != null) 
            {
                stateMachine.shootingState.target = targetPlayer;
                stateMachine.SwitchToState(stateMachine.shootingState);
                return;
            }
            sinceLookedForTarget = 0f;
        }

        if (stateMachine.pathCorners.Count <= 0)
        {
            stateMachine.SwitchToState(stateMachine.roamState);
        }
    }
}

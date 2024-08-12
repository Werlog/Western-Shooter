using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBotState
{
    protected BotStateMachine stateMachine;

    public BaseBotState(BotStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void OnTick();
    public abstract void OnDamaged(PlayerDamagedEventArgs e);
    public abstract void OnAttractAttention(AttentionAttractEventArgs e);
}

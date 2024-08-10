using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShootingBotState : BaseBotState
{
    public Player target;

    private PlayerItemHandler itemHandler;

    private float switchDirDelay = 0.2f;

    private float sinceSwitchedDir = 0f;

    private float sinceStarted = 0f;
    private float shootDelay = 1f;

    private float inaccuracy = 5f;

    bool currentDir = true;

    public ShootingBotState(BotStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        if (itemHandler == null)
        {
            itemHandler = stateMachine.GetComponent<PlayerItemHandler>();
        }
    }

    public override void ExitState()
    {
        sinceSwitchedDir = 0f;
        sinceStarted = 0f;
    }

    public override void OnTick()
    {
        if (!LookForTarget())
        {
            stateMachine.SwitchToState(stateMachine.roamState);
            return;
        }

        sinceSwitchedDir += TickManager.Singleton.TimeBetweenTicks;
        
        if (sinceSwitchedDir > switchDirDelay)
        {
            currentDir = !currentDir;
            sinceSwitchedDir -= switchDirDelay;
        }

        if (currentDir)
            stateMachine.inputs[1] = true;
        else
            stateMachine.inputs[3] = true;

        sinceStarted += TickManager.Singleton.TimeBetweenTicks;

        Vector3 lookRot = Quaternion.LookRotation((target.self.transform.position - stateMachine.transform.position).normalized).eulerAngles;
        lookRot.x += Random.Range(-inaccuracy * 0.5f, inaccuracy * 0.5f);
        lookRot.y += Random.Range(-inaccuracy * 0.5f, inaccuracy * 0.5f);
        lookRot.z += Random.Range(-inaccuracy * 0.5f, inaccuracy * 0.5f);
        stateMachine.Look(lookRot);

        if (sinceStarted > shootDelay)
            itemHandler.currentHeldObject?.OnAction(HeldObjectAction.ATTACK);
    }

    public bool LookForTarget()
    {
        if (target.self == null) return false;
        if (!target.IsAlive) return false;

        if (Physics.Linecast(stateMachine.transform.position, target.self.transform.position, out RaycastHit hit))
        {
            if (!hit.collider.CompareTag("Player"))
                return false;
        }

        return true;
    }
}

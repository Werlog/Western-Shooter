using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotStateMachine : MonoBehaviour
{
    [HideInInspector]
    public PlayerMovement movement;

    public IdleBotState idleState;
    public RandomRoamBotState roamState;

    public bool[] inputs;
    public Vector3 orientation;
    public Vector3 look;

    private uint requestNumber;

    private BaseBotState activeState;

    private void Awake()
    {
        inputs = new bool[5];
        orientation = Vector3.zero;
        look = Vector3.zero;

        movement = GetComponent<PlayerMovement>();

        idleState = new IdleBotState(this);
        roamState = new RandomRoamBotState(this);

        TickManager.Singleton.TickEventHandler += OnTick;

        SwitchToState(roamState);
    }

    private void OnDestroy()
    {
        TickManager.Singleton.TickEventHandler -= OnTick;
    }

    private void OnTick(object sender, TickEventArgs e)
    {
        if (!enabled) return;

        activeState?.OnTick();

        movement.ReceiveInputs(new ClientInputs((bool[])inputs.Clone(), orientation, look, requestNumber));
        requestNumber++;
        ClearInputs();
    }

    public void Look(Vector3 angles)
    {
        look = angles;
        orientation = new Vector3(0f, angles.y, 0f);
    }

    private void ClearInputs()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = false;
        }
    }

    public void SwitchToState(BaseBotState state)
    {
        activeState?.ExitState();

        activeState = state;
        activeState?.EnterState();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotStateMachine : MonoBehaviour
{
    [HideInInspector]
    public PlayerMovement movement;

    public IdleBotState idleState;
    public RandomRoamBotState roamState;
    public ShootingBotState shootingState;
    public ChasingBotState chasingState;

    private BaseBotState activeState;

    public bool[] inputs;
    public Vector3 orientation;
    public Vector3 look;
    private uint requestNumber;

    public Player player;

    [HideInInspector]
    public List<Vector3> pathCorners = new List<Vector3>();
    [HideInInspector]
    public Vector3 currentCorner;

    // Used for things that should capture the bot's attention, for example gunshots in the distance
    // so it goes there to investigate, etc.
    public delegate void AttentionAttractEvent(object sender, AttentionAttractEventArgs e);

    public static event AttentionAttractEvent AttentionEvent;

    private void Awake()
    {
        inputs = new bool[5];
        orientation = Vector3.zero;
        look = Vector3.zero;

        movement = GetComponent<PlayerMovement>();

        idleState = new IdleBotState(this);
        roamState = new RandomRoamBotState(this);
        shootingState = new ShootingBotState(this);
        chasingState = new ChasingBotState(this);

        TickManager.Singleton.TickEventHandler += OnTick;

        SwitchToState(roamState);
    }

    private void Start()
    {
        player = movement.player;

        Player.DamagedEvent += OnDamaged;
        AttentionEvent += OnAttention;
    }

    private void OnDestroy()
    {
        TickManager.Singleton.TickEventHandler -= OnTick;
        AttentionEvent -= OnAttention;
    }

    private void OnTick(object sender, TickEventArgs e)
    {
        if (!gameObject.activeSelf || !enabled) return;

        activeState?.OnTick();

        movement.ReceiveInputs(new ClientInputs((bool[])inputs.Clone(), orientation, look, requestNumber));
        requestNumber++;
        ClearInputs();
    }

    private void OnDamaged(object sender, PlayerDamagedEventArgs e)
    {
        if (e.damagedPlayer.PlayerID == player.PlayerID)
        {
            activeState.OnDamaged(e);
        }
    }

    private void OnAttention(object sender, AttentionAttractEventArgs e)
    {
        if (!player.IsAlive) return;
        activeState.OnAttractAttention(e);
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

    public bool PathTo(Vector3 position)
    {
        pathCorners.Clear();
        currentCorner = Vector3.zero;

        NavMeshPath path = new NavMeshPath();

        bool success = NavMesh.CalculatePath(transform.position, position, NavMesh.AllAreas, path);
        if (!success) return false;

        pathCorners.AddRange(path.corners);

        return true;
    }

    public void Pathfind()
    {
        CheckForNextCorner();
        if (currentCorner != Vector3.zero)
        {
            Vector3 position = transform.position;
            Vector3 lookRot = Quaternion.LookRotation((currentCorner - position).normalized).eulerAngles;

            Look(lookRot);
            inputs[0] = true;
        }
    }

    private void CheckForNextCorner()
    {
        if (pathCorners.Count <= 0) return;

        Vector3 pos1 = transform.position;
        Vector3 pos2 = currentCorner;

        pos1.y = 0f;
        pos2.y = 0f;

        if (currentCorner == Vector3.zero || Vector3.Distance(pos1, pos2) <= 0.25f)
        {
            pathCorners.RemoveAt(0);
            if (pathCorners.Count > 0)
            {
                currentCorner = pathCorners[0];
            }
            else
            {
                currentCorner = Vector3.zero;
            }
        }
    }

    public Player FindShootingTarget(float maxAngle = 85)
    {
        Vector3 currentPos = transform.position;
        foreach (Player player in GameManager.Singleton.players.Values)
        {
            if (Physics.Linecast(currentPos + Vector3.up * 0.7f, player.self.transform.position, out RaycastHit hit))
            {
                if (Vector3.Distance(hit.point, currentPos) <= 0.1f) continue;
                if (hit.collider.CompareTag("Player"))
                {
                    Vector3 direction = (hit.point - currentPos).normalized;
                    float angle = Vector3.Angle(direction, Quaternion.Euler(look) * Vector3.forward);
                    if (angle <= maxAngle)
                    {
                        return player;
                    }
                }
            }
        }

        return null;
    }

    public void LookForTargetAndShoot(float maxAngle = 85)
    {
        Player target = FindShootingTarget(maxAngle);

        if (target != null)
        {
            shootingState.target = target;
            SwitchToState(shootingState);
        }
    }

    public static void TriggerEvent(Vector3 position)
    {
        AttentionEvent?.Invoke(null, new AttentionAttractEventArgs(position));
    }
}
public class AttentionAttractEventArgs : EventArgs
{
    public readonly Vector3 position;

    public AttentionAttractEventArgs(Vector3 position)
    {
        this.position = position;
    }
}
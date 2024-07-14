using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 15f;
    [SerializeField] private float midAirMultiplier = 0.1f;
    [SerializeField] private float maxSlopeAngle = 45f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 100f;

    [Header("Drag")]
    [SerializeField] private float groundDrag = 3f;
    [SerializeField] private float airDrag = 2.5f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask;
    public float playerHeight = 2f;
    [SerializeField] private float groundCheckRadius = 0.1f;

    private Vector3 inputDirection;

    private bool jumped;

    public bool IsGrounded { get; private set; }
    public bool IsOnSlope { get; private set; }
    public RaycastHit SlopeHit { get; private set; }

    private Queue<ClientInputs> inputQueue;

    private int inputFrequency = 0;

    private Rigidbody rb;

    private bool jumpRequest = false;

    private Vector3 savedVelocity;

    public Player player;

    private bool[] inputs;

    private uint currentRequestNumber;

    private LowerAnimation animationId;

    void Start()
    {
        inputQueue = new Queue<ClientInputs>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        inputs = new bool[5];

        TickManager.Singleton.TickEventHandler += OnTick;
    }

    private void OnDestroy()
    {
        TickManager.Singleton.TickEventHandler -= OnTick;
    }

    public void OnTick(object sender, TickEventArgs e)
    {
        inputFrequency--;
        while (inputQueue.Count > 0 && inputFrequency <= 0)
        {
            SetupInputs();
            UpdateInputDirection();

            CheckSlope();
            CheckGround();
            ControlDrag();
            Movement();
            LimitVelocity();

            SendPosition();
        }
    }

    private void SetupInputs()
    {
        ClientInputs curInputs = inputQueue.Dequeue();
        SetInputs(curInputs.Inputs);
        orientation.rotation = Quaternion.Euler(curInputs.Orientation);
        currentRequestNumber = curInputs.RequestNumber;
        inputFrequency++;

        if (inputQueue.Count > 100 && inputFrequency >= 0)
        {
            NetworkManager.Singleton.Server.DisconnectClient(player.PlayerID);
        }
    }

    public void ReceiveInputs(ClientInputs newInputs)
    {
        inputQueue.Enqueue(newInputs);
    }

    private void SetInputs(bool[] newInputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = newInputs[i];
        }
    }

    private void UpdateInputDirection()
    {
        inputDirection = Vector3.zero;

        if (inputs[0])
        {
            inputDirection.z += 1f;
        }
        if (inputs[1])
        {
            inputDirection.x -= 1f;
        }
        if (inputs[2])
        {
            inputDirection.z -= 1f;
        }
        if (inputs[3])
        {
            inputDirection.x += 1f;
        }

        jumpRequest = inputs[4];
    }

    private void Movement()
    {
        rb.isKinematic = false;
        rb.velocity = savedVelocity;

        if (IsGrounded)
        {
            if (inputDirection.x != 0f || inputDirection.z != 0)
            {
                animationId = LowerAnimation.Walking;
            }
            else animationId = LowerAnimation.Idle;
        }
        else animationId = LowerAnimation.Falling;

        Vector3 movementDirection = orientation.right * inputDirection.x + orientation.forward * inputDirection.z;
        if (IsOnSlope)
        {
            movementDirection = Vector3.ProjectOnPlane(movementDirection, SlopeHit.normal);
        }
        if (IsGrounded) movementDirection.Normalize();
        movementDirection *= movementSpeed * 5f * (IsGrounded ? 1f : midAirMultiplier);

        if (IsGrounded && !jumped && jumpRequest)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            jumped = true;
            jumpRequest = false;
        }

        rb.useGravity = !IsOnSlope;

        rb.AddForce(movementDirection, ForceMode.Force);

        Physics.Simulate(TickManager.Singleton.TimeBetweenTicks);
        savedVelocity = rb.velocity;
        rb.isKinematic = true;
    }

    private void SendPosition()
    {
        if (TickManager.Singleton.CurrentTick % 2 == 1) return;

        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClient.playerPosition);
        message.AddUShort(player.PlayerID);
        message.AddUInt(currentRequestNumber);
        message.AddUInt(TickManager.Singleton.CurrentTick);
        message.AddVector3(transform.position);
        message.AddVector3(orientation.eulerAngles);
        message.AddVector3(savedVelocity);
        message.AddByte((byte)animationId);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    private void CheckSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, playerHeight * 0.5f + 0.2f, groundMask))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            IsOnSlope = angle <= maxSlopeAngle && angle != 0;
            SlopeHit = hit;
            return;
        }
        IsOnSlope = false;
    }

    private void CheckGround()
    {
        Vector3 checkLoc = transform.position + Vector3.down * (playerHeight / 2f);
        IsGrounded = Physics.CheckBox(checkLoc, new Vector3(groundCheckRadius, 0.2f, groundCheckRadius), Quaternion.identity, groundMask);
        if (IsGrounded) jumped = false;
    }

    private void LimitVelocity()
    {
        if (IsOnSlope && rb.velocity.magnitude > movementSpeed)
        {
            rb.velocity = rb.velocity.normalized * movementSpeed;
            return;
        }

        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (horizontalVelocity.magnitude > movementSpeed)
        {
            Vector3 maxVel = horizontalVelocity.normalized * movementSpeed;
            rb.velocity = new Vector3(maxVel.x, rb.velocity.y, maxVel.z);
        }
    }

    private void ControlDrag()
    {
        rb.drag = IsGrounded ? groundDrag : airDrag;
    }
}
public enum LowerAnimation : byte
{
    Idle = 0,
    Walking = 1,
    Falling = 2,
}
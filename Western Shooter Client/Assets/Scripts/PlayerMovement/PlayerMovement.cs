using Riptide;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Client-Side Prediction")]
    [SerializeField] private Transform orientationRewind;
    [SerializeField] private float positionDistanceTolerance = 0.02f;
    [SerializeField] private float velocityDifferenceTolerance = 0.02f;

    private Vector3 inputDirection;

    private bool jumped;

    public bool IsGrounded { get; private set; }
    public bool IsOnSlope { get; private set; }
    public RaycastHit SlopeHit { get; private set; }

    private Rigidbody rb;

    private bool jumpRequest = false;

    public Vector3 SavedVelocity { get; private set; }

    private bool[] inputs;

    private Dictionary<uint, PredictedState> predictedPositions;

    private uint requestNumber = 0;

    void Start()
    {
        predictedPositions = new Dictionary<uint, PredictedState>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        inputs = new bool[5];

        TickManager.Singleton.TickEventHandler += OnTick;
    }

    private void OnDestroy()
    {
        TickManager.Singleton.TickEventHandler -= OnTick;
    }

    void Update()
    {
        GetInputs();
    }

    public void OnTick(object sender, TickEventArgs e)
    {
        SendInputs();

        CheckSlope();
        CheckGround();
        ControlDrag();
        PredictedState state = Movement(inputs, null);
        predictedPositions.Add(requestNumber, state);
        LimitVelocity();

        ClearInputs();
        requestNumber++;
    }

    private void GetInputs()
    {
        if (Input.GetKey(KeyCode.W))
        {
            inputs[0] = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputs[1] = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputs[2] = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputs[3] = true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            inputs[4] = true;
        }
    }

    private void ClearInputs()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = false;
        }
    }

    private Vector3 ConvertInputs(bool[] inputs)
    {
        Vector3 converted = Vector3.zero;

        if (inputs[0])
        {
            converted.z += 1f;
        }
        if (inputs[1])
        {
            converted.x -= 1f;
        }
        if (inputs[2])
        {
            converted.z -= 1f;
        }
        if (inputs[3])
        {
            converted.x += 1f;
        }

        return converted;
    }

    private PredictedState Movement(bool[] withInputs, PredictedState rewindedState)
    {
        inputDirection = ConvertInputs(withInputs);
        jumpRequest = withInputs[4];

        rb.isKinematic = false;
        rb.velocity = SavedVelocity;

        Transform orientation = this.orientation;
        if (rewindedState != null)
        {
            orientationRewind.eulerAngles = rewindedState.Orientation;
            orientation = orientationRewind;
        }

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
        }

        rb.useGravity = !IsOnSlope;

        rb.AddForce(movementDirection, ForceMode.Force);

        Physics.Simulate(TickManager.Singleton.TimeBetweenTicks);
        SavedVelocity = rb.velocity;
        rb.isKinematic = true;

        PredictedState state = new PredictedState(withInputs, requestNumber, orientation.eulerAngles, transform.position, SavedVelocity);

        return state;
    }

    private void SendInputs()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ClientToServer.inputs);
        message.AddUInt(requestNumber);
        message.AddVector3(orientation.eulerAngles);
        message.AddBools(inputs, false);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void CheckPrediction(uint curRequestNumber, Vector3 correctPosition, Vector3 correctVelocity)
    {
        if (predictedPositions.TryGetValue(curRequestNumber, out PredictedState predicted))
        {
            float positionDiff = Vector3.Distance(predicted.predictedPosition, correctPosition);
            float velocityDiff = Vector3.Distance(predicted.predictedVelocity, correctVelocity);

            if (positionDiff > positionDistanceTolerance)
            {
                Debug.Log($"Position prediction incorrect: Expected: {predicted.predictedPosition}, Got: {correctPosition}, Distance: {positionDiff}");
                Reconciliate(curRequestNumber, correctPosition, correctVelocity);
            }else if (velocityDiff > velocityDifferenceTolerance)
            {
                Debug.Log($"Velocity prediction incorrect: Expected: {predicted.predictedPosition}, Got: {correctPosition}, Distance: {positionDiff}");
                Reconciliate(curRequestNumber, correctPosition, correctVelocity);
            }
            DiscardUselessPastStates(curRequestNumber);
        }
    }

    private void Reconciliate(uint fromRequestNumber, Vector3 correctPosition, Vector3 correctVelocity)
    {
        if (predictedPositions.TryGetValue(fromRequestNumber, out PredictedState predicted))
        {
            transform.position = correctPosition;

            predicted.predictedPosition = correctPosition;
            predicted.predictedVelocity = correctVelocity;

            SavedVelocity = correctVelocity;

            for (uint i = fromRequestNumber + 1; i < requestNumber; i++)
            {
                if (!predictedPositions.ContainsKey(i))
                {
                    Debug.LogWarning($"Reconciliation Failed: Cannot find a predicted position with the request number {i}");
                    break;
                }

                PredictedState prevState = predictedPositions[i];

                PredictedState newlyPredicted = Movement(prevState.Inputs, prevState);

                prevState.predictedPosition = newlyPredicted.predictedPosition;
                prevState.predictedVelocity = newlyPredicted.predictedVelocity;
            }
        }
    }


    private void DiscardUselessPastStates(uint curRequestNumber)
    {
        /*
        List<uint> positionsToRemove = predictedPositions
            .Where(pair => pair.Key <= requestNumber)
            .Select(pair => pair.Key).ToList();

        foreach (uint pastPosition in positionsToRemove)
        {
            predictedPositions.Remove(pastPosition);
        }
        */

        Dictionary<uint, PredictedState> keepStates = new Dictionary<uint, PredictedState>();
        foreach (uint i in predictedPositions.Keys)
        {
            if (i > curRequestNumber)
            {
                keepStates.Add(i, predictedPositions[i]);
            }
        }

        predictedPositions.Clear();
        predictedPositions = keepStates;
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

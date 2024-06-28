using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictedState
{
    public bool[] Inputs { get; private set; }
    public uint RequestNumber { get; private set; }
    public Vector3 Orientation { get; private set; }
    public Vector3 predictedPosition;
    public Vector3 predictedVelocity;

    public PredictedState(bool[] inputs, uint requestNumber, Vector3 orientation, Vector3 predictedPosition, Vector3 predictedVelocity)
    {
        Inputs = inputs;
        Orientation = orientation;
        RequestNumber = requestNumber;
        this.predictedPosition = predictedPosition;
        this.predictedVelocity = predictedVelocity;
    }
}

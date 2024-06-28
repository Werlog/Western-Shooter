using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientInputs
{
    public bool[] Inputs { get; private set; }
    public Vector3 Orientation { get; private set; }
    public uint RequestNumber { get; private set; }

    public ClientInputs(bool[] inputs, Vector3 orientation, uint requestNumber)
    {
        Inputs = inputs;
        Orientation = orientation;
        RequestNumber = requestNumber;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class RagdollSimulator : MonoBehaviour
{
    private Rigidbody[] rigidbodies;
    private Vector3[] velocities;

    public void OnTick(object sender, TickEventArgs e)
    {
        LoadSavedVelocities();
        SetRigidbodiesKinematic(false);
        Physics.Simulate(TickManager.Singleton.TimeBetweenTicks);
        SaveVelocities();
        SetRigidbodiesKinematic(true);
    }

    private void Awake()
    {
        GetRigidbodies();
    }

    private void Start()
    {
        TickManager.Singleton.TickEventHandler += OnTick;
    }

    private void OnDestroy()
    {
        TickManager.Singleton.TickEventHandler -= OnTick;
    }

    private void SetRigidbodiesKinematic(bool kinematic)
    {
        foreach (Rigidbody body in rigidbodies)
        {
            body.isKinematic = kinematic;
        }
    }

    private void LoadSavedVelocities()
    {
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].velocity = velocities[i];
        }
    }

    private void SaveVelocities()
    {
        for (int i = 0; i < velocities.Length; i++)
        {
            velocities[i] = rigidbodies[i].velocity;
        }
    }

    public void AddRagdollForce(Vector3 force)
    {
        for (int i = 0;i < velocities.Length; i++)
        {
            velocities[i] = velocities[i] + force;
        }
    }

    private void GetRigidbodies()
    {
        rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
        velocities = new Vector3[rigidbodies.Length];

        for (int i = 0; i < velocities.Length; i++)
        {
            velocities[i] = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
        }
    }
}

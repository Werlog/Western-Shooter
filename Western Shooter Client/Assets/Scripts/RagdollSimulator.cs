using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class RagdollSimulator : MonoBehaviour
{
    private Rigidbody[] rigidbodies;
    private Vector3[] velocities;

    float randomForceDelay = 5f;

    float sinceRandomForce = 0f;

    public void OnTick(object sender, TickEventArgs e)
    {
        LoadSavedVelocities();
        SetRigidbodiesKinematic(false);
        Physics.Simulate(TickManager.Singleton.TimeBetweenTicks);
        SaveVelocities();
        SetRigidbodiesKinematic(true);
    }

    private void Start()
    {
        GetRigidbodies();
        Debug.Log(rigidbodies.Length);
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

    private void Update()
    {
        sinceRandomForce += Time.deltaTime;
        if (sinceRandomForce > randomForceDelay)
        {
            Vector3 force = new Vector3(Random.Range(-3f, 3f), 10f, Random.Range(-3f, 3f));
            AddRagdollForce(force);

            sinceRandomForce = 0f;
        }
    }

    private void GetRigidbodies()
    {
        rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
        velocities = new Vector3[rigidbodies.Length];
    }
}

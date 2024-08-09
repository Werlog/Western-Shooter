using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class RagdollSimulator : MonoBehaviour
{
    [SerializeField] private Rigidbody body1;
    [SerializeField] private Rigidbody body2;


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

    public void AddRagdollForce(Vector3 force, bool increasedToBody = false)
    {
        for (int i = 0; i < velocities.Length; i++)
        {
            if (increasedToBody && (rigidbodies[i] == body1 || rigidbodies[i] == body2))
            {
                velocities[i] = velocities[i] + force * 2;
                continue;
            }

            velocities[i] = velocities[i] + force;
        }
    }

    private void GetRigidbodies()
    {
        rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
        velocities = new Vector3[rigidbodies.Length];
    }
}

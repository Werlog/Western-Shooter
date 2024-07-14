using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Interpolator : MonoBehaviour
{
    [SerializeField] private float movementThreshold = 0.05f;
    [SerializeField] private RemotePlayerAnimationController animationController;

    private List<TransformUpdate> transformUpdates;
    private TransformUpdate from;
    private TransformUpdate to;

    private float timeElapsed = 0f;

    void Awake()
    {
        from = new TransformUpdate(TickManager.Singleton.CurrentTick, transform.position, LowerAnimation.Idle);
        to = new TransformUpdate(TickManager.Singleton.CurrentTick + 2, transform.position, LowerAnimation.Idle);
        transformUpdates = new List<TransformUpdate>();
    }


    void Update()
    {
        float percentage = timeElapsed / (TickManager.Singleton.TimeBetweenPositionUpdates * 0.95f);
        if (to != null)
            InterpolatePosition(percentage);
        if (percentage >= 1f)
        {
            transformUpdates.Remove(to);
            if (to != null)
                from = to;
            to = GetOldestTransformUpdate();
            if (to != null) 
            {
                animationController.SetLowerBodyAnimation(to.Animation);
                timeElapsed = 0f;
            }
        }
        timeElapsed += Time.deltaTime;
    }

    public void NewUpdate(TransformUpdate update)
    {
        if (update.Tick <= TickManager.Singleton.InterpolationTick) return;
        TransformUpdate newest = GetNewestTransformUpdate();
        if (newest != null)
        {
            float dist = Vector3.Distance(update.Position, newest.Position);
            if (dist < movementThreshold) return;
        }
        for (int i = 0; i < transformUpdates.Count; i++)
        {
            if (transformUpdates[i].Tick < update.Tick)
            {
                transformUpdates.Insert(i, update);
                return;
            }
        }

        transformUpdates.Add(update);
    }

    private void InterpolatePosition(float percentage)
    {
        transform.position = Vector3.Lerp(from.Position, to.Position, percentage);
    }

    public TransformUpdate GetOldestTransformUpdate()
    {
        uint oldestTick = uint.MaxValue;
        TransformUpdate update = null;
        for (int i = 0; i < transformUpdates.Count; i++)
        {
            if (transformUpdates[i].Tick < oldestTick)
            {
                oldestTick = transformUpdates[i].Tick;
                update = transformUpdates[i];
            }
        }

        return update;
    }

    public TransformUpdate GetNewestTransformUpdate()
    {
        uint newestTick = uint.MinValue;
        TransformUpdate update = null;
        for (int i = 0; i < transformUpdates.Count; i++)
        {
            if (transformUpdates[i].Tick > newestTick)
            {
                newestTick = transformUpdates[i].Tick;
                update = transformUpdates[i];
            }
        }

        return update;
    }
}

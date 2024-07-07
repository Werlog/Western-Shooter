using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewModelSway : MonoBehaviour
{
    [Header("Position")]
    public float intesity;
    public float intesityX;
    public float speed;
    [SerializeField] private PlayerMovement movement;
    public float returnDuration = 0.2f;

    public Transform targetTransform;
    private Vector3 offset;
    private Vector3 originalOffset;
    private float sinTime;
    private float elapsedDuration;
    private Vector3 lastOffsetPosition;
    private float sinceMoved;

    [Header("Velocity Offset")]
    [SerializeField] private float velocityOffsetSpeed = 5f;
    [SerializeField] private float velocityOffsetIntensity = 0.1f;

    [Header("Rotation")]
    public float rotationSwayMultiplier;
    public float smooth;

    [Header("Crouching")]
    public float crouchSwayMultiplier = 0.5f;
    [HideInInspector]
    public bool crouching;

    private float curIntensity;
    private Vector3 velocityOffset;

    private void Awake()
    {
        originalOffset = transform.localPosition;
    }

    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Vertical"), 0f, Input.GetAxisRaw("Horizontal"));
        if (input.magnitude > 0 && movement.IsGrounded)
        {
            if (sinTime == 0)
            {
                sinTime += 0.05f;
            }
            sinTime += crouching ? Time.deltaTime * speed * crouchSwayMultiplier : Time.deltaTime * speed;
            sinceMoved = 0f;
        }
        else
        {
            sinceMoved += Time.deltaTime;
            if (sinceMoved >= 0.15f)
            {
                sinTime = 0f;
                curIntensity = 0f;
            }
        }

        if (sinTime != 0)
        {
            float percentage = curIntensity / intesity;
            if (percentage < 0.95f)
            {
                curIntensity = Mathf.Lerp(curIntensity, intesity, Time.deltaTime * 5);
            }


            float sinAmountY = Mathf.Abs(curIntensity * Mathf.Sin(sinTime));
            Vector3 sinAmountX = transform.right * curIntensity * Mathf.Cos(sinTime) * intesityX;
            offset = new Vector3(originalOffset.x, originalOffset.y + (crouching ? sinAmountY * crouchSwayMultiplier : sinAmountY), originalOffset.z);
            offset += crouching ? sinAmountX * crouchSwayMultiplier : sinAmountX;

            lastOffsetPosition = offset;
            elapsedDuration = 0f;
        }
        else
        {
            float percentage = elapsedDuration / returnDuration;
            if (percentage < 1)
            {
                offset = Vector3.Lerp(lastOffsetPosition, Vector3.zero, percentage);
                elapsedDuration += Time.deltaTime;
            }
        }

        // Velocity
        Vector3 direction = -movement.SavedVelocity.normalized;
        direction *= velocityOffsetIntensity;
        velocityOffset = Vector3.Lerp(velocityOffset, direction, Time.deltaTime * velocityOffsetSpeed);


        transform.position = targetTransform.position + offset + velocityOffset;
        // Rotation
        float mouseX = Input.GetAxis("Mouse X") * rotationSwayMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSwayMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}

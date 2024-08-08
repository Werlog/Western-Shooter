using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCamera : MonoBehaviour
{
    [Header("Fly Speed Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float minSpeed = 5f;
    [SerializeField] private float maxSpeed = 40f;
    [SerializeField] private float scrollSpeed = 2f;

    void Update()
    {
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        inputDirection.Normalize();

        Vector3 movementDirection = transform.right * inputDirection.x + transform.forward * inputDirection.z;
        movementDirection *= speed * Time.deltaTime;

        transform.position += movementDirection;

        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (scroll != 0)
        {
            speed += scroll * scrollSpeed;

            speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCameraLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;

    float xRotation = 0f;
    float yRotation = 0f;

    private void Awake()
    {
        xRotation = transform.localRotation.eulerAngles.x;
        yRotation = transform.localRotation.eulerAngles.y;
    }

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}

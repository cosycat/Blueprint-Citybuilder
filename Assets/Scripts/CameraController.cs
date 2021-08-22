using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Camera _thisCamera;
    private Vector3 _currentMovement = new Vector3();

    public float cameraSpeed = 1;
    public float zoomSpeed = 1;
    public float maxZoom = 1;
    public float minZoom = 10;


    private void Awake()
    {
        _thisCamera = GetComponent<Camera>();
    }


    public void OnLook(InputAction.CallbackContext context)
    {
        _currentMovement = context.ReadValue<Vector2>();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        // Debug.Log(context.ReadValue<float>());
        float newZoom = _thisCamera.orthographicSize + -context.ReadValue<float>() * zoomSpeed;
        _thisCamera.orthographicSize = Mathf.Clamp(newZoom, maxZoom, minZoom);
    }

    private void FixedUpdate()
    {
        if (_currentMovement.magnitude > 0)
        {
            transform.position += _currentMovement * Time.fixedDeltaTime * cameraSpeed; // TODO somehow also factor in zoom level (faster on wider zoom)
            // Debug.Log("Looking around!");
            Debug.Log(_currentMovement);
        }
    }
}
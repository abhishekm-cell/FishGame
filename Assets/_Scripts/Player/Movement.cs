using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float minY, maxY;
    [SerializeField] private float maxRotation = 25f;
    [SerializeField] private float rotationSmooth = 10f;

    private float targetRotation;
    [SerializeField] private float offset;
    [SerializeField] private float swipeOffset;

    [Header("Swipe Settings")]
    [SerializeField] private float swipeSensitivity = 0.01f;

    private Vector2 startTouchPos;
    private bool isSwiping;

    private void Start()
    {
        Camera camera = Camera.main;
        float camHalfH = camera.orthographicSize;

        minY = camera.transform.position.y - camHalfH;
        maxY = camera.transform.position.y + camHalfH;
    }
    
    void Update()
    {
        HandleSwipe();
        ApplyRotation();
    }

    void HandleSwipe()
    {
        if (Input.touchCount == 0)
        {
            isSwiping = false;
            targetRotation = 0f;
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            startTouchPos = touch.position;
            isSwiping = true;
        }
        else if (touch.phase == TouchPhase.Moved && isSwiping)
        {
            float swipeDeltaY = touch.position.y - startTouchPos.y;
            float swipeDir = Mathf.Clamp(swipeDeltaY / Screen.height, -1f, 1f);
            
            float moveAmount = swipeDeltaY * swipeSensitivity * moveSpeed * Time.deltaTime;

            Vector2 newPos = transform.position;
            newPos.y = Mathf.MoveTowards(newPos.y, newPos.y + moveAmount, swipeOffset);
            newPos.y = Mathf.Clamp(newPos.y, minY + offset, maxY - offset);

            transform.position = newPos;

            startTouchPos = touch.position;

            // Calculate target rotation based on swipe direction
            if (swipeDir > 0)
            {
                targetRotation = maxRotation; //up
            }
            else if (swipeDir < 0)
            {
                targetRotation = -maxRotation; //down
            }
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isSwiping = false;
            targetRotation = 0f; 
        }
    }

    void ApplyRotation()
    {
        // Smoothly interpolate current rotation towards target rotation
        float currentZ = transform.eulerAngles.z;
        
        // Normalize angle to -180 to 180 range
        if (currentZ > 180f)
            currentZ -= 360f;

        float newRotation = Mathf.LerpAngle(currentZ, targetRotation, rotationSmooth * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, newRotation);
    }
}




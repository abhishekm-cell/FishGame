using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Rigidbody2D rb2D;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void 
}
/*
using UnityEngine;

public class SwipeVerticalMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float minY = -4f;
    [SerializeField] private float maxY = 4f;

    [Header("Swipe Settings")]
    [SerializeField] private float swipeSensitivity = 0.01f;

    private Vector2 startTouchPos;
    private bool isSwiping;

    void Update()
    {
        HandleSwipe();
    }

    void HandleSwipe()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            startTouchPos = touch.position;
            isSwiping = true;
        }
        else if (touch.phase == TouchPhase.Moved && isSwiping)
        {
            float swipeDeltaY = touch.position.y - startTouchPos.y;

            float moveAmount = swipeDeltaY * swipeSensitivity * moveSpeed * Time.deltaTime;

            Vector3 newPos = transform.position;
            newPos.y += moveAmount;
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

            transform.position = newPos;

            // Update start position for smooth continuous movement
            startTouchPos = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isSwiping = false;
        }
    }
}

*/
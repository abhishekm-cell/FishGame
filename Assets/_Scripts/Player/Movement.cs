using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    // need to add size scale threshold, players starts from 0.75 and grows till 2

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float minY, maxY;
    [SerializeField] private float maxRotation = 25f;
    [SerializeField] private float rotationSmooth = 10f;
    private float lastEatTime;
    [SerializeField] private float eatAnimCooldown = 0.2f;
    private float targetRotation;
    private Coroutine activeEffect;
    private float delaydeath;
    
    [Header("Swipe Settings")]
    [SerializeField] private float swipeSensitivity = 0.01f;
    [SerializeField] private float baseSwipeSensitivity;
    [SerializeField] private float offset;
    [SerializeField] private float swipeOffset;

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Animator anim;

    

    private Vector2 startTouchPos;
    private bool isSwiping;

    private void Start()
    {
        baseMoveSpeed = moveSpeed;
        baseSwipeSensitivity = swipeSensitivity;

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

    public void ConsumeFood(FoodData foodData)
    {
        
        if (foodData == null)
        {
            Debug.LogError("ConsumeFood called with NULL FoodData");
            return;
        }

        if (anim != null)
        {
            if (Time.time - lastEatTime > eatAnimCooldown)
            {
                anim.SetTrigger("Eat");
                lastEatTime = Time.time;
            }
        }
        

        // Stop current effect
        if (activeEffect != null)
        {
            StopCoroutine(activeEffect);
            ResetStats(); // IMPORTANT
            activeEffect = null;
        }

        switch (foodData.foodEffect)
        {
            case FoodEffect.ReduceSpeed:
                activeEffect = StartCoroutine(ReduceSpeedEffect(foodData));
                Debug.Log("Eating fish: Reduce Speed");
                break;

            case FoodEffect.LaggyTouch:
                activeEffect = StartCoroutine(LaggyTouchEffect(foodData));
                Debug.Log("Eating fish: Laggy Touch");
                break;

            default:
                Debug.LogWarning($"Unhandled food effect: {foodData.foodEffect}");
                break;
        }
    }


    private IEnumerator ReduceSpeedEffect(FoodData foodData)
    {
        moveSpeed = baseMoveSpeed * (1f - foodData.effectStrength);
        yield return new WaitForSeconds(foodData.effectDuration);
        ResetStats();
    }

    private IEnumerator LaggyTouchEffect(FoodData foodData)
    {
        moveSpeed = baseMoveSpeed * (1f - foodData.effectStrength);
        swipeSensitivity = baseSwipeSensitivity * (1f - foodData.effectStrength);
        yield return new WaitForSeconds(foodData.effectDuration);
        ResetStats();
    }
    
    private void ResetStats()
    {
        moveSpeed = baseMoveSpeed;
        swipeSensitivity = baseSwipeSensitivity;
    }

    public void PlayerDie()
    {
        this.gameObject.SetActive(false);
    }

    

}




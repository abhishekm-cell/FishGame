using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class FishFood : MonoBehaviour
{
    [SerializeField]private FoodData data;
    private const string playerTag = "Player";
    private GameManager gManager;

    public void Init(FoodData foodData , GameManager gameManager)
    {
        data = foodData;
        this.gManager = gameManager;
    }

    void Update()
    {
        transform.Translate(Vector3.left * data.moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (data == null)
        {
            Debug.LogError($"{name} collided but FoodData is NULL. Was Init() called?");
            return;
        }

        // Player collision
        if (collision.CompareTag(playerTag))
        {
            if(gManager == null)
            {
                Debug.Log("Game Manager is NULL");  
            }
            gManager.GetPlayerMovement().ConsumeFood(data);
            Debug.Log("Collided with Player");

            StartCoroutine(DelayDeath());
            return;
        }

        // DeSpawner collision
        if (collision.gameObject.layer == LayerMask.NameToLayer("DeSpawner"))
        {
            Events.RequestDespawn?.Invoke(gameObject, data.prefab);
        }
    }

    private IEnumerator DelayDeath()
    {
        yield return new WaitForSeconds(data.deathDelay);
        Events.RequestDespawn?.Invoke(gameObject, data.prefab);
    }


    // public void OnSpawn()
    // {
    //     Init(data);
    // }

    // public void OnDespawn()
    // {
    //     Destroy(gameObject);
    // }

    
}
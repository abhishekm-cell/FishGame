using System;
using System.Collections;
using UnityEngine;

public class FishFood : MonoBehaviour
{
    public FoodData data;
    private const string playerTag = "Player";
    private GameManager gManager;
    [SerializeField] private Animator anim;

    public void Init(FoodData foodData )
    {
        data = foodData;
    }
    public void SetReference(GameManager gameManager) => this.gManager = gameManager;
    

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
                Debug.LogError("GameManager is NULL. SetReference() was not called.");
                return;  
            }
            if(gManager.GetPlayerGrowth() == null)
            {
                Debug.Log("PlayerGrowth is NULL");
                return;
            }
            if(!gManager.GetPlayerGrowth().CanEatFood(data))
            {
                Debug.Log("Player too small to eat this fish");
                anim.SetTrigger("Eat");
                gManager.GetPlayerMovement().PlayerDie();
                gManager.TriggerGameOver();
                //Events.ShowGameOverInvoke();
                return;
            }

            gManager.GetPlayerGrowth().RegisterFood(data);
            // gManager.SetScore(data.points);
            // Events.UpdateScoreInvoke(data.points);
            Events.ScoreGainedInvoke(data.points);

            gManager.GetPlayerMovement().ConsumeFood(data);
            
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
        Debug.Log("Delaying Death");
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
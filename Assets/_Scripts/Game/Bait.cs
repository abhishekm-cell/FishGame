
using UnityEngine;

public class Bait : MonoBehaviour
{
    public FoodData data;
    //[SerializeField] private int baitPoints;
     public void Init(FoodData foodData )
    {
        data = foodData;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Events.RequestDespawn?.Invoke(gameObject,data.prefab);
            Events.ScoreGainedInvoke(data.points);
            Debug.Log("Bait has been eaten");
            
        }
    }
}
using UnityEngine;

public class Bait : MonoBehaviour
{
    [SerializeField] private Transform Hook;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Bait has been eaten");
            
        }
    }
}
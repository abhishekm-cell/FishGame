using System;
using System.Data.Common;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private ObstacleData obsdata;
    private GameManager gManager;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject hook;
    [SerializeField] private GameObject prefab;

    [SerializeField] private float amplitude ; // How far up and down
    [SerializeField] private float frequency; // How fast it moves

    private Vector2 startPos; 
    public void Init(ObstacleData data, GameManager gameManager)
    {
        obsdata = data;
        gManager = gameManager;
    }
    void Start()
    {
        frequency =  UnityEngine.Random.Range(obsdata.minFrequency, obsdata.maxFrequency); 
        amplitude = UnityEngine.Random.Range(obsdata.minAmplitude, obsdata.maxAmplitude);
    }


    void Update()
    {
        MoveLeft();
        MoveUpDown();
    }

    private void MoveUpDown()
    {
        float newY = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(transform.position.x, startPos.y + newY, transform.position.z);
    }

    private void MoveLeft()
    {
        transform.Translate(Vector3.left * UnityEngine.Random.Range(moveSpeed, obsdata.horiSpeed) * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("DeSpawner"))
        {
            Events.RequestDespawn?.Invoke(gameObject,obsdata.prefab);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //gManager.GetPlayerMovement().Die();
            Debug.Log("GAME OVER");
        }
    }


}


using System;
using System.Data.Common;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private ObstacleData obsdata;
    private GameManager gManager;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Collider2D hookCollider;
    [SerializeField] private GameObject prefab, baitPrefab;

    [SerializeField] private float amplitude ; // How far up and down
    [SerializeField] private float frequency; // How fast it moves
    public bool isReeling = false;

    private Vector2 startPos; 
    public void Init(ObstacleData data, GameManager gameManager)
    {
        obsdata = data;
        gManager = gameManager;
        baitPrefab.SetActive(true);
        // baitPrefab = data.baitPrefab;
        // data.baitPrefab.SetActive(true);
    }
    public void SetReference(GameManager gameManager) => this.gManager = gameManager;
    void Start()
    {
        
        frequency =  UnityEngine.Random.Range(obsdata.minFrequency, obsdata.maxFrequency); 
        amplitude = UnityEngine.Random.Range(obsdata.minAmplitude, obsdata.maxAmplitude);
    }
    void OnEnable()
    {
        Events.GameOverRequested += ResetReeling;
    }
    void OnDisable()
    {
        Events.GameOverRequested -= ResetReeling;
    }

    private void ResetReeling()
    {
        isReeling = false;
    }

    void Update()
    {
        if(!isReeling)
        {
            MoveLeft();
            MoveUpDown();
        }
        else
        {
            ReelIn();
        }
        
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

    private void ReelIn()
    {
        transform.Translate(Vector3.up * obsdata.reelSpeed * Time.deltaTime);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("DeSpawner"))
        {
            Events.RequestDespawn?.Invoke(gameObject,obsdata.prefab);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("DeSpawner"))
        {
            Events.RequestDespawn?.Invoke(gameObject,obsdata.prefab);
        }
        if (other.CompareTag("Player")) 
        {
            Debug.Log("Player hooked!");

            isReeling = true;
           /*Events.ShowGameOverInvoke();*/
            
            if (gManager.GetPlayerMovement() != null)
            {
                gManager.GetPlayerMovement().StartReel(transform, obsdata.reelSpeed);
                AudioManager.Instance.PlaySFX(SoundType.Reeling);
            }   
        } 
    }

}


using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class FishController : MonoBehaviour
{
    [SerializeField] private FishDataSO fishData;
 
    public int SizeValue { get; private set; }
 
    private int _direction;
    private float _speed;
    private GamesManager _gameManager;
    private ObjectPool _pool;
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private CircleCollider2D _col;
    public event System.Action OnReturnedToPool;
    private Animator _anim;
 
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _col = GetComponent<CircleCollider2D>();
        _rb.gravityScale = 0f;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _anim = GetComponent<Animator>();
 
        // Resolved in Awake so they're ready before Init() is called on prewarm objects.
        // Start() never fires on pooled objects that are inactive at scene load.
        _gameManager = ServiceLocator.Instance.Get<GamesManager>();
        _pool = ServiceLocator.Instance.Get<ObjectPool>();
    }
 
    /// <summary>Called by SpawnManager each time this fish is pulled from the pool.</summary>
    public void Init(int sizeValue, int direction, float yWorld)
    {
        SizeValue  = sizeValue;
        _direction = direction;
 
        var tier = fishData.GetTier(sizeValue);
        _speed   = tier.speed;
 
        float startX = direction == 1 ? -10f : 10f;
        transform.position = new Vector3(startX, yWorld, 0f);
        transform.localScale  = Vector3.one * tier.visualScale;
 
        _sr.flipX = direction > 0;
 
        _anim.Play("Swim");
 
        gameObject.SetActive(true);
    }
 
    void FixedUpdate()
    {
        if (_gameManager == null) return;
        if (_gameManager.State != GameState.Playing) return;
 
        _rb.MovePosition(_rb.position + Vector2.right * _direction * _speed * Time.fixedDeltaTime);
 
        if (Mathf.Abs(transform.position.x) > 12f)
            ReturnToPool();
    }
 
    
 
    public void ReturnToPool()
    {
        OnReturnedToPool?.Invoke();
        OnReturnedToPool = null; 
        _pool.ReturnFish(gameObject, SizeValue);
    }
}

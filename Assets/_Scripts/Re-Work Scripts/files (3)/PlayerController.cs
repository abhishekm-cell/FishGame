using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private FishDataSO fishData;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Bounds — match your camera ortho size")]
    [SerializeField] private float xMin = -8f;
    [SerializeField] private float xMax =  8f;
    [SerializeField] private float yMin = -5f;
    [SerializeField] private float yMax =  5f;

    public int CurrentSize { get; private set; } = 1;

    private GamesManager  _gameManager;
    private Rigidbody2D _rb;
    private CircleCollider2D _col;
    private SpriteRenderer _sr;
    private Vector2 _input;

    void Awake()
    {
        _rb  = GetComponent<Rigidbody2D>();
        _col = GetComponent<CircleCollider2D>();
        _sr  = GetComponent<SpriteRenderer>();
        _rb.gravityScale = 0f;
    }

    void Start()
    {
        if (ServiceLocator.Instance == null)
        {
            Debug.LogError("STOP HERE: ServiceLocator.Instance is null");
            enabled = false;
            return;
        }

        _gameManager = ServiceLocator.Instance.Get<GamesManager>();

        
    }

    void Update()
    {
        if (_gameManager.State != GamesManager.GameState.Playing) return;

        _input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        
        //Debug.Log($"here is what YOURE PRESSING{_input}");

        if (_input.x != 0)
        {
            _sr.flipX = _input.x == -1;
        }
            
    }

    void FixedUpdate()
    {
        if (_gameManager.State != GamesManager.GameState.Playing) return;

        var next = _rb.position + _input * moveSpeed * Time.fixedDeltaTime;
        next.x = Mathf.Clamp(next.x, xMin, xMax);
        next.y = Mathf.Clamp(next.y, yMin, yMax);
        _rb.MovePosition(next);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_gameManager.State != GamesManager.GameState.Playing) return;

        if (other.TryGetComponent<FishController>(out var fish))
        {
            if (fish.SizeValue < CurrentSize)
            {
                int eaten = fish.SizeValue;
                fish.ReturnToPool();
                GrowTo(CurrentSize + 1);
                EventBus.Publish(new OnPlayerAte
                {
                    eatenFishSize  = eaten,
                    newPlayerSize  = CurrentSize
                });
            }
            else if (fish.SizeValue > CurrentSize)
            {
                // Debug: log the fish that ate the player
                var tier = fishData.GetTier(fish.SizeValue);
                Debug.Log($"[PlayerController] Eaten by fish | " +
                        $"SizeValue: {fish.SizeValue} | " +
                        $"Speed: {tier.speed} | " +
                        $"ColliderRadius: {tier.colliderRadius} | " +
                        $"VisualScale: {tier.visualScale} | " +
                        $"Sprite: {(tier.sprite != null ? tier.sprite.name : "null")} | " +
                        $"PlayerSize at death: {CurrentSize}");

                EventBus.Publish(new OnPlayerDied { reason = "fish" });
            }
            // Same size — no effect
        }
        
    }

    public void ResetPlayer()
    {
        transform.position = Vector3.zero;
        CurrentSize = 1;
        ApplySize();
    }

    private void GrowTo(int newSize)
    {
        CurrentSize = Mathf.Min(newSize, 5);
        ApplySize();
    }

    private void ApplySize()
    {
        var tier = fishData.GetTier(CurrentSize);
        _col.radius = tier.colliderRadius;
        transform.localScale = Vector3.one * tier.visualScale;
    }
}

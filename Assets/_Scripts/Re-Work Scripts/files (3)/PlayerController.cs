using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private FishDataSO fishData;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float maxRotation = 25f;

    [Header("Bounds — match your camera ortho size")]
    [SerializeField] private float xMin = -8f;
    [SerializeField] private float xMax =  8f;
    [SerializeField] private float yMin = -5f;
    [SerializeField] private float yMax =  5f;

    // Minimum drag distance (in pixels) before touch counts as directional input
    [SerializeField] private float touchDeadzone = 10f;

    public int CurrentSize { get; private set; } = 1;

    private GamesManager  _gameManager;
    private Rigidbody2D _rb;
    private CircleCollider2D _col;
    private SpriteRenderer _sr;
    private Vector2 _input;
    private bool _isReeling = false;
    private Transform _hookTransform;

    // Touch tracking
    private int _touchFingerId = -1;
    private Vector2 _touchStartPos;

    void Awake()
    {
        _rb  = GetComponent<Rigidbody2D>();
        _col = GetComponent<CircleCollider2D>();
        _sr  = GetComponent<SpriteRenderer>();
        _rb.gravityScale = 0f;
        ServiceLocator.Instance.Register<PlayerController>(this);
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

    void OnEnable()
    {
        EventBus.Subscribe<OnHookCaught>(OnHookCaught);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<OnHookCaught>(OnHookCaught);
    }

    void Update()
    {
        if (_gameManager.State != GamesManager.GameState.Playing) return;

        // --- Keyboard input (unchanged) ---
        Vector2 keyInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // --- Touch input ---
        Vector2 touchInput = ReadTouchInput();

        // Touch takes priority when active; fall back to keyboard
        _input = touchInput != Vector2.zero ? touchInput : keyInput;

        if (_input.x != 0)
            _sr.flipX = _input.x < 0f;
    }

    void FixedUpdate()
    {
        if (_gameManager.State != GamesManager.GameState.Playing) return;

        if (_isReeling && _hookTransform != null)
        {
            _rb.MovePosition(Vector2.Lerp(_rb.position, _hookTransform.position, 15f * Time.fixedDeltaTime));
            return;
        }

        var next = _rb.position + _input * moveSpeed * Time.fixedDeltaTime;
        next.x = Mathf.Clamp(next.x, xMin, xMax);
        next.y = Mathf.Clamp(next.y, yMin, yMax);
        _rb.MovePosition(next);

        // Rotate to face movement direction, clamped to ±maxRotation degrees
        if (_input != Vector2.zero)
        {
            // _input.x is already flipped-aware via flipX, so use raw Y and absolute X
            // Tilt up when moving up, tilt down when moving down
            float targetAngle = _input.y * 80f;

            // If sprite is flipped (moving left), invert the tilt so it still makes sense visually
            if (_sr.flipX) targetAngle = -targetAngle;

            float smoothAngle = Mathf.LerpAngle(_rb.rotation, targetAngle, 10f * Time.fixedDeltaTime
            );
            _rb.SetRotation(smoothAngle);
        }
        else
        {
            // Return to flat when idle
            float smoothAngle = Mathf.LerpAngle(_rb.rotation, 0f, 8f * Time.fixedDeltaTime);
            _rb.SetRotation(smoothAngle);
        }
    }

    /// <summary>
    /// Tracks a single finger. Returns a normalised 8-direction vector,
    /// or Vector2.zero when no touch is active / within the deadzone.
    /// </summary>
    private Vector2 ReadTouchInput()
    {
        // Register a new finger if none is tracked
        if (_touchFingerId == -1)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                if (t.phase == TouchPhase.Began)
                {
                    _touchFingerId = t.fingerId;
                    _touchStartPos = t.position;
                    break;
                }
            }
        }

        if (_touchFingerId == -1) return Vector2.zero;

        // Find the tracked finger among current touches
        Touch? tracked = null;
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).fingerId == _touchFingerId)
            {
                tracked = Input.GetTouch(i);
                break;
            }
        }

        // Finger lifted or lost — reset
        if (!tracked.HasValue ||
            tracked.Value.phase == TouchPhase.Ended ||
            tracked.Value.phase == TouchPhase.Canceled)
        {
            _touchFingerId = -1;
            return Vector2.zero;
        }

        Vector2 delta = tracked.Value.position - _touchStartPos;

        // Inside deadzone — no movement yet
        if (delta.magnitude < touchDeadzone) return Vector2.zero;

        // Snap to 8 directions (45° increments)
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        float snapped = Mathf.Round(angle / 45f) * 45f;
        float rad = snapped * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
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
                var tier = fishData.GetTier(fish.SizeValue);
                Debug.Log($"[PlayerController] Eaten by fish | " + $"SizeValue: {fish.SizeValue} | " + $"Speed: {tier.speed} | " + $"ColliderRadius: {tier.colliderRadius} | " +
                $"VisualScale: {tier.visualScale} | " + $"Sprite: {(tier.sprite != null ? tier.sprite.name : "null")} | " + $"PlayerSize at death: {CurrentSize}");

                EventBus.Publish(new OnPlayerDied { reason = "fish" });
            }
        }
    }

    public void ResetPlayer()
    {
        transform.position = Vector3.zero;
        CurrentSize = 1;
        _touchFingerId = -1;   
        _isReeling = false;
        _hookTransform = null;
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
        //_col.radius = tier.colliderRadius;
        transform.localScale = Vector3.one * tier.visualScale;
    }

    private void OnHookCaught(OnHookCaught e)
    {
        _isReeling = true;
        _hookTransform = e.hookTransform; 
    }



}

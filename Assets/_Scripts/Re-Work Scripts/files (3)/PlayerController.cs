using UnityEditor.Rendering;
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

    
    [SerializeField] private float touchDeadzone = 10f;

    public int CurrentSize { get; private set ; } = 2;
    public int TierCount => fishData.TierCount;
    private int _fishEatenThisTier = 0;
    private const int EatsToGrow = 10;

    private GamesManager  _gameManager;
    private Rigidbody2D _rb;
    private CircleCollider2D _col;
    private SpriteRenderer _sr;
    private Vector2 _input;
    private bool _isReeling = false;
    private Transform _hookTransform;

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
        if (_gameManager.State != GameState.Playing) return;

        
        Vector2 keyInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        
        Vector2 touchInput = ReadTouchInput();

        
        _input = touchInput != Vector2.zero ? touchInput : keyInput;

        if (_input.x != 0)
            _sr.flipX = _input.x < 0f;
    }

    void FixedUpdate()
    {
        if (_gameManager.State != GameState.Playing) return;

        if (_isReeling && _hookTransform != null)
        {
            _rb.MovePosition(Vector2.Lerp(_rb.position, _hookTransform.position, 15f * Time.fixedDeltaTime));
            return;
        }

        var next = _rb.position + _input * moveSpeed * Time.fixedDeltaTime;
        next.x = Mathf.Clamp(next.x, xMin, xMax);
        next.y = Mathf.Clamp(next.y, yMin, yMax);
        _rb.MovePosition(next);

        
        if (_input != Vector2.zero)
        {
            
            float targetAngle = _input.y * 80f;

            
            if (_sr.flipX) targetAngle = -targetAngle;

            float smoothAngle = Mathf.LerpAngle(_rb.rotation, targetAngle, 10f * Time.fixedDeltaTime
            );
            _rb.SetRotation(smoothAngle);
        }
        else
        {
            
            float smoothAngle = Mathf.LerpAngle(_rb.rotation, 0f, 8f * Time.fixedDeltaTime);
            _rb.SetRotation(smoothAngle);
        }
    }

    
    private Vector2 ReadTouchInput()
    {
        
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

        
        Touch? tracked = null;
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).fingerId == _touchFingerId)
            {
                tracked = Input.GetTouch(i);
                break;
            }
        }

        
        if (!tracked.HasValue ||
            tracked.Value.phase == TouchPhase.Ended ||
            tracked.Value.phase == TouchPhase.Canceled)
        {
            _touchFingerId = -1;
            return Vector2.zero;
        }

        Vector2 delta = tracked.Value.position - _touchStartPos;

        
        if (delta.magnitude < touchDeadzone) return Vector2.zero;

        
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        float snapped = Mathf.Round(angle / 45f) * 45f;
        float rad = snapped * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Player] Trigger hit: {other.gameObject.name} | tag: {other.tag} | layer: {LayerMask.LayerToName(other.gameObject.layer)}");
        if (_gameManager.State != GameState.Playing) return;
        if (_isReeling) return; 

        var fish = other.GetComponentInParent<FishController>();
        if (fish == null) return;

        if (fish.SizeValue < CurrentSize)
        {
            fish.ReturnToPool();
            _fishEatenThisTier++;

            if (_fishEatenThisTier >= EatsToGrow)
            {
                _fishEatenThisTier = 0;
                GrowTo(CurrentSize + 1);
            }

            EventBus.Publish(new OnPlayerAte
            {
                eatenFishSize = fish.SizeValue,
                newPlayerSize = CurrentSize
            });
        }
        else if (fish.SizeValue > CurrentSize)
        {
            EventBus.Publish(new OnPlayerDied { reason = "fish" });
        }
    }

    private void GrowTo(int newSize)
    {
        int maxSize = fishData.TierCount; // add this property to FishDataSO
        CurrentSize = Mathf.Min(newSize, maxSize);
        ApplySize();
        Debug.Log($"[Player] Grew to size {CurrentSize}");
    }

    public void ResetPlayer()
    {
        transform.position = Vector3.zero;
        CurrentSize = 2;
        _fishEatenThisTier = 0; // add this
        _touchFingerId = -1;
        _isReeling = false;
        _hookTransform = null;
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

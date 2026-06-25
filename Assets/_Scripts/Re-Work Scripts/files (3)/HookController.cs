using UnityEngine;

public class HookController : MonoBehaviour
{
    [SerializeField] private float descendSpeed = 3f;
    [SerializeField] private float ascendSpeed  = 4f;
    [SerializeField] private float reelSpeed    = 5f; // match ObstacleData.reelSpeed

    private float _topY;
    private float _bottomY;
    private GamesManager  _gameManager;
    private ObjectPool _pool;
    private PlayerController _playerMovement; // cache on Init

    private bool _isReeling = false;

    private enum Phase { Descending, Ascending }
    private Phase _phase;

    void Awake()
    {
        _gameManager = ServiceLocator.Instance.Get<GamesManager>();
        _pool = ServiceLocator.Instance.Get<ObjectPool>();
        //_playerMovement = ServiceLocator.Instance.Get<PlayerController>(); // or however you resolve it
    }

    public void Init(float startX, float topY, float bottomY)
    {
        _topY = topY;
        _bottomY = bottomY;
        _phase = Phase.Descending;
        _isReeling = false;
        transform.position = new Vector3(startX, topY, 0f);
        gameObject.SetActive(true);
    }

    void OnEnable()
    {
        EventBus.Subscribe<OnGameOver>(OnGameOver);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<OnGameOver>(OnGameOver);
    }

    private void OnGameOver(OnGameOver _) => _isReeling = false;

    void Update()
    {
        if (_gameManager == null) return;
        if (_gameManager.State != GamesManager.GameState.Playing) return;

        if (_isReeling)
        {
            transform.Translate(Vector3.up * reelSpeed * Time.deltaTime);
            if (transform.position.y >= _topY + 1f)
                ReturnToPool();
            return;
        }

        if (_phase == Phase.Descending)
        {
            transform.Translate(Vector3.down * descendSpeed * Time.deltaTime);
            if (transform.position.y <= _bottomY)
                _phase = Phase.Ascending;
        }
        else
        {
            transform.Translate(Vector3.up * ascendSpeed * Time.deltaTime);
            if (transform.position.y >= _topY + 1f)
                ReturnToPool();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isReeling = true;
            EventBus.Publish(new OnHookCaught { hookTransform = transform });
            // remove any OnPlayerDied publish from here
        }
    }

    public void ReturnToPool()
    {
        if (_isReeling)
            EventBus.Publish(new OnPlayerDied { reason = "hook" }); 
        
        _isReeling = false;
        gameObject.SetActive(false);
        _pool?.ReturnHook(gameObject);
    }


}

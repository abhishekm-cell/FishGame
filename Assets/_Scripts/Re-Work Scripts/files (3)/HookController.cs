using UnityEngine;

public class HookController : MonoBehaviour
{
    [SerializeField] private float descendSpeed = 3f;
    [SerializeField] private float ascendSpeed  = 4f;

    private float _topY;
    private float _bottomY;
    private GamesManager  _gameManager;
    private ObjectPool _pool;

    private enum Phase { Descending, Ascending }
    private Phase _phase;

    void Awake()
    {
        _gameManager = ServiceLocator.Instance.Get<GamesManager>();
        _pool = ServiceLocator.Instance.Get<ObjectPool>();
    }

    public void Init(float startX, float topY, float bottomY)
    {
        _topY = topY;
        _bottomY = bottomY;
        _phase = Phase.Descending;
        transform.position = new Vector3(startX, topY, 0f);
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (_gameManager == null) return;
        if (_gameManager.State != GamesManager.GameState.Playing) return;

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
            {
                ReturnToPool();
            }
                
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EventBus.Publish(new OnHookCaught());
        }
            
    }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
        _pool?.ReturnHook(gameObject);
    }
}

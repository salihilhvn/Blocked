using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    [Header("Prefabs for Pooling")]
    public GameObject floatingTextPrefab;
    public GameObject confettiPrefab;

    private ObjectPool<GameObject> floatingTextPool;
    private ObjectPool<GameObject> confettiPool;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePools();
    }

    private void InitializePools()
    {
        if (floatingTextPrefab != null)
        {
            floatingTextPool = new ObjectPool<GameObject>(
                createFunc: () => {
                    var obj = Instantiate(floatingTextPrefab, transform);
                    var ft = obj.GetComponent<FloatingText>();
                    if (ft != null) ft.pool = floatingTextPool;
                    return obj;
                },
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 30
            );
        }

        if (confettiPrefab != null)
        {
            confettiPool = new ObjectPool<GameObject>(
                createFunc: () => {
                    var obj = Instantiate(confettiPrefab, transform);
                    var r = obj.AddComponent<ReturnToPoolDelay>();
                    r.pool = confettiPool;
                    r.delay = 3f;
                    return obj;
                },
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: 3,
                maxSize: 5
            );
        }
    }

    public GameObject SpawnFloatingText(Vector3 position)
    {
        if (floatingTextPool == null) return null;
        var obj = floatingTextPool.Get();
        obj.transform.position = position;
        return obj;
    }

    public GameObject SpawnConfetti(Vector3 position)
    {
        if (confettiPool == null) return null;
        var obj = confettiPool.Get();
        obj.transform.position = position;
        return obj;
    }
}

public class ReturnToPoolDelay : MonoBehaviour
{
    public ObjectPool<GameObject> pool;
    public float delay;

    private void OnEnable()
    {
        Invoke(nameof(ReturnObj), delay);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void ReturnObj()
    {
        if (pool != null) pool.Release(gameObject);
    }
}

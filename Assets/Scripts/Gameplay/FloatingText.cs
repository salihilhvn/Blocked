using UnityEngine;
using TMPro;
using UnityEngine.Pool;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float fadeSpeed = 1.5f;
    public float lifetime = 1f;
    
    [HideInInspector]
    public ObjectPool<GameObject> pool;

    private TMP_Text textMesh;
    private Color textColor;
    private float timer;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        if (textMesh != null)
        {
            textColor = textMesh.color;
        }
    }

    public void Setup(int amount)
    {
        if (textMesh != null)
        {
            textMesh.text = "+" + amount.ToString();
            textColor.a = 1f; // Reset alpha in case it was pooled
            textMesh.color = textColor;
        }
        timer = lifetime;
    }

    private void Update()
    {
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        timer -= Time.deltaTime;

        if (textMesh != null)
        {
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;
        }

        if (timer < 0)
        {
            if (pool != null) pool.Release(gameObject);
            else Destroy(gameObject);
        }
    }
}

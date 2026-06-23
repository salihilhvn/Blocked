using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float fadeSpeed = 1.5f;
    public float lifetime = 1f;

    private TMP_Text textMesh;
    private Color textColor;
    private float timer;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        if (textMesh == null)
        {
            Debug.LogError("FloatingText requires a TMP_Text (TextMeshPro) component!");
        }
        else
        {
            textColor = textMesh.color;
        }
    }

    public void Setup(int amount)
    {
        if (textMesh != null)
        {
            textMesh.text = "+" + amount.ToString();
        }
        timer = lifetime;
    }

    private void Update()
    {
        // Yukarı doğru süzül
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);

        // Zamanı azalt
        timer -= Time.deltaTime;

        // Saydamlaş
        if (textMesh != null)
        {
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;
        }

        // Ömrü bitince sil
        if (timer < 0)
        {
            Destroy(gameObject);
        }
    }
}

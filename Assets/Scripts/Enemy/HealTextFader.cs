using UnityEngine;

public class HealTextFader : MonoBehaviour
{
    public float floatSpeed = 1.5f;
    public float lifetime = 1.2f;

    private TextMesh textMesh;
    private float timer = 0f;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        if (textMesh == null)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 上浮
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime, Space.World);

        // 淡出（线性）
        float alpha = 1.0f - (timer / lifetime);
        Color color = textMesh.color;
        color.a = Mathf.Clamp01(alpha);
        textMesh.color = color;

        // 时间到，销毁
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
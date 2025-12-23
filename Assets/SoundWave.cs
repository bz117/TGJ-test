using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SoundWave : MonoBehaviour
{
    public float growSpeed = 5.0f; 
    public int segments = 50;
    public float lineWidth = 0.1f;

    private LineRenderer line;
    private float currentRadius = 0f;
    private bool isExpiring = false;
    private CircleCollider2D col; // 提前声明引用

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        col = GetComponent<CircleCollider2D>(); // 在初始化时获取一次
        line.useWorldSpace = false;
        line.positionCount = segments + 1;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.loop = true;
        
        // 确保材质存在，解决看不见的问题
        if (line.material == null || line.material.name.Contains("Default"))
        {
            line.material = new Material(Shader.Find("Sprites/Default"));
        }
    }

    // 由 Spawner 在 Update 中持续调用
    public void TickGrow()
    {
        if (isExpiring) return;

        currentRadius += growSpeed * Time.deltaTime;
        DrawCircle();
    }

    // 由 Spawner 在松开按键时调用
    public void StartExpiring()
    {
        if (!isExpiring)
        {
            StartCoroutine(FadeAndDestroy());
        }
    }
    void DrawCircle()
    {
        // 1. 将更新半径放在循环外面，每一帧只执行一次
        if (col != null) 
        {
            col.radius = currentRadius;
        }

        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * currentRadius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * currentRadius;
            line.SetPosition(i, new Vector3(x, y, 0));
            // 这里删掉了原本的 GetComponent 代码
            angle += (360f / segments);
        }
    }

    private System.Collections.IEnumerator FadeAndDestroy()
    {
        isExpiring = true;
        float alpha = 1.0f;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * 4f; // 消失快一点
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                line.colorGradient.colorKeys,
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
            line.colorGradient = gradient;
            yield return null;
        }
        Destroy(gameObject);
    }
}
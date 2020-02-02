using UnityEngine;

public class OffsetScrolling : MonoBehaviour
{
    public float scrollSpeed;

    private Renderer mainRenderer;

    void Start()
    {
        mainRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        mainRenderer.material.SetTextureOffset("_MainTex", new Vector2(-offset, 0));
    }
}
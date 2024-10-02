using UnityEngine;
public class DrawingBrush : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        gameObject.transform.position = mousePos;
        spriteRenderer.enabled = Input.GetMouseButton(0);
    }
    
}

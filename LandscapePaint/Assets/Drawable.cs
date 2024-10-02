using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Drawable : MonoBehaviour
{
    public Camera drawingCamera;
    public RawImage overlayImage;
    private RenderTexture _texture;
    
    void Start() {
        drawingCamera.aspect = 1;
        _texture = new RenderTexture(327, 327, 24);

        drawingCamera.targetTexture = _texture;
        overlayImage.texture = drawingCamera.targetTexture;
    }

    public void Clear() {
        drawingCamera.clearFlags = CameraClearFlags.Color;
        StartCoroutine(ResetClear());
    }

    IEnumerator ResetClear() {
        yield return new WaitForNextFrameUnit();
        drawingCamera.clearFlags = CameraClearFlags.Nothing;
    }

    public Texture2D Capture(int width, int height) 
    {
        // Capture canvas
        Texture2D texture = new Texture2D(327, 327, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        
        RenderTexture.active = _texture;
        drawingCamera.Render();

        texture.ReadPixels(new Rect(0, 0, 327, 327), 0, 0);
        texture.Apply();

        // Rescale
        RenderTexture scalingRt = RenderTexture.GetTemporary(width, height);
        scalingRt.filterMode = FilterMode.Point;
        RenderTexture.active = scalingRt;
        Graphics.Blit(texture, scalingRt);

        Texture2D scaledTexture = new Texture2D(width, height);
        scaledTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        scaledTexture.Apply();

        RenderTexture.active = _texture;
        RenderTexture.ReleaseTemporary(scalingRt);

        return scaledTexture;
    }
    
}

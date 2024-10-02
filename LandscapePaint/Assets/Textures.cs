using UnityEngine;

public static class Textures
{
    public static Texture2D TextureFromColors(Color[] colors, int width, int height) {
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }
}

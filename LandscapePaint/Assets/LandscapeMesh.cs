
using UnityEngine;

public class LandscapeMesh : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void Draw(MeshData data, Texture2D texture) {
        meshFilter.sharedMesh = data.Mesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}

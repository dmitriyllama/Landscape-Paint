using UnityEngine;

public static class Meshes
{

    public static MeshData GenerateLandscapeMesh(float[,] heightMap) {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        MeshData data = new MeshData(width, height);

        int vertIndex = 0;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {                
                data.verts[vertIndex] = new Vector3(x, heightMap[x,y], y);
                data.uvs[vertIndex] = new Vector2(x/(float)width, y/(float)height);

                if (x < width-1 && y < height-1) {
                    data.AddTriangle(vertIndex, vertIndex+width+1, vertIndex+width);
                    data.AddTriangle(vertIndex+width+1, vertIndex, vertIndex+1);
                }
                
                vertIndex++;
            }
        }

        return data;
    }
}

public class MeshData {
    public Vector3[] verts;
    public int[] tris;
    public Vector2[] uvs;

    int triIndex = 0;

    public MeshData(int width, int height) {
        verts = new Vector3[width * height];
        tris = new int[(width-1) * (height-1) * 6];
        uvs = new Vector2[width*height];
    }

    public void AddTriangle(int a, int b, int c) {
        tris[triIndex] = a;
        tris[triIndex+1] = b;
        tris[triIndex+2] = c;
        triIndex += 3;
    }

    public Mesh Mesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoise(int width, int height, float scale, int seed, int details, float persistance, float lacunarity)
    {
        float[,] noise = new float[width, height];

        System.Random rng = new System.Random(seed);
        Vector2[] detailsOffsets = new Vector2[details];
        for (int i = 0; i < details; i++) {
            float offsetX = rng.Next(-100000, 100000);
            float offsetY = rng.Next(-100000, 100000);
            detailsOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0) {
            scale = 0.01f;
        }

        float min = float.MaxValue;
        float max = float.MinValue;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float amplitude = 1;
                float frequency = 1;
                float noiseValue = 0;

                for (int i = 0; i < details; i++) {
                    float sampleX = x / scale * frequency + detailsOffsets[i].x;
                    float sampleY = y / scale * frequency + detailsOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseValue += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseValue < min) {
                    min = noiseValue;
                } else if (noiseValue > max) {
                    max = noiseValue;
                }
                noise[x,y] = noiseValue;
            }
        }

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                noise[x,y] = Mathf.InverseLerp(min, max, noise[x,y]);
            }
        }

        return noise;
    }
}

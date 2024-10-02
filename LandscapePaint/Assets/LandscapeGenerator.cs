using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LandscapeGenerator : MonoBehaviour
{
    public LandscapeMesh mesh;
    public Drawable drawCanvas;

    public int width;
    public int height;
    public int seed;

    [Header ("Plains")]
    public Color plainsColor;
    public float plainsScale;
    public float plainsElevation;
    public float plainsSteepness;
    public int plainsDetails;
    public float plainsPersistance;
    public float plainsLacunarity;

    [Header ("Hills")]
    public Color hillsColor;
    public float hillsScale;
    public float hillsElevation;
    public float hillsSteepness;
    public int hillsDetails;
    public float hillsPersistance;
    public float hillsLacunarity;

    [Header ("Rivers")]
    public Color riversColor;
    public float riversScale;
    public float riversElevation;
    public float riversSteepness;
    public int riversDetails;
    public float riversPersistance;
    public float riversLacunarity;

    Color[] _colors;
    float[,] _plainsNoise;
    float[,] _hillsNoise;
    float[,] _riversNoise;
    float[] _hillsMask;
    float[] _riversMask;

    void Start() {
        _plainsNoise = Noise.GenerateNoise(width, height, plainsScale, seed, plainsDetails, plainsPersistance, plainsLacunarity);
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                _plainsNoise[x,y] *= plainsSteepness;
                _plainsNoise[x,y] += plainsElevation;
            }
        }

        _hillsNoise = Noise.GenerateNoise(width, height, hillsScale, seed, hillsDetails, hillsPersistance, hillsLacunarity);
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                _hillsNoise[x,y] *= hillsSteepness;
                _hillsNoise[x,y] -= hillsElevation;
            }
        }

        _riversNoise = Noise.GenerateNoise(width, height, riversScale, seed, riversDetails, riversPersistance, riversLacunarity);
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                _riversNoise[x,y] *= riversSteepness;
                _riversNoise[x,y] -= riversElevation;
            }
        }

        _colors = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                _colors[x + y*width] = plainsColor;
            }
        }

        _hillsMask = new float[width * height];
        _riversMask = new float[width * height];

        StartCoroutine(InitialGenerate());
    }


    IEnumerator InitialGenerate() {
        yield return new WaitForNextFrameUnit();
        Generate();
    }


    // FIXME: Methods below are nearly identical. Too bad!
    public void ReadHills() {
        // Read and reformat mask to 0/1 values, clear overlapping masks
        Color[] texture = drawCanvas.Capture(width, height).GetPixels();
        float[] unblurredMask = new float[width * height];
        for (int i = 0; i < width*height; i++) {
            unblurredMask[i] = texture[i].r > 0.9 ? 1 : 0;
            if (unblurredMask[i] == 1) {
                _riversMask[i] = 0;
            }
        }

        // Apply blurring
        float[] blurredMask = new float[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float sum = unblurredMask[x + y*width];
                float num = 1;
                if (y != 0) {
                    num++;
                    sum += unblurredMask[x + (y-1)*width];
                }
                if (y != height-1) {
                    num++;
                    sum += unblurredMask[x + (y+1)*width];
                }
                if (x != 0) {
                    num++;
                    sum += unblurredMask[x-1 + y*width];
                }
                if (x != width-1) {
                    num++;
                    sum += unblurredMask[x+1 + y*width];
                }
                sum /= (float)num;
                blurredMask[x + y*width] = sum;
            }
        }

        _hillsMask = blurredMask;
        Generate();
    }

    public void ReadRivers() {
        // Read and reformat mask to 0/1 values, clear overlapping masks
        Color[] texture = drawCanvas.Capture(width, height).GetPixels();
        float[] unblurredMask = new float[width * height];
        for (int i = 0; i < width*height; i++) {
            unblurredMask[i] = texture[i].r > 0.9 ? 1 : 0;
            if (unblurredMask[i] == 1) {
                _hillsMask[i] = 0;
            }
        }

                // Apply blurring
        float[] blurredMask = new float[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float sum = unblurredMask[x + y*width];
                float num = 1;
                if (y != 0) {
                    num++;
                    sum += unblurredMask[x + (y-1)*width];
                }
                if (y != height-1) {
                    num++;
                    sum += unblurredMask[x + (y+1)*width];
                }
                if (x != 0) {
                    num++;
                    sum += unblurredMask[x-1 + y*width];
                }
                if (x != width-1) {
                    num++;
                    sum += unblurredMask[x+1 + y*width];
                }
                sum /= (float)num;
                blurredMask[x + y*width] = sum;
            }
        }

        _riversMask = unblurredMask;
        Generate();
    }

    public void Generate() {
        float[,] noise = (float[,]) _plainsNoise.Clone();
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float hillsMask = _hillsMask[x + y*width];
                float riversMask = _riversMask[x + y*width];

                noise[x,y] = _plainsNoise[x,y];
                _colors[x + y*width] = plainsColor;

                noise[x,y] = hillsMask*_hillsNoise[x,y] + (1-hillsMask)*noise[x,y];
                if (hillsMask > 0.5) {
                    _colors[x + y*width] = hillsColor;
                }

                noise[x,y] = riversMask*_riversNoise[x,y] + (1-riversMask)*noise[x,y];
                if (riversMask > 0.9) {
                    _colors[x + y*width] = riversColor;
                }
            }
        } 
        mesh.Draw(Meshes.GenerateLandscapeMesh(noise), Textures.TextureFromColors(_colors, width, height));
    }
}

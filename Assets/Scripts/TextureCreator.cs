using UnityEngine;
using System.Collections;

public class TextureCreator : MonoBehaviour {
    [Range(2,512)]
    public int resolution = 256;
    [Range(1, 3)]
    public int dimensions = 3;
    [Range(1, 8)]
    public int octaves = 1;
    [Range(1f, 4f)]
    public float lacunarity = 2f;
    [Range(0f, 1f)]
    public float persistence = 0.5f;
    public NoiseMethodType type;
    public float frequency = 1.0f;
    public Gradient coloring;
    private Texture2D texture;
	void OnEnable () {
        if (texture == null)
        {
            texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Trilinear;
            texture.anisoLevel = 9;
            texture.name = "Procedural Texture";
            GetComponent<MeshRenderer>().material.mainTexture = texture;
        }
            fillTexture();
	}
	
    public void fillTexture()
    {
        if (texture.width != resolution)
        {
            texture.Resize(resolution, resolution);
        }
        float stepSize = 1.0f / resolution;
        Random.seed = 40;
        Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
        Vector3 point10 = transform.TransformPoint(new Vector3(0.5f, -0.5f));
        Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
        Vector3 point11 = transform.TransformPoint(new Vector3(0.5f, 0.5f));
        NoiseMethod method = Noise.noiseMethods[(int)type][dimensions - 1];
        for (int y = 0; y < resolution; y++)
        {
            Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
            for (int x = 0; x < resolution; x++)
            {
                Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
                if (type != NoiseMethodType.Value)
                {
                    sample = sample * 0.5f + 0.5f;
                }
                texture.SetPixel(x, y, coloring.Evaluate(sample));
            }
        }
        texture.Apply();
    }
    // Update is called once per frame
    private void Update()
    {
        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            fillTexture();
        }
    }
}

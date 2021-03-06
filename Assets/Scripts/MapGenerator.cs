﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MapGenerator : MonoBehaviour
{

    public int size_x = 100;
    public int size_z = 50;
    public float tileSize = 1.0f;
    public int numberOfTerreains;
    public float frequency = 1f;

    [Range(1, 8)]
    public int octaves = 1;

    [Range(1f, 4f)]
    public float lacunarity = 2f;

    [Range(0f, 1f)]
    public float persistence = 0.5f;

    [Range(1, 3)]
    public int dimensions = 3;

    [Range(1, 100)]
    public float height;

    public NoiseMethodType type;

    public Gradient coloring;

    int numTiles;
    int numTris;

    int vsize_x;
    int vsize_z;
    int numVerts;
    int[] triangles;
    int[,] tileNumber;
    bool[,] isTileOccupied;

    public Texture2D terrainTiles;
    public int tileResolution;
    Mesh mesh;
    public Vector3[] vertices;
    public Vector3[,] verticesPositions;
    Color[] colors;
    Vector3[] normals;
    Vector2[] uv;

    // Use this for initialization
    void Start()
    {
        numTiles = size_x * size_z;
        numTris = numTiles * 2;

        vsize_x = size_x + 1;
        vsize_z = size_z + 1;
        numVerts = vsize_x * vsize_z;

        mesh = new Mesh();
        triangles = new int[numTris * 3];
        vertices = new Vector3[numVerts];
        colors = new Color[vertices.Length];
        verticesPositions = new Vector3[vsize_z, vsize_x];
        isTileOccupied = new bool[size_z, size_x];
        tileNumber = new int[size_z, size_x];
        normals = new Vector3[numVerts];
        uv = new Vector2[numVerts];
        BuildMesh();
    }

    public void BuildMesh()
    {

        // Generate the standard Flat mesh data
        genereteMeshData();
        // Modify the height data for verticies
        modifyMeshData();
        mainModify();
        // Apply texture to the data
        // genereteTexture();
        // Create a new Mesh, populate with the data and change verticies array to single dimention
        createMesh();
        


    }

    Color[][] chopUpTiles()
    {
        int numTilesPerRow = terrainTiles.width / tileResolution;
        int numRows = terrainTiles.height / tileResolution;

        Color[][] tiles = new Color[numTilesPerRow * numRows][];

        for(int y=0; y < numRows; y++)
        {
            for(int x = 0; x < numTilesPerRow; x++)
            {
                tiles[y * numTilesPerRow + x] = terrainTiles.GetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution);
            }
        }
        return tiles;
    }

    private void genereteTexture()
    {

        int texWidth = vsize_x * tileResolution;
        int texHight = vsize_z * tileResolution;
        Texture2D texture = new Texture2D(texWidth, texHight);

        Color[][] tiles = chopUpTiles();
        for(int y = 1; y<size_z; y++)
        {
            for(int x=1; x<size_x; x++)
            {
                Color[] p;
                if (verticesPositions[y, x].y >= 1.0f)
                {
                    p = tiles[1];
                }
                else
                {
                    p = tiles[2];
                }
                texture.SetPixels(x*tileResolution-8, y*tileResolution-8, tileResolution, tileResolution, p);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
        mesh_renderer.sharedMaterials[0].mainTexture = texture;
    }

    private void genereteMeshData()
    {
        int x, z;
        for (z = 0; z < vsize_z; z++)
        {
            for (x = 0; x < vsize_x; x++)
            {
                verticesPositions[z, x] = new Vector3(x * tileSize, 0, z * tileSize);
                vertices[z * vsize_x + x] = new Vector3(x * tileSize, 0, z * tileSize);
                colors[z * vsize_x + x] = Color.black;
                normals[z * vsize_x + x] = Vector3.up;
                uv[z * vsize_x + x] = new Vector2((float)x / vsize_x, (float)z / vsize_z);
            }
        }
        Debug.Log("Done Verts!");
        mesh.vertices = vertices;
        for (z = 0; z < size_z; z++)
        {
            for (x = 0; x < size_x; x++)
            {
                int squareIndex = z * size_x + x;
                int triOffset = squareIndex * 6;
                triangles[triOffset + 0] = z * vsize_x + x + 0;
                triangles[triOffset + 1] = z * vsize_x + x + vsize_x + 0;
                triangles[triOffset + 2] = z * vsize_x + x + vsize_x + 1;

                triangles[triOffset + 3] = z * vsize_x + x + 0;
                triangles[triOffset + 4] = z * vsize_x + x + vsize_x + 1;
                triangles[triOffset + 5] = z * vsize_x + x + 1;
            }
        }
        Debug.Log("Done Triangles!");
    }

    private void createMesh()
    {
 //       for (int z = 0; z < vsize_z; z++)
//        {
//            for (int x = 0; x < vsize_x; x++)
 //           {
//                vertices[z * vsize_x + x] = verticesPositions[z, x];
 //           }
 //       }
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        // Assign our mesh to our filter/renderer/collider
        MeshFilter mesh_filter = GetComponent<MeshFilter>();
        MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
        MeshCollider mesh_collider = GetComponent<MeshCollider>();

        mesh_filter.mesh = mesh;
        mesh_collider.sharedMesh = mesh;
        Debug.Log("Done Mesh!");
    }

    public void mainModify()
    {
        /*
        Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
        Vector3 point10 = transform.TransformPoint(new Vector3(0.5f, -0.5f));
        Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
        Vector3 point11 = transform.TransformPoint(new Vector3(0.5f, 0.5f));
        */
        Vector3 point00 = verticesPositions[0,0];
        Vector3 point10 = verticesPositions[0, 1];
        Vector3 point01 = verticesPositions[1, 0];
        Vector3 point11 = verticesPositions[1, 1];

        NoiseMethod method = Noise.noiseMethods[(int)type][dimensions - 1];
        float stepSize = 1f/size_x;
        for (int y = 0; y < vsize_z; y++)
        {
            Vector3 point0 = Vector3.Lerp(point00, point01, y * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, y * stepSize);
            for (int x = 0; x < vsize_x; x++)
            {
                Vector3 point = Vector3.Lerp(point0, point1, x * stepSize);
                float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
                sample = type == NoiseMethodType.Value ? (sample - 0.5f) : (sample * 0.5f);
                vertices[y * vsize_x + x].y = sample * height;
                colors[y*vsize_x+x] = coloring.Evaluate(sample + 0.5f);
            }
        }
        mesh.vertices = vertices;
        mesh.colors = colors;
    }

    private void modifyMeshData()
    {
        int rpositionz;
        int rpositionx;
        int rsizez;
        int rsizex;
        int rheight;
        bool noError;
        for (int i=0; i < numberOfTerreains;i++)
        {
            do
            {
                noError = false;
                rpositionz = Random.Range(1, size_z);
                rpositionx = Random.Range(1, size_x);
                rsizez = Random.Range(size_z/16, size_z / 4);
                rsizex = Random.Range(size_x/16, size_z / 4);
                rheight = Random.Range(-2, 2);
                if (rpositionz + rsizez < size_z
                    && rpositionx + rsizex < size_x)
                {
                        createTerrain(rpositionx, rpositionz, rsizex, rsizez, rheight);
                        noError = true;
                }
                else
                {
                    rpositionz = Random.Range(1, size_z);
                    rpositionx = Random.Range(1, size_x);
                    rsizez = Random.Range(size_z / 16, size_z / 4);
                    rsizex = Random.Range(size_x / 16, size_z / 4);
                    rheight = Random.Range(-1, 1);
                }
            } while (!noError);


        }


    }

    private void createTerrain(int positonx, int positonz, int sizex, int sizez, float minOrMaxHeight)
    {
        for (int z = positonz; z < positonz + sizez; z++)
        {
            for (int x = positonx; x < positonx + sizex; x++)
            {
                if(z == positonz || x == positonx || z == positonz + sizez-1|| x == positonx + sizex -1)
                {
                    if (minOrMaxHeight > 0
                        && verticesPositions[z + 1, x + 1].y < 1.0f
                        && verticesPositions[z + 1, x + 1].y >= 0)
                    verticesPositions[z + 1, x + 1].y -= 0.5f;
                    else if (minOrMaxHeight < 0
                        && verticesPositions[z + 1, x + 1].y > -1.0f
                        && verticesPositions[z + 1, x + 1].y < 0)
                    verticesPositions[z + 1, x + 1].y += 0.5f;
                }
                else
                {
                     if (minOrMaxHeight > 0
                        && verticesPositions[z + 1, x + 1].y < 1.0f)
                        verticesPositions[z + 1, x + 1].y += minOrMaxHeight;
                     if (minOrMaxHeight <= 0
                        && verticesPositions[z+1,x+1].y > -1.0f)
                        verticesPositions[z + 1, x + 1].y += minOrMaxHeight;
                }
                isTileOccupied[z, x] = true;
            }
        }
    }

    private bool isThereClearence(int positonx, int positonz, int sizex, int sizez)
    {
        for (int z = positonz; z < positonz + sizez; z++)
        {
            for (int x = positonx; x < positonx + sizex; x++)
            {
                if (isTileOccupied[z, x])
                {
                    return false;
                }
            }
        }
        
                return true;
    }
}
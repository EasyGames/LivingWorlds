using UnityEngine;
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
    int numTiles;
    int numTris;

    int vsize_x;
    int vsize_z;
    int numVerts;
    int[] triangles;

    public Texture2D terrainTiles;
    public int tileResolution;

    Vector3[] vertices;
    Vector3[,] verticesPositions;
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

        triangles = new int[numTris * 3];
        vertices = new Vector3[numVerts];
        verticesPositions = new Vector3[vsize_z, vsize_x];
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
        // Apply texture to the data
        genereteTexture();
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
                if (verticesPositions[y, x].y >= 1.5f)
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
                //vertices[z * vsize_x + x] = new Vector3(x * tileSize, 0, z * tileSize);
                normals[z * vsize_x + x] = Vector3.up;
                uv[z * vsize_x + x] = new Vector2((float)x / vsize_x, (float)z / vsize_z);
            }
        }
        Debug.Log("Done Verts!");

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
        for (int z = 0; z < vsize_z; z++)
        {
            for (int x = 0; x < vsize_x; x++)
            {
                vertices[z * vsize_x + x] = verticesPositions[z, x];
            }
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
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

    private void modifyMeshData()
    {
        for (int z = 1; z < size_z; z++)
        {
            for (int x = 1; x < size_x; x++)
            {
                verticesPositions[z, x].y = Random.Range(1.0f,2.0f);
            }
        }
    }
}
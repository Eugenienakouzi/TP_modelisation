using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class ProceduralTerrain : MonoBehaviour
{
    private const int width = 50;
    private const int height = 50;

    public float scale = 0.1f;
    private float amplitude = 5f;

    private float deformationStrength = 1f;

    public RayCast raycast;

    private const float radius = 1f;

    private Mesh mesh;

    private MeshCollider meshCollider;

    public Vector3[] vertices;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        vertices = new Vector3[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                vertices[z * width + x] = new Vector3(x, Mathf.PerlinNoise(vertices[x].x * scale, vertices[x].z *
                scale) * amplitude, z);
            }

        }

        int index = 0;
        for (int x = 0; x < width - 1; x++)
        {
            for (int z = 0; z < height - 1; z++)
            {
                int topLeft = z * width + x;
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + width;
                int bottomRight = bottomLeft + 1;

                // Triangle 1 
                triangles[index++] = topLeft;
                triangles[index++] = bottomLeft;
                triangles[index++] = topRight;

                // Triangle 2 
                triangles[index++] = topRight;
                triangles[index++] = bottomLeft;
                triangles[index++] = bottomRight;
            }
        }
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider.sharedMesh = mesh;
        
    }

    // Update is called once per frame
    void Update()
    {
        DeformJob deformJob = new DeformJob
        {
            vertices = new NativeArray<Vector3>(vertices, Allocator.TempJob),
            hitPoint = raycast.hitPoint,
            radius = radius,
            strength = deformationStrength
        };
        JobHandle jobHandle = deformJob.Schedule(vertices.Length, 64);
        jobHandle.Complete();

        vertices = deformJob.vertices.ToArray();
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = null;
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }

    [BurstCompile]
    public struct DeformJob : IJobParallelFor
    {
        public NativeArray<Vector3> vertices;
        public Vector3 hitPoint;
        public float radius;
        public float strength;
        public void Execute(int index)
        {
            Vector3 vertex = vertices[index];
            float distance = Vector3.Distance(vertex, hitPoint);
            if (distance < radius)
            {
                vertex.y += strength;
                vertices[index] = vertex;
            }
        }
    }
}

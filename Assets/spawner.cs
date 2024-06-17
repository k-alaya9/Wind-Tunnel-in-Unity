
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

[StructLayout(LayoutKind.Sequential, Size = 60)]


public struct Particle
{
    public float pressure;
    public float density;
    public Vector3 currentForce;
    public Vector3 velocity;
    public Vector3 position;
    public Vector4 color;

}


public class spawner : MonoBehaviour
{
    public int numParticlesToDisplay = 100;
    public ComputeShader computeShader;
    public Mesh particleMesh;
    public Material material;
    private Vector3 numToSpawn = new Vector3(Staticdata.numToSpawn, Staticdata.numToSpawn, Staticdata.numToSpawn);
    public int numParticles;
    public Vector3 spawnBoxCenter = new Vector3(0, 3, 0);
    public Vector3 spawnBox = new Vector3(4, 2, 1.5f);
    public Vector3 boxSize;
    private float particleRadius = Staticdata.particleRadius;
    private float boundDamping = -Staticdata.boundDamping;
    private float viscosity = Staticdata.viscosity;
    public float particleMass = 1f;
    private float gasConstant = Staticdata.gasConstant;
    private float restingDensity = Staticdata.restingDensity;
    public float deltaTime = 0.0001f;
    private float gravity = Staticdata.gravity;
    private Particle[] particles;
    private Particle[] initialParticles; 
    private ComputeBuffer particleBuffer;

    private ComputeBuffer renderBuffer;
    private ComputeBuffer argsBuffer;
    private Bounds bounds;
    private static readonly int SizeProperty = Shader.PropertyToID("_size");
    private static readonly int ParticlesBufferProperty = Shader.PropertyToID("_particles");

    private int kernelComputeDensityPressure;
    private int kernelComputeForces;
    private int kernelIntegrate;
    public Transform modelTransform; 

    private Particle[] renderParticles;

    private static float windSpeedValue=Staticdata.windSpeedValue;
    private Vector3 windSpeed = new Vector3(-WindSpeedValue, 0, 0);

    // Reference to the MeshTriangulator to access the triangles
    public MeshTriangulator meshTriangulator;
    //private Octree octree;
    //public Bounds octreeBounds = new Bounds(Vector3.zero, new Vector3(10, 10, 10)); // Adjust the size as needed

    private KDTree kdTree;
    private GameObject lineObject;
    private LineRenderer lineRenderer;
    public Material lineMaterial;
    public GameObject meshGameObject;

    public global::System.Single BoundDamping { get => boundDamping; set => boundDamping = value; }
    public global::System.Single Viscosity { get => viscosity; set => viscosity = value; }
    public global::System.Single GasConstant { get => gasConstant; set => gasConstant = value; }
    public global::System.Single RestingDensity { get => restingDensity; set => restingDensity = value; }
    public global::System.Single Gravity { get => gravity; set => gravity = value; }
    public global::System.Single ParticleRadius { get => particleRadius; set => particleRadius = value; }
    public  static global::System.Single WindSpeedValue { get => windSpeedValue; set => windSpeedValue = value; }

    //int optimalDepth=0;

    void Awake()
    {
        //octree = new Octree(octreeBounds, maxDepth, maxTriangles);
        //MeshTriangulator.SetOctree(octree);
        Debug.Log(numToSpawn.x);
        InitParticles();
        InitBuffers();
        bounds = new Bounds(Vector3.zero, boxSize);

        int optimalDepth = Mathf.CeilToInt(Mathf.Log(numParticles, 2));
        kdTree = new KDTree(optimalDepth);
        InitializeLineRenderer();
        kdTree.Build(particles);

       
    }

   
    void InitParticles()
    {
        Vector3 spawnTopLeft = spawnBoxCenter - spawnBox / 2;
        List<Particle> _particles = new List<Particle>();

        for (int x = 0; x < numToSpawn.x; x++)
        {
            for (int y = 0; y < numToSpawn.y; y++)
            {
                for (int z = 0; z < numToSpawn.z; z++)
                {
                    Vector3 spawnPosition = spawnTopLeft + new Vector3(x * ParticleRadius * 2, y * ParticleRadius * 2, z * ParticleRadius * 2) + Random.onUnitSphere * ParticleRadius * 0.1f;
                    Particle p = new Particle
                    {
                        //velocity = new Vector3(Random.value, Random.value, Random.value)*100 , // Initialize with some random velocity
                        position = spawnPosition,
                        //color = new Vector4(1, 1, 1, 1)
                    };

                    _particles.Add(p);
                }
            }
        }
        numParticles = _particles.Count;
        renderParticles = new Particle[numParticlesToDisplay];
      
        particleBuffer = new ComputeBuffer(numParticles, 60);
       
        particleBuffer.SetData(_particles.ToArray());
        particles = _particles.ToArray();
          
        //optimalDepth = Mathf.CeilToInt(Mathf.Log(particles.Length, 2));


    }
    void InitBuffers()
    {
        uint[] args = { particleMesh.GetIndexCount(0), (uint)numParticles, particleMesh.GetIndexStart(0), particleMesh.GetBaseVertex(0), 0 };
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);
        renderBuffer = new ComputeBuffer(numParticlesToDisplay, 60);

        RunComputeShader();
    }

    void Update()
    {
       
        RunComputeShader();
      


        kdTree.Build(particles);

        DetectCollisions();
        
        particleBuffer.SetData(particles);

        System.Array.Copy(particles, renderParticles, numParticlesToDisplay);
        renderBuffer.SetData(renderParticles);
        
        Graphics.DrawMeshInstancedIndirect(particleMesh, 0, material, bounds, argsBuffer, castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);
        VisualizeKDTree();

    }
    void InitializeLineRenderer()
    {
        lineObject = new GameObject("LineRenderer");
        lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 2.1f;
        lineRenderer.endWidth = 2.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0, 1, 0, 0.5f), 0.0f), new GradientColorKey(new Color(0, 0, 1, 0.5f), 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0.0f), new GradientAlphaKey(0.5f, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }


    void VisualizeKDTree()
    {
        ClearVisualization();
        List<Vector3> positions = new List<Vector3>();
        CollectPositions(kdTree.Root, positions);
        DrawLine(positions);
    }

    void CollectPositions(KDTree.KDNode node, List<Vector3> positions)
    {
        if (node == null) return;

        positions.Add(node.Particle.position);

        if (node.Left != null)
        {
            CollectPositions(node.Left, positions);
        }

        if (node.Right != null)
        {
            CollectPositions(node.Right, positions);
        }
    }

    void DrawLine(List<Vector3> positions)
    {
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }

    void ClearVisualization()
    {
        lineRenderer.positionCount = 0;
    }

    void RunComputeShader()
    {
        kernelComputeDensityPressure = computeShader.FindKernel("ComputeDensityPressure");
        kernelComputeForces = computeShader.FindKernel("ComputeForces");
        kernelIntegrate = computeShader.FindKernel("Integrate");

        computeShader.SetInt("particleLength", numParticles);
        computeShader.SetFloat("particleMass", particleMass);
        computeShader.SetFloat("viscosity", Viscosity);
        computeShader.SetFloat("gasConstant", GasConstant);
        computeShader.SetFloat("restDensity", RestingDensity);
        computeShader.SetFloat("boundDamping", BoundDamping);

        computeShader.SetFloat("radius", ParticleRadius);
        computeShader.SetFloat("radius2", ParticleRadius * ParticleRadius);
        computeShader.SetFloat("radius3", ParticleRadius * ParticleRadius * ParticleRadius);
        computeShader.SetFloat("radius4", ParticleRadius * ParticleRadius * ParticleRadius * ParticleRadius);
        computeShader.SetFloat("radius5", ParticleRadius * ParticleRadius * ParticleRadius * ParticleRadius * ParticleRadius);

        computeShader.SetFloat("pi", Mathf.PI);
        computeShader.SetFloat("densityWeightConstant", 0.00497359197162172924277761760539f);
        computeShader.SetFloat("spikyGradient", -0.09947183943243458485555235210782f);
        computeShader.SetFloat("viscLaplacian", 0.39788735772973833942220940843129f);

        computeShader.SetFloat("timestep", deltaTime);
        computeShader.SetVector("boxSize", boxSize);
        computeShader.SetFloat("gravity", Gravity);
        computeShader.SetVector("windSpeed", windSpeed);

        computeShader.SetBuffer(kernelComputeDensityPressure, "_particles", particleBuffer);
        computeShader.SetBuffer(kernelComputeForces, "_particles", particleBuffer);
        computeShader.SetBuffer(kernelIntegrate, "_particles", particleBuffer);
     

        int threadGroups = Mathf.CeilToInt(numParticles / 100.0f);

        computeShader.Dispatch(kernelComputeDensityPressure, threadGroups, 1, 1);
        computeShader.Dispatch(kernelComputeForces, threadGroups, 1, 1);
        computeShader.Dispatch(kernelIntegrate, threadGroups, 1, 1);

        particleBuffer.GetData(particles);
        System.Array.Copy(particles, renderParticles, numParticlesToDisplay);
        renderBuffer.SetData(renderParticles);

        material.SetFloat(SizeProperty, 16.0f);
        material.SetBuffer(ParticlesBufferProperty, renderBuffer);
    }

   
    void OnDestroy()
    {
        if (particleBuffer != null)
            particleBuffer.Release();
        if (renderBuffer != null)
            renderBuffer.Release();
        if (argsBuffer != null)
            argsBuffer.Release();
    }

    //void DetectCollisions()
    //{
    //    List<GameObject> triangles = MeshTriangulator.GetTriangles();

    //    foreach (var triangleGO in triangles)
    //    {
    //        Mesh mesh = triangleGO.GetComponent<MeshFilter>().mesh;
    //        Vector3[] vertices = mesh.vertices;
    //        int[] indices = mesh.triangles;

    //        Vector3 triangleNormal = triangleGO.GetComponent<TriangleCollider>().Normal;
    //        Vector3 trianglePosition = triangleGO.transform.position;

    //        for (int i = 0; i < particles.Length; i++)
    //        {
    //            Vector3 particlePosition = particles[i].position;

    //            for (int j = 0; j < indices.Length; j += 3)
    //            {
    //                Vector3 v0 = vertices[indices[j]];
    //                Vector3 v1 = vertices[indices[j + 1]];
    //                Vector3 v2 = vertices[indices[j + 2]];


    //                v0 = triangleGO.transform.TransformPoint(v0);
    //                v1 = triangleGO.transform.TransformPoint(v1);
    //                v2 = triangleGO.transform.TransformPoint(v2);


    //                if (IsParticleOnPlane(triangleNormal, v0, particlePosition))
    //                {

    //                    if (IsPointInTriangle(v0, v1, v2, particlePosition))
    //                    {
    //                        //Debug.Log("Collision detected between particle and triangle.");

    //                        particles[i].velocity = Vector3.Reflect(particles[i].velocity, triangleNormal);


    //                        particles[i].position += triangleNormal * particleRadius * boundDamping;
    //                        particles[i].color = new Vector4(1, 0, 0, 1);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}


    //public void DetectCollisions()
    //{
    //    List<GameObject> triangles = MeshTriangulator.GetTriangles();

    //    foreach (var triangleGO in triangles)
    //    {
    //        Mesh mesh = triangleGO.GetComponent<MeshFilter>().mesh;
    //        Vector3[] vertices = mesh.vertices;
    //        int[] indices = mesh.triangles;

    //        Vector3 triangleNormal = triangleGO.GetComponent<TriangleCollider>().Normal;
    //        Vector3 trianglePosition = triangleGO.transform.position;

    //        for (int j = 0; j < indices.Length; j += 3)
    //        {
    //            Vector3 v0 = vertices[indices[j]];
    //            Vector3 v1 = vertices[indices[j + 1]];
    //            Vector3 v2 = vertices[indices[j + 2]];

    //            v0 = triangleGO.transform.TransformPoint(v0);
    //            v1 = triangleGO.transform.TransformPoint(v1);
    //            v2 = triangleGO.transform.TransformPoint(v2);

    //            Vector3 triangleCenter = (v0 + v1 + v2) / 3;
    //            float searchRadius = Vector3.Distance(v0, v2); 


    //            List<int> nearbyParticleIndices = kdTree.RangeSearchIndices(triangleCenter, searchRadius);

    //            foreach (int particleIndex in nearbyParticleIndices)
    //            {
    //                Particle particle = particles[particleIndex];
    //                Vector3 particlePosition = particle.position;

    //                if (IsParticleOnPlane(triangleNormal, v0, particlePosition))
    //                {
    //                    if (IsPointInTriangle(v0, v1, v2, particlePosition))
    //                    {
    //                        //Debug.Log("Collision detected between particle and triangle 22.");

    //                        particle.velocity = Vector3.Reflect(particle.velocity, triangleNormal) * boundDamping;
    //                        //particle.position += triangleNormal * particleRadius * boundDamping;
    //                        particle.position += triangleNormal * particleRadius;
    //                        //particle.color = new Vector4(0, 0, 1, 1);

    //                        // Directly update the particle in the main array
    //                        particles[particleIndex] = particle;
    //                    }
    //                }
    //            }
    //        }
    //    }


    //    particleBuffer.SetData(particles);
    //}


    public void DetectCollisions()
    {
    
        Transform meshTransform = meshGameObject.transform; 
        MeshFilter meshFilter = meshTransform.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] indices = mesh.triangles;
        Vector3[] normals = mesh.normals;

        for (int i = 0; i < indices.Length; i += 3)
        {
            Vector3 v0 = vertices[indices[i]];
            Vector3 v1 = vertices[indices[i + 1]];
            Vector3 v2 = vertices[indices[i + 2]];

            v0 = meshTransform.TransformPoint(v0);
            v1 = meshTransform.TransformPoint(v1);
            v2 = meshTransform.TransformPoint(v2);

            Vector3 triangleCenter = (v0 + v1 + v2) / 3;
            float searchRadius = Vector3.Distance(v0, v2);

            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;
            Vector3 triangleNormal = Vector3.Cross(edge1, edge2).normalized;

            // Assuming you have a method to get nearby particle indices
            List<int> nearbyParticleIndices = kdTree.RangeSearchIndices(triangleCenter, searchRadius);

            foreach (int particleIndex in nearbyParticleIndices)
            {
                Particle particle = particles[particleIndex];
                Vector3 particlePosition = particle.position;

                if (IsParticleOnPlane(triangleNormal, v0, particlePosition))
                {
                    if (IsPointInTriangle(v0, v1, v2, particlePosition))
                    {
                        particle.velocity = Vector3.Reflect(particle.velocity, triangleNormal) * BoundDamping;
                        particle.position += triangleNormal * ParticleRadius;
                        particles[particleIndex] = particle; // Update the particle in the main array
                    }
                }
            }
        }

        particleBuffer.SetData(particles);
    }

    public class ParticleComparer : IComparer<Particle>
    {
        private int axis;

        public ParticleComparer(int axis)
        {
            this.axis = axis;
        }

        public int Compare(Particle a, Particle b)
        {
            float aValue = axis == 0 ? a.position.x : axis == 1 ? a.position.y : a.position.z;
            float bValue = axis == 0 ? b.position.x : axis == 1 ? b.position.y : b.position.z;
            return aValue.CompareTo(bValue);
        }
    }



    bool IsParticleOnPlane(Vector3 normal, Vector3 vertex, Vector3 particlePosition)
    {
        float distance = Vector3.Dot(normal, particlePosition - vertex);
        return Mathf.Abs(distance) <= ParticleRadius;
    }

    bool IsPointInTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 point)
    {
        Vector3 u = v1 - v0;
        Vector3 v = v2 - v0;
        Vector3 w = point - v0;

        float uu = Vector3.Dot(u, u);
        float uv = Vector3.Dot(u, v);
        float vv = Vector3.Dot(v, v);
        float wu = Vector3.Dot(w, u);
        float wv = Vector3.Dot(w, v);

        float denominator = uv * uv - uu * vv;
        float s = (uv * wv - vv * wu) / denominator;
        float t = (uv * wu - uu * wv) / denominator;

        return (s >= 0) && (t >= 0) && (s + t <= 1);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);

        if (!Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(spawnBoxCenter, spawnBox);
        }

        if (kdTree != null)
        {
            kdTree.DrawTreeGizmos();
        }


    }

}


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
    public Vector3Int numToSpawn = new Vector3Int(10, 10, 10);
    private int numParticles;
    public Vector3 spawnBoxCenter = new Vector3(0, 3, 0);
    public Vector3 spawnBox = new Vector3(4, 2, 1.5f);
    public Vector3 boxSize;
    public float particleRadius = 0.1f;
    public float boundDamping = -0.3f;
    public float viscosity = -0.003f;
    public float particleMass = 1f;
    public float gasConstant = 2f;
    public float restingDensity = 1f;
    public float deltaTime = 0.0001f;
    public float gravity = -9.8f;
    private Particle[] particles;
    private ComputeBuffer particleBuffer;
    private ComputeBuffer renderBuffer;
    private ComputeBuffer argsBuffer;
    private Bounds bounds;
    private static readonly int SizeProperty = Shader.PropertyToID("_size");
    private static readonly int ParticlesBufferProperty = Shader.PropertyToID("_particles");

    private int kernelComputeDensityPressure;
    private int kernelComputeForces;
    private int kernelIntegrate;

    private Particle[] renderParticles;

    public Vector3 windSpeed = new Vector3(0, 0, 0);

    // Reference to the MeshTriangulator to access the triangles
    public MeshTriangulator meshTriangulator;

    void Awake()
    {
        InitParticles();
        InitBuffers();
        bounds = new Bounds(Vector3.zero, boxSize);
    }

    //void InitParticles()
    //{
    //    Vector3 spawnTopLeft = spawnBoxCenter - spawnBox / 2;
    //    List<Particle> _particles = new List<Particle>();

    //    for (int x = 0; x < numToSpawn.x; x++)
    //    {
    //        for (int y = 0; y < numToSpawn.y; y++)
    //        {
    //            for (int z = 0; z < numToSpawn.z; z++)
    //            {
    //                Vector3 spawnPosition = spawnTopLeft + new Vector3(x * particleRadius * 2, y * particleRadius * 2, z * particleRadius * 2) + Random.onUnitSphere * particleRadius * 0.1f;
    //                Particle p = new Particle
    //                {
    //                    position = spawnPosition
    //                };

    //                _particles.Add(p);
    //            }
    //        }
    //    }
    //    numParticles = _particles.Count;
    //    renderParticles = new Particle[numParticlesToDisplay];
    //    particleBuffer = new ComputeBuffer(numParticles, 44);
    //    particleBuffer.SetData(_particles.ToArray());
    //    particles = _particles.ToArray();
    //}
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
                    Vector3 spawnPosition = spawnTopLeft + new Vector3(x * particleRadius * 2, y * particleRadius * 2, z * particleRadius * 2) + Random.onUnitSphere * particleRadius * 0.1f;
                    Particle p = new Particle
                    {
                        position = spawnPosition,
                        color = new Vector4(1, 1, 1, 1)
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
    }


    void InitBuffers()
    {
        uint[] args = { particleMesh.GetIndexCount(0), (uint)numParticles, particleMesh.GetIndexStart(0), particleMesh.GetBaseVertex(0), 0 };
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);
        renderBuffer = new ComputeBuffer(numParticlesToDisplay, 60);

        RunComputeShader();
    }

    private void FixedUpdate()
    {
        // RunComputeShader();
        // computeShader.SetFloat("timestep", deltaTime);
        // computeShader.SetVector("boxSize", boxSize);

        // computeShader.Dispatch(kernelComputeDensityPressure, numParticles / 100, 1, 1);
        // computeShader.Dispatch(kernelComputeForces, numParticles / 100, 1, 1);
        // computeShader.Dispatch(kernelIntegrate, numParticles / 100, 1, 1);

        // material.SetFloat(SizeProperty, 16.0f);
        // material.SetBuffer(ParticlesBufferProperty, particleBuffer);

        // Graphics.DrawMeshInstancedIndirect(particleMesh, 0, material, bounds, argsBuffer, castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);
    }

    void Update()
    {
        RunComputeShader();
        DetectCollisions();

        particleBuffer.SetData(particles);
        renderBuffer.SetData(renderParticles);

        Graphics.DrawMeshInstancedIndirect(particleMesh, 0, material, bounds, argsBuffer, castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);
    }


    //void RunComputeShader()
    //{
    //    kernelComputeDensityPressure = computeShader.FindKernel("ComputeDensityPressure");
    //    kernelComputeForces = computeShader.FindKernel("ComputeForces");
    //    kernelIntegrate = computeShader.FindKernel("Integrate");

    //    computeShader.SetInt("particleLength", numParticles);
    //    Debug.Log(numParticles);
    //    computeShader.SetFloat("particleMass", particleMass);
    //    computeShader.SetFloat("viscosity", viscosity);
    //    computeShader.SetFloat("gasConstant", gasConstant);
    //    computeShader.SetFloat("restDensity", restingDensity);
    //    computeShader.SetFloat("boundDamping", boundDamping);

    //    computeShader.SetFloat("radius", particleRadius);
    //    computeShader.SetFloat("radius2", particleRadius * particleRadius);
    //    computeShader.SetFloat("radius3", particleRadius * particleRadius * particleRadius);
    //    computeShader.SetFloat("radius4", particleRadius * particleRadius * particleRadius * particleRadius);
    //    computeShader.SetFloat("radius5", particleRadius * particleRadius * particleRadius * particleRadius * particleRadius);

    //    computeShader.SetFloat("pi", Mathf.PI);
    //    computeShader.SetFloat("densityWeightConstant", 0.00497359197162172924277761760539f);
    //    computeShader.SetFloat("spikyGradient", -0.09947183943243458485555235210782f);
    //    computeShader.SetFloat("viscLaplacian", 0.39788735772973833942220940843129f);

    //    computeShader.SetFloat("timestep", deltaTime);
    //    computeShader.SetVector("boxSize", boxSize);
    //    computeShader.SetVector("windSpeed", windSpeed);

    //    computeShader.SetBuffer(kernelComputeDensityPressure, "_particles", particleBuffer);
    //    computeShader.SetBuffer(kernelComputeForces, "_particles", particleBuffer);
    //    computeShader.SetBuffer(kernelIntegrate, "_particles", particleBuffer);

    //    int threadGroups = Mathf.CeilToInt(numParticles / 100.0f);

    //    computeShader.Dispatch(kernelComputeDensityPressure, threadGroups, 1, 1);
    //    computeShader.Dispatch(kernelComputeForces, threadGroups, 1, 1);
    //    computeShader.Dispatch(kernelIntegrate, threadGroups, 1, 1);

    //    particleBuffer.GetData(particles);
    //    System.Array.Copy(particles, renderParticles, numParticlesToDisplay);
    //    renderBuffer.SetData(renderParticles);

    //    material.SetFloat(SizeProperty, 16.0f);
    //    material.SetBuffer(ParticlesBufferProperty, renderBuffer);
    //}
    void RunComputeShader()
    {
        kernelComputeDensityPressure = computeShader.FindKernel("ComputeDensityPressure");
        kernelComputeForces = computeShader.FindKernel("ComputeForces");
        kernelIntegrate = computeShader.FindKernel("Integrate");

        computeShader.SetInt("particleLength", numParticles);
        computeShader.SetFloat("particleMass", particleMass);
        computeShader.SetFloat("viscosity", viscosity);
        computeShader.SetFloat("gasConstant", gasConstant);
        computeShader.SetFloat("restDensity", restingDensity);
        computeShader.SetFloat("boundDamping", boundDamping);

        computeShader.SetFloat("radius", particleRadius);
        computeShader.SetFloat("radius2", particleRadius * particleRadius);
        computeShader.SetFloat("radius3", particleRadius * particleRadius * particleRadius);
        computeShader.SetFloat("radius4", particleRadius * particleRadius * particleRadius * particleRadius);
        computeShader.SetFloat("radius5", particleRadius * particleRadius * particleRadius * particleRadius * particleRadius);

        computeShader.SetFloat("pi", Mathf.PI);
        computeShader.SetFloat("densityWeightConstant", 0.00497359197162172924277761760539f);
        computeShader.SetFloat("spikyGradient", -0.09947183943243458485555235210782f);
        computeShader.SetFloat("viscLaplacian", 0.39788735772973833942220940843129f);

        computeShader.SetFloat("timestep", deltaTime);
        computeShader.SetVector("boxSize", boxSize);
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





    void DetectCollisions()
    {
        List<GameObject> triangles = MeshTriangulator.GetTriangles();

        foreach (var triangleGO in triangles)
        {
            Mesh mesh = triangleGO.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            int[] indices = mesh.triangles;

            Vector3 triangleNormal = triangleGO.GetComponent<TriangleCollider>().Normal;
            Vector3 trianglePosition = triangleGO.transform.position;

            for (int i = 0; i < particles.Length; i++)
            {
                Vector3 particlePosition = particles[i].position;

                for (int j = 0; j < indices.Length; j += 3)
                {
                    Vector3 v0 = vertices[indices[j]];
                    Vector3 v1 = vertices[indices[j + 1]];
                    Vector3 v2 = vertices[indices[j + 2]];


                    v0 = triangleGO.transform.TransformPoint(v0);
                    v1 = triangleGO.transform.TransformPoint(v1);
                    v2 = triangleGO.transform.TransformPoint(v2);


                    if (IsParticleOnPlane(triangleNormal, v0, particlePosition))
                    {

                        if (IsPointInTriangle(v0, v1, v2, particlePosition))
                        {
                            Debug.Log("Collision detected between particle and triangle.");

                            particles[i].velocity = Vector3.Reflect(particles[i].velocity, triangleNormal);


                            particles[i].position += triangleNormal * particleRadius * boundDamping;
                            particles[i].color = new Vector4(1, 0, 0, 1);
                        }
                    }
                }
            }
        }
    }

    bool IsParticleOnPlane(Vector3 normal, Vector3 vertex, Vector3 particlePosition)
    {
        float distance = Vector3.Dot(normal, particlePosition - vertex);
        return Mathf.Abs(distance) <= particleRadius;
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);

        if (!Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(spawnBoxCenter, spawnBox);
        }
    }
}

 







// using System.Runtime.InteropServices;
// using System.Collections.Generic;
// using UnityEngine;
// [System.Serializable]
//[StructLayout(LayoutKind.Sequential, Size = 44)]
//public struct Particle
//{
//    public float pressure; // 4
//    public float density; // 8
//    public Vector3 currentForce; // 20
//    public Vector3 velocity; // 32
//    public Vector3 position; // 44
//}
//public class spawner : MonoBehaviour
//{
//    public ComputeShader computeShader;
//    public Mesh particleMesh;
//    public Material material;
//    public Vector3Int numToSpawn = new Vector3Int(10, 10, 10);
//    private int numParticles;
//    public Vector3 spawnBoxCenter = new Vector3(0, 3, 0);
//    public Vector3 spawnBox = new Vector3(4, 2, 1.5f);
//    // public Transform spawnArea;
//    public Vector3 boxSize;
//    public float particleRadius = 0.1f;
//    public float boundDamping = -0.3f;
//    public float viscosity = -0.003f;
//    public float particleMass = 1f;
//    public float gasConstant = 2f;
//    public float restingDensity = 1f;
//    public float deltaTime = 0.0001f;
//    public float gravity = -9.8f;
//    private Particle[] particles;
//    private ComputeBuffer particleBuffer;
//    private ComputeBuffer argsBuffer;
//    private Bounds bounds;
//    private static readonly int SizeProperty = Shader.PropertyToID("_size");
//    private static readonly int ParticlesBufferProperty = Shader.PropertyToID("_particles");

//    private int kernelComputeDensityPressure;
//    private int kernelComputeForces;
//    private int kernelIntegrate;

//    void Awake()
//    {


//        InitParticles();
//        InitBuffers();
//        bounds = new Bounds(Vector3.zero, boxSize);
//    }

//    void InitParticles()
//    {
//        Vector3 spawnTopLeft = spawnBoxCenter - spawnBox / 2;
//        List<Particle> _particles = new List<Particle>();

//        for (int x = 0; x < numToSpawn.x; x++)
//        {
//            for (int y = 0; y < numToSpawn.y; y++)
//            {
//                for (int z = 0; z < numToSpawn.z; z++)
//                {
//                    Vector3 spawnPosition = spawnTopLeft + new Vector3(x * particleRadius * 2, y * particleRadius * 2, z * particleRadius * 2) + Random.onUnitSphere * particleRadius * 0.1f;
//                    Particle p = new Particle
//                    {
//                        position = spawnPosition
//                    };

//                    _particles.Add(p);
//                }
//            }
//        }
//        numParticles = _particles.Count;
//        // Debug.Log(numParticles);
//        particleBuffer = new ComputeBuffer(numParticles, 44);
//        particleBuffer.SetData(_particles.ToArray());
//        particles = _particles.ToArray();
//    }

//    void InitBuffers()
//    {
//        uint[] args = { particleMesh.GetIndexCount(0), (uint)numParticles, particleMesh.GetIndexStart(0), particleMesh.GetBaseVertex(0), 0 };
//        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
//        argsBuffer.SetData(args);

//        RunComputeShader();
//    }

//    private void FixedUpdate()
//    {
//        // RunComputeShader();
//        // computeShader.SetFloat("timestep", deltaTime);
//        // computeShader.SetVector("boxSize", boxSize);


//        // computeShader.Dispatch(kernelComputeDensityPressure, numParticles / 100, 1, 1);
//        // computeShader.Dispatch(kernelComputeForces, numParticles / 100, 1, 1);
//        // computeShader.Dispatch(kernelIntegrate, numParticles / 100, 1, 1);

//        // material.SetFloat(SizeProperty, 16.0f);
//        // material.SetBuffer(ParticlesBufferProperty, particleBuffer);

//        // Graphics.DrawMeshInstancedIndirect(particleMesh, 0, material, bounds, argsBuffer, castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);
//    }

//    private void Update()
//    {
//        RunComputeShader();
//        // particleBuffer.GetData(particles);
//        // for (int i = 0; i < particles.Length; i++)
//        // {
//        //  Debug.Log("density "+i+" is :"+particles[i].density);   
//        // }

//        Graphics.DrawMeshInstancedIndirect(particleMesh, 0, material, bounds, argsBuffer, castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);
//    }

//    void RunComputeShader()
//    {
//        kernelComputeDensityPressure = computeShader.FindKernel("ComputeDensityPressure");
//        kernelComputeForces = computeShader.FindKernel("ComputeForces");
//        kernelIntegrate = computeShader.FindKernel("Integrate");
//        computeShader.SetInt("particleLength", numParticles);
//        computeShader.SetFloat("particleMass", particleMass);
//        computeShader.SetFloat("viscosity", viscosity);
//        computeShader.SetFloat("gasConstant", gasConstant);
//        computeShader.SetFloat("restDensity", restingDensity);
//        computeShader.SetFloat("boundDamping", boundDamping);

//        computeShader.SetFloat("radius", particleRadius);
//        computeShader.SetFloat("radius2", particleRadius * particleRadius);
//        computeShader.SetFloat("radius3", particleRadius * particleRadius * particleRadius);
//        computeShader.SetFloat("radius4", particleRadius * particleRadius * particleRadius * particleRadius);
//        computeShader.SetFloat("radius5", particleRadius * particleRadius * particleRadius * particleRadius * particleRadius);

//        computeShader.SetFloat("pi", Mathf.PI);
//        computeShader.SetFloat("densityWeightConstant", 0.00497359197162172924277761760539f);
//        computeShader.SetFloat("spikyGradient", -0.09947183943243458485555235210782f);
//        computeShader.SetFloat("viscLaplacian", 0.39788735772973833942220940843129f);


//        computeShader.SetVector("boxSize", boxSize);

//        computeShader.SetFloat("timestep", deltaTime);

//        computeShader.SetBuffer(kernelComputeDensityPressure, "_particles", particleBuffer);
//        computeShader.SetBuffer(kernelComputeForces, "_particles", particleBuffer);
//        computeShader.SetBuffer(kernelIntegrate, "_particles", particleBuffer);


//        int threadGroups = Mathf.CeilToInt(numParticles / 100.0f);

//        computeShader.Dispatch(kernelComputeDensityPressure, numParticles, 1, 1);
//        computeShader.Dispatch(kernelComputeForces, numParticles, 1, 1);
//        computeShader.Dispatch(kernelIntegrate, numParticles, 1, 1);

//        material.SetFloat(SizeProperty, 16.0f);
//        material.SetBuffer(ParticlesBufferProperty, particleBuffer);
//    }

//    // void OnDestroy()
//    // {
//    //     if (particleBuffer != null)
//    //     {
//    //         particleBuffer.Release();
//    //     }

//    //     if (argsBuffer != null)
//    //     {
//    //         argsBuffer.Release();
//    //     }
//    // }
//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.blue;
//        Gizmos.DrawWireCube(Vector3.zero, boxSize);

//        if (!Application.isPlaying)
//        {
//            Gizmos.color = Color.cyan;
//            Gizmos.DrawWireCube(spawnBoxCenter, spawnBox);
//        }
//    }
//}

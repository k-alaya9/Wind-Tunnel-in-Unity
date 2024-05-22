using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[StructLayout(LayoutKind.Sequential, Size = 44)]
public struct Particle
{
    public float pressure;
    public float density;
    public Vector3 currentForce;
    public Vector3 velocity;
    public Vector3 position;
}
public class spawner : MonoBehaviour
{   

    public int numParticlesToDisplay=100;
    public ComputeShader computeShader;
    public Mesh particleMesh;
    public Material material;
   public Vector3Int numToSpawn = new Vector3Int(10,10,10);
   private int numParticles;
    public Vector3 spawnBoxCenter = new Vector3(0,3,0);
    public Vector3 spawnBox = new Vector3(4,2,1.5f);
    public Vector3 boxSize;
    public float particleRadius = 0.1f;
    public float boundDamping = -0.3f;
    public float viscosity = -0.003f;
    public float particleMass = 1f;
    public float gasConstant = 2f; 
    public float restingDensity = 1f; 
    public float deltaTime = 0.0001f;
    public float gravity=-9.8f;
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

    void Awake()
    {
        InitParticles();
        InitBuffers();
        bounds = new Bounds(Vector3.zero, boxSize);
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
                    Vector3 spawnPosition = spawnTopLeft + new Vector3(x * particleRadius * 2, y * particleRadius * 2, z * particleRadius * 2) + Random.onUnitSphere * particleRadius * 0.1f;
                    Particle p = new Particle
                    {
                        position = spawnPosition
                    };

                    _particles.Add(p);
                }
            }
        }
        numParticles=_particles.Count;
        renderParticles = new Particle[numParticlesToDisplay];
        particleBuffer = new ComputeBuffer(numParticles, 44);
        particleBuffer.SetData(_particles.ToArray());
        particles = _particles.ToArray();
    }

    void InitBuffers()
    {
        uint[] args = { particleMesh.GetIndexCount(0), (uint)numParticles, particleMesh.GetIndexStart(0), particleMesh.GetBaseVertex(0), 0 };
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);
          renderBuffer = new ComputeBuffer(numParticlesToDisplay, 44); 

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

    private void Update()
    {

        RunComputeShader();
        // particleBuffer.GetData(particles);
        // for (int i = 0; i < particles.Length; i++)
        // {
        //  Debug.Log("density "+i+" is :"+particles[i].density);   
        // }
        
        Graphics.DrawMeshInstancedIndirect(particleMesh, 0, material, bounds, argsBuffer, castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);
    }

    void RunComputeShader()
    {   
        kernelComputeDensityPressure = computeShader.FindKernel("ComputeDensityPressure");
        kernelComputeForces = computeShader.FindKernel("ComputeForces");
        kernelIntegrate = computeShader.FindKernel("Integrate");

        computeShader.SetFloat("gravity",gravity);
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
        computeShader.SetFloat("pi", 3.1415926535897932384626433832795028841971f);

        computeShader.SetFloat("timestep", deltaTime);
        computeShader.SetVector("boxSize", boxSize);

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
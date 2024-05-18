// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// public class spawner : MonoBehaviour
// {
//     public GameObject particlePrefab;
//     public int numParticles = 100;
//     public float particleSpacing = 0.2f;
//     public float smoothingRadius = 0.4f;
//     public float stiffness = 1.0f;
//     public float restDensity = 1.0f;
//     public float viscosity = 0.1f;
//     public float gravity = 9.81f;
//     public Transform spawnArea;
//     private GameObject[] particles;
//     private Vector3[] positions;
//     private Vector3[] velocities;
//     private Vector2[] densities;

//     void Start()
//     {
//         particles = new GameObject[numParticles];
//         positions = new Vector3[numParticles];
//         velocities = new Vector3[numParticles];
//         densities = new Vector2[numParticles];

//         // Spawn particles
//         for (int i = 0; i < numParticles; i++)
//         {
//             Vector3 spawnPos = spawnArea.position + Random.insideUnitSphere * particleSpacing;
//             particles[i] = Instantiate(particlePrefab, spawnPos, Quaternion.identity);
//             positions[i] = spawnPos;
//             velocities[i] = Vector3.zero;
//             densities[i] = Vector2.zero;
//         }
//     }

//     void Update()
//     {
//         FixedUpdate();
//     }

//     void FixedUpdate()
//     {
//         // Calculate densities
//         for (int i = 0; i < numParticles; i++)
//         {
//             densities[i] = CalculateDensity(i);
//         }

//         // Calculate pressure forces and viscosity
//         for (int i = 0; i < numParticles; i++)
//         {
//             Vector3 pressureForce = CalculatePressureForce(i);
//             Vector3 viscosityForce = CalculateViscosity(i);
//             Vector3 gravityForce = Vector3.down * gravity;

//             // Apply forces
//             velocities[i] += (pressureForce + viscosityForce + gravityForce) * Time.fixedDeltaTime;
//             velocities[i].z=0;
//             velocities[i].y=0;
//         }

//         // Update positions
//         for (int i = 0; i < numParticles; i++)
//         {
//             positions[i] += velocities[i] * Time.fixedDeltaTime;
//             particles[i].transform.position = positions[i] ;
//         }
//     }

//     Vector2 CalculateDensity(int particleIndex)
//     {
//         float density = 0;

//         for (int i = 0; i < numParticles; i++)
//         {
//             if (i == particleIndex)
//                 continue;

//             float r = Vector3.Distance(positions[particleIndex], positions[i]);
//             if (r >= smoothingRadius)
//                 continue;

//             density += Mathf.Pow(smoothingRadius - r, 3);
//         }

//         density *= 315.0f / (64.0f * Mathf.PI * Mathf.Pow(smoothingRadius, 9));
//         return new Vector2(density, 0); // Return density and ignore near density for now
//     }

//     Vector3 CalculatePressureForce(int particleIndex)
//     {
//         float pressure = stiffness * (densities[particleIndex].x - restDensity);
//         Vector3 pressureForce = Vector3.zero;

//         for (int i = 0; i < numParticles; i++)
//         {
//             if (i == particleIndex)
//                 continue;

//             Vector3 dir = positions[particleIndex] - positions[i];
//             float r = dir.magnitude;
//             if (r >= smoothingRadius)
//                 continue;

//             pressureForce += dir.normalized * pressure * (1.0f / densities[i].x) * (-45.0f / (Mathf.PI * Mathf.Pow(smoothingRadius, 6))) * Mathf.Pow(smoothingRadius - r, 2);
//         }

//         return pressureForce;
//     }

//     Vector3 CalculateViscosity(int particleIndex)
//     {
//         Vector3 viscosityForce = Vector3.zero;

//         for (int i = 0; i < numParticles; i++)
//         {
//             if (i == particleIndex)
//                 continue;

//             Vector3 dir = positions[i] - positions[particleIndex];
//             float r = dir.magnitude;
//             if (r >= smoothingRadius)
//                 continue;

//             viscosityForce += viscosity * (velocities[i] - velocities[particleIndex]) * (1.0f / densities[i].x) * (45.0f / (Mathf.PI * Mathf.Pow(smoothingRadius, 6))) * (smoothingRadius - r);
//         }

//         return viscosityForce;
//     }
// }
    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public ComputeShader computeShader;
    public Mesh particleMesh;
    public Material material;
    public int numParticles = 1000;
    public float particleSpacing = 0.2f;
    public float smoothingRadius = 0.4f;
    public float stiffness = 1.0f;
    public float restDensity = 1.0f;
    public float viscosity = 0.1f;
    public float gravity = 9.81f;
    public Transform spawnArea;
    public Vector3 boxSize;

    private ComputeBuffer particleBuffer;
    private ComputeBuffer argsBuffer;
    private Bounds bounds;
    public float deltaTime=0.0001f;
    private static readonly int SizeProperty = Shader.PropertyToID("_size");
    private static readonly int ParticlesBufferProperty = Shader.PropertyToID("_particles");

    struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 currentForce;
        public float density;
        public float pressure;
    }

    void Awake()
    {
        InitParticles();
        InitBuffers();
        bounds = new Bounds(Vector3.zero, boxSize);
    }

    void InitParticles()
    {
        Particle[] particles = new Particle[numParticles];
        for (int i = 0; i < numParticles; i++)
        {
            Vector3 spawnPos = spawnArea.position + Random.insideUnitSphere * particleSpacing;
            particles[i] = new Particle
            {
                position = spawnPos,
                velocity= new Vector3(0.5f,0,0)
            };
        }

        particleBuffer = new ComputeBuffer(numParticles,44);
        particleBuffer.SetData(particles);

    }
private int Kernel;
    void InitBuffers()
    {
        Kernel = computeShader.FindKernel("CSMain");
        uint[] args ={ particleMesh.GetIndexCount(0), (uint)numParticles, particleMesh.GetIndexStart(0), particleMesh.GetBaseVertex(0),
        0 };
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);
        material.SetFloat(SizeProperty, 1.0f);
        material.SetBuffer(ParticlesBufferProperty, particleBuffer);
        RunComputeShader();
    }
    private void FixedUpdate(){
    RunComputeShader();
    computeShader.Dispatch(Kernel,numParticles, 1, 1);

    Graphics.DrawMeshInstancedIndirect(particleMesh, 0, material, bounds, argsBuffer, castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);

    }
    private void Update()
    {

    Graphics.DrawMeshInstancedIndirect(particleMesh, 0, material,bounds, argsBuffer, castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);

    }

    void RunComputeShader()
    {
        // Particle[] particles = new Particle[numParticles];
        // particleBuffer.GetData(particles);

        // for (int i = 0; i < numParticles; i++)
        // {
        //     Debug.Log("Particle " + i + " initial position: " + particles[i].position);
        // }
        computeShader.SetFloat("deltaTime",deltaTime);
        computeShader.SetFloat("smoothingRadius", smoothingRadius);
        computeShader.SetFloat("stiffness", stiffness);
        computeShader.SetFloat("restDensity", restDensity);
        computeShader.SetFloat("viscosity", viscosity);
        computeShader.SetVector("boxSize",boxSize);
        computeShader.SetVector("gravity", new Vector3(0, -gravity, 0));
        computeShader.SetBuffer(Kernel, "particles", particleBuffer);


        // particleBuffer.GetData(particles);
        // for (int i = 0; i < numParticles; i++)
        // {
        //     Debug.Log("Particle " + i + " updated position: " + particles[i].position);
        // }
    }

    void OnDestroy()
    {
            particleBuffer.Release();

            argsBuffer.Release();
        
    }
}

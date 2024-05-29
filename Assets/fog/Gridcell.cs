using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridCell : MonoBehaviour
{
    private List<Particle> particles = new List<Particle>();

    public void AddParticle(Particle particle)
    {
        particles.Add(particle);
    }

    public List<Particle> GetParticles()
    {
        return particles;
    }

    public void Clear()
    {
        particles.Clear();
    }
}
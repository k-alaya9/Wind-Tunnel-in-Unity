//using System.Collections.Generic;
//using UnityEngine;

//public class OctreeNode
//{
//    private Bounds bounds;
//    private List<GameObject> triangles;
//    private List<Particle> particles; // New: List of particles in this node
//    private OctreeNode[] children;
//    private int maxTriangles;
//    private int maxDepth;
//    private int depth;

//    public OctreeNode(Bounds bounds, int depth, int maxDepth, int maxTriangles)
//    {
//        this.bounds = bounds;
//        this.depth = depth;
//        this.maxDepth = maxDepth;
//        this.maxTriangles = maxTriangles;
//        this.triangles = new List<GameObject>();
//        this.particles = new List<Particle>(); // New: Initialize particle list
//    }

//    public void Insert(GameObject triangle)
//    {
//        if (depth >= maxDepth || (children == null && triangles.Count < maxTriangles))
//        {
//            triangles.Add(triangle);
//            return;
//        }

//        if (children == null)
//        {
//            Subdivide();
//        }

//        foreach (var child in children)
//        {
//            if (child.bounds.Intersects(triangle.GetComponent<Renderer>().bounds))
//            {
//                child.Insert(triangle);
//            }
//        }
//    }

//    public void Insert(Particle particle)
//    {
//        if (depth >= maxDepth || (children == null && particles.Count < maxTriangles))
//        {
//            particles.Add(particle);
//            return;
//        }

//        if (children == null)
//        {
//            Subdivide();
//        }

//        foreach (var child in children)
//        {
//            if (child.bounds.Contains(particle.position))
//            {
//                child.Insert(particle);
//            }
//        }
//    }

//    public void Subdivide()
//    {
//        children = new OctreeNode[8];
//        Vector3 size = bounds.size / 2f;
//        Vector3 center = bounds.center;

//        for (int i = 0; i < 8; i++)
//        {
//            Vector3 offset = new Vector3(
//                ((i & 1) == 0 ? -1 : 1) * size.x / 2f,
//                ((i & 2) == 0 ? -1 : 1) * size.y / 2f,
//                ((i & 4) == 0 ? -1 : 1) * size.z / 2f
//            );

//            children[i] = new OctreeNode(new Bounds(center + offset, size), depth + 1, maxDepth, maxTriangles);
//        }

//        foreach (var triangle in triangles)
//        {
//            foreach (var child in children)
//            {
//                if (child.bounds.Intersects(triangle.GetComponent<Renderer>().bounds))
//                {
//                    child.Insert(triangle);
//                }
//            }
//        }

//        triangles.Clear();
//    }

//    public List<GameObject> RetrievePotentialCollisions(Bounds particleBounds)
//    {
//        List<GameObject> potentialCollisions = new List<GameObject>();

//        if (!bounds.Intersects(particleBounds))
//        {
//            return potentialCollisions;
//        }

//        if (children == null)
//        {
//            potentialCollisions.AddRange(triangles);
//        }
//        else
//        {
//            foreach (var child in children)
//            {
//                potentialCollisions.AddRange(child.RetrievePotentialCollisions(particleBounds));
//            }
//        }

//        return potentialCollisions;
//    }

//    public List<Particle> RetrievePotentialParticles(Bounds particleBounds)
//    {
//        List<Particle> potentialParticles = new List<Particle>();

//        if (!bounds.Intersects(particleBounds))
//        {
//            return potentialParticles;
//        }

//        if (children == null)
//        {
//            potentialParticles.AddRange(particles);
//        }
//        else
//        {
//            foreach (var child in children)
//            {
//                potentialParticles.AddRange(child.RetrievePotentialParticles(particleBounds));
//            }
//        }

//        return potentialParticles;
//    }
//}

//public class Octree
//{
//    private OctreeNode root;

//    public Octree(Bounds bounds, int maxDepth, int maxTriangles)
//    {
//        root = new OctreeNode(bounds, 0, maxDepth, maxTriangles);
//    }

//    public void Insert(GameObject triangle)
//    {
//        root.Insert(triangle);
//    }

//    public void Insert(Particle particle)
//    {
//        root.Insert(particle);
//    }

//    public List<GameObject> RetrievePotentialCollisions(Bounds particleBounds)
//    {
//        return root.RetrievePotentialCollisions(particleBounds);
//    }

//    public List<Particle> RetrievePotentialParticles(Bounds particleBounds)
//    {
//        return root.RetrievePotentialParticles(particleBounds);
//    }
//}
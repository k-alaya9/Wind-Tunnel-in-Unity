using System.Collections.Generic;
using UnityEngine;
using System;
public class KDTree
{
    public class KDNode
    {
        public Particle Particle;
        public int Index; 
        public KDNode Left;
        public KDNode Right;
        public int Depth;
    }

    public KDNode Root;
    private int maxDepth;

    public KDTree(int maxDepth)
    {
        this.maxDepth = maxDepth;
    }

    public void Build(Particle[] particles, int depth = 0)
    {
        Root = BuildRecursive(particles, 0, particles.Length, 0);
        //Root = BuildRecursive(particles, depth);
    }

    private KDNode BuildRecursive(Particle[] particles, int start, int end, int depth)
    {
        if (start >= end)
            return null;

        int mid = (start + end) / 2;
        int axis = depth % 3;
 
        Array.Sort(particles, start, end - start, new ParticleComparer(axis));

        KDNode node = new KDNode
        {
            Particle = particles[mid],
            Index = mid, 
            Depth = depth,
            Left = BuildRecursive(particles, start, mid, depth + 1),
            Right = BuildRecursive(particles, mid + 1, end, depth + 1)
        };

        return node;
    }
    //private KDNode BuildRecursive(Particle[] particles, int depth)
    //{
    //    if (particles.Length == 0 || depth >= maxDepth)
    //        return null;

    //    int axis = depth % 3;
    //    var sortedParticles = SortParticles(particles, axis);
    //    int medianIndex = sortedParticles.Length / 2;

    //    KDNode node = new KDNode
    //    {
    //        Particle = sortedParticles[medianIndex],
    //        Depth = depth,
    //        Left = BuildRecursive(sortedParticles[0..medianIndex], depth + 1),
    //        Right = BuildRecursive(sortedParticles[(medianIndex + 1)..], depth + 1)
    //    };

    //    return node;
    //}

    private Particle[] SortParticles(Particle[] particles, int axis)
    {
        switch (axis)
        {
            case 0:
                System.Array.Sort(particles, (a, b) => a.position.x.CompareTo(b.position.x));
                break;
            case 1:
                System.Array.Sort(particles, (a, b) => a.position.y.CompareTo(b.position.y));
                break;
            case 2:
                System.Array.Sort(particles, (a, b) => a.position.z.CompareTo(b.position.z));
                break;
        }
        return particles;
    }

    //public List<Particle> RangeSearch(Vector3 center, float range)
    //{
    //    List<Particle> result = new List<Particle>();
    //    RangeSearchRecursive(Root, center, range * range, result);
    //    return result;
    //}

    //private void RangeSearchRecursive(KDNode node, Vector3 center, float rangeSquared, List<Particle> result)
    //{
    //    if (node == null)
    //        return;

    //    if ((node.Particle.position - center).sqrMagnitude <= rangeSquared)
    //        result.Add(node.Particle);

    //    int axis = node.Depth % 3;
    //    float centerAxisValue = axis == 0 ? center.x : axis == 1 ? center.y : center.z;
    //    float nodeAxisValue = axis == 0 ? node.Particle.position.x : axis == 1 ? node.Particle.position.y : node.Particle.position.z;

    //    if (centerAxisValue - Mathf.Sqrt(rangeSquared) <= nodeAxisValue)
    //        RangeSearchRecursive(node.Left, center, rangeSquared, result);
    //    if (centerAxisValue + Mathf.Sqrt(rangeSquared) >= nodeAxisValue)
    //        RangeSearchRecursive(node.Right, center, rangeSquared, result);
    //}
    public List<int> RangeSearchIndices(Vector3 center, float range)
    {
        List<int> resultIndices = new List<int>();
        RangeSearchRecursiveIndices(Root, center, range * range, resultIndices);
        return resultIndices;
    }

    private void RangeSearchRecursiveIndices(KDNode node, Vector3 center, float rangeSquared, List<int> resultIndices)
    {
        if (node == null)
            return;

        if ((node.Particle.position - center).sqrMagnitude <= rangeSquared)
            resultIndices.Add(node.Index); // Add the index of the particle

        int axis = node.Depth % 3;
        float centerAxisValue = axis == 0 ? center.x : axis == 1 ? center.y : center.z;
        float nodeAxisValue = axis == 0 ? node.Particle.position.x : axis == 1 ? node.Particle.position.y : node.Particle.position.z;

        if (centerAxisValue - Mathf.Sqrt(rangeSquared) <= nodeAxisValue)
            RangeSearchRecursiveIndices(node.Left, center, rangeSquared, resultIndices);
        if (centerAxisValue + Mathf.Sqrt(rangeSquared) >= nodeAxisValue)
            RangeSearchRecursiveIndices(node.Right, center, rangeSquared, resultIndices);
    }





    // Method to draw the KD tree using Gizmos
    public void DrawTreeGizmos()
    {
        DrawNodeGizmos(Root);
    }

    private void DrawNodeGizmos(KDNode node)
    {
        if (node == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(node.Particle.position, 0.1f);

        if (node.Left != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(node.Particle.position, node.Left.Particle.position);
            DrawNodeGizmos(node.Left);
        }

        if (node.Right != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(node.Particle.position, node.Right.Particle.position);
            DrawNodeGizmos(node.Right);
        }

    }
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

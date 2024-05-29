using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class MeshTriangulator : MonoBehaviour
{
    private static Transform _meshTransform;
    private static MeshFilter _meshFilter;
    private static MeshRenderer _meshRenderer;
    private static Mesh _mesh;
    private static Vector3[] _v3Verts;
    private static Vector3[] _v3Normals;
    private static Vector2[] _v3Uvs;

    private static List<GameObject> _triangles = new List<GameObject>();

    public static void Triangulate(Transform _objTransform)
    {
        _meshTransform = _objTransform;
        _meshFilter = _meshTransform.GetComponent<MeshFilter>();
        _meshRenderer = _meshTransform.GetComponent<MeshRenderer>();
        _mesh = _meshFilter.mesh;
        _v3Verts = _mesh.vertices;
        _v3Normals = _mesh.normals;
        _v3Uvs = _mesh.uv;

        for (int intSubmeshIndex = 0, smCount = _mesh.subMeshCount; intSubmeshIndex < smCount; intSubmeshIndex++)
        {
            int[] _intTriangles = _mesh.GetTriangles(intSubmeshIndex);
            int _intNumberOfTriangles = _intTriangles.Length;

            for (int i = 0; i < _intNumberOfTriangles; i += 3)
            {
                Vector3[] _v3TriVertexs = new Vector3[3];
                Vector2[] _v3TriUvs = new Vector2[3];

                for (int n = 0; n < 3; n++)
                {
                    int index = _intTriangles[i + n];
                    _v3TriVertexs[n] = _v3Verts[index];
                    _v3TriUvs[n] = _v3Uvs[index];
                }

               
                Vector3 edge1 = _v3TriVertexs[1] - _v3TriVertexs[0];
                Vector3 edge2 = _v3TriVertexs[2] - _v3TriVertexs[0];
                Vector3 triangleNormal = Vector3.Cross(edge1, edge2).normalized;

                Mesh _mTriMesh = new Mesh();
                _mTriMesh.vertices = _v3TriVertexs;
                _mTriMesh.normals = new Vector3[] { triangleNormal, triangleNormal, triangleNormal }; // Use the calculated normal for all vertices
                _mTriMesh.uv = _v3TriUvs;
                _mTriMesh.triangles = new int[] { 0, 1, 2 };

                GameObject _goNewTriangle = new GameObject(new StringBuilder().Append("Triangle").Append((i / 3) + intSubmeshIndex).ToString());
                _goNewTriangle.transform.position = _meshTransform.position;
                _goNewTriangle.transform.rotation = _meshTransform.rotation;
                _goNewTriangle.transform.localScale = _meshTransform.localScale;
                _goNewTriangle.AddComponent<MeshRenderer>().material = _meshRenderer.materials[intSubmeshIndex];

                Material coloredMaterial = new Material(Shader.Find("Standard"));
                Color triangleColor = new Color(Random.value, Random.value, Random.value);
                coloredMaterial.color = triangleColor;
                _goNewTriangle.GetComponent<MeshRenderer>().material = coloredMaterial;

                _goNewTriangle.AddComponent<MeshFilter>().mesh = _mTriMesh;

                
                _goNewTriangle.AddComponent<DrawNormal>();


                _goNewTriangle.AddComponent<TriangleCollider>().Normal = triangleNormal;

                _triangles.Add(_goNewTriangle);
            }
        }
    }

    public static List<GameObject> GetTriangles()
    {
        return _triangles;
    }
}

public class TriangleCollider : MonoBehaviour
{
    public Vector3 Normal;

    void OnCollisionEnter(Collision collision)
    {
        // Handle collision using the normal
        Debug.Log("Collision detected with normal: " + Normal);
    }
} 

public class DrawNormal : MonoBehaviour
{
    private Mesh mesh;

    void OnDrawGizmos()
    {
        if (mesh == null)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                mesh = meshFilter.sharedMesh;
            }
        }

        if (mesh != null)
        {
            Gizmos.color = Color.red;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 vertex0 = transform.TransformPoint(vertices[triangles[i]]);
                Vector3 vertex1 = transform.TransformPoint(vertices[triangles[i + 1]]);
                Vector3 vertex2 = transform.TransformPoint(vertices[triangles[i + 2]]);

                Vector3 faceCenter = (vertex0 + vertex1 + vertex2) / 3;
                Vector3 normal = transform.TransformDirection(normals[triangles[i]]);

                Gizmos.DrawLine(faceCenter, faceCenter + normal * 0.1f);
            }
        }
    }
}

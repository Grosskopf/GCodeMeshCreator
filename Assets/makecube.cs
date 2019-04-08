using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makecube : MonoBehaviour
{
    List<Vector3> newVertices =new List<Vector3>();
    List<Vector2> newUV = new List<Vector2>();
    List<int> newTriangles = new List<int>();
    public MeshFilter filter;

    void Start()
    {
        newVertices.Add(new Vector3(-1, -1, 0));
        newVertices.Add(new Vector3(-1, 1, 0));
        newVertices.Add(new Vector3(1, 1, 0));
        newVertices.Add(new Vector3(1, -1, 0));
        /*newUV.Add(new Vector2(0, 0));
        newUV.Add(new Vector2(0, 1));
        newUV.Add(new Vector2(1, 1));
        newUV.Add(new Vector2(1, 0));*/
        newTriangles.Add(0);
        newTriangles.Add(1);
        newTriangles.Add(3);
        newTriangles.Add(1);
        newTriangles.Add(2);
        newTriangles.Add(3);
        Mesh mesh = new Mesh();
        filter.mesh = mesh;
        mesh.vertices = newVertices.ToArray();// mesh.uv = newUV.ToArray();
        mesh.triangles = newTriangles.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

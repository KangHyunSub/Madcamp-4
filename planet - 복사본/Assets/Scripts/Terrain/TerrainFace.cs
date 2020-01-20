using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    ShapeGenerator shapeGenerator;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;
    public int chunkNumber = 4;
    MeshFilter[] meshFilters;
    GameObject parentObject;
    Material planetMaterial;

    public TerrainFace(ShapeGenerator shapeGenerator, GameObject parentObject, Material planetMaterial, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.parentObject = parentObject;
        this.resolution = resolution;
        this.localUp = localUp;
        this.planetMaterial = planetMaterial;

        
        if(parentObject.GetComponentsInChildren<MeshFilter>().Length != 0)
        {
            meshFilters = parentObject.GetComponentsInChildren<MeshFilter>();
        } 

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[chunkNumber * chunkNumber];
        }
        for (int i = 0; i < meshFilters.Length; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = parentObject.transform;

                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshObj.AddComponent<MeshRenderer>();
                meshObj.AddComponent<MeshCollider>();
                meshObj.tag = parentObject.tag;
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = planetMaterial;
        }

        for (int u = 0; u < chunkNumber; u++)
        {
            for (int v = 0; v < chunkNumber; v++)
            {
                ConstructMeshAtChunks(u,v, meshFilters[v + u * chunkNumber].sharedMesh);
                meshFilters[v + u * chunkNumber].GetComponent<MeshCollider>().sharedMesh = meshFilters[v + u * chunkNumber].sharedMesh;
            }
        }
       
    }

    public void ConstructMeshAtChunks(int u, int v, Mesh mesh)
    {
        Vector2 UVstep = new Vector2(1f / chunkNumber, 1f / chunkNumber);
        Vector2 step = new Vector2(UVstep.x / (resolution - 1), UVstep.y / (resolution - 1));
        Vector2 offset = new Vector3(-0.5f + u * UVstep.x, -0.5f + v * UVstep.y);

        Vector3[] vertices = new Vector3[resolution * resolution];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = offset + new Vector2(x * step.x , y * step.y);
                Vector3 pointOnUnitCube = percent.x * axisA + percent.y * axisB + localUp * 0.5f;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;

                    triIndex += 6;
                }
            }
        }

        Vector3[] flatShadedVertices = new Vector3[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            flatShadedVertices[i] = vertices[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShadedVertices;

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}

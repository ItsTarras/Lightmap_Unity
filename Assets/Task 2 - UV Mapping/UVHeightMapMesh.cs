using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class generates a mesh using a height map texture. The texture defines
 * the size of the mesh, and also the height of each vertex in the mesh. 
 * Additionally, a UV map is attached which will be used to texture the mesh
 *
 * You are required to fill in the code to calculate the size of the mesh,
 * generate the vertices at the right location and height as defined by the
 * height map texture, generate the UV coordinates for each vertex in the mesh,
 * as well as calculate the appropriate triangle indicies for the mesh, and finally
 * assign the UV texture to the material.
 * Several parts can be copied across from Lab 1.
 * 
 * PROD321 - Interactive Computer Graphics and Animation 
 * Copyright 2023, University of Canterbury
 * Written by Adrian Clark
 */

public class UVHeightMapMesh : MonoBehaviour
{
    // Defines the height map texture used to create the mesh and set the heights
    public Texture2D heightMapTexture;

    // Defines the uv map texture used to texture the mesh
    public Texture2D uvMapTexture;

    // Defines the height scale that we multiply the height of each vertex by
    public float heightScale = 30;

    // Start is called before the first frame update
    void Start()
    {
        // Create a list to store our vertices
        List<Vector3> vertices = new List<Vector3>();

        // Create a list to store our triangles
        List<int> triangles = new List<int>();

        // Create a list to store our UVs
        List<Vector2> uvs = new List<Vector2>();

        /**** 
        * 
        * TODO: Add code here to calculate length/width for grid
        * 
        ****/
        int meshLength = heightMapTexture.height;
        int meshWidth = heightMapTexture.width;


        // Generate our Vertices and UVS
        // Loop through the meshes length and width
        for (int z = 0; z < meshLength; z++)
        {
            for (int x = 0; x < meshWidth; x++)
            {
                //get the height value of the vertex
                float vertexHeight = heightMapTexture.GetPixel(x, z).r;

                //create a new vector.
                Vector3 newVector = new Vector3(x, vertexHeight * heightScale, z);

                //add the new vector to the vertices list.
                vertices.Add(newVector);


                uvs.Add(new Vector2((float)x / (float)(meshWidth), (float)z / (float)(meshLength)));
            }
        }

        // Generate our triangle Indicies
        // Loop through the meshes length-1 and width-1
        for (int z = 0; z < meshLength - 1; z++)
        {
            for (int x = 0; x < meshWidth - 1; x++)
            {
                /**** 
                * 
                * TODO: Change the variable initialisation code below to calculate 
                * the Top Left, Top Right, Bottom Right and Bottom Left vertex indices 
                * for the current triangles
                * 
                ****/
                int vTL = ((z) * meshWidth) + (x);
                int vTR = ((z) * meshWidth) + (x + 1);
                int vBL = ((z + 1) * meshWidth) + (x);
                int vBR = ((z + 1) * meshWidth) + (x + 1);

                // Create the two triangles which make each element in the quad
                // Triangle Top Left->Bottom Left->Bottom Right
                triangles.Add(vTL);
                triangles.Add(vBL);
                triangles.Add(vBR);

                // Triangle Top Left->Bottom Right->Top Right
                triangles.Add(vTL);
                triangles.Add(vBR);
                triangles.Add(vTR);
            }
        }

        // Create our mesh object
        Mesh mesh = new Mesh();

        // Change the mesh index format to use 32 bit unsigned ints, this increases
        // The number of vertices we can have in one mesh from ~65,000 to ~4,000,000,000
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        // Assign the vertices, triangle indicies and uvs to the mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        // Use recalculate normals to calculate the vertex normals for our mesh
        mesh.RecalculateNormals();

        // Create a new mesh filter, and assign the mesh from before to it
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // Create a new renderer for our mesh, and use the standard shader
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));

        /**** 
         * 
         * TODO: Assign the UV mapped texture to the mesh renderers main texture
         * 
         ****/
        meshRenderer.material.mainTexture = uvMapTexture;
    }

}

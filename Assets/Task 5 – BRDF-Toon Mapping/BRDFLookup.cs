﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class generates a mesh using a height map texture. The texture defines
 * the size of the mesh, and also the height of each vertex in the mesh
 *
 * You are required to fill in the code to calculate BRDF lighting on the mesh.
 * This should be done in the update function, so that the lighting on the mesh
 * will be updated as transforms between the light, camera and mesh change.
 * Your code should include all the variables required as configurable
 * in the inspector. You do not have to implement the lighting functions themselves
 * the are implemented in the Lighting.cs file, and can be called using 
 * Lighting.CalculateBRDFLookupLightingColours
 * 
 * PROD321 - Interactive Computer Graphics and Animation 
 * Copyright 2023, University of Canterbury
 * Written by Adrian Clark
 */

public class BRDFLookup : MonoBehaviour
{
    // Defines the height map texture used to create the mesh and set the heights
    public Texture2D heightMapTexture;
    public Texture2D textureMap;

    // Defines the height scale that we multiply the height of each vertex by
    public float heightScale = 30;

    // Store our mesh filter at class level so we can access it in the Start
    // and the Update function
    MeshFilter meshFilter;

    // Reference to the camera which our scene will be rendered from
    public Camera renderingCamera;
    public Light lightSource;
    public Transform meshTransform;
    public float atten;

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public int height;
    public int width;

    /**** 
    * 
    * TODO: Add code here to store the various parameters you will need for BDRF lighting
    * You should have a variable for each of the configurable parameters of these lighting models. 
    * You can see these lighting model functions in Lighting.cs, and I've copied the function 
    * declaration here:
    * 
    * public static Color[] CalculateBRDFLookupLightingColours(Texture2D lookupTexture, float atten, Vector3 cameraWorldPos, Light lightSource, Transform meshTransform, Vector3[] verts, Vector3[] normals)
    * 
    * Some of these variables you'll need to define, and some are already defined in this class.
    * 
    ****/

    // Start is called before the first frame update
    void Start()
    {
        // If no camera was defined, use the camera tagged with MainCamera
        if (renderingCamera == null)
            renderingCamera = Camera.main;

        // Create a list to store our vertices
        //List<Vector3> vertices = new List<Vector3>();

        // Create a list to store our triangles
        //List<int> triangles = new List<int>();

        // Calculate the Height and Width of our mesh from the heightmap's
        // height and width 
        height = heightMapTexture.height;
        width = heightMapTexture.width;

        // Generate our Vertices
        // Loop through the meshes length and width
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create a new vertex using the x and z positions, and get the
                // y position as the pixel from the height map texture. As it's
                // gray scale we can use any colour channel, in this case red.
                // Multiply the pixel value by the height scale to get the final
                // y value
                vertices.Add(new Vector3(x, heightMapTexture.GetPixel(x, z).r * heightScale, z));
            }
        }

        // Generate our triangle Indicies
        // Loop through the meshes length-1 and width-1
        for (int z = 0; z < height - 1; z++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                // Multiply the Z value by the mesh width to get the number
                // of pixels in the rows, then add the value of x to get the
                // final index. Increase the values of X and Z accordingly
                // to get the neighbouring indicies
                int vTL = z * width + x;
                int vTR = z * width + x + 1;
                int vBR = (z + 1) * width + x + 1;
                int vBL = (z + 1) * width + x;

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

        // Assign the vertices and triangle indicies
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        // Use recalculate normals to calculate the vertex normals for our mesh
        mesh.RecalculateNormals();

        // Create a new mesh filter, and assign the mesh from before to it
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // Create a new renderer for our mesh, and use the custom
        // Coloured Vertex Unlit Shader for the material
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Custom/ColouredVertexUnlitShader"));
    }

    // Update is called once per frame
    void Update()
    {
        Color[] resultArray = Lighting.CalculateBRDFLookupLightingColours(textureMap, atten, renderingCamera.transform.position, lightSource, meshTransform, vertices.ToArray(), meshFilter.mesh.normals);
        meshFilter.mesh.colors = resultArray;
    }
}

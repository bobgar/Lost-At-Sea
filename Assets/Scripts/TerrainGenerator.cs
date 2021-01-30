using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    const int MAX_HEIGHT = 40;
    const int TERRAIN_WIDTH = 40;
    const int RESOLUTION = 16;

    public WorldManager worldManager;
    public Terrain terrain;
    protected TerrainData terrainData;
    // Start is called before the first frame update
    void Start()
    {
                
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    const float PERLIN_MIN = 0.70710678118654752440084436210485f;

    public void InitializeTerrain()
    {
        transform.position += new Vector3(-TERRAIN_WIDTH / 2, -30, -TERRAIN_WIDTH / 2);
        terrain = GetComponent<Terrain>();
        terrainData = new TerrainData();
        terrainData.size = new Vector3(TERRAIN_WIDTH, MAX_HEIGHT, TERRAIN_WIDTH);
        terrainData.heightmapResolution = RESOLUTION;
        terrain.terrainData = terrainData;
        UpdateTerrain();
    }

    public void UpdateTerrain()
    {
        int vertices = terrainData.heightmapResolution;
        float[,] heights = new float[vertices, vertices];        
        float px =0, pz = 0;
        string chunkString = "location x = "+ (transform.position.x / (float)TERRAIN_WIDTH) + "   z = "  + (transform.position.z / (float)TERRAIN_WIDTH);
        for (int x = 0; x < vertices; x++)
        {
            for(int z = 0; z < vertices; z++)
            {
                //Negative 1 on the vericies divide is so that each end is at an even number so chunks line up smoothly.
                px = ((float)(x) / (float)(vertices-1) + (transform.position.x / ((float)TERRAIN_WIDTH)));
                pz = ((float)(z) / (float)(vertices-1) + (transform.position.z / ((float)TERRAIN_WIDTH)));

                heights[z, x] = ((worldManager.Noise.GetSimplexFractal( px * 20 , pz * 20 ) + PERLIN_MIN) / (PERLIN_MIN*2)) ;
                
                if( x==0 && z == 0)
                {
                    chunkString += "  at 0,0  px = " + px * 20 + "  pz = " + pz * 20;
                }
            }
            //mapString += "\n";
        }
        //Debug.Log(mapString);

        terrain.terrainData.SetHeights(0, 0, heights);

        terrain.Flush();

        chunkString += "  at 16,16  px = " + px * 20 + "  pz = " + pz * 20;
        Debug.Log(chunkString);
        
    }

    public void SetNeighbors(Terrain left, Terrain right, Terrain top, Terrain bottom)
    {
        terrain.SetNeighbors(left, top, right, bottom);
        terrain.Flush();
    }
}

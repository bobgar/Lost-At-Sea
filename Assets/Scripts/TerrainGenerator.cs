using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

public class TerrainGenerator : MonoBehaviour
{
    // -SQRT(.5) , SQRT(.5) are the Perlin limits.  I use a precalculated const for this.
    const float PERLIN_MIN = 0.70710678118654752440084436210485f;

    const int MAX_HEIGHT = 40;
    const int TERRAIN_WIDTH = 40;
    const int RESOLUTION = 33;

    public Texture2D baseTexture;
    public Texture2D borderTexture;
    public Texture2D highTexture;

    public WorldManager worldManager;
    public Terrain terrain;
    protected TerrainData terrainData;

    private Vector3 highestPoint = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void InitializeTerrain()
    {
        transform.position += new Vector3(-TERRAIN_WIDTH / 2, -25, -TERRAIN_WIDTH / 2);
        terrain = GetComponent<Terrain>();
        terrainData = new TerrainData();
        terrainData.size = new Vector3(TERRAIN_WIDTH, MAX_HEIGHT, TERRAIN_WIDTH);
        terrainData.heightmapResolution = RESOLUTION;
        terrainData.alphamapResolution = RESOLUTION;

        TerrainLayer[] terrainLayers = new TerrainLayer[3];
        terrainLayers[0] = new TerrainLayer();
        terrainLayers[0].diffuseTexture = baseTexture;
        terrainLayers[0].tileSize = new Vector2(10f, 10f);        
        terrainLayers[1] = new TerrainLayer();
        terrainLayers[1].diffuseTexture = borderTexture;
        terrainLayers[1].tileSize = new Vector2(10f, 10f);
        terrainLayers[2] = new TerrainLayer();
        terrainLayers[2].diffuseTexture = highTexture;
        terrainLayers[2].tileSize = new Vector2(10f, 10f);

        terrainData.terrainLayers = terrainLayers;

        terrain.terrainData = terrainData;

        UpdateTerrain();
        //Debug.Log("heighpoint = " + highestPoint);
        if (highestPoint.y > .7f)
        {
            Debug.Log("Placing Feature");
            GameObject go = worldManager.GetLandFeature();
            GameObject igo;
            if (go.GetComponent<vThirdPersonController>() != null)
            {
                //igo = Instantiate(go);                
                igo = go;             
            }
            else
            {
                igo = Instantiate(go, transform);
            }
            
            float h = terrain.SampleHeight(transform.position + highestPoint);
            igo.transform.position = transform.position + new Vector3(highestPoint.x, 0, highestPoint.z) + new Vector3(0,h,0);            
        }
        else
        {
            //Instantiate water point of interest
        }
    }

    public void UpdateTerrain()
    {        
        int vertices = terrainData.heightmapResolution;
        float[,] heights = new float[vertices, vertices];        
        float px =0, pz = 0;
        string chunkString = "location x = "+ (transform.position.x / (float)TERRAIN_WIDTH) + "   z = "  + (transform.position.z / (float)TERRAIN_WIDTH);

        float[,,] alpha1 = new float[terrainData.alphamapResolution, terrainData.alphamapResolution, 3];        

        for (int x = 0; x < vertices; x++)
        {
            for(int z = 0; z < vertices; z++)
            {
                //Negative 1 on the vericies divide is so that each end is at an even number so chunks line up smoothly.
                px = ((float)(x) / (float)(vertices-1) + (transform.position.x / ((float)TERRAIN_WIDTH)));
                pz = ((float)(z) / (float)(vertices-1) + (transform.position.z / ((float)TERRAIN_WIDTH)));

                float height = ((worldManager.Noise.GetSimplexFractal( px * 20 , pz * 20 ) + PERLIN_MIN) / (PERLIN_MIN*2)) * .4f + ((worldManager.Noise2.GetSimplexFractal(px * 2, pz * 2) + PERLIN_MIN) / (PERLIN_MIN * 2)) * .6f ;

                if( (height ) > highestPoint.y)
                {
                    highestPoint = new Vector3(x, (height), z);
                }

                heights[z, x] = height;

                float perturb = ((worldManager.Noise.GetSimplexFractal(20 + px * 40, -40 + pz * 40)) / (PERLIN_MIN * 2)) / 5.0f + height;

                if (perturb <= .63f)
                {
                    alpha1[z, x, 0] = 1f;
                    alpha1[z, x, 1] = 0f;
                    alpha1[z, x, 2] = 0f;
                }
                else if (perturb > .68f && height < .73f)
                {
                    alpha1[z, x, 0] = 0f;
                    alpha1[z, x, 1] = 1f;
                    alpha1[z, x, 2] = 0f;
                } else if (perturb > .73f && height < .77f)
                {
                    alpha1[z, x, 0] = 1f;
                    alpha1[z, x, 1] = 0f;
                    alpha1[z, x, 2] = 0f;
                }
                else
                {
                    alpha1[z, x, 0] = 0f;
                    alpha1[z, x, 1] = 0f;
                    alpha1[z, x, 2] = 1f;
                }
            }
        }
        
        terrain.terrainData.SetHeights(0, 0, heights);

        terrain.terrainData.SetAlphamaps(0, 0, alpha1);

        TerrainCollider tc = terrain.GetComponent<TerrainCollider>();
        tc.terrainData = terrain.terrainData;        

        terrain.Flush();
    }

    public void SetNeighbors(Terrain left, Terrain right, Terrain top, Terrain bottom)
    {
        terrain.SetNeighbors(left, top, right, bottom);
        terrain.Flush();
    }
}

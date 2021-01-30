using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    const int tileSize = 40;
    protected Camera Camera;
    public int viewDistance = 8;
    public GameObject waterTile;
    public GameObject waterContainer;
    public GameObject terrainTile;
    public GameObject terrainContainer;
    private List<List<GameObject>> waterTiles = new List<List<GameObject>>();
    private List<List<TerrainGenerator>> terrainTiles = new List<List<TerrainGenerator>>();

    protected int curOffsetX = 0;
    protected int curOffsetZ = 0;

    public FastNoise Noise { get; set; }

    private void Awake()
    {
        //System.DateTime time = new System.DateTime();
        Noise = new FastNoise(System.DateTime.Now.Second);
        Debug.Log(Noise.GetSimplexFractal(0, 0));
    }

    // Start is called before the first frame update
    void Start()
    {
        Camera = Camera.main;
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            waterTiles.Add(new List<GameObject>());
            terrainTiles.Add(new List<TerrainGenerator>());
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                // Create tiled water layer
                GameObject tile = Instantiate(waterTile, waterContainer.transform);
                tile.transform.position = new Vector3(x * tileSize, 0, z * tileSize);
                waterTiles[x + viewDistance].Add(tile);

                // Create tiled terrain layer
                tile = Instantiate(terrainTile, terrainContainer.transform);
                tile.transform.position = new Vector3(x * tileSize, 0, z * tileSize);
                TerrainGenerator tg = tile.GetComponent<TerrainGenerator>();
                terrainTiles[x + viewDistance].Add(tg);
                tg.worldManager = this;
                tg.InitializeTerrain();
            }
        }

        for (int x = terrainTiles.Count; x <= terrainTiles.Count; x++)
        {
            for (int z = -terrainTiles.Count; z <= terrainTiles.Count; z++)
            {
                terrainTiles[x][z].SetNeighbors(x < 0 ? null : terrainTiles[x - 1][z].terrain,
                    x > terrainTiles.Count - 1 ? null : terrainTiles[x + 1][z].terrain,
                    z < 0 ? null : terrainTiles[x][z - 1].terrain,
                    z > terrainTiles.Count - 1 ? null : terrainTiles[x][z + 1].terrain);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int offsetX = (int)(Camera.transform.position.x / tileSize);
        if (offsetX < curOffsetX) {
            Debug.Log("ox = " + offsetX );
            curOffsetX = offsetX;

            for (int i = 0; i < waterTiles.Count; i++)
            {
                waterTiles[waterTiles.Count - 1][i].transform.position -= new Vector3(tileSize * (viewDistance * 2 + 1), 0, 0);
            }
            List<GameObject> tileList = waterTiles[waterTiles.Count - 1];
            waterTiles.RemoveAt(waterTiles.Count - 1);
            waterTiles.Insert(0, tileList);
        } else if (offsetX > curOffsetX) {
            Debug.Log("ox = " + offsetX );
            curOffsetX = offsetX;

            for (int i = 0; i < waterTiles.Count; i++)
            {
                waterTiles[0][i].transform.position += new Vector3(tileSize * (viewDistance * 2 + 1), 0, 0);
            }
            List<GameObject> tileList = waterTiles[0];
            waterTiles.RemoveAt(0);
            waterTiles.Add(tileList);
        }

        int offsetZ = (int)(Camera.transform.position.z / tileSize);
        if (offsetZ < curOffsetZ)
        {
            Debug.Log("oz = " + offsetZ);
            curOffsetZ = offsetZ;

            for (int i = 0; i < waterTiles.Count; i++)
            {
                GameObject t = waterTiles[i][waterTiles.Count-1];
                waterTiles[i].RemoveAt(waterTiles.Count - 1);
                t.transform.position -= new Vector3(0, 0, tileSize * (viewDistance * 2 + 1));
                waterTiles[i].Insert(0,t);
            }
        }
        else if (offsetZ > curOffsetZ)
        {
            Debug.Log("oz = " + offsetZ);
            curOffsetZ = offsetZ;


            for (int i = 0; i < waterTiles.Count; i++)
            {
                GameObject t = waterTiles[i][0];
                waterTiles[i].RemoveAt(0);
                t.transform.position += new Vector3(0, 0, tileSize * (viewDistance * 2 + 1));                
                waterTiles[i].Add(t);
            }
        }
    }
}

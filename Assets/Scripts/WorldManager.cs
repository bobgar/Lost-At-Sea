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

    public GameObject[] landFeatures;

    public FastNoise Noise { get; set; }
    public FastNoise Noise2 { get; set; }

    private void Awake()
    {
        //System.DateTime time = new System.DateTime();
        Noise = new FastNoise(System.DateTime.Now.Millisecond);
        Noise2 = new FastNoise(System.DateTime.Now.Millisecond + 5);
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
                TerrainGenerator tg = CreateTerrainTile(x, z);                
                terrainTiles[x + viewDistance].Add(tg);
            }
        }

        SetNeighbors();
        
    }

    protected TerrainGenerator CreateTerrainTile(int x, int z)
    {
        GameObject tile = Instantiate(terrainTile, terrainContainer.transform);
        tile.transform.position = new Vector3(Mathf.Floor(x * tileSize), 0, Mathf.Floor(z * tileSize));
        TerrainGenerator tg = tile.GetComponent<TerrainGenerator>();
        tg.worldManager = this;
        tg.InitializeTerrain();
        return tg;
    }

    void SetNeighbors()
    {
        for (int x = terrainTiles.Count; x < terrainTiles.Count; x++)
        {
            for (int z = -terrainTiles.Count; z < terrainTiles.Count; z++)
            {
                terrainTiles[x][z].SetNeighbors(x < 0 ? null : terrainTiles[x - 1][z].terrain,
                    x > terrainTiles.Count - 1 ? null : terrainTiles[x + 1][z].terrain,
                    z < 0 ? null : terrainTiles[x][z - 1].terrain,
                    z > terrainTiles.Count - 1 ? null : terrainTiles[x][z + 1].terrain);
                terrainTiles[x][z].terrain.Flush();
            }
        }
    }

    void SnapTerrain(GameObject g)
    {
        //g.transform.position += new Vector3(Mathf.Floor(g.transform.position.x), Mathf.Floor(g.transform.position.y), Mathf.Floor(g.transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        int offsetX = (int)(Camera.transform.position.x / tileSize);
        if (offsetX < curOffsetX) {
            Debug.Log("ox = " + offsetX );
            curOffsetX = offsetX;

            //Update Water
            for (int i = 0; i < waterTiles.Count; i++)
            {
                waterTiles[waterTiles.Count - 1][i].transform.position -= new Vector3(tileSize * (viewDistance * 2 + 1), 0, 0);
            } 
            List<GameObject> tileList = waterTiles[waterTiles.Count - 1];
            waterTiles.RemoveAt(waterTiles.Count - 1);
            waterTiles.Insert(0, tileList);


            //Update Terrain
            List<TerrainGenerator> terrainList = new List<TerrainGenerator>();
            for (int i = 0; i < terrainTiles.Count; i++)
            {                
                Destroy(terrainTiles[terrainTiles.Count - 1][i].gameObject);                
                TerrainGenerator tg = CreateTerrainTile(curOffsetX - terrainTiles.Count/2, i + curOffsetZ - terrainTiles.Count / 2);                
                terrainList.Add(tg);
            }
            //List<TerrainGenerator> terrainList = terrainTiles[terrainTiles.Count - 1];
            terrainTiles.RemoveAt(terrainTiles.Count - 1);
            terrainTiles.Insert(0, terrainList);
            SetNeighbors();
        } else if (offsetX > curOffsetX) {
            Debug.Log("ox = " + offsetX );
            curOffsetX = offsetX;

            //Update Water
            for (int i = 0; i < waterTiles.Count; i++)
            {
                waterTiles[0][i].transform.position += new Vector3(tileSize * (viewDistance * 2 + 1), 0, 0);
            }
            List<GameObject> tileList = waterTiles[0];
            waterTiles.RemoveAt(0);
            waterTiles.Add(tileList);

            //Update Terrain
            List<TerrainGenerator> terrainList = new List<TerrainGenerator>();
            for (int i = 0; i < terrainTiles.Count; i++)
            {
                Destroy(terrainTiles[0][i].gameObject);
                TerrainGenerator tg = CreateTerrainTile(curOffsetX + terrainTiles.Count / 2, i + curOffsetZ - terrainTiles.Count / 2);
                terrainList.Add(tg);
            }            
            terrainTiles.RemoveAt(0);
            terrainTiles.Add(terrainList);
            SetNeighbors();
        }

        int offsetZ = (int)(Camera.transform.position.z / tileSize);
        if (offsetZ < curOffsetZ)
        {
            Debug.Log("oz = " + offsetZ);
            curOffsetZ = offsetZ;

            //Update Water
            for (int i = 0; i < waterTiles.Count; i++)
            {
                GameObject t = waterTiles[i][waterTiles.Count-1];
                waterTiles[i].RemoveAt(waterTiles.Count - 1);
                t.transform.position -= new Vector3(0, 0, tileSize * (viewDistance * 2 + 1));
                waterTiles[i].Insert(0,t);
            }

            //Update Terrain
            for (int i = 0; i < terrainTiles.Count; i++)
            {
                Destroy( terrainTiles[i][terrainTiles.Count - 1]);
                terrainTiles[i].RemoveAt(terrainTiles.Count - 1);
                TerrainGenerator tg = CreateTerrainTile(curOffsetX + i - terrainTiles.Count / 2, curOffsetZ - terrainTiles.Count / 2);
                terrainTiles[i].Insert(0, tg);
            }
            SetNeighbors();
        }
        else if (offsetZ > curOffsetZ)
        {
            Debug.Log("oz = " + offsetZ);
            curOffsetZ = offsetZ;

            //Update Water
            for (int i = 0; i < waterTiles.Count; i++)
            {
                GameObject t = waterTiles[i][0];
                waterTiles[i].RemoveAt(0);
                t.transform.position += new Vector3(0, 0, tileSize * (viewDistance * 2 + 1));                
                waterTiles[i].Add(t);
            }

            //Update Terrain
            for (int i = 0; i < waterTiles.Count; i++)
            {
                Destroy( terrainTiles[i][0].gameObject);
                terrainTiles[i].RemoveAt(0);
                TerrainGenerator tg = CreateTerrainTile(curOffsetX + i - terrainTiles.Count / 2, curOffsetZ + terrainTiles.Count / 2);
                terrainTiles[i].Add(tg);
            }
            SetNeighbors();
        } 
    }

    public GameObject GetLandFeature()
    {
        return landFeatures[Random.Range(0, landFeatures.Length)];
    }
}

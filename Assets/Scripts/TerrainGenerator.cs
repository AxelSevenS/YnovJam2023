using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {
    
    [SerializeField] private int terrainWidth = 256; 
    [SerializeField] private int terrainHeight = 256; 
    [SerializeField] private int terrainDepth = 20; 


    private Terrain _terrain;

    public Terrain terrain {
        get {
            _terrain ??= GetComponent<Terrain>();
            return _terrain;
        }
    }


    [ContextMenu("Generate Terrain")]
    private void GenerateTerrain() {
        terrain.terrainData = new TerrainData();
        
        terrain.terrainData.size = new Vector3(terrainWidth, terrainDepth, terrainHeight);
        terrain.terrainData.SetHeights(0, 0, GenerateHeights());
    }

    private float[,] GenerateHeights() {
        float[,] heights = new float[terrainWidth, terrainHeight];

        for (int x = 0; x < terrainWidth; x++) {
            for (int y = 0; y < terrainHeight; y++) {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    private float CalculateHeight(int x, int y) {
        float xCoord = (float)x / terrainWidth * 3;
        float yCoord = (float)y / terrainHeight * 3;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}

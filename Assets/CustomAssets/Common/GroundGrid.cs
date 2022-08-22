using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGrid : MonoBehaviour{
    public static GroundGrid instance;


    public int width = 30;
    public int height = 30;
    [Space]
    public GameObject groundTilePrefab;
    public GameObject wallTilePrefab;
    [Space]
    [Range(0f, 1f)]
    public float rainIntensity = 0.5f;
    [Range(0.01f, 0.5f)]
    public float rainScale = 0.1f;
    [Range(0.1f, 1f)]
    public float rainSpeed = 0.02f;
    public Vector2 rainDirection = Vector2.one;
    public float rainUpdateInterval = 1f;
    public bool showClouds = true;
    [Space]
    public float lightningTreshold = 0.5f;

    [HideInInspector]
    public Vector2 currentNoiseOffset = Vector2.zero;
    [HideInInspector]
    public Dictionary<Vector2, GroundTile> tiles = new Dictionary<Vector2, GroundTile>();

    private void Awake() {
        instance = this;
    }

    void Start()
    {
        for (int x = -1; x <= width; x++) {
            for (int z = -1; z <= height; z++) {
                Vector2 pos = new Vector2(x, z);
                GroundTile tile = null;
                if (x == -1 || z == -1 || x == width || z == height)
                {
                    tile = Instantiate(wallTilePrefab, transform.position + new Vector3(x, 0, z), Quaternion.identity, transform).GetComponent<GroundTile>();
                }
                else
                {
                    tile = Instantiate(groundTilePrefab, transform.position + new Vector3(x, 0, z), Quaternion.identity, transform).GetComponent<GroundTile>();
                }
                tile.worldPos = pos;
                tiles.Add(pos, tile);
            }
        }

        StartCoroutine(rainRoutine());
    }

    public Vector3 GetRandomTile()
    {
        return new Vector3(Random.Range(0, width), 0, Random.Range(0, height));
    }

    public Vector3 GetRandomTile(Vector3 _initPos, int _offset)
    {
        Vector3 _target = new Vector3(
            Mathf.Clamp(Random.Range(_initPos.x - _offset, _initPos.x + _offset), 0, width), 
            _initPos.y,
            Mathf.Clamp(Random.Range(_initPos.z - _offset, _initPos.z + _offset), 0, height)
            );

        return _target;
    }

    public GroundTile getTileAt(Vector2 pos) {
        Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        if (tiles.ContainsKey(gridPos)) return tiles[gridPos];
        return null;
    }

    IEnumerator rainRoutine() {
        while (true) {
            applyPerlinStorm();
            yield return new WaitForSecondsRealtime(rainUpdateInterval);
        }
    }

    void applyPerlinStorm() {
        foreach (KeyValuePair<Vector2, GroundTile> kvp in tiles) {
            float xCoord = currentNoiseOffset.x + kvp.Key.x * rainScale;
            float yCoord = currentNoiseOffset.y + kvp.Key.y * rainScale;
            float evaluation = Mathf.PerlinNoise(xCoord, yCoord);
            bool rained = evaluation >= 1 - rainIntensity;
            kvp.Value.setRain(rained);
            kvp.Value.setLightning(evaluation > lightningTreshold);
        }
        if (showClouds) {
            foreach (KeyValuePair<Vector2, GroundTile> kvp in tiles) {
                kvp.Value.changeCloudCoverage();
            }
        }
        currentNoiseOffset += rainDirection.normalized * rainSpeed;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour{

    public bool isRaining = false;
    public bool isWet = false;
    public bool isLighningStorm = false;
    [Space]
    public float lightningFrequency = 1f;
    public float lightningProbability = 0.5f;
    public float lightningDuration = 1f;
    [Space]
    public float wetDuration = 1f;
    public Material dryMat;
    public Material wetMat;
    public MeshRenderer ground;
    [Space]
    public Material fogyMat;
    public Material cloudyMat;
    public Material stormyMat;
    public Material fullyCoveredMat;
    public Material lightningMat;
    public MeshRenderer sky;
    public GameObject lightningObject;
    public PickupSpawner trashEmitter;
    [Space]
    public List<GameObject> rainObjects = new List<GameObject>();


    [HideInInspector]
    public Vector2 worldPos = Vector2.zero;

    Coroutine lightningRoutineSave = null;

    public void setLightning(bool value) {
        isLighningStorm = value;
        if (isLighningStorm && lightningRoutineSave == null) {
            lightningRoutineSave = StartCoroutine(lightningRoutine());
        } else if (lightningRoutineSave != null && !isLighningStorm) {
            StopCoroutine(lightningRoutineSave);
            lightningRoutineSave = null;
            lightningObject.SetActive(false);
            changeCloudCoverage();
        }
    }

    public void setRain(bool value) {
        if (isRaining && !value) {
            isRaining = false;
            StartCoroutine(dryRoutine());
            foreach (GameObject go in rainObjects) go.SetActive(false);

        } else if (!isRaining && value) {
            isRaining = true;
            setWet(true);
            foreach (GameObject go in rainObjects) go.SetActive(true);

        }
    }

    IEnumerator dryRoutine() {
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        float dryTime = (wetDuration + UnityEngine.Random.Range(0f, 1f) * wetDuration) / 2f;
        yield return new WaitForSeconds(dryTime);
        if (isRaining) yield break;
        setWet(false);
    }

    IEnumerator lightningRoutine() {
        while (true) {
            yield return new WaitForSecondsRealtime(lightningFrequency);
            UnityEngine.Random.InitState((int)(worldPos.SqrMagnitude() + worldPos.x));
            if (UnityEngine.Random.Range(0f, 1f) < lightningProbability) {
                sky.gameObject.SetActive(true);
                sky.sharedMaterial = lightningMat;
                lightningObject.SetActive(true);
                yield return new WaitForSecondsRealtime(lightningDuration);
                lightningObject.SetActive(false);
                changeCloudCoverage();
            }
        }
    }

    public void setWet(bool value) {
        if (value) {
            isWet = true;
            GetComponentInChildren<MeshRenderer>().sharedMaterial = wetMat;
        } else {
            isWet = false;
            GetComponentInChildren<MeshRenderer>().sharedMaterial = dryMat;
        }
    }

    public void changeCloudCoverage() {
        int wetNeighbors = 0;
        Vector2 pos = new Vector2(worldPos.x + 1, worldPos.y);
        if (GroundGrid.instance.tiles.ContainsKey(pos) && GroundGrid.instance.tiles[pos].isRaining) wetNeighbors++;
        pos = new Vector2(worldPos.x - 1, worldPos.y);
        if (GroundGrid.instance.tiles.ContainsKey(pos) && GroundGrid.instance.tiles[pos].isRaining) wetNeighbors++;
        pos = new Vector2(worldPos.x, worldPos.y + 1);
        if (GroundGrid.instance.tiles.ContainsKey(pos) && GroundGrid.instance.tiles[pos].isRaining) wetNeighbors++;
        pos = new Vector2(worldPos.x, worldPos.y - 1);
        if (GroundGrid.instance.tiles.ContainsKey(pos) && GroundGrid.instance.tiles[pos].isRaining) wetNeighbors++;

        sky.gameObject.SetActive(true);
        if (wetNeighbors == 1) sky.sharedMaterial = fogyMat;
        else if (wetNeighbors == 2) sky.sharedMaterial = cloudyMat;
        else if (wetNeighbors == 3) sky.sharedMaterial = stormyMat;
        else if (wetNeighbors == 4) sky.sharedMaterial = fullyCoveredMat;
        if (wetNeighbors == 0) sky.gameObject.SetActive(false);
    }

    public void spawnTrash(int amount) {
        trashEmitter.spawn(amount);
    }

}

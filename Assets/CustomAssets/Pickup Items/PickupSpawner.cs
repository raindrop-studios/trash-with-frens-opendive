using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour{

    public GameObject pickupPrefab;

    public float velocity = 1f;
    public float horizontalDeviation = 1f;
    public float randomness = 0.2f;

    private void Start() {
        //StartCoroutine(spawnTest());
    }

    IEnumerator spawnTest() {
        while (true) {
            yield return new WaitForSecondsRealtime(1);
            spawn(1);
        }
    }


    public void spawn(int amount) {
        for (int i = 0; i < amount; i++) {
            Rigidbody pickup = PhotonNetwork.Instantiate(pickupPrefab.name, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            pickup.AddRelativeForce(new Vector3(Random.Range( -horizontalDeviation - randomness, horizontalDeviation + randomness), Random.Range(velocity -randomness, velocity+randomness), Random.Range(-horizontalDeviation - randomness, horizontalDeviation+ randomness) ));
            pickup.GetComponent<PickupItem>().enabled = true;
        }
    }
    
}

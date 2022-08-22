using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupItem : MonoBehaviour{
    public GameObject pickupZone;

    public Rigidbody rb;
    public Collider physicsCollider;


    public float despawnTime = 10f;

    private void Start() {
        if(GetComponent<PhotonView>().AmOwner) StartCoroutine(despawnRoutine());
    }

    IEnumerator despawnRoutine() {
        yield return new WaitForSecondsRealtime(despawnTime);
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        Destroy(rb);
        Destroy(physicsCollider);
        pickupZone.gameObject.SetActive(true);
        transform.position = collision.contacts[0].point;
    }

    public abstract void onPickup(Collider player);

}

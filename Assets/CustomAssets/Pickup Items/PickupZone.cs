using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupZone : MonoBehaviour{

    public PickupItem item;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            Destroy(transform.parent.gameObject);
            item.onPickup(other);
        }
    }

}

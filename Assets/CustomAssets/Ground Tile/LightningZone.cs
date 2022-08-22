using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningZone : MonoBehaviour{

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            PlayerController enemy = other.GetComponentInParent<PlayerController>();
            if (enemy == null) return;
            enemy.photonView.RPC("RPCGetZapped", enemy.photonView.Owner);
        }
    }
}

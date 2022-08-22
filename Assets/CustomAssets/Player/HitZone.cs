using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitZone : MonoBehaviour{

    public PhotonView dealer;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            PlayerController enemy = other.GetComponentInParent<PlayerController>();
            if (enemy == null && enemy.photonView != dealer) return;
            enemy.photonView.RPC("RPCGetHit", enemy.photonView.Owner, dealer.Owner);
        }
        if (other.tag == "Boss" && !dealer.CompareTag("Boss")) {
            BossController enemy = other.GetComponentInParent<BossController>();
            if (enemy == null && enemy.photonView != dealer) return;
            enemy.photonView.RPC("RPCGetHit", enemy.photonView.Owner, dealer.Owner);
        }
        if (other.tag == "Dumpster")
        {
            if(other.GetComponentInParent<DumpsterController>().owner == this.transform.root.gameObject)
            {
                //TBD
            }
            else
            {
                DumpsterController enemy = other.GetComponentInParent<DumpsterController>();
                enemy.photonView.RPC("RPCGetHit", enemy.photonView.Owner, dealer.Owner);
            }
        }
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InviteZone : MonoBehaviour{

    public ClanManager me;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" ) {
            ClanManager enemy = other.GetComponentInParent<ClanManager>();
            if (enemy == null || enemy == me) return;
            enemy.CreateInvite(me);

        }
    }

    

}

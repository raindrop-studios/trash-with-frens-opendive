using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaindropPickup : PickupItem {
    public int value = 1;
    public PhotonView photonView;

    public override void onPickup(Collider player) {
        if (photonView.AmOwner) {
            PlayerController pc = player.GetComponentInParent<PlayerController>();
            pc.photonView.RPC("RPCRewardRain", pc.photonView.Owner, 1);
        }
    }
}

using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ClanManager : MonoBehaviour{

    [Header("References")]
    public PhotonView photonView;
    public Button joinButton;
    public Button leaveButton;
    public GameObject waitResponse;

    string clanName;

    [HideInInspector] public bool amInivting = false;
    bool acceptInvite = false;

    ClanManager invitor = null;

    private void Start() {
        joinButton.onClick.AddListener(delegate { acceptInvite = true; });
        leaveButton.onClick.AddListener(delegate { photonView.RPC("RPCSetClan", RpcTarget.AllBuffered, ""); });
    }

    public string GetClan() {
        if (clanName == null || clanName.Length == 0) return null;
        return clanName;
    }


    #region INVITE_HANDSHAKE

    //INVITEE, invitor client
    public void CreateInvite(ClanManager invitor) {

        if (amInivting) return;
        if (GetClan() == invitor.GetClan() && invitor.GetClan() != null) return;

        this.invitor = invitor;
        invitor.amInivting = true;
        amInivting = true;
        photonView.RPC("RPCReceiveInvite", photonView.Owner, PhotonNetwork.LocalPlayer);
        invitor.ShowWaitResponse(true);
    }

    //INVITEE, invitee client
    [PunRPC]
    public void RPCReceiveInvite(Photon.Realtime.Player origin) {
        StartCoroutine(JoinTimeOut(origin, 10f));
    }

    //INVITEE input
    IEnumerator JoinTimeOut(Photon.Realtime.Player origin, float timeout) {
        Debug.Log("Wait invite input");
        float timer = 0;
        acceptInvite = false;
        ShowJoinButton(true);
        while (!acceptInvite) {
            timer += Time.deltaTime;
            if (timer > timeout) {
                photonView.RPC("RPCInviteResponse", origin, false);
                ShowJoinButton(false);
                yield break;
            }
            yield return null;
        }
        ShowJoinButton(false);
        photonView.RPC("RPCInviteResponse", origin, true);
    }


    //INVITEE, invitor client
    [PunRPC]
    public void RPCInviteResponse(bool response) {
        if (!response) return;

        if (invitor.GetClan() == null) {
            string newClan = RandomClanName(6);
            invitor.photonView.RPC("RPCSetClan", RpcTarget.AllBuffered, newClan);
            photonView.RPC("RPCSetClan", RpcTarget.AllBuffered, newClan);
        } else {
            photonView.RPC("RPCSetClan", RpcTarget.AllBuffered, invitor.GetClan());
        }

        invitor.ShowWaitResponse(false);
        invitor.amInivting = false;
        amInivting = false;
    }


    //ALL 
    [PunRPC]
    public void RPCSetClan(string newClan) {
        clanName = newClan;
        if (photonView.AmOwner) {
            Game.instance.UpdateCharacterUI();
            ShowLeaveButton(GetClan() != null);
        }
    }

    #endregion

    //Utils
    public static string RandomClanName(int length) {
        var random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return ("STREET_" + (new string(Enumerable.Range(1, length).Select(_ => chars[random.Next(chars.Length)]).ToArray()))).ToUpper();
    }

    void ShowJoinButton(bool show) {
        joinButton.gameObject.SetActive(show);
    }

    void ShowLeaveButton(bool show) {
        leaveButton.gameObject.SetActive(show);
    }

    void ShowWaitResponse(bool show) {
        waitResponse.gameObject.SetActive(show);
    }

}

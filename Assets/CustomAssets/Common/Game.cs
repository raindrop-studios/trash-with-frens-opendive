using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour{
    public static Game instance;

    [Header("Reference")]
    public GameObject playerPrefab;
    public GameObject dumpsterPrefab;
    public GameObject bossPrefab;
    [Space]
    public float rainGainSpeed = 5f;
    public int rainTransferOnHit = 1;

    [Header("Player stats")]
    public PlayerController player;
    [Space]
    public int playerRaindrops = 5;
    public int playerLives = 3;
    public int playerHP = 100;
    public int playerHPRegen = 1;

    public int playerDamage = 3;

    [Header("Dumpster stats")]
    public DumpsterController dumpster;
    public int dumpsterSpawnOffset = 2;
    [Space]
    public int dumpsterRaindrops = 5;
    public int dumpsterLives = 3;
    public int dumpsterHP = 100;
    public int dumpsterHPRegen = 1;

    [Header("General")]
    public bool online = false;

    private void Awake() {
        instance = this;
    }

    void Start(){
        online = PhotonNetwork.IsConnected && PhotonNetwork.InRoom;
        Vector3 initPos = GroundGrid.instance.GetRandomTile();
        SpawnPlayer(initPos);
        SpawnDumpster(initPos);
        SpawnBoss();
        StartCoroutine(RainCheckRoutine());
        UpdateCharacterUI();
        UpdateDumpsterUI();
    }

    void SpawnDumpster(Vector3 _initPos)
    {
        _initPos = GroundGrid.instance.GetRandomTile(_initPos, dumpsterSpawnOffset);

        if (online) dumpster = PhotonNetwork.Instantiate(dumpsterPrefab.name, _initPos, Quaternion.identity).GetComponent<DumpsterController>();
        else dumpster = Instantiate(dumpsterPrefab, _initPos, Quaternion.identity).GetComponent<DumpsterController>();

        dumpster.SetOwner(player.gameObject);
        player.dumpster = dumpster.gameObject;
    }

    void SpawnPlayer(Vector3 _initPos) {
        if (online) player = PhotonNetwork.Instantiate(playerPrefab.name, _initPos, Quaternion.identity).GetComponent<PlayerController>();
        else player = Instantiate(playerPrefab, _initPos, Quaternion.identity).GetComponent<PlayerController>();
    }

    void SpawnBoss() {
        if (online) PhotonNetwork.InstantiateRoomObject(bossPrefab.name, new Vector3(1,0,1), Quaternion.identity);
    }

    public void UpdateCharacterUI() {
        player.photonView.RPC("RPCUpdateCharacterUI", RpcTarget.AllBuffered, playerRaindrops, playerLives, player.GetComponent<ClanManager>().GetClan());
    }

    public void UpdateDumpsterUI()
    {
        dumpster.photonView.RPC("RPCUpdateDumpsterUI", RpcTarget.AllBuffered, dumpsterRaindrops, dumpsterLives, dumpster.GetComponent<ClanManager>().GetClan());
    }

    IEnumerator RainCheckRoutine() {
        while (true) {
            yield return new WaitForSecondsRealtime(rainGainSpeed);
            if (player.GetCurrentTileRain()) AddRain(1);
        }
    }

    public void AddRain(int rain) {
        playerRaindrops += rain;
        UpdateCharacterUI();
    }

    public int RemoveRain(int rain) {
        if (rain > playerRaindrops) {
            int amount = playerRaindrops;
            playerRaindrops = 0;
            UpdateCharacterUI();
            return amount;
        }
        playerRaindrops -= rain;
        UpdateCharacterUI();
        return rain;
    }

    public void AddDumpsterRain(int rain)
    {
        dumpsterRaindrops += rain;
        UpdateDumpsterUI();
    }

    public int RemoveDumpsterRain(int rain)
    {
        if (rain > dumpsterRaindrops)
        {
            int amount = dumpsterRaindrops;
            dumpsterRaindrops = 0;
            UpdateCharacterUI();
            return amount;
        }
        dumpsterRaindrops -= rain;
        UpdateDumpsterUI();
        return rain;
    }

    public bool RemoveAllRain() {
        bool change = playerRaindrops > 0;
        playerRaindrops = 0;
        UpdateCharacterUI();
        return change;
    }

    public void RemoveLife() {
        if (playerLives > 0) playerLives--;
        UpdateCharacterUI();
    }
}

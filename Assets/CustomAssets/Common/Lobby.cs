using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Linq;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class Lobby : MonoBehaviourPunCallbacks {

    public TMP_Text statusText;
    public TMP_InputField roomField;
    public Button goButton;
    public Button randomButton;
    public Button cancelButton;
    [Space]
    public int playersPerRoom = 4;
    [Space]
    public string backScene = "MainLobby";
    public string roomScene = "RoomScene";



    string roomName = null;

    public const string TYPE_KEY = "type";

    private void Start() {

        if(goButton != null) goButton.onClick.AddListener(goRoom);
        if (randomButton != null) randomButton.onClick.AddListener(randomRoom);

        if (cancelButton != null) cancelButton.onClick.AddListener(delegate { cancel(); });

        if (goButton != null) goButton.interactable = false;
        if (cancelButton != null) cancelButton.interactable = false;
        if (roomField != null) roomField.interactable = false;
        if (randomButton != null) randomButton.interactable = false;
        connectToServer();
    }

    //SERVER ##################################################################

    //connect to server
    private void connectToServer() {
        setInfoText("Connecting");
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
        else
            OnConnectedToMaster();
    }

    //join lobby
    public override void OnConnectedToMaster() {
        setInfoText("Joinning lobby");
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
        else
            OnJoinedLobby();
    }

    //in lobby
    public override void OnJoinedLobby() {
        setInfoText("Ready");
        if (goButton != null) goButton.interactable = true;
        if (cancelButton != null) cancelButton.interactable = true;
        if (roomField != null) roomField.interactable = true;
        if (randomButton != null) randomButton.interactable = true;
    }


    //ROOMS ###########################################################################

    //if cancel
    void cancel() {
        if (PhotonNetwork.InLobby) {
            PhotonNetwork.LeaveLobby();
        }
        //SceneManager.LoadScene("HomeLobby");
        CrossSceneTransitioner.instance.loadScene(backScene);
    }

    //if go
    void goRoom() {
        if (roomField.text.Length > 3) {
            if (goButton != null) goButton.interactable = false;
            if (roomField != null) roomField.interactable = false;
            if (randomButton != null) randomButton.interactable = false;
            if (cancelButton != null) cancelButton.interactable = false;
            roomName = (roomField.text).ToUpper();
            joinRoom();
        } else {
            setInfoText("Min 4 characters...");
        }
    }

    void randomRoom() {
        PhotonNetwork.JoinRandomRoom(getRoomInfo().CustomRoomProperties, 0);
    }

    void createRoom() {
        setInfoText("Creating " + roomName);
        PhotonNetwork.CreateRoom(roomName, getRoomInfo());
    }

    void joinRoom() {
        setInfoText("Joining " + roomName);
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        createRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        roomName = randomRoomName(6);
        createRoom();
    }

    public override void OnJoinedRoom() {
        setInfoText("Done!");
        //PhotonNetwork.LoadLevel("Downtown");
        PhotonNetwork.IsMessageQueueRunning = false;
        CrossSceneTransitioner.instance.loadScene(roomScene, true);
    }

    //UTILS ############################################################################

    void setInfoText(string text) {
        if (statusText != null) statusText.text = text;
    }

    RoomOptions getRoomInfo() {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = (byte)playersPerRoom;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { TYPE_KEY };
        roomOptions.CustomRoomProperties = new Hashtable();
        roomOptions.CustomRoomProperties.Add(TYPE_KEY, 1);
        return roomOptions;
    }

    public static string randomRoomName(int length) {
        var random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return new string(Enumerable.Range(1, length).Select(_ => chars[random.Next(chars.Length)]).ToArray()).ToUpper();
    }

}
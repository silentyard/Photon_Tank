using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Launcher : Photon.PunBehaviour {

    string gameversion = "moba_first_v1.0";

    //UI
    public GameObject inputField;
    public GameObject connecting_text;

    bool WantToConnect = false;

	// Use this for initialization
	void Start () {

        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;

        inputField.SetActive(true);
        connecting_text.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    #region PunBehaviour Callbacks

    public override void OnConnectedToMaster()
    {
        if(WantToConnect)   //user will connect to master just after leaving the room
            PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        PhotonNetwork.JoinOrCreateRoom("Room1", new RoomOptions() { MaxPlayers = 4  }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room" + "My name is: " + PhotonNetwork.playerName);

        if (PhotonNetwork.room.PlayerCount == 1)
        {
            Hashtable team = new Hashtable();   //player custom properties
            team.Add("team", "blue");
            PhotonNetwork.player.SetCustomProperties(team);

            Hashtable player_count = new Hashtable();   //room custom properties (for players per team)
            player_count.Add("blue_count", 1);
            player_count.Add("red_count", 0);
            PhotonNetwork.room.SetCustomProperties(player_count);

            PhotonNetwork.LoadLevel("WaitingRoom");
        }
    }

    #endregion

    public void Connect()
    {
        WantToConnect = true;

        PhotonNetwork.playerName = GameObject.Find("Player_name").GetComponent<Text>().text + " "; //assign player name

        inputField.SetActive(false);
        connecting_text.SetActive(true);
        
        if (!PhotonNetwork.connected) //may have just came back from room, it'll still be connected
        {
            PhotonNetwork.ConnectUsingSettings(gameversion);
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}

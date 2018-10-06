using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class gameManager : Photon.PunBehaviour {

    public static gameManager instance;

    List<PhotonPlayer> playerlist;
    public Vector3[] spawnPosition;
    public int gametime;

	// Use this for initialization
	void Start () {
        instance = this;

        playerlist = PhotonNetwork.playerList.ToList();
        playerlist.Sort();

        int player_index = GetThisPlayerIndex();
        PhotonNetwork.Instantiate("Tank", spawnPosition[player_index], Quaternion.identity, 0);

        //Timer
        StartCoroutine(UIManager.instance.StartCountDown(gametime));

        //Cursor
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

    int GetThisPlayerIndex()
    {
        int i;
        for(i = 0; i < playerlist.Count; i++)
        {
            if (PhotonNetwork.player.ID == playerlist[i].ID)
                break;
        }
        return i;
    }

    public void GameOver()
    {
        Hashtable hash = new Hashtable();
        hash.Add("ready", false);
        PhotonNetwork.player.SetCustomProperties(hash);

        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.IsOpen = true;
            PhotonNetwork.LoadLevel("AfterGame");
        }
    }
}

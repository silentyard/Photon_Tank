using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AfterGameManager : Photon.PunBehaviour {

    public Text end_message;

	// Use this for initialization
	void Start () {
        string win_team = (string)PhotonNetwork.room.CustomProperties["win"];
        end_message.text = win_team + " team wins!!!";
        Cursor.visible = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void back_to_waiting_room()
    {
        PhotonNetwork.LoadLevel("WaitingRoom");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class WaitingRoomManager : Photon.PunBehaviour {

    public Text blue_team_players, red_team_players;
    public GameObject blue_player_panel, red_player_panel, player_info_prefab;
    public Button start_button;
    int ready_people = 0;

    private void Start()
    {
        StartCoroutine(PlayerRefresh());

        if (!PhotonNetwork.isMasterClient)
            start_button.GetComponentInChildren<Text>().text = "Ready";
        else
            start_button.interactable = false;

        SetReady(PhotonNetwork.player, false);
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if ((int)PhotonNetwork.room.CustomProperties["blue_count"] <= (int)PhotonNetwork.room.CustomProperties["red_count"])   //根據人數分配隊伍
        {
            Hashtable team = new Hashtable();
            team.Add("team", "blue");
            newPlayer.SetCustomProperties(team);

            ModifyPlayerCount("blue_count", 1);
        }
        else
        {
            Hashtable team = new Hashtable();
            team.Add("team", "red");
            newPlayer.SetCustomProperties(team);

            ModifyPlayerCount("red_count", 1);
        }
        Debug.Log("Player:" + newPlayer.NickName + " has joined !" + newPlayer.CustomProperties["team"].ToString());

        StartCoroutine(PlayerRefresh());
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("Player:" + otherPlayer.NickName + "has left!");

        if (otherPlayer.CustomProperties["team"].ToString() == "blue")
            ModifyPlayerCount("blue_count", -1);
        else
            ModifyPlayerCount("red_count", -1);


        if (PhotonNetwork.isMasterClient) //this may become Master
        {
            start_button.GetComponentInChildren<Text>().text = "Start";
            SetReady(PhotonNetwork.player, false);
        }
        StartCoroutine(PlayerRefresh());
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    void ModifyPlayerCount(string key, int count)
    {
        int new_count = (int)PhotonNetwork.room.CustomProperties[key] + count;
        Hashtable hash = new Hashtable();
        hash.Add(key, new_count);
        PhotonNetwork.room.SetCustomProperties(hash);
    }

    [PunRPC]
    IEnumerator PlayerRefresh()
    {
        yield return new WaitForSeconds(0.2f); //solution for synchronization
        
        foreach(Transform child in blue_player_panel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in red_player_panel.transform)
        {
            Destroy(child.gameObject);
        }

        ready_people = 0;
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            GameObject info = Instantiate(player_info_prefab);
            info.GetComponentsInChildren<Text>()[0].text = player.NickName;
            info.GetComponentsInChildren<Text>()[1].text = (bool)player.CustomProperties["ready"] ? "Ready" : "";
            ready_people += (bool)player.CustomProperties["ready"] ? 1 : 0;

            if (player.CustomProperties["team"].ToString() == "blue")
            {
                info.transform.SetParent(blue_player_panel.transform);
            }
            else if(player.CustomProperties["team"].ToString() == "red")
            {
                info.transform.SetParent(red_player_panel.transform);
            }
        }
        if (PhotonNetwork.isMasterClient)
            checkStart();
    }

    public void LeaveRoom()
    {
        ChatManager.instance.Disconnect();
        PhotonNetwork.LeaveRoom();
    }

    public void StartOrReady()
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.LoadLevel("Game");
        }
        else if( !(bool)PhotonNetwork.player.CustomProperties["ready"] )
        {
            photonView.RPC("ReadyOrCancel", PhotonTargets.All, PhotonNetwork.player, true);
            SetReady(PhotonNetwork.player, true);
            start_button.GetComponentInChildren<Text>().text = "Cancel Ready";
        }
        else
        {
            photonView.RPC("ReadyOrCancel", PhotonTargets.All, PhotonNetwork.player, false);
            SetReady(PhotonNetwork.player, false);
            start_button.GetComponentInChildren<Text>().text = "Ready";
        }

    }

    public void SetReady(PhotonPlayer player, bool ready)
    {
        Hashtable hash = new Hashtable();
        hash.Add("ready", ready);
        player.SetCustomProperties(hash);
    }

    [PunRPC]
    public void ReadyOrCancel(PhotonPlayer caller, bool roc)
    {
        string name = caller.NickName;
        string team = caller.CustomProperties["team"].ToString();
        string ReadyOrNot = roc ? "Ready" : "";
        if(team == "blue")
        {
            foreach(Transform info in blue_player_panel.transform)
            {
                if(info.GetComponentsInChildren<Text>()[0].text == name)
                {
                    info.GetComponentsInChildren<Text>()[1].text = ReadyOrNot;
                    break;
                }
            } 
        }
        else if (team == "red")
        {
            foreach (Transform info in red_player_panel.transform)
            {
                if (info.GetComponentsInChildren<Text>()[0].text == name)
                {
                    info.GetComponentsInChildren<Text>()[1].text = ReadyOrNot;
                    break;
                }
            }
        }

        if (PhotonNetwork.isMasterClient)
        {
            if (roc)
            {
                ready_people += 1;
            }
            else
            {
                ready_people -= 1;
            }

            checkStart();
        }
    }

    public void checkStart()
    {
        if (ready_people == PhotonNetwork.room.PlayerCount - 1  //扣掉自己
            //&& (int)PhotonNetwork.room.CustomProperties["red_count"] >= 1
            //&& (int)PhotonNetwork.room.CustomProperties["blue_count"] >= 1
            )
            start_button.interactable = true;
        else
            start_button.interactable = false;
    }

    public void switchTeam()
    {
        string current_team = PhotonNetwork.player.CustomProperties["team"].ToString();

        Hashtable new_team = new Hashtable();
        if (current_team == "red")
        {
            new_team.Add("team", "blue");
            ModifyPlayerCount("blue_count", 1);
            ModifyPlayerCount("red_count", -1);
        }
        else if (current_team == "blue")
        {
            new_team.Add("team", "red");
            ModifyPlayerCount("red_count", 1);
            ModifyPlayerCount("blue_count", -1);
        }
        PhotonNetwork.player.SetCustomProperties(new_team);

        photonView.RPC("PlayerRefresh", PhotonTargets.All);
    }
}

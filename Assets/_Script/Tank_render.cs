using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank_render : Photon.PunBehaviour {

    public Material[] tank_color;
    MeshRenderer[] mr;

    // Use this for initialization
    private void Awake()
    {
        mr = GetComponentsInChildren<MeshRenderer>();
    }

    void Start () {
        if (photonView.isMine)
        {
            if (PhotonNetwork.player.CustomProperties["team"].ToString() == "blue")
            {
                photonView.RPC("change_tank_color", PhotonTargets.All, 0);
            }
            else
            {
                photonView.RPC("change_tank_color", PhotonTargets.All, 1);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

    }

    [PunRPC]
    public void change_tank_color(int color_index)
    {
        foreach (MeshRenderer target in mr)
        {
            Material[] temp_materials = target.materials;
            temp_materials[0] = tank_color[color_index];
            target.materials = temp_materials;
        }
    }
}

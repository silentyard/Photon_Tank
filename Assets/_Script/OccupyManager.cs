using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class OccupyManager : Photon.PunBehaviour {

    public CanvasGroup occupy_panel;
    public Text occupy_text;
    public Image blue_fill;
    public Image red_fill;
    public float occupy_speed;

    float radius;
    float blue_occupy_progress = 0;
    float red_occupy_progress = 0;
    LayerMask player = 512;
    Coroutine blue_occupy, red_occupy;

	// Use this for initialization
	void Start () {
        radius = GetComponent<RectTransform>().rect.width * GetComponent<RectTransform>().localScale.x;
        blue_occupy = null;
        red_occupy = null;
        occupy_panel.alpha = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Players"))
            return;

        Collider[] cols = Physics.OverlapSphere(transform.position, radius/2f, player);
        int blue_player_count = 0;
        int red_player_count = 0;

        foreach(Collider col in cols)
        {
            Debug.Log(col.gameObject);
            if(col.gameObject.GetPhotonView().owner.CustomProperties["team"].ToString() == "blue")
            {
                blue_player_count++;
            }
            else
            {
                red_player_count++;
            }
        }

        if (blue_player_count > 0 && red_player_count > 0)
        {
            if (red_occupy != null)
            {
                StopCoroutine(red_occupy);
                red_occupy = null;
            }
            if (blue_occupy != null)
            {
                StopCoroutine(blue_occupy);
                blue_occupy = null;
            }
            occupy_text.text = "";
            occupy_panel.alpha = 0;
            return;
        }
        else if(blue_player_count > 0 && red_player_count == 0 && blue_occupy == null)
        {
            if (red_occupy != null)
            {
                StopCoroutine(red_occupy);
                red_occupy = null;
            }
            blue_occupy = StartCoroutine(Occupy(0));
        }
        else if(red_player_count > 0 && blue_player_count == 0 && red_occupy == null)
        {
            if (blue_occupy != null)
            {
                StopCoroutine(blue_occupy);
                blue_occupy = null;
            }
            red_occupy = StartCoroutine(Occupy(1));
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Players"))
            return;

        Collider[] cols = Physics.OverlapSphere(transform.position, radius/2f, player);
        int blue_player_count = 0;
        int red_player_count = 0;
        foreach (Collider col in cols)
        {
            if (col == other) //避免算到離開trigger的物體
                continue;
            if (col.gameObject.GetPhotonView().owner.CustomProperties["team"].ToString() == "blue")
            {
                blue_player_count++;
            }
            else
            {
                red_player_count++;
            }
        }

        if (blue_player_count == 0 && red_player_count == 0)
        {
            if (red_occupy != null)
            {
                StopCoroutine(red_occupy);
                red_occupy = null;
            }
            if (blue_occupy != null)
            {
                StopCoroutine(blue_occupy);
                blue_occupy = null;
            }
            occupy_text.text = "";
            occupy_panel.alpha = 0;
        }
    }

    IEnumerator Occupy(int team)
    {
        occupy_panel.alpha = 1;
        if (team == 0)
        {
            occupy_text.text = "Blue team occupying !!!";
            red_fill.GetComponent<CanvasGroup>().alpha = 0;
            blue_fill.GetComponent<CanvasGroup>().alpha = 1;
            while (true)
            {
                blue_occupy_progress += occupy_speed;
                blue_fill.fillAmount = blue_occupy_progress / 100f;
                if (blue_occupy_progress >= 100)
                {
                    Hashtable win = new Hashtable();
                    win.Add("win", "blue");
                    PhotonNetwork.room.SetCustomProperties(win);

                    gameManager.instance.GameOver();
                }
                yield return new WaitForSeconds(1f);
            }
        }
        else if(team == 1)
        {
            occupy_text.text = "Red team occupying !!!";
            blue_fill.GetComponent<CanvasGroup>().alpha = 0;
            red_fill.GetComponent<CanvasGroup>().alpha = 1;
            while (true)
            {
                red_occupy_progress += occupy_speed;
                red_fill.fillAmount = red_occupy_progress / 100f;
                if(red_occupy_progress >= 100)
                {
                    Hashtable win = new Hashtable();
                    win.Add("win", "red");
                    PhotonNetwork.room.SetCustomProperties(win);

                    gameManager.instance.GameOver();
                }

                yield return new WaitForSeconds(1f);
            }
        }
    }
}

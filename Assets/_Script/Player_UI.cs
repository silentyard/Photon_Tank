using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_UI : Photon.PunBehaviour {

    Tank target;
    public Text player_name, health_text;
    public Image health;
    public Image try_to_aim, aim;
    LayerMask players = 512, map = 1024;

    public Color self, enemy, ally;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        health.fillAmount = target.current_hp / target.max_hp;
        health_text.text = target.current_hp.ToString() + " / " + target.max_hp.ToString();
        transform.position = Camera.main.WorldToScreenPoint(target.transform.position + new Vector3(0, 2, 0)) + new Vector3(0f, 30f, 0f);

        RaycastHit hit, hit2;
        Physics.Raycast(target.transform.position + new Vector3(0, 1.3f, 0), Camera_manager.instance.transform.forward, out hit, Mathf.Infinity, players + map);
        try_to_aim.rectTransform.position = Camera.main.WorldToScreenPoint(hit.point - target.transform.forward * 0.5f);

        Physics.Raycast(target.transform.position + new Vector3(0, 1.3f, 0), target.turret.transform.forward, out hit2, Mathf.Infinity, players + map);
        aim.rectTransform.position = Camera.main.WorldToScreenPoint(hit2.point - target.transform.forward * 0.5f);
    }

    public void SetTarget(Tank _target)
    {
        target = _target;
        player_name.text = target.photonView.owner.NickName;
        if(target.photonView.owner.CustomProperties["team"].ToString() != PhotonNetwork.player.CustomProperties["team"].ToString())
        {
            health.color = enemy;
        }
        else if(target.photonView.isMine == false)
        {
            health.color = ally;
        }
        else
        {
            health.color = self;
        }
    }

    public IEnumerator CoolDown(float time)
    {
        float start = Time.time;
        while(Time.time - start < time)
        {
            aim.fillAmount = (Time.time - start) / time;
            yield return new WaitForSeconds(0f);
        }
    }
}

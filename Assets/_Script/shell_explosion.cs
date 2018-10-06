using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shell_explosion : Photon.PunBehaviour {

    public float explode_radius;
    public int atk;
    LayerMask player = 512;

	// Use this for initialization
	void Start () {
        Collider[] hit_tank = Physics.OverlapSphere(transform.position, explode_radius, player);
        foreach(Collider tank in hit_tank)
        {
            tank.gameObject.GetComponent<Tank>().take_damage(atk);
        }
        if (photonView.isMine)
            StartCoroutine(DelayDestroy(3));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator DelayDestroy(int delaytime)
    {
        yield return new WaitForSeconds(delaytime);
        PhotonNetwork.Destroy(gameObject);
    }
}

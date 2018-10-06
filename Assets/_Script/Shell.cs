using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : Photon.PunBehaviour {

    public float fly_speed;
    public float max_lifetime;
    Rigidbody rb;
    public GameObject owner;

    public GameObject bump_particle;

    bool bounce = false;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(DelayDestroy());
	}
	
	// Update is called once per frame
	void Update () {
        if (photonView.isMine || !PhotonNetwork.connected)
        {
            rb.MovePosition(rb.position + transform.forward * fly_speed * Time.deltaTime);
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.isMine)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("map") && !bounce)
            {
                Vector3 new_direction = Vector3.Reflect(transform.forward, collision.contacts[0].normal);
                transform.rotation = Quaternion.LookRotation(new_direction);
                bounce = true;
            }
            else
            {
                GameObject explosion = PhotonNetwork.Instantiate("ShellExplosion", rb.position, rb.rotation, 0);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(max_lifetime);
        if(photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}

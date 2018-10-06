using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Photon.PunBehaviour {

    public float move_speed, turn_speed, turret_turn_speed;
    public float max_hp, current_hp;
    public float fire_cd;
    float last_fire;
    public GameObject ui;       //prefab
    public GameObject turret;   //ref
    bool weapon;
    GameObject this_ui;
    Rigidbody rb;

    public AudioClip shoot, idle, destroy;
    [Range(0f, 1f)]
    public float shoot_volume, idle_volume;
    AudioSource shoot_source, idle_source;

    GameObject[] small_shells;

    // Use this for initialization
    private void Awake()
    {
        shoot_source = gameObject.AddComponent<AudioSource>();
        idle_source = gameObject.AddComponent<AudioSource>();

        shoot_source.clip = shoot;
        idle_source.clip = idle;

        shoot_source.loop = false;
        idle_source.loop = true;

        shoot_source.volume = shoot_volume;
        idle_source.volume = idle_volume;
    }

    void Start () {
        rb = GetComponent<Rigidbody>();

        //UI
        this_ui = Instantiate(ui);
        this_ui.transform.SetParent(GameObject.Find("Canvas").transform);
        this_ui.GetComponent<Player_UI>().SetTarget(this);

        //Camera
        if (photonView.isMine)
        {
            Camera_manager.instance.SetTarget(this);
        }
        else
        {
            this_ui.GetComponent<Player_UI>().aim.gameObject.SetActive(false);
        }

        //fire
        last_fire = -fire_cd;
        small_shells = new GameObject[3];

        weapon = true;
        UIManager.instance.SwitchWeapon(weapon);

        //sound
        idle_source.Play();

    }
	
	// Update is called once per frame
	void Update () {

        //movement
        if (photonView.isMine && !ChatManager.instance.input_focused)
        {
            /*Vector3 Camera_toward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;

            Vector3 Camera_to_tank_move = Vector3.ProjectOnPlane(Camera_toward, transform.right);
            Vector3 Camera_to_tank_turn_1 = Vector3.ProjectOnPlane(Camera_toward, transform.forward);
            float Camera_to_tank_turn_2 = Camera_to_tank_turn_1.magnitude * Vector3.Dot(Camera_to_tank_turn_1, transform.right);

            rb.MovePosition(rb.position + Camera_to_tank_move * Input.GetAxis("Vertical") * move_speed * Time.deltaTime);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Input.GetAxis("Vertical") * turn_speed * Time.deltaTime * Camera_to_tank_turn_2, 0) );*/

            rb.MovePosition(rb.position + transform.forward * Input.GetAxis("Vertical") * move_speed * Time.deltaTime);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Input.GetAxis("Horizontal") * turn_speed * Time.deltaTime, 0));

            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
                idle_source.pitch = 1.5f;
            else
                idle_source.pitch = 1f;
        }

        //Fire
        if (Input.GetMouseButtonDown(0) && photonView.isMine && !ChatManager.instance.input_focused)
        {
            if(weapon && Time.time - last_fire > fire_cd)
            {
                shoot_source.Play();
                GameObject shell = PhotonNetwork.Instantiate("Shell", rb.position + turret.transform.forward * 2f + new Vector3(0,1.3f,0), turret.transform.rotation, 0);
                shell.GetComponent<Shell>().owner = gameObject;
                last_fire = Time.time;
                StartCoroutine(this_ui.GetComponent<Player_UI>().CoolDown(fire_cd));
            }
            else if (!weapon)
            {
                for(int i=0; i<small_shells.Length; i++)
                {
                    if(small_shells[i] == null)
                    {
                        small_shells[i] = PhotonNetwork.Instantiate("Small_Shell", rb.position + turret.transform.forward * 2f + new Vector3(0, 1.3f, 0), turret.transform.rotation, 0);
                        break;
                    }
                }
            }
        }

        //switch weapon
        if(Input.GetKeyDown(KeyCode.R) && photonView.isMine && !ChatManager.instance.input_focused)
        {
            weapon = !weapon;
            UIManager.instance.SwitchWeapon(weapon);
        }
	}

    public void take_damage(int dmg)
    {
        current_hp -= dmg;
        if (photonView.isMine)
            StartCoroutine(UIManager.instance.damaged());

        if(current_hp <= 0)
        {
            if (photonView.owner.CustomProperties["team"].ToString() == "blue")
                UIManager.instance.ModifyPlayerCount(0);
            else
                UIManager.instance.ModifyPlayerCount(1);

            if (photonView.isMine)
            {
                idle_source.clip = destroy;
                idle_source.Play();
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}

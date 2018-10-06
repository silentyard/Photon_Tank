using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_manager : MonoBehaviour {

    public static Camera_manager instance;
    public float rotate_speed;
    public float x_z_from_target;
    public float y_from_target;
    public Tank target;

    // Use this for initialization
    void Start() {
        instance = this;
    }

    // Update is called once per frame
    void Update() {
        if (ChatManager.instance.input_focused)
            return;
        //transform.eulerAngles += new Vector3(-1 * Input.GetAxis("Mouse Y") * rotate_speed * Time.deltaTime, Input.GetAxis("Mouse X") * rotate_speed * Time.deltaTime, 0);
        transform.eulerAngles += new Vector3(0, Input.GetAxis("Mouse X") * rotate_speed * Time.deltaTime, 0);
        transform.position = target.transform.position + new Vector3(0, y_from_target, 0); //無法在第一個frame就get target

        //砲台移動方式(是否為瞬間)
        target.turret.transform.eulerAngles = transform.eulerAngles;
        //target.turret.transform.forward = Vector3.RotateTowards(target.turret.transform.forward, transform.forward, target.turret_turn_speed * Time.deltaTime, 0);

    }

    public void SetTarget(Tank _target)
    {
        transform.position = _target.transform.position + new Vector3(0, y_from_target, 0);
        Camera.main.transform.position = transform.position - _target.transform.forward * x_z_from_target;

        transform.rotation = _target.transform.rotation;
        target = _target;
    }

    public void SetSensitivity(float num)
    {
        rotate_speed = num;
    }
}

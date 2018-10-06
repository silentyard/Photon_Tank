using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UIManager : MonoBehaviour {

    public static UIManager instance;
    public Text TimeText, blue_player_count, red_player_count, ping, fps_text;
    public Image dmg_caution, shell, small_shell;
    public Color dmg_color;
    public float color_flash_speed;
    
    float startTime;
    int blue_player_num, red_player_num;

    float fps_rate = 0.5f, last_fps_update = 0, frames = 0;

    // Use this for initialization
    private void Awake()
    {
        instance = this;
        blue_player_num = (int)PhotonNetwork.room.CustomProperties["blue_count"];
        red_player_num = (int)PhotonNetwork.room.CustomProperties["red_count"];
    }

    void Start () {
        TimeText.text = gameManager.instance.gametime.ToString();

        blue_player_count.text = "Blue: " + blue_player_num.ToString();
        red_player_count.text = "Red: " + red_player_num.ToString();
    }

    // Update is called once per frame
    void Update () {
        ping.text = "ping: " + PhotonNetwork.GetPing().ToString();

        if(Time.time - last_fps_update > fps_rate)
        {
            fps_text.text = "fps: " + frames / fps_rate;
            frames = 0;
            last_fps_update = Time.time;
        }
        else
        {
            frames++;
        }

        if (Input.GetKey(KeyCode.LeftControl))
            Cursor.visible = true;
        else
            Cursor.visible = false;
    }

    public IEnumerator StartCountDown(int gametime)
    {
        int timer = gametime;
        while (timer > 0)
        {
            TimeText.text = (timer--).ToString();
            yield return new WaitForSeconds(1.0f);
        }
        TimeText.text = "Game Over !!!";
    }

    public void ModifyPlayerCount(int team)
    {
        if (team == 0)
            blue_player_num--;
        else
            red_player_num--;

        blue_player_count.text = "Blue: " + blue_player_num.ToString();
        red_player_count.text = "Red: " + red_player_num.ToString();

        if (blue_player_num == 0)
        {
            Hashtable hash = new Hashtable();
            hash.Add("win", "red");
            PhotonNetwork.room.SetCustomProperties(hash);
            gameManager.instance.GameOver();
        }
        else if (red_player_num == 0)
        {
            Hashtable hash = new Hashtable();
            hash.Add("win", "blue");
            PhotonNetwork.room.SetCustomProperties(hash);
            gameManager.instance.GameOver();
        }
    }

    public IEnumerator damaged()
    {
        dmg_caution.color = dmg_color;
        while(dmg_caution.color.a != 0)
        {
            dmg_caution.color = Color.Lerp(dmg_caution.color, Color.clear, color_flash_speed * Time.deltaTime);
            yield return new WaitForSeconds(0f);
        }
    }

    public void SwitchWeapon(bool weapon) {
        if (weapon)
        {
            shell.color = new Color(1, 1, 1, 1);
            small_shell.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            shell.color = new Color(1, 1, 1, 0.5f);
            small_shell.color = new Color(1, 1, 1, 1);
        }
    }
}

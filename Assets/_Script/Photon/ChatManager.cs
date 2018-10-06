using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener {

    public static ChatManager instance;
    ChatClient chatclient;
    string chatAppVersion = "1.0";
    public string[] ChannelToSubscribeOnConnect;
    string current_channel;

    public GameObject chatPanel;
    public Text chatContent;
    public InputField chatInput;
    public bool input_focused = false;

    // Use this for initialization
    void Start () {
        instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(chatPanel);

        current_channel = ChannelToSubscribeOnConnect[0];
        //chatInput.onEndEdit.AddListener(delegate { stopInput(); });
        chatPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if(chatclient != null)
            chatclient.Service();

        if (chatclient != null && Input.GetKeyDown(KeyCode.Return))
        {
            if (!input_focused)
            {
                chatInput.ActivateInputField();
                input_focused = true;
            }
            else
            {
                if (chatInput.text != "")
                {
                    chatclient.PublishMessage(current_channel, chatInput.text);
                    chatInput.text = "";
                    Canvas.ForceUpdateCanvases();
                    chatPanel.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0f;
                }
                chatInput.DeactivateInputField();
                input_focused = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
            input_focused = false;
    }

    public void Connect()
    {
        chatclient = new ChatClient(this);
        //chatclient.ChatRegion = "EU";
        chatclient.Connect(PhotonNetwork.PhotonServerSettings.ChatAppID, chatAppVersion, new ExitGames.Client.Photon.Chat.AuthenticationValues(PhotonNetwork.playerName));
        Debug.Log("Connecting as:" + PhotonNetwork.playerName);
    }

    public void Disconnect()
    {
        chatclient.PublishMessage("System", "<b>" + chatclient.AuthValues.UserId + " has left the room! </b>");
        Debug.Log("Disconnected from server");
        chatclient.Disconnect();
    }

    void stopInput()
    {
        input_focused = false;
    }

    void OnDestroy()
    {
        if(chatclient != null)
            chatclient.Disconnect();
    }

    void OnApplicationQuit()
    {
        if (chatclient != null)
            chatclient.Disconnect();
    }

    #region ChatClient callback

    void IChatClientListener.DebugReturn(DebugLevel level, string message)
    {
        if (level == DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    void IChatClientListener.OnDisconnected()
    {
        Destroy(chatPanel);
        Destroy(gameObject);
        chatclient = null;
    }

    void IChatClientListener.OnConnected()
    {
        Debug.Log("Photon Chat: connected with ID: " + chatclient.AuthValues.UserId);
        chatclient.Subscribe(ChannelToSubscribeOnConnect);
        chatclient.PublishMessage("System", "<b>" + chatclient.AuthValues.UserId + " has joined the room! </b>");

        chatPanel.SetActive(true);
        //chatContent.text = "You have joined a room !\n";
    }

    void IChatClientListener.OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange: " + state);
    }

    void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string msgs = chatContent.text;
        if (channelName == "System")
        {
            for(int i = 0; i < senders.Length; i++)
            {
                msgs += string.Format("{0}\n ", messages[i]);

            }
        }
        else if(channelName == "All")
        {
            for (int i = 0; i < senders.Length; i++)
            {
                msgs += string.Format("{0}: {1}\n ", senders[i], messages[i]);
            }
        }
        chatContent.text = msgs;
        Canvas.ForceUpdateCanvases();
        chatPanel.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0f;
    }

    void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnSubscribed(string[] channels, bool[] results)
    {
        for(int i = 0; i < channels.Length; i++)
        {
            Debug.Log("Try to subscribe to channel: " + channels[i] + ", " + (results[i] ? "succeeded" : "failed"));
        }
    }

    void IChatClientListener.OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}

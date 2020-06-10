using System.Collections;
using System.Collections.Generic;
using agora_gaming_rtc;
using UnityEngine;
using UnityEngine.UI;


public class InteractionHandler : MonoBehaviour {


    public GameObject _loginView;
    public GameObject _channelView;
    public InputField _channelName;
    public GameObject _selfVideo;
    private Texture _selfVideoTex;

    public GameObject scrollBar; 
    public GameObject videoPrefab;
    public GameObject RemoteVidPanel;

    private HashSet<uint> users = new HashSet<uint>();
    private uint numUsers;
        
	// Use this for initialization
	void Start () {
        AgoraInterface.Instance.OnInitialize += Instance_OnInitialize;
        _selfVideoTex = new Texture2D(512, 512);

	}
	
    void Instance_OnInitialize()
    {
        AgoraInterface.Instance.Agora.OnUserJoined += Agora_OnUserJoined;
    }

    void Agora_OnUserJoined(uint uid, int elapsed)
    {
        if (users.Contains(uid))
            return;

        if (numUsers == 0)
        {
            print("num users = 0");
            AgoraInterface.Instance.Agora.EnableVideoObserver();

        }
        else {
            var video = Instantiate(videoPrefab);
            //video.transform.parent = scrollBar.gameObject.transform; 
            video.transform.SetParent(scrollBar.gameObject.transform, false);

            var numChildren = scrollBar.gameObject.transform.childCount;
            var rect = video.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector3(0, -(numChildren - 1) * (rect.rect.height + 20), 0);


            var vs = video.GetComponent<VideoSurface>();
            vs.SetForUser(uid);
            vs.SetEnable(true);
        }
        numUsers++;
        users.Add(uid);
    }


	// Update is called once per frame
	void Update () {
		
	}


    public void onJoinButtonClicked()
    {
        Debug.Log("Join Clicked");
        _loginView.SetActive(false);
        _channelView.SetActive(true);

        AgoraInterface.Instance.JoinChannel(_channelName.text);

    }

    public void OnMuteButtonClicked()
    {
        Debug.Log("Mute Clicked");

        AgoraInterface.Instance.Mute();
    }

    public void OnLeaveButtonClicked()
    {
        Debug.Log("Leave Clicked");   
        _loginView.SetActive(true);
        _channelView.SetActive(false);

        AgoraInterface.Instance.LeaveChannel();
    }

    public void OpenPanel()
    {
        Animator anim = RemoteVidPanel.GetComponent<Animator>();
        if(anim != null )
        {
            bool isOpen = anim.GetBool("open");
            anim.SetBool("open", !isOpen);
        }
    }
}

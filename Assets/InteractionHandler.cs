using System.Collections;
using System.Collections.Generic;
using agora_gaming_rtc;
using UnityEngine;
using UnityEngine.UI;


public class InteractionHandler : MonoBehaviour {


    public GameObject _loginView;
    public GameObject _channelView;
    public GameObject _backgroundVideo;
    public InputField _channelName;
    public GameObject _selfVideo;

    public GameObject scrollBar; 
    public GameObject videoPrefab;
    public GameObject RemoteVidPanel;

    private uint _backgroundVideoUid;

    private List<GameObject> _userTiles = new List<GameObject>();
    private HashSet<uint> _users = new HashSet<uint>();
    private uint _numUsers;


    public static InteractionHandler Instance
    {
        get; private set;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        AgoraInterface.Instance.OnInitialize += Instance_OnInitialize;
	}
	
    void Instance_OnInitialize()
    {
        AgoraInterface.Instance.Agora.OnUserJoined += Agora_OnUserJoined;
        AgoraInterface.Instance.Agora.OnUserOffline += Agora_OnUserOffline;
        AgoraInterface.Instance.Agora.OnJoinChannelSuccess += Agora_OnJoinChannelSuccess;
    }

    void Agora_OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // Find user and remove it from list
        var tileIndex = _userTiles.FindIndex((video) => video.GetComponent<VideoHandler>().Uid == uid);
        var tile = _userTiles[tileIndex];
        _userTiles.Remove(tile);
        _users.Remove(uid);
        DestroyImmediate(tile);

        var numChildren = scrollBar.gameObject.transform.childCount;
        if(numChildren > tileIndex)
        {
            for(int i = tileIndex; i < numChildren; i++)
            {
                var rect = _userTiles[i].GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector3(0, -i * (rect.rect.height + 20), 0);
            }
        }

        // Change background uid if needed
        if(_backgroundVideoUid == uid)
        {
            SwapBackgroundVideo(0);
        }
    }

    void Agora_OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        AgoraInterface.Instance.Agora.EnableVideoObserver();
        _selfVideo.GetComponent<VideoSurface>().EnableFilpTextureApply(true, true);
        _backgroundVideo.GetComponent<VideoSurface>().EnableFilpTextureApply(true, true);
    }

    void Agora_OnUserJoined(uint uid, int elapsed)
    {
        if (_users.Contains(uid))
            return;

     
        var video = Instantiate(videoPrefab);
        video.transform.SetParent(scrollBar.gameObject.transform, false);
        video.GetComponent<VideoHandler>().Uid = uid;
        _userTiles.Add(video);

        // Set video object inside scrollbar
        var numChildren = scrollBar.gameObject.transform.childCount;
        var rect = video.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector3(0, -(numChildren - 1) * (rect.rect.height + 20), 0);


        var vs = video.GetComponent<VideoSurface>();
        vs.SetForUser(uid);
        vs.SetEnable(true);
        vs.EnableFilpTextureApply(true, true);

        _numUsers++;
        _users.Add(uid);
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

    public void SwapBackgroundVideo(uint uid)
    {
        var videoSurface = _backgroundVideo.GetComponent<VideoSurface>();
        videoSurface.SetForUser(uid);
        _backgroundVideoUid = uid;
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

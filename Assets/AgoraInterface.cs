using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;

public class AgoraInterface : MonoBehaviour {
    
    public string appId = "";

    public IRtcEngine Agora{
        get; private set;
    }

    public delegate void OnInitialized();
    public event OnInitialized OnInitialize;

    public static AgoraInterface Instance{
        get; private set;
    }

    private void Awake()
    {
        if(Instance == null)
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
        
        Agora = IRtcEngine.GetEngine(appId);
        Agora.EnableVideo();


        if(OnInitialize != null)
        {
            OnInitialize();
        }
	}

	// Update is called once per frame
	void Update () {
		
	}

    public void Mute()
    {
        Agora.DisableAudio();
    }

    public void JoinChannel(string channel)
    {
        Agora.JoinChannel(channel, null, 0);
    }

    public void LeaveChannel()
    {
        Agora.LeaveChannel();
        Agora.DisableVideoObserver();
       // IRtcEngine.Destroy();
        //Agora = null;
    }


}

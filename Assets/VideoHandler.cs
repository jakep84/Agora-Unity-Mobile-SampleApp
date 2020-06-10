using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VideoHandler : MonoBehaviour, IPointerClickHandler {

	public uint Uid
	{ set { uid = value; } }

	private uint uid;

	public void OnPointerClick(PointerEventData eventData)
	{
		InteractionHandler.Instance.SwapBackgroundVideo(uid);
	}
}

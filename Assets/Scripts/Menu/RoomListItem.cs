using Photon.Realtime;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text roomNameText;
    public RoomInfo info;

    public void Setup(RoomInfo _info){
        info = _info;
        roomNameText.text = _info.Name;
    }

    public void OnClick(){
        Launcher.instance.JoinRoom(info);
    }
}

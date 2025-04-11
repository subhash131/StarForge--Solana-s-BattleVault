using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] Text roomNameText;
    public RoomInfo info;

    public void Setup(RoomInfo _info){
        info = _info;
        roomNameText.text = _info.Name;
    }

    public void OnClick(){
        Launcher.instance.JoinRoom(info);
    }
}

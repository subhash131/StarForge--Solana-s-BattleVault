using UnityEngine;

public class ConnectToPhotonNetwork : MonoBehaviour
{
    void Start(){
        Launcher.ConnectRealtime();
    }

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectToPhotonNetwork : MonoBehaviour
{
    void Start(){
        Launcher.ConnectRealtime();
    }

}

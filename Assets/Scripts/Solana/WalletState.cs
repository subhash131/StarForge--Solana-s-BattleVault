using SonicHunt.Accounts;
using Unity;
using UnityEngine;

public class WalletState : MonoBehaviour{

    public static WalletState instance;
    public User userAccount;
    public bool isWalletConnected = false;

    void Awake(){
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
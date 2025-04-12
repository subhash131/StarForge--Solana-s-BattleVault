using UnityEngine;
using TMPro;
using Solana.Unity.SDK;
using System;

public class WalletDisconnect : MonoBehaviour
{
    private TextMeshProUGUI buttonTextComponent;

    void Start(){
        buttonTextComponent = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonTextComponent == null) {
            Debug.LogError("No TextMeshProUGUI component found in children!");
            return;
        }
    }

    private void OnEnable(){
        Web3.OnLogout += OnLogout;
    }

    private void OnDisable(){
        Web3.OnLogout -= OnLogout;
    }

    private void OnLogout() {
        try {
            if (MenuManager.instance != null) {
                MenuManager.instance.OpenMenu("ConnectWalletMenu");
            } else {
                Debug.LogWarning("MenuManager.instance is null. Cannot open ConnectWalletMenu.");
            }
        }catch (Exception e) {
            Debug.LogError("Error in OnLogout: " + e.Message);
            Launcher.instance.errorText.text = "Error in OnLogout: " + e.Message;
            MenuManager.instance.OpenMenu("ErrorMenu");
        }
       
    }

}
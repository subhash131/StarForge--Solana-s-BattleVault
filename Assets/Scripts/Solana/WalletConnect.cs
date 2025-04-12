using UnityEngine;
using TMPro;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using System;

public class WalletConnect : MonoBehaviour {
    private string buttonText = "Connect Wallet"; 
    private TextMeshProUGUI buttonTextComponent;  

    void Start(){
        buttonTextComponent = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonTextComponent == null){
            Debug.LogError("No TextMeshProUGUI component found in children!");
            return;
        }
        buttonTextComponent.text = buttonText;
    }

    private void OnEnable(){ Web3.OnLogin += OnLogin; }

    private void OnDisable(){ Web3.OnLogin -= OnLogin; }

    private void OnLogin(Account account){
        if (buttonTextComponent == null) return;
        else buttonText = "Connect Wallet";
        buttonTextComponent.text = buttonText;

        if(!string.IsNullOrEmpty(account.PublicKey)){
            MenuManager.instance.OpenMenu("FundMenu");
        }
    }
}
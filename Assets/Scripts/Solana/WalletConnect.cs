using UnityEngine;
using TMPro;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;

public class WalletConnect : MonoBehaviour {
    private string buttonText = "Connect Wallet"; 
    private TextMeshProUGUI buttonTextComponent;  

    void Start()
    {
        buttonTextComponent = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonTextComponent == null)
        {
            Debug.LogError("No TextMeshProUGUI component found in children!");
            return;
        }
        buttonTextComponent.text = buttonText;
    }

    private void OnEnable()
    {
        Web3.OnLogin += OnLogin;
    }

    private void OnDisable()
    {
        Web3.OnLogin -= OnLogin;
    }

    private void OnLogin(Account account)
    {
        if (buttonTextComponent == null) return;

        if (account != null)
        {
            buttonText = account.PublicKey.ToString()[..4] + "..." + account.PublicKey.ToString().Substring(account.PublicKey.ToString().Length - 4, 4);   
        }
        else
        {
            buttonText = "Connect Wallet";
        }
        buttonTextComponent.text = buttonText;
    }
}
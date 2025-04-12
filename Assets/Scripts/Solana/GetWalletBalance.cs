using Solana.Unity.SDK;
using TMPro;
using UnityEngine;

public class GetWalletBalance : MonoBehaviour
{
    private TMP_Text walletBalance;

    void Start() {
        walletBalance = GetComponent<TMP_Text>();
        if (walletBalance == null) {
            Debug.LogError("No TMP_Text component found in children!");
            return;
        }
        Web3.OnBalanceChange += UpdateWalletBalance;
    }

    private void UpdateWalletBalance(double balance){
        if (walletBalance == null) return;
        walletBalance.text = balance.ToString("F2") + " SOL"; 
    }
}

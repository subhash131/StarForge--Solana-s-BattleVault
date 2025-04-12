using Solana.Unity.SDK;
using TMPro;
using UnityEngine;

public class GetWalletAddress : MonoBehaviour {
    private TMP_Text walletAddress;

    void Start() {
        walletAddress = GetComponent<TMP_Text>();
        if (walletAddress == null) {
            Debug.LogError("No TMP_Text component found in children!");
            return;
        }
        walletAddress.text =  Web3.Account.PublicKey;
    }
}

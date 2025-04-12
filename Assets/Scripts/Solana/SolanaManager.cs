using System;
using System.Collections.Generic;
using Solana.Unity.Programs;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Types;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using SonicHunt;
using SonicHunt.Accounts;
using SonicHunt.Program;
using TMPro;
using UnityEngine;

public class SolanaManager : MonoBehaviour
{
    public static SolanaManager instance;
    [SerializeField] private TMP_Text walletAddressText;
    [SerializeField] private TMP_Text walletBalanceText;
    [SerializeField] private TMP_Text fundBalanceText;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField fundsInputField;
    public TMP_Text messageText;
    public User userAccount;
    public GameObject createUser;
    public GameObject getUser;
    public GameObject addFunds;
    public Master masterAccount;
    private readonly PublicKey masterAddress = new PublicKey("Evphv17bimm3Kuh5a3kR9PJxJo76K3s3WGdxUDGMacnr");

    public string programId = "5eE7SdLv2PA7DimYPNsu2GjrnNjvXKDrm1MKb3RB4V8J"; 

    private void Awake() {
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        
    }

    private void OnEnable(){ 
        Web3.OnBalanceChange += UpdateWalletBalance;
        Web3.OnLogin += OnLogin;
        Web3.OnLogout += OnLogout; 
    }

    private void OnDisable(){ 
        Web3.OnBalanceChange -= UpdateWalletBalance;
        Web3.OnLogin -= OnLogin;
        Web3.OnLogout -= OnLogout; 
    }

    private void UpdateWalletBalance(double balance){
        if (walletBalanceText == null) return;
        walletBalanceText.text = $"Wallet Balance: {balance.ToString("F2")} SOL"; 
    }

      private void OnLogout() {
        userAccount = null;
        playerNameText.text = "Hi Guest";
        fundBalanceText.text = "";
        walletAddressText.text = "";
        playerNameInputField.text = "";
        messageText.text = "";
        
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

    private void OnLogin(Account account){
        if (walletAddressText == null) return;
        walletAddressText.text = account.PublicKey.ToString()[..4] + "..." + account.PublicKey.ToString()[^4..];
        if (!string.IsNullOrEmpty(account.PublicKey))
        {
            GetUserAccount();
            MenuManager.instance.OpenMenu("BlockchainMenu");
        }
    }

    public async void GetUserAccount(){
        if (Web3.Instance == null || Web3.Wallet == null){
            Debug.LogError("Web3 or Wallet not initialized!");
            return;
        }
        try{
            var client = new SonicHuntClient(
                Web3.Wallet.ActiveRpcClient,
                Web3.Wallet.ActiveStreamingRpcClient,
                new PublicKey(programId)
            );
            var result = await client.GetUserAsync(GetUserPDA(), Commitment.Confirmed);

            if (result.WasSuccessful){
                userAccount = result.ParsedResult;
                playerNameText.text = $"Hi {result.ParsedResult.Username}";  
                fundBalanceText.text = $"Funds: {LamportsToSol(result.ParsedResult.Funds)}";  
                getUser.SetActive(true);
                createUser.SetActive(false);
                addFunds.SetActive(false);
            }else{
                messageText.text = "Failed to fetch user account!";
                createUser.SetActive(true);
                getUser.SetActive(false);
                addFunds.SetActive(false);
            }
        }catch(Exception e){
            messageText.text = "User not found!";
            addFunds.SetActive(false);
            createUser.SetActive(true);
            getUser.SetActive(false);
            Debug.LogError($"Error fetching user account: {e.Message}");
        }
    }

    
    public PublicKey GetUserPDA(){
        var seed = new byte[] { (byte)'u', (byte)'s', (byte)'e', (byte)'r'}; 
        var authorityKey = Web3.Wallet.Account.PublicKey.KeyBytes; 
        bool success = PublicKey.TryFindProgramAddress(
            new[] { seed, authorityKey },
            new PublicKey(programId),
            out PublicKey pda,
            out _
        );
        return success ? pda : null;
    }
   
    public async void CreateUserAccount(){

        var playerName = playerNameInputField.text;

         if (string.IsNullOrEmpty(playerName)){
            messageText.text = "Username is empty!";
            Debug.LogError("Username is empty!");
            return;
        }

        messageText.text = "Creating user account...";
        if (Web3.Instance == null || Web3.Wallet == null){
            Debug.LogError("Web3 or Wallet not initialized!");
            messageText.text = "Wallet not found!";
            return;
        }

        try{
            PublicKey userPDA = GetUserPDA();
            if (userPDA == null){
                messageText.text = "Failed to generate user PDA!";
                return;
            }
            messageText.text = "Confirm the transaction";
            var accounts = new AddUserAccounts{
                User = userPDA,
                Authority = Web3.Wallet.Account.PublicKey,
                SystemProgram = SystemProgram.ProgramIdKey
            };
            var instruction = SonicHuntProgram.AddUser(accounts, playerName, new PublicKey(programId));

            string latestBlockHash = await Web3.Wallet.GetBlockHash(); 

            byte[] tx = new TransactionBuilder()
                .SetRecentBlockHash(latestBlockHash)
                .SetFeePayer(Web3.Account)
                .AddInstruction(instruction)
                .Build(new List<Account> { Web3.Account });            

            var res = await Web3.Wallet.SignAndSendTransaction(Transaction.Deserialize(tx));
            
            if (res.WasSuccessful){
                Debug.Log($"User account created successfully! Transaction: {res.Result}");
                messageText.text = "User account created successfully!";
                addFunds.SetActive(false);
                createUser.SetActive(false);
                getUser.SetActive(true);
            }else{
                Debug.LogError($"Failed to create user account: {res.Reason}");
                messageText.text = $"Failed to create account: {res.Reason}";
            }
        }catch(Exception e){
            Debug.LogError($"Error creating user account: {e.Message}");
            messageText.text = $"Error creating user account: {e.Message}";
        }
    }


    public async void AddFunds(){
        var fundInput = fundsInputField.text;

         if (string.IsNullOrEmpty(fundInput) || !double.TryParse(fundInput, out double fundAmount) || fundAmount <= 0){
            messageText.text = "Invalid Fund amount!";
            return;
        }

        messageText.text = "Adding funds...";
        if (Web3.Instance == null || Web3.Wallet == null){
            Debug.LogError("Web3 or Wallet not initialized!");
            messageText.text = "Wallet not found!";
            return;
        }

        try{
            PublicKey userPDA = GetUserPDA();
            if (userPDA == null){
                messageText.text = "Failed to generate user PDA!";
                return;
            }
          
            messageText.text = "Confirm the transaction";
            var accounts = new AddFundsAccounts{
                User = userPDA,
                Master= GetMasterPDA(),
                Authority = Web3.Wallet.Account.PublicKey,
                Owner = masterAddress,
                SystemProgram = SystemProgram.ProgramIdKey
            };
            var instruction = SonicHuntProgram.AddFunds(
                    accounts, 
                    SolToLamports((decimal)fundAmount), 
                    new PublicKey(programId)
                    );

            string latestBlockHash = await Web3.Wallet.GetBlockHash(); 

            byte[] tx = new TransactionBuilder()
                .SetRecentBlockHash(latestBlockHash)
                .SetFeePayer(Web3.Account)
                .AddInstruction(instruction)
                .Build(new List<Account> { Web3.Account });            

            var res = await Web3.Wallet.SignAndSendTransaction(Transaction.Deserialize(tx));
            
            if (res.WasSuccessful){
                Debug.Log($"Funds added successfully! Transaction: {res.Result}");
                messageText.text = "Funds added successfully!";
                var updatedFunds = userAccount.Funds + SolToLamports((decimal)fundAmount);
                userAccount.Funds = updatedFunds;
                fundBalanceText.text = $"Funds: {updatedFunds}";  

            }else{
                Debug.LogError($"Failed to add Funds: {res.Reason}");
                messageText.text = $"Failed to add Funds: {res.Reason}";
            }
        }catch(Exception e){
            Debug.LogError($"Error adding funds: {e.Message}");
            messageText.text = $"Error adding funds: {e.Message}";
        }
    }


      public async void GetMaster(){
        try{
            var client = new SonicHuntClient(
                Web3.Wallet.ActiveRpcClient,
                Web3.Wallet.ActiveStreamingRpcClient,
                new PublicKey(programId)
            );
            var result = await client.GetMasterAsync(GetMasterPDA(), Commitment.Confirmed);

            if (result.WasSuccessful && result.WasSuccessful){
                masterAccount = result.ParsedResult;
                Debug.Log($"Master Account Fetched! Owner: {masterAccount.Owner}");
            }
            else{
                Debug.LogError($"Failed to fetch Master account: {result.OriginalRequest}");
            }
        }
        catch (Exception ex){
            Debug.LogError($"Error fetching Master account: {ex.Message}");
        }
    }

    private PublicKey GetMasterPDA(){
        var seed = new byte[] { (byte)'m', (byte)'a', (byte)'s', (byte)'t', (byte)'e', (byte)'r' }; 
        bool success = PublicKey.TryFindProgramAddress(
            new[] { seed },
            new PublicKey(programId),
            out PublicKey pda,
            out _
        );
        return success ? pda : null;
    }


    public void NavigateToAddFunds(){
        messageText.text = "";
        addFunds.SetActive(true);
        createUser.SetActive(false);
        getUser.SetActive(false);
    }
    public void NavigateToGetUser(){
        messageText.text = "";
        addFunds.SetActive(false);
        createUser.SetActive(false);
        getUser.SetActive(true);
    }

    public ulong SolToLamports(decimal sol) {
        return (ulong)(sol * 1_000_000_000m);
    }

    public decimal LamportsToSol(ulong lamports) {
        return lamports / 1_000_000_000m;
    }

   
}

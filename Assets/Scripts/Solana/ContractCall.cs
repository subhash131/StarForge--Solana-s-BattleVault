using UnityEngine;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using Solana.Unity.Programs; 
using Solana.Unity.Rpc.Builders;
using SonicHunt;
using Solana.Unity.Rpc.Types;
using System;
using SonicHunt.Program;
using System.Collections.Generic;
using Solana.Unity.Rpc.Models;

public class ContractCall : MonoBehaviour{
    
    public async void CreateUserAccount(){
        Launcher.instance.messageText.text = "Creating user account...";
        if (Web3.Instance == null || Web3.Wallet == null){
            Debug.LogError("Web3 or Wallet not initialized!");
            return;
        }

        var playerName = Launcher.instance.playerNameInputField.text;
        var programId = new PublicKey(Launcher.programId);
        if (string.IsNullOrEmpty(playerName)){
            Debug.LogError("Username is empty!");
            return;
        }

        try{
            PublicKey userPDA = GetUsernamePDA();
            Launcher.instance.messageText.text = $"userPDA: {userPDA}";
            if (userPDA == null){
                Launcher.instance.messageText.text = "Failed to generate user PDA!";
                return;
            }
            var accounts = new AddUserAccounts{
                User = userPDA,
                Authority = Web3.Wallet.Account.PublicKey,
                SystemProgram = SystemProgram.ProgramIdKey
            };
            var instruction = SonicHuntProgram.AddUser(accounts, playerName, programId);

            Launcher.instance.messageText.text = $"instruction: {instruction}";

            string latestBlockHash = await Web3.Wallet.GetBlockHash(); 
            Launcher.instance.messageText.text = $"latestBlockHash: {latestBlockHash}";

            byte[] tx = new TransactionBuilder()
                .SetRecentBlockHash(latestBlockHash)
                .SetFeePayer(Web3.Account)
                .AddInstruction(instruction)
                .Build(new List<Account> { Web3.Account });

            

            var res = await Web3.Wallet.SignAndSendTransaction(Transaction.Deserialize(tx));
            
            if (res.WasSuccessful){
                Debug.Log($"User account created successfully! Transaction: {res.Result}");
                Launcher.instance.messageText.text = "res.Result: " + res;
            }else{
                Debug.LogError($"Failed to create user account: {res.Reason}");
                Launcher.instance.messageText.text = $"Failed to create user account: {res.Reason}";
            }
        }catch(Exception e){
            Debug.LogError($"Error creating user account: {e.Message}");
            Launcher.instance.messageText.text = $"Error creating user account: {e.Message}";
        }
    }

    public async void GetMaster(){
        if (Web3.Instance == null || Web3.Wallet == null){
            Debug.LogError("Web3 or Wallet not initialized!");
            return;
        }

        try{
            var client = new SonicHuntClient(
                Web3.Wallet.ActiveRpcClient,
                Web3.Wallet.ActiveStreamingRpcClient,
                new PublicKey("5eE7SdLv2PA7DimYPNsu2GjrnNjvXKDrm1MKb3RB4V8J")
            );

            var result = await client.GetMasterAsync(GetMasterPDA(), Commitment.Confirmed);

            if (result.WasSuccessful && result.WasSuccessful){
                var master = result.ParsedResult;
                Debug.Log($"Master Account Fetched! Owner: {master.Owner}");
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
            new PublicKey(Launcher.programId),
            out PublicKey pda,
            out _
        );
        return success ? pda : null;
    }

    private PublicKey GetUsernamePDA(){
        var seed = new byte[] { (byte)'u', (byte)'s', (byte)'e', (byte)'r'}; 
        var authorityKey = Web3.Wallet.Account.PublicKey.KeyBytes; 
        bool success = PublicKey.TryFindProgramAddress(
            new[] { seed, authorityKey },
            new PublicKey(Launcher.programId),
            out PublicKey pda,
            out _
        );
        return success ? pda : null;
    }

}
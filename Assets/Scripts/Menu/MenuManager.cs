using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public Menu[] menus;

    void Awake() {
        instance = this;

        if(SceneManager.GetActiveScene().buildIndex == 0){
            if(WalletState.instance.isWalletConnected){
                OpenMenu("GetUser");
            }
            else{
                OpenMenu("ConnectWalletMenu");
            }
        }
    }       
    
    public void OpenMenu(string menuName){
        for(int i=0; i<menus.Length; i++){
            if(menus[i].menuName == menuName){
                menus[i].Open();
            }
            else if(menus[i].isOpen){
                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu){
        for(int i=0; i<menus.Length; i++){
            if(menus[i].isOpen){
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu){
        menu.Close();
    }

    
    public void SwitchScene(int number){
        SceneManager.LoadScene(number);
    }   
     
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameMenu : MonoBehaviour
{

    public GameObject mainGameMenu;
    public GameObject singlePlayerMenu;
    public GameObject multiplayerMenu;
    public ServerClientHandling ServerClientScript;

    private void Start()
    {
        
    }
    public void startSingleplayer()
    {
        singlePlayerMenu.SetActive(true);
        nextScreen(false);
    }
    public void startMultiplayer(bool hostGame)
    {
        multiplayerMenu.SetActive(true);
        if (hostGame)
        {
            ServerClientScript.StartServer();
        } else
        {
            ServerClientScript.ConnectToServer();
        }
        nextScreen(true);
    }
    public void nextScreen(bool multiplayer)
    {
        mainGameMenu.SetActive(false);
    }
}

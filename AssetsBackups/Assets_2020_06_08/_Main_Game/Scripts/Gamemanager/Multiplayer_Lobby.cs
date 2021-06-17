using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Multiplayer_Lobby : MonoBehaviour
{
    public ListManager listManager;

    private Menu_Script sceneMenu;
    private additionalInfo lobbyInfos;

    private int connectionChangeState=0;
    // Start is called before the first frame update
    void Start()
    {
        sceneMenu = GetComponent<Menu_Script>();
        lobbyInfos = new additionalInfo(infoIdentifier.hostSelection); 
        sceneMenu.sceneInfos = lobbyInfos;

        listManager.entrySelected = entrySelection;
        listManager.entryDeSelected = entryDeSelection;

    }


    public void hostJoinedLobby(string ip)
    {
        listManager.addEntry(ip);
    }
    public void hostLeftLobby(string ip)
    {
        listManager.deleteEntry(ip);
    }

    private void entrySelection(string _name)
    {
        //Debug.Log($"{_name} was Selected over Delegate and ListManager.");
        lobbyInfos.hostIsSelected = true;
        lobbyInfos.joinIp = _name;
    }
    private void entryDeSelection(string _name)
    {
        //Debug.Log($"{_name} was DeSelected over Delegate and ListManager.");
        //lobbyInfos.hostIsSelected = false;
    }
}

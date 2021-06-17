using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Networking;

public class DotsAndBoxesSetup : MonoBehaviour
{
    public Button[] buttons;
    public InputField[] inputFields;

    public InputField widthInputField;
    public InputField heightInputField;
    public Text playersText;
    public Text AIText;

    private Menu_Script menu_Script;
    private NetworkManager networkManager;
    private NetworkInfo NetInfo;

    private int currentPlayerAmount=1;
    private int currentAIAmount = 1;
    private int width;
    private int height;
    private void Start()
    {
        menu_Script = GetComponent<Menu_Script>();
        Menu_Script.Buttonclick buttons = menu_Script.buttonClicks;
        menu_Script.buttonClicks = switchScene;
        menu_Script.buttonClicks += buttons;
        networkManager = menu_Script.networkManager;
        NetInfo = networkManager.myNetworkInfo;
        if (!NetInfo.isSingleplayer)
        {
            if (!NetInfo.isHost)
            {
                changeAllInteractions(false);
            }
            else
            {
                changeAllInteractions(true);
            }
            currentPlayerAmount = 1 + NetInfo.numberOfClients;
            currentAIAmount = 0;
            refreshNumberOfPlayers();
        }
    }
    public void addPlayer(int i)
    {
        if (currentPlayerAmount + currentAIAmount + i > 1 )
        {
            if (currentPlayerAmount + i >= NetInfo.numberOfClients)
            {
                currentPlayerAmount += i;
            }
        }
        refreshNumberOfPlayers();
    }
    public void addAI(int i)
    {
        if (currentPlayerAmount + currentAIAmount + i > 1)
        {
            if(currentAIAmount + i >= 0)
            {
                currentAIAmount += i;
            }
        }
        refreshNumberOfPlayers();
    }
    public void OnEndEdit()
    {
        int _width=width;
        int _height=height;
        int.TryParse(widthInputField.text, out _width);
        int.TryParse(heightInputField.text, out _height);
        if (_width > 1)
        {
            width = _width;
        }
        widthInputField.text = width.ToString();
        if (_height > 1)
        {
            height = _height;
        }
        heightInputField.text = height.ToString();
    }
    private void refreshNumberOfPlayers()
    {
        playersText.text = currentPlayerAmount.ToString();
        AIText.text = currentAIAmount.ToString();
    }
    private void changeAllInteractions(bool newState)
    {
        foreach (Button b in buttons)
        {
            b.interactable = newState;
        }
        foreach (InputField input in inputFields)
        {
            input.interactable = newState;
        }
    }
    private void switchScene(sceneNames name)
    {
        OnEndEdit();
        menu_Script.sceneInfos.localPlayers = currentPlayerAmount - NetInfo.numberOfClients;
        menu_Script.sceneInfos.networkPlayers = NetInfo.numberOfClients;
        menu_Script.sceneInfos.AIPlayers = currentAIAmount;
        menu_Script.sceneInfos.numberOfPlayers = currentPlayerAmount + currentAIAmount;
        menu_Script.sceneInfos.gridPointsWidth = width;
        menu_Script.sceneInfos.gridPointsHeight = height;
        //menu_Script.sceneInfos.DebugInfos();
    }
}

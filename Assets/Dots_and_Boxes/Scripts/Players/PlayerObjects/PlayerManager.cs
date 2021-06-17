using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DotsAndBoxes;
public class PlayerManager : MonoBehaviour
{
    private GameObjectLibrary goLib;
    private GameObject UIParent;
    private NetworkManager networkManager;

    private Text[] playerPointsText = new Text[2];
    private Text currentTurnText;

    private VariablesContainer vars;
    private Player[] playerList;
    private IInputHandler[] playerInputs;
    private int currentPlayerID;
    private IInputHandler currentInput;

    public void initialize(ref VariablesContainer varsRef)
    {
        UIParent = goLib.UIParent;
        findUI();


        vars = varsRef;
        setupPlayers();
        
        currentInput = playerInputs[currentPlayerID];
    }

    public void setupForNetwork()
    {
        goLib = GetComponent<GameObjectLibrary>();
        networkManager = goLib.menu_Script.networkManager;
        networkManager.packetHandlers.Add((int)PacketsNum.dotsNBoxes, networkDataHandle);
    }

    private void findUI()
    {
        //TODO: More general UI assignment
        playerPointsText[0]= UIParent.transform.GetChild(1).GetComponent<Text>();
        playerPointsText[1] = UIParent.transform.GetChild(3).GetComponent<Text>();
        currentTurnText = UIParent.transform.GetChild(5).GetComponent<Text>();
    }
    public void setupPlayers()
    {

        PointGridManager pointGrid = GetComponent<PointGridManager>();
        //playerPointsText = new Text[vars.NumberOfPlayers];
        playerList = new Player[vars.NumberOfPlayers];
        playerInputs = new IInputHandler[vars.NumberOfPlayers];
        for(int i = 0; i < vars.NumberOfPlayers;i++)
        {
            playerList[i] = new Player(i,vars.PlayerInputTypes[i],ref vars);
            //TODO: Check for doubles in Player. Instead: playerInputs[i]=playerList[i].input;

            switch (playerList[i].InputType)
            {
                case inputTypes.Mouse:
                    //Debug.Log("MousePlayer is there!");
                    goto default;

                case inputTypes.Touch:
                    //Debug.Log("TouchPlayer is there!");
                    goto default;

                case inputTypes.Keyboard:
                    //Debug.Log("KeyboardPlayer is there!");
                    goto default;

                case inputTypes.NetworkPlayer:
                    playerInputs[i] = new NetworkInput(ref vars);
                    //Debug.Log("NetworkPlayer is there!");
                    break;

                case inputTypes.AI:
                    playerInputs[i] = new AIInput(ref vars);
                    //Debug.Log("AI is there!");
                    break;

                default:
                    playerInputs[i] = new PlayerInput(ref vars);
                    break;
            }
            playerInputs[i].assignDelegates( ref pointGrid.ElementGotEntered, ref pointGrid.ElementGotExited);
            
        }    
        currentPlayerID = 0;
    }
    public void switchToNextPlayer()
    {
        //TODO: currentPlayerID+=1%=vars.NumberOfPlayers should do the same. 
        if (currentPlayerID < playerList.Length-1)
        {
            currentPlayerID += 1;
        } else
        {
            currentPlayerID = 0;
        }

        currentInput = playerInputs[currentPlayerID];
        currentTurnText.text = "Player " + (currentPlayerID + 1).ToString();

    }
    public void addPoints(int points)
    {
        playerList[currentPlayerID].addPoints(points);
        //TODO: Enable UI for more than 2 players!
        if (currentPlayerID < 2)
        {
            playerPointsText[currentPlayerID].text = playerList[currentPlayerID].Score.ToString();
        }
        //Debug.Log("PlayerID:" + currentPlayerID + " has " + playerList[currentPlayerID].Score +  " points.");
    }
    public InputState handledInput()
    {
        InputState currentInputState = currentInput.handleInput(vars.GameState);

        if (!networkManager.myNetworkInfo.isSingleplayer || networkManager.myNetworkInfo.isHost)
        {
            switch (playerList[currentPlayerID].InputType)
            {
                case inputTypes.NetworkPlayer:
                    break;

                case inputTypes.AI:
                    if (networkManager.myNetworkInfo.isHost)
                    {
                        using (Packet _packet = new Packet((int)PacketsNum.dotsNBoxes))
                        {
                            currentInputState.writePacket(_packet);
                            networkManager.sendTCPPacket(_packet);
                        }
                    }
                    break;

                default:
                    using (Packet _packet = new Packet((int)PacketsNum.dotsNBoxes))
                    {
                        currentInputState.writePacket(_packet);
                        networkManager.sendTCPPacket(_packet);
                    }
                    break;
            }
        }

        return currentInputState;
    }
    public void resetInputState()
    {
        currentInput.resetInputState();
    }

    public void networkDataHandle(int _myId, Packet _packet)
    {
        InputState currentInputState;
        if (playerList!=null)
        {
            switch (playerList[currentPlayerID].InputType)
            {
                case inputTypes.NetworkPlayer:
                    currentInputState = currentInput.handleInput(vars.GameState);
                    break;

                default:
                    currentInputState = new InputState();
                    break;
            }
            currentInputState.readPacket(_packet);
        }
    }
}

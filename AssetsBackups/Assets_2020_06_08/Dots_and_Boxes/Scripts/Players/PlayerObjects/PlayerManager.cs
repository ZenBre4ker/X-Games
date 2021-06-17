using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DotsAndBoxes;
public class PlayerManager : MonoBehaviour
{
    public Text[] playerPointsText = new Text[2];
    public Text currentTurnText;

    private VariablesContainer vars;
    private Player[] playerList;
    private IInputHandler[] playerInputs;
    private int currentPlayerID;
    private IInputHandler currentInput;

    public void initialize(ref VariablesContainer varsRef)
    {
        vars = varsRef;
        setupPlayers();
        
        currentInput = playerInputs[currentPlayerID];
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
            if (playerList[i].InputType == inputTypes.AI)
            {
                playerInputs[i] = new AIInput(ref vars);
                Debug.Log("AI is there!");
            }else
            {
                playerInputs[i] = new PlayerInput(ref vars);
                Debug.Log("Player is there!");
            }
            playerInputs[i].assignDelegates( ref pointGrid.ElementGotEntered, ref pointGrid.ElementGotExited);
            
        }    
        currentPlayerID = 0;
    }
    public void switchToNextPlayer()
    {
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
        playerPointsText[currentPlayerID].text = playerList[currentPlayerID].Score.ToString();
        //Debug.Log("PlayerID:" + currentPlayerID + " has " + playerList[currentPlayerID].Score +  " points.");
    }
    public InputState handledInput()
        {
            return currentInput.handleInput(vars.GameState);
        }
    public void resetInputState()
    {
        currentInput.resetInputState();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsAndBoxes;
public class Player
{
    private VariablesContainer vars;
    private int score;
    private IInputHandler input;
    private int playerID;
    private inputTypes inputType;
    public int Score { get { return score; } }
    public IInputHandler Input { get { return input; } }
    public int PlayerID { get { return playerID; } }
    public inputTypes InputType { get { return inputType; } }


    public Player(int newPlayerID, inputTypes newInputType,ref VariablesContainer varsRef)
    {
        playerID = newPlayerID;
        inputType = newInputType;
        vars = varsRef;
        switch (inputType)
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
                input = new NetworkInput(ref vars);
                //Debug.Log("NetworkPlayer is there!");
                break;

            case inputTypes.AI:
                input = new AIInput(ref vars);
                //Debug.Log("AI is there!");
                break;

            default:
                input = new PlayerInput(ref vars);
                break;
        }
        resetPoints();
    }
    public void addPoints(int points)
    {
        score += points;
    }

    public void resetPoints(){
        score = 0;
    }
}

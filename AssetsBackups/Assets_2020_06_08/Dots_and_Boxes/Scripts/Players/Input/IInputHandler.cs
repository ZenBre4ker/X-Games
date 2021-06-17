using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsAndBoxes;
public interface IInputHandler
{
    InputState handleInput(gameStates gameState);
    //void assignDelegates(ref clickedObjectNumber clickedDotEvent, ref clickedObjectNumber enteredDotEvent, ref clickedObjectNumber exitedDotEvent );
    void assignDelegates( ref clickedObjectNumber enteredDotEvent, ref clickedObjectNumber exitedDotEvent);
    void resetInputState();
}

public class InputState
{

    public bool firstPointIsChosen;
    public bool secondPointIsChosen;

    public int firstElementNumber;
    public int secondElementNumber;

    public Vector2 drawFromCoordinates;
    public Vector2 drawToCoordinates;

    public bool drawLine;

    public InputState()
    {
        resetState();
    }

    public void resetState()
    {

        firstPointIsChosen = false;
        secondPointIsChosen = false;

        firstElementNumber = -1;
        secondElementNumber = -1;

        drawFromCoordinates = new Vector2();
        drawToCoordinates = new Vector2();

        drawLine = false;

    }
}

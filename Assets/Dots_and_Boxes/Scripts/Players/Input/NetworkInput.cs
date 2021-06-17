using DotsAndBoxes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInput : IInputHandler
{
    private VariablesContainer vars;
    private InputState input;
    public NetworkInput(ref VariablesContainer varsRef)
    {
        input = new InputState();
        vars = varsRef;
    }
    public void assignDelegates(ref clickedObjectNumber enteredDotEvent, ref clickedObjectNumber exitedDotEvent)
    {
        enteredDotEvent += doNothing;
        exitedDotEvent += doNothing;
    }

    public InputState handleInput(gameStates gameState)
    {
        //get data over network
        return input;
    }

    public void resetInputState()
    {
        input.resetState();
    }
    private void doNothing(int elementNumber)
    {
        //Just to make sure the delegates get assigned. Better would be an delegate?.Invoke
    }
}

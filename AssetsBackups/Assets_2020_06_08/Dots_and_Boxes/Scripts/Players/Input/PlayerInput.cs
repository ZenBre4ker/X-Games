using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DotsAndBoxes;

//gameStates { Default, First_Selection, Second_Selection, CheckSelection, NextPlayer };
public class PlayerInput : IInputHandler
{
    private VariablesContainer vars;
    private InputState input;

    private bool cursorStateChanged;
    private bool[] isCursorDownStates;

    private int lastElement, currentElement;

    private int row, column;
    public PlayerInput(ref VariablesContainer varsRef)
    {
        input = new InputState();
        vars = varsRef;
        isCursorDownStates = new bool[2] { false, false };
        cursorStateChanged = false;
    }
    public void assignDelegates( ref clickedObjectNumber enteredDotEvent, ref clickedObjectNumber exitedDotEvent)
    {
        enteredDotEvent += elementGotEntered;
        exitedDotEvent += elementGotExited;
    }
    public InputState handleInput(gameStates gameState)
    {
        isCursorDownStates[0] = isCursorDown();
        if(isCursorDownStates[0] != isCursorDownStates[1])
        {
            //Debug.Log("cursorStateChanged from " + isCursorDownStates[1] + " to " + isCursorDownStates[0]);
            isCursorDownStates[1] = isCursorDownStates[0];
            cursorStateChanged = true;
        } else
        {
            cursorStateChanged = false;
        }

        switch (gameState)
        {
            case gameStates.First_Selection:
                _firstSelection();
                break;

            case gameStates.Second_Selection:
                _secondSelection();
                break;
        }

        return input;
    }
    private bool isCursorDown()
    {
        return (UnityEngine.Input.touchCount == 1 || UnityEngine.Input.GetMouseButton(0));
    }

    private void _firstSelection()
    {
        if (cursorStateChanged && isCursorDownStates[0])
        {
            //Debug.Log("First Element clicked: " + currentElement);
            if (currentElement >= 0)
            {
                PointGridManager.getInfoOfElementNumber(currentElement, ref vars, out row, out column);

                input.firstPointIsChosen = true;
                input.firstElementNumber = currentElement;
                input.drawFromCoordinates = PointGridManager.getPositionOfPoint(row, column, ref vars);
                input.drawToCoordinates = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
                input.drawLine = true;

                lastElement = currentElement;
            }
        }
    }
    private void _secondSelection()
    {
        input.drawToCoordinates = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
        if (cursorStateChanged && !isCursorDownStates[0])
        {
            if (currentElement != lastElement)
            {
                if (currentElement >= 0)
                {
                    PointGridManager.getInfoOfElementNumber(currentElement, ref vars, out row, out column);
                    input.drawToCoordinates = PointGridManager.getPositionOfPoint(row, column, ref vars);
                }

                input.secondPointIsChosen = true;
                input.secondElementNumber = currentElement;

                //Debug.Log("Second Element clicked: " + currentElement);
            } else
            {
                lastElement = -2; //so that after clicking the first element, no matter what or where you click after that it gets registered as new click
            }
        }
    }
    public void resetInputState()
    {
        lastElement = -1;
        input.resetState();
    }
    /*private void elementGotClicked(int elementNumber)
    {
        //clickedElement = elementNumber;
        //Debug.Log(elementNumber + " got clicked.");
    }*/

    private void elementGotEntered(int elementNumber)
    {
        currentElement = elementNumber;
        //Debug.Log(elementNumber + " got entered.");
    }

    private void elementGotExited(int elementNumber)
    {
        if(currentElement == elementNumber)
        {
            currentElement = -1;
        }
        //Debug.Log(elementNumber + " got exited.");
    }
}

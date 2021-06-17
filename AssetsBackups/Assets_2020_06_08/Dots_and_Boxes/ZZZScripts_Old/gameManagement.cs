using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsAndBoxes;
//TODO:
//BUG: Lines can still be attached to red points
//BUG: Sometimes points dont become red


public class gameManagement : MonoBehaviour
{
    public GameObject touchSpheres, black_Line,black_Square;

    private variablesManagement varManager;
    private initializeStartingGrid initGrid;
    private GameObject instantiateThis,currentObject,parentObject;
    private int[] clickedElements,numberOfTouchPoints;
    private int deltaElementNumber,lineElement, currentEnteredElement;
    private Vector2 startPos,deltaPos,endPos;
    private float deltaLength;
    private Quaternion currentRotation;

    void Start()
    {
        initGrid = new initializeStartingGrid();
        varManager = this.GetComponent<variablesManagement>();

        varManager.gameState = gameStates.Default;
        clickedElements = new int[2] { 0, 0 };
        numberOfTouchPoints = new int[2] { 0, 0 };
        currentEnteredElement = -1;
        
        varManager.initializeLists();
        initGrid.initGrid(varManager,this,touchSpheres,ref parentObject);

        varManager.gameState = gameStates.First_Selection;
        
    }
    private void Update()
    {
        handleGameStates();
        handleTouchControls();
    }

    private void handleGameStates()
    {
        switch (varManager.gameState)
        {
            case gameStates.Default:

                break;
            case gameStates.First_Selection:
                firstSelectionVoid();
                break;
            case gameStates.Second_Selection:
                secondSelectionVoid();
                break;
            case gameStates.CheckSelection:
                checkSelectionVoid();
                break;
            case gameStates.NextPlayer:
                //TODO: To be changed for player change
                varManager.gameState = gameStates.First_Selection;
                break;
        }
    }

    private void handleTouchControls()
    {
        numberOfTouchPoints[1] = numberOfTouchPoints[0];
        numberOfTouchPoints[0] = Input.touchCount;
    }
    private void firstSelectionVoid()
    {

    }
    private void secondSelectionVoid()
    {
        endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        setLineTransform(currentObject, startPos, endPos);
        if (currentEnteredElement > -1 && currentEnteredElement != clickedElements[0]) 
        {
            if (Input.GetMouseButtonUp(0) == true || (numberOfTouchPoints[0] == 0 && numberOfTouchPoints[0] < numberOfTouchPoints[1]))
            {
                iGotClicked(varManager.pointGameObjects[currentEnteredElement].transform.localPosition, currentEnteredElement);
            }
        }
    }
    private void checkSelectionVoid()
    {
        deltaElementNumber = clickedElements[1] - clickedElements[0];
        switch (Mathf.Abs(deltaElementNumber))
        {
            case 1:
                if (Mathf.Floor(clickedElements[1] / (varManager.gridSize[1] - 1)) - Mathf.Floor(clickedElements[1] / (varManager.gridSize[1] - 1)) == 0)
                {
                    lineElement = Mathf.Min(clickedElements[0], clickedElements[1]) - Mathf.FloorToInt(clickedElements[0] / varManager.gridSize[1]);
                    setLineForPoints(deltaElementNumber, lineElement);
                } else
                {
                    resetSelectedPoints();
                }
                break;
            default:
                if (Mathf.Abs(deltaElementNumber) == varManager.gridSize[1])
                {
                    lineElement = (varManager.gridSize[1] - 1) * varManager.gridSize[0] + Mathf.Min(clickedElements[0], clickedElements[1]);
                    setLineForPoints(deltaElementNumber, lineElement);
                } else
                {
                    resetSelectedPoints();
                }
                break;

        }
    }
    private void setLineForPoints(int difference, int lineNumber)
    {
        if (varManager.lineStates[lineNumber] == 0)
        {
            varManager.gameState = gameStates.NextPlayer;
            varManager.lineStates[lineNumber] = 1;
            setSquareForLines(lineNumber);
            for (int i = 0; i < 2; i++)
            {
                if (varManager.pointNumbers[clickedElements[i]]== varManager.maxPointNumbers[clickedElements[i]])
                {
                    varManager.pointClickScripts[clickedElements[i]].changeColor();
                }
            }
        } else
        {
            resetSelectedPoints();
        }

    }
    private void setSquareForLines(int lineNumber)
    {
        float xPos, yPos, xSize, ySize;
        int squareNumber;
        for (int i = 0; i < 2; i++)
        {
            squareNumber = varManager.lineToSquarePointer[lineNumber, i];
            if (squareNumber >= 0)
            {
                varManager.squareStates[squareNumber] += 1;
                if (varManager.squareStates[squareNumber] == 4)
                {
                    xPos = -varManager.gridWidth / 2 + (Mathf.Floor(squareNumber / (varManager.gridSize[1] - 1)) + 0.5f) * varManager.gridWidth / (varManager.gridSize[0] - 1);
                    yPos = -varManager.gridHeight / 2 + (squareNumber % (varManager.gridSize[1] - 1) + 0.5f) * varManager.gridHeight / (varManager.gridSize[1] - 1);
                    xSize = varManager.gridWidth / (varManager.gridSize[0] - 1);
                    ySize = varManager.gridHeight / (varManager.gridSize[1] - 1);
                    instantiateThis = GameObject.Instantiate(black_Square);
                    instantiateThis.transform.localPosition = new Vector3(xPos, yPos, 0f);
                    instantiateThis.transform.localScale = new Vector3(xSize, ySize, 0f); 
                    instantiateThis.transform.SetParent(parentObject.transform);
                }
            }
        }
    }
    private void resetSelectedPoints()
    {
        GameObject.Destroy(currentObject);
        varManager.gameState = gameStates.First_Selection;
        for (int i = 0; i < 2; i++){
            varManager.pointNumbers[clickedElements[i]] -= 1;
            varManager.pointClickScripts[clickedElements[i]].permanentlyActivated = false;
        }
    }
    private void setLineTransform(GameObject currentLine, Vector2 startingPosition, Vector2 endPosition)
    {
        deltaPos = endPosition - startingPosition;
        deltaLength = deltaPos.magnitude;
        currentRotation = Quaternion.FromToRotation(Vector2.up, deltaPos);

        currentLine.transform.localPosition = startingPosition;
        currentLine.transform.localRotation = currentRotation;
        currentLine.transform.GetChild(0).transform.localScale = Vector3.up * deltaLength + Vector3.right + Vector3.forward;
    }
    public void iGotClicked(Vector2 myCoordinates, int myElementNumber)
    {
        if (varManager.pointNumbers[myElementNumber]<varManager.maxPointNumbers[myElementNumber])
        {
            if (varManager.gameState == gameStates.First_Selection)
            {
                clickedElements[0] = myElementNumber;
                startPos = myCoordinates;

                instantiateThis = GameObject.Instantiate(black_Line);
                instantiateThis.transform.SetParent(parentObject.transform);
                currentObject = instantiateThis;
                currentObject.transform.localPosition = startPos;

                varManager.gameState = gameStates.Second_Selection;

            } else
            {
                clickedElements[1] = myElementNumber;
                endPos = myCoordinates;
                setLineTransform(currentObject, startPos, endPos);

                varManager.gameState = gameStates.CheckSelection;
            }

            varManager.pointNumbers[myElementNumber] += 1;
            if (varManager.pointNumbers[myElementNumber]== varManager.maxPointNumbers[myElementNumber])
            {
                varManager.pointClickScripts[myElementNumber].permanentlyActivated = true;
            }
        } else if (varManager.gameState == gameStates.Second_Selection)
        {
            clickedElements[1] = myElementNumber;
            endPos = myCoordinates;
            setLineTransform(currentObject, startPos, endPos);

            varManager.gameState = gameStates.CheckSelection;
        }
    }

    public void iGotEntered(int myElementNumber)
    {
        currentEnteredElement = myElementNumber;
    }
    public void iGotExited(int myElementNumber)
    {
        if (currentEnteredElement == myElementNumber)
        {
            currentEnteredElement = -1;
        }
    }

}

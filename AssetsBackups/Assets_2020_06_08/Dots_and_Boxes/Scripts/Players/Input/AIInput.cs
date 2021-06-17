using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsAndBoxes;
public class AIInput : IInputHandler
{
    private InputState input;
    private VariablesContainer vars;

    private int firstElement, secondElement, freeSquares, freeChoseables;

    List<int> freeSquareList, freeChoseableSquares,almostSquares, prioritySquares;

    private int row, column;

    bool searchOptimalLine;
    public AIInput(ref VariablesContainer varsRef)
    {
        input = new InputState();
        vars = varsRef;
        searchOptimalLine = true;

        freeSquares = vars.SquareStates.Length;
        freeChoseables = freeSquares;
        freeSquareList = new List<int>();
        almostSquares = new List<int>();
        prioritySquares = new List<int>();
        for (int i=0;i< freeSquares; i++)
        {
            freeSquareList.Add(i);
        }
        freeChoseableSquares = new List<int>(freeSquareList);
    }
    public void assignDelegates( ref clickedObjectNumber enteredDotEvent,ref  clickedObjectNumber exitedDotEvent)
    {
        
    }

    public InputState handleInput(gameStates gameState)
    {
        //Debug.Log("Handle AI Input of GameState " + gameState.ToString());
        if (searchOptimalLine)
        {
            findOptimalLine();
            setInputState();
        }

        //For Debugging AI
        /*if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        { 
            setInputState();
        }*/
        return input;
    }


    public void resetInputState()
    {
        input.resetState();
        searchOptimalLine = true;
    }

    private void findOptimalLine()
    {
        int lineNumber,squarePointer, squarePointer2, squareState, squareState2, squareCounter;
        bool hasChoseableNeighbors = false;
        List<int> freeLines = new List<int>();

        squareCounter = freeSquareList.Count;
        for (int i = 0; i < squareCounter; i++)
        {
            squarePointer = freeSquareList[i];
            squareState = vars.SquareStates[squarePointer];
            if (squareState >= 4)
            {
                freeSquareList.RemoveAt(i--);
                squareCounter--;
            }
        }

        squareCounter = prioritySquares.Count;
        for (int i = 0; i < squareCounter; i++)
        {
            squarePointer = prioritySquares[i];
            squareState = vars.SquareStates[squarePointer];
            if (squareState > 3)
            {
                prioritySquares.RemoveAt(i--);
                squareCounter--;
            }
        }

        squareCounter = almostSquares.Count;
        for (int i = 0; i < squareCounter; i++)
        {
            squarePointer = almostSquares[i];
            squareState = vars.SquareStates[squarePointer];
            if (squareState > 2)
            {
                if (squareState == 3)
                {
                    prioritySquares.Add(squarePointer);
                }
                almostSquares.RemoveAt(i--);
                squareCounter--;
            }
        }

        squareCounter = freeChoseableSquares.Count;
        for (int i = 0; i < squareCounter; i++)
        {
            squarePointer = freeChoseableSquares[i];
            squareState = vars.SquareStates[squarePointer];
            hasChoseableNeighbors = false;
            for (int j = 0; j < 4; j++)
            {
                lineNumber = vars.SquareToLinesPointer[squarePointer, j];
                if (vars.LineStates[lineNumber] < 1)
                {
                    squarePointer2 = -1;
                    squareState2 = -1;
                    for (int k = 0; k < 2; k++)
                    {
                        if (k == 0 || squarePointer2 == squarePointer)
                        {
                            squarePointer2 = vars.LineToSquarePointer[lineNumber, k];
                            if (squarePointer2 >= 0)
                            {
                                squareState2 = vars.SquareStates[squarePointer2];
                            }
                            else
                            {
                                squareState2 = -1;
                            }
                        }

                    }
                    if (squareState2 != 2)
                    {
                        hasChoseableNeighbors = true;
                    }
                }
            }
            if (squareState > 1 || !hasChoseableNeighbors)
            {
                if(squareState == 2 || !hasChoseableNeighbors)
                {
                    almostSquares.Add(squarePointer);
                } else if (squareState==3)
                {
                    prioritySquares.Add(squarePointer);
                }
                freeChoseableSquares.RemoveAt(i--);
                squareCounter--;
            }
        }
        if (prioritySquares.Count > 0)
        {
            squarePointer = prioritySquares[Random.Range(0, prioritySquares.Count)];
            for (int i = 0; i < 4; i++)
            {
                lineNumber = vars.SquareToLinesPointer[squarePointer, i];
                if (vars.LineStates[lineNumber] < 1)
                {
                    freeLines.Add(lineNumber);
                    searchOptimalLine = false;
                    break;
                }
            }
        } else if ( freeChoseableSquares.Count >0)
        {
            squarePointer = freeChoseableSquares[Random.Range(0, freeChoseableSquares.Count)];
            for (int j = 0; j < 4; j++)
            {
                lineNumber = vars.SquareToLinesPointer[squarePointer, j];
                if (vars.LineStates[lineNumber] < 1)
                {
                    squarePointer2 = -1;
                    squareState2 = -1;
                    for (int k = 0; k < 2; k++)
                    {
                        if (k == 0 || squarePointer2 == squarePointer)
                        {
                            squarePointer2 = vars.LineToSquarePointer[lineNumber, k];
                            if (squarePointer2 >= 0)
                            {
                                squareState2 = vars.SquareStates[squarePointer2];
                            }
                            else
                            {
                                squareState2 = -1;
                            }
                        }

                    }
                    if (squareState2 != 2)
                    {
                        freeLines.Add(lineNumber);
                        searchOptimalLine = false;
                    }
                }
            }
        } else if(almostSquares.Count>0)
        {
            squarePointer = almostSquares[Random.Range(0, almostSquares.Count)];
            for (int i = 0; i < 4; i++)
            {
                lineNumber = vars.SquareToLinesPointer[squarePointer, i];
                if (vars.LineStates[lineNumber] < 1)
                {
                    freeLines.Add(lineNumber);
                    searchOptimalLine = false;
                }
            }

        }
        if (freeLines.Count > 0)
        {
            lineNumber = freeLines[Random.Range(0, freeLines.Count)];
            PointGridManager.getElementNumbersOfLine(lineNumber, out firstElement, out secondElement, ref vars);
        } else
        {
            searchOptimalLine = false;
        }
        /*int minSquareState = 4, maxSquareState = 0, minSquareNumber = 0, squareNumber = 0, squareNumber2=-1, lineNumber = 0,squareState=0, pointer=0;
        bool hasChoseableNeighbors = false;
        List<int> freeLines = new List<int>();
        for(int i=0; i < freeSquares; i++)
        {
            pointer = freeSquareList[i];
            squareState = vars.SquareStates[pointer];
            if (squareState < 4 )
            {
                if(squareState > maxSquareState)
                {
                    maxSquareState = squareState;
                    squareNumber = pointer;
                    if (maxSquareState > 2)
                    {
                        break;
                    }
                } else if(squareState < minSquareState)
                {
                    minSquareState = squareState;
                    minSquareNumber = pointer;
                }
            } else
            {
                freeSquareList.RemoveAt(i--);
                freeSquares -= 1;
            }
        }
        for(int i = 0; i < freeChoseables; i++)
        {
            pointer = freeChoseableSquares[i];
            squareState = vars.SquareStates[pointer];
            if (squareState > 1)
            {
                freeChoseableSquares.RemoveAt(i--);
                freeChoseables -= 1;
            } else
            {
                hasChoseableNeighbors = false;
                for (int j = 0; j < 4; j++)
                {
                    lineNumber = vars.SquareToLinesPointer[pointer, j];
                    if (vars.LineStates[lineNumber] < 1)
                    {
                        squareState = -1;
                        for (int k = 0; k < 2; k++)
                        {
                            if (k == 0 || squareNumber2 == squareNumber)
                            {
                                squareNumber2 = vars.LineToSquarePointer[lineNumber, k];
                                if (squareNumber2 >= 0)
                                {
                                    squareState = vars.SquareStates[squareNumber2];
                                }
                                else
                                {
                                    squareState = -1;
                                }
                            }

                        }
                        if (squareState != 2)
                        {
                            hasChoseableNeighbors = true;
                        }
                    }
                }
                if (!hasChoseableNeighbors)
                {
                    freeChoseableSquares.RemoveAt(i--);
                    freeChoseables -= 1;
                } 
            }
        }
        if (maxSquareState < 2 && freeSquares>0)
        {
            squareNumber = freeSquareList[Random.Range(0,freeSquares)];
        } else if (maxSquareState == 2 && freeChoseables>0)
        {
            squareNumber = freeChoseableSquares[Random.Range(0, freeChoseables)];
        }
        for (int i = 0; i < 4; i++)
        {
            lineNumber = vars.SquareToLinesPointer[squareNumber, i];
            if (vars.LineStates[lineNumber] < 1)
            {
                squareState = -1;
                for (int k = 0; k < 2; k++)
                {
                    if (k == 0 || squareNumber2 == squareNumber)
                    {
                        squareNumber2 = vars.LineToSquarePointer[lineNumber, k];
                        if (squareNumber2 >= 0)
                        {
                            squareState = vars.SquareStates[squareNumber2];
                        }
                        else
                        {
                            squareState = 2;
                        }
                    }

                }
                if (squareState != 2 || freeChoseableSquares.Count<1)
                {
                    freeLines.Add(lineNumber);
                    searchOptimalLine = false;
                }
            }
        }

        int chooser = Random.Range(0, freeLines.Count);
        Debug.Log(freeLines.Count + " " + freeChoseableSquares.Count + " " + chooser);
        lineNumber = freeLines[chooser];
        PointGridManager.getElementNumbersOfLine(lineNumber, out firstElement, out secondElement, ref vars);*/

        /*Debug.Log("Min Square State: " + minSquareState + " Max Square State: " + maxSquareState);
        Debug.Log("Free Squares: " + freeSquares + " Free Choseable: " + freeChoseables);
        for (int i = 0; i < freeChoseables; i++)
        {
            Debug.Log(freeChoseableSquares[i]);
        }*/
    }
    private void setInputState()
    {
        input.firstPointIsChosen = true;
        input.secondPointIsChosen = true;

        input.secondElementNumber = firstElement;
        input.firstElementNumber = secondElement;

        PointGridManager.getInfoOfElementNumber(firstElement, ref vars, out row, out column);
        input.drawFromCoordinates = PointGridManager.getPositionOfPoint(row, column, ref vars);

        PointGridManager.getInfoOfElementNumber(secondElement, ref vars, out row, out column);
        input.drawToCoordinates = PointGridManager.getPositionOfPoint(row, column, ref vars);

        input.drawLine = true;
    }
}

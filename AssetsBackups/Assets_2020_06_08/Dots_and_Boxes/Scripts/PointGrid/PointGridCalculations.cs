using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsAndBoxes;
public partial class PointGridManager : MonoBehaviour
{
    public static int getElementNumberOfPoint(int row, int column,ref VariablesContainer varsRef)
    {
        int elementNumber = row * varsRef.GridSize[1] + column;
            return elementNumber;
    }
    public static void getInfoOfElementNumber(int elementNumber, ref VariablesContainer varsRef, out int row, out int column)
    {
        //Rows go x-times the Gridsize[1] into the elementNumber
        row = elementNumber/varsRef.GridSize[1];

        //The rest, of the division by the Gridsize, determine the column
        column = elementNumber%varsRef.GridSize[1];
    }
    public static Vector2 getPositionOfPoint( int row, int column,ref VariablesContainer varsRef)
    {
        float rowPosition = varsRef.GridWidth*(row*1f / (varsRef.GridSize[0] - 1) - 0.5f);
        float colPosition = varsRef.GridHeight*(column*1f / (varsRef.GridSize[1] - 1)-0.5f);
        Vector2 startPos = new Vector2(rowPosition, colPosition);
        return startPos;
    }
    public static int getLineNumberOfElements(int firstElement, int secondElement, ref VariablesContainer varsRef)
    {
        int lineNumber;
        int deltaElementNumber = firstElement - secondElement;
        switch (Mathf.Abs(deltaElementNumber))
        {
            case 1:
                    lineNumber = Mathf.Min(firstElement, secondElement) - firstElement / varsRef.GridSize[1];
                break;
            default:
                    lineNumber = Mathf.Min(firstElement, secondElement) + (varsRef.GridSize[1] - 1) * varsRef.GridSize[0];
                break;
        }
        return lineNumber;
    }
    public static void getElementNumbersOfLine(int lineNumber, out int firstElement,out int secondElement, ref VariablesContainer varsRef)
    {
        int row, column;
        firstElement=0;
        secondElement=0;
        if(lineNumber < (varsRef.GridSize[0] - 1) * varsRef.GridSize[1])
        {
            row = lineNumber / (varsRef.GridSize[1] - 1);
            column = lineNumber % (varsRef.GridSize[1] - 1);
            firstElement = row * varsRef.GridSize[1] + column; 
            secondElement = firstElement + 1;
        } else
        {
            lineNumber -= (varsRef.GridSize[0] - 1) * varsRef.GridSize[1];
            row = lineNumber / (varsRef.GridSize[1]);
            column = lineNumber % (varsRef.GridSize[1]);
            firstElement = row * varsRef.GridSize[1] + column;
            secondElement = firstElement + varsRef.GridSize[1];
        }
    }

    public static bool isPointAvailable(int elementNumber, ref VariablesContainer varsRef)
    {
        if (elementNumber >= 0)
        {
            if (varsRef.PointNumbers[elementNumber] < varsRef.MaxPointNumbers[elementNumber])
            {
                return true;
            }
            else
            {
                return false;
            }
        } else
        {
            return false;
        }
    }
    public static bool canLineBeDrawn (int firstElement, int secondElement, ref VariablesContainer varsRef)
    {
        int deltaElementNumber = secondElement - firstElement;
        switch (Mathf.Abs(deltaElementNumber))
        {
            case 1:
                //when two elements are neighbours, their numbers only differ by one,
                //but with the one-dimensional storage, they can also be on another column
                //therefore their integerdivision by columnlength should be the same
                if (firstElement / varsRef.GridSize[1] == secondElement / varsRef.GridSize[1] )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                if (Mathf.Abs(deltaElementNumber) == varsRef.GridSize[1])
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }
    }
    public static void drawLine(GameObject currentLine, Vector2 startingPosition, Vector2 endPosition)
    {
        Vector2 deltaPos = endPosition - startingPosition;
        float deltaLength = deltaPos.magnitude;
        Quaternion currentRotation = Quaternion.FromToRotation(Vector2.up, deltaPos);

        currentLine.transform.localPosition = startingPosition;
        currentLine.transform.localRotation = currentRotation;
        currentLine.transform.GetChild(0).transform.localScale = Vector3.right + Vector3.up * deltaLength + Vector3.forward;
    }

    public static void drawSquare(int squareNumber,GameObject square, GameObject parent, ref VariablesContainer varsRef)
    {
        float xPos, yPos, xSize, ySize;
        xPos = -varsRef.GridWidth / 2 + (Mathf.Floor(squareNumber / (varsRef.GridSize[1] - 1)) + 0.5f) * varsRef.GridWidth / (varsRef.GridSize[0] - 1);
        yPos = -varsRef.GridHeight / 2 + (squareNumber % (varsRef.GridSize[1] - 1) + 0.5f) * varsRef.GridHeight / (varsRef.GridSize[1] - 1);
        xSize = varsRef.GridWidth / (varsRef.GridSize[0] - 1);
        ySize = varsRef.GridHeight / (varsRef.GridSize[1] - 1);
        square.transform.localPosition = new Vector3(xPos, yPos, 0f);
        square.transform.localScale = new Vector3(xSize, ySize, 0f);
        square.transform.SetParent(parent.transform);
    }
}

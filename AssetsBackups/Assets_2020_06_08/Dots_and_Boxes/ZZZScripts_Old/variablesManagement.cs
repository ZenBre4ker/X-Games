using UnityEngine;
using DotsAndBoxes;
//public enum gameStates { Default, First_Selection, Second_Selection, CheckSelection, NextPlayer };
public class variablesManagement : MonoBehaviour
{
    public GameObject[] pointGameObjects { get; set; }
    public int[] pointNumbers { get; set; }
    public int[] maxPointNumbers { get; set; }
    public int[] lineStates { get; set; }
    public int[,] lineToSquarePointer { get; set; }
    public int[] squareStates { get; set; }
    public clickOnBehaviour[] pointClickScripts { get; set; }
    public int[] gridSize { get; set; } //Width x Height

    [Header("Number of Points in X and Y Direction")]
    public int gridPointsWidth = 5;
    public int gridPointsHeight = 5;

    [Header("Actual Size in X and Y Direction (Floats)")]
    public float gridWidth = 4f;
    public float gridHeight = 4f;

    public gameStates gameState;


    public void initializeLists()
    {
        gridSize = new int[2] { gridPointsWidth, gridPointsHeight }; //Width x Height
        pointGameObjects = new GameObject[gridSize[0] * gridSize[1]];
        pointClickScripts = new clickOnBehaviour[gridSize[0] * gridSize[1]];
        pointNumbers = new int[gridSize[0] * gridSize[1]];
        maxPointNumbers = new int[gridSize[0] * gridSize[1]];
        lineStates = new int[(gridSize[0] - 1) * gridSize[1] + gridSize[0] * (gridSize[1] - 1)];
        lineToSquarePointer = new int[(gridSize[0] - 1) * gridSize[1] + gridSize[0] * (gridSize[1] - 1),2];
        squareStates = new int[(gridSize[0] - 1) * (gridSize[1] - 1)];
        calculateMaxPointNumbers();
        calculateLineToSquarePointers();
    }

    private void calculateMaxPointNumbers()
    {
        int elementNumber;
        //Set BorderElements to 3 and MiddleElements to 4
        for (int i = 0; i < gridSize[0]; i ++)
        {
            for (int j = 0; j < gridSize[1]; j ++)
            {
                elementNumber = i * gridSize[1] + j;
                if(i==0 || i==gridSize[0]-1 || j == 0 || j == gridSize[1]-1)
                {
                    maxPointNumbers[elementNumber] = 3;
                } else
                {
                    maxPointNumbers[elementNumber] = 4;
                }
                    
            }
        }
        //Set CornerElements to 2
        for (int i = 0; i < gridSize[0]; i+=gridSize[0]-1)
        {
            for (int j = 0; j < gridSize[1]; j+=gridSize[1]-1)
            {
                elementNumber= i * gridSize[1] + j;
                maxPointNumbers[elementNumber] = 2;
            }
        }
    }
    private void calculateLineToSquarePointers()
    {
        //Every Line can point to 2 Squares
        //-1 stands for no square on the playing field

        //Assigning all horizontal lines to squares first
        int counter = 0;
        for (int i = 0; i < gridSize[0]; i++)
        {
            for (int j = 0; j < gridSize[1]-1; j++)
            {
                if (i == 0)
                {
                    lineToSquarePointer[counter, 0] = -1;
                    lineToSquarePointer[counter, 1] = counter;
                }
                else if (i < gridSize[0] - 1)
                {
                    lineToSquarePointer[counter, 0] = counter- (gridSize[1] - 1);
                    lineToSquarePointer[counter,1] = counter;
                }
                else
                {
                    lineToSquarePointer[counter, 0] = counter - (gridSize[1] - 1);
                    lineToSquarePointer[counter,1] = -1;
                }
                counter++;
            }

        }

        //Then assigning all vertical lines to squares
        int square = 0;
        for (int i = 0; i < gridSize[0] - 1; i++)
        {
            for (int j = 0; j < gridSize[1]; j++)
            {
                if (j == 0)
                {
                    lineToSquarePointer[counter, 0] = -1;
                    lineToSquarePointer[counter, 1] = square;
                }
                else if (j < gridSize[1] - 1)
                {
                    lineToSquarePointer[counter, 0] = square;
                    lineToSquarePointer[counter, 1] = ++square;
                }
                else
                {
                    lineToSquarePointer[counter, 0] = square++;
                    lineToSquarePointer[counter, 1] = -1;
                }
                counter++;
            }

        }


    }
}

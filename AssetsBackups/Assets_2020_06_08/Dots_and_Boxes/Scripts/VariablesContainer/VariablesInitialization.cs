using UnityEngine;

namespace DotsAndBoxes
{
    public partial class VariablesContainer : MonoBehaviour
    {
        public VariablesContainer()
        {
            resetFields();
        }
        public VariablesContainer(int pointsInWidth, int pointsInHeight, float widthSize, float heightSize)
        {
            gridPointsWidth = pointsInWidth;
            gridPointsHeight = pointsInHeight;
            gridWidth = widthSize;
            gridHeight = heightSize;
            resetFields();
        }
        public void initialize()
        {
            resetFields();
        }
        public void initialize(int pointsInWidth, int pointsInHeight, float widthSize, float heightSize)
        {
            gridPointsWidth = pointsInWidth;
            gridPointsHeight = pointsInHeight;
            gridWidth = widthSize;
            gridHeight = heightSize;
            resetFields();
        }

        private void resetFields()
        {
            GameState = gameStates.Default;
            gridSize = new int[2] { gridPointsWidth, gridPointsHeight }; //Width x Height
            pointGameObjects = new GameObject[gridSize[0] * gridSize[1]];
            pointColliderScripts = new PointColliderEvents[gridSize[0] * gridSize[1]];
            pointNumbers = new int[gridSize[0] * gridSize[1]];
            maxPointNumbers = new int[gridSize[0] * gridSize[1]];
            LineStates = new int[(gridSize[0] - 1) * gridSize[1] + gridSize[0] * (gridSize[1] - 1)];
            LineGameObjects = new GameObject[(gridSize[0] - 1) * gridSize[1] + gridSize[0] * (gridSize[1] - 1)];
            lineToSquarePointer = new int[(gridSize[0] - 1) * gridSize[1] + gridSize[0] * (gridSize[1] - 1), 2];
            squareToLinesPointer = new int[(gridSize[0] - 1) * (gridSize[1] - 1),4];
            SquareStates = new int[(gridSize[0] - 1) * (gridSize[1] - 1)];
            SquareGameObjects = new GameObject[(gridSize[0] - 1) * (gridSize[1] - 1)];
            calculateMaxPointNumbers(ref maxPointNumbers);
            calculateSquareToLinesPointer(ref lineToSquarePointer, ref squareToLinesPointer);
        }
        private void calculateMaxPointNumbers(ref int[] maxPointNumbersRef)
        {
            int elementNumber;
            //Set BorderElements to 3 and MiddleElements to 4
            for (int i = 0; i < gridSize[0]; i++)
            {
                for (int j = 0; j < gridSize[1]; j++)
                {
                    elementNumber = i * gridSize[1] + j;
                    if (i == 0 || i == gridSize[0] - 1 || j == 0 || j == gridSize[1] - 1)
                    {
                        maxPointNumbersRef[elementNumber] = 3;
                    }
                    else
                    {
                       maxPointNumbersRef[elementNumber] = 4;
                    }

                }
            }
            //Set CornerElements to 2
            for (int i = 0; i < gridSize[0]; i += gridSize[0] - 1)
            {
                for (int j = 0; j < gridSize[1]; j += gridSize[1] - 1)
                {
                    elementNumber = i * gridSize[1] + j;
                    maxPointNumbers[elementNumber] = 2;
                }
            }
        }

        private void calculateSquareToLinesPointer(ref int[,] lineToSquarePointerRef, ref int[,] squareToLinesPointerRef)
        {
            //Every Line can point to 2 Squares
            //-1 stands for no square on the playing field

            //Assigning all horizontal lines to squares first
            int line = 0;
            int[] squareAssignments = new int[squareToLinesPointerRef.Length / 4];
            int[] square = new int[2];

            for (int i = 0; i < gridSize[0]; i++)
            {
                for (int j = 0; j < gridSize[1] - 1; j++)
                {
                    if (i == 0)
                    {
                        square[0] = -1;
                        square[1] = line;
                    }
                    else if (i < gridSize[0] - 1)
                    {
                        square[0] = line - (gridSize[1] - 1);
                        square[1] = line;
                    }
                    else
                    {
                        square[0] = line - (gridSize[1] - 1);
                        square[1] = -1;
                    }
                    lineToSquarePointerRef[line, 0] = square[0];
                    lineToSquarePointerRef[line, 1] = square[1];

                    //Now inverting the structure and assigning to each square the corresponding lines
                    for (int k = 0; k < 2; k++)
                    {
                        if (square[k] >= 0)
                        {
                            squareToLinesPointerRef[square[k], squareAssignments[square[k]]] = line;
                            squareAssignments[square[k]] += 1;
                        }
                    }
                    line++;

                }

            }

            //Then assigning all vertical lines to squares
            int squareNumber = 0;
            for (int i = 0; i < gridSize[0] - 1; i++)
            {
                for (int j = 0; j < gridSize[1]; j++)
                {
                    if (j == 0)
                    {
                        square[0] = -1;
                        square[1] = squareNumber;
                    }
                    else if (j < gridSize[1] - 1)
                    {
                        square[0] = squareNumber;
                        square[1] = ++squareNumber;
                    }
                    else
                    {
                        square[0] = squareNumber++;
                        square[1] = -1;
                    }
                    lineToSquarePointerRef[line, 0] = square[0];
                    lineToSquarePointerRef[line, 1] = square[1];

                    //Now inverting the structure and assigning to each square the corresponding lines
                    for (int k = 0; k < 2; k++)
                    {
                        if (square[k] >= 0)
                        {
                            squareToLinesPointerRef[square[k], squareAssignments[square[k]]] = line;
                            squareAssignments[square[k]] += 1;
                        }
                    }
                    line++;
                }

            }
        }
    }
}

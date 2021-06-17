using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class initializeStartingGrid : MonoBehaviour
{
    public void initGrid(variablesManagement varManager, gameManagement gameManager, GameObject prefab,ref GameObject newParent)
    {
        //Variables
        int currentElementNumber = 0;
        int[] gridSize = varManager.gridSize; //Width x Height
        float gridWidth = varManager.gridWidth, gridHeight = varManager.gridHeight;
        Vector2 startPos;
        clickOnBehaviour clickScript;
        GameObject instantiateThis;

        //Parent empty GameObject
        GameObject playingField = new GameObject();
        playingField.name = "playingField";
        playingField.transform.localPosition = Vector3.zero;
        newParent = playingField;

        //iterating over width and height of the grid
        for (int i = 0; i < gridSize[0]; i++)
        {
            for (int j = 0; j < gridSize[1]; j++)
            {
                //calculate Number and Position
                currentElementNumber = i * gridSize[1] + j;
                startPos = new Vector2(-gridWidth / 2 + i * gridWidth / (gridSize[0] - 1), -gridHeight / 2 + j * gridHeight / (gridSize[1] - 1));

                //Instantiate the Prefab with Name,Position and Parent
                instantiateThis = GameObject.Instantiate(prefab);
                instantiateThis.name = "Ball " + currentElementNumber.ToString();
                instantiateThis.transform.localPosition = startPos;
                instantiateThis.transform.SetParent(playingField.transform);

                //Supply GameObject-Variables with corressponding Values
                clickScript = instantiateThis.GetComponent<clickOnBehaviour>();
                clickScript.gMScript = gameManager;
                clickScript.elementNumber = currentElementNumber;

                //Fill in the Lists of the Manager with the GameObjects
                varManager.pointGameObjects[currentElementNumber] = instantiateThis;
                varManager.pointNumbers[currentElementNumber] = 0;
                varManager.pointClickScripts[currentElementNumber] = clickScript;
            }
        }
    }
}

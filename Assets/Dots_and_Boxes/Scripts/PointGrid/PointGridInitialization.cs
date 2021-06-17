using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsAndBoxes;
public partial class PointGridManager : MonoBehaviour
{
    public void initGrid(ref VariablesContainer varsRef)
    {
        //Variables
        int currentElementNumber = 0;
        GameObject instantiateThis;
        PointColliderEvents pointColliderScript;

        //Parent empty GameObject
        playingField = new GameObject();
        playingField.name = "pointGrid";
        playingField.transform.localPosition = Vector3.zero;
        playingField.transform.SetParent(this.transform);

        //iterating over width and height of the grid
        for (int i = 0; i < varsRef.GridSize[0]; i++)
        {
            for (int j = 0; j < varsRef.GridSize[1]; j++)
            {
                //calculate Number
                currentElementNumber = getElementNumberOfPoint(i, j, ref varsRef);

                //Instantiate the Prefab with Name,Position and Parent
                instantiateThis = GameObject.Instantiate(varsRef.touchSpheres);
                instantiateThis.name = "Ball " + currentElementNumber.ToString();
                instantiateThis.transform.localPosition = getPositionOfPoint(i, j, ref varsRef);
                instantiateThis.transform.SetParent(playingField.transform);

                //Supply GameObject-Variables with corressponding Values
                pointColliderScript = instantiateThis.AddComponent<PointColliderEvents>();
                pointColliderScript.Initialize(currentElementNumber, this, true, varsRef.looseSprite, varsRef.connectedSprite);

                //Fill in the Lists of the Manager with the GameObjects
                varsRef.PointGameObjects[currentElementNumber] = instantiateThis;
                varsRef.PointNumbers[currentElementNumber] = 0;
                varsRef.PointColliderScripts[currentElementNumber] = pointColliderScript;
            }
        }
    }
}
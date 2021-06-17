using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointColliderEvents : MonoBehaviour
{
    private PointGridManager pointGridManager;
    private Sprite looseSprite;
    private Sprite connectedSprite;

    private int elementNumber;
    private bool isLoose;

    public void Initialize(int elementNumberInit, PointGridManager pointGridManagerInit, bool isLooseInit, Sprite looseSpriteInit, Sprite connectedSpriteInit)
    {
        elementNumber = elementNumberInit;
        pointGridManager = pointGridManagerInit;
        isLoose = isLooseInit;
        looseSprite = looseSpriteInit;
        connectedSprite = connectedSpriteInit;

        if (isLoose)
        {
            this.GetComponent<SpriteRenderer>().sprite = looseSprite;
        } else
        {
            this.GetComponent<SpriteRenderer>().sprite = connectedSprite;
        }
        
    }
    public void ChangeAppearance()
    {
        if (isLoose)
        {
            this.GetComponent<SpriteRenderer>().sprite = connectedSprite;
            isLoose = false;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().sprite = looseSprite;
            isLoose = true;
        }
    }

    /*private void OnMouseDown()
    {
        pointGridManager.ElementGotClicked(elementNumber);
    }*/
    private void OnMouseEnter()
    {
        pointGridManager.ElementGotEntered?.Invoke(elementNumber);
    }
    private void OnMouseExit()
    {
        pointGridManager.ElementGotExited?.Invoke(elementNumber);
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickOnBehaviour : MonoBehaviour
{
    public Sprite blackBall;
    public Sprite redBall;
    public gameManagement gMScript;
    public int elementNumber;
    public bool permanentlyActivated;

    private bool isBlack;
    void Start()
    {
        isBlack = true;
        permanentlyActivated = false;
    }

    private void OnMouseDown()
    {
        gMScript.iGotClicked(this.transform.localPosition,elementNumber);
    }
    private void OnMouseEnter()
    {
        gMScript.iGotEntered(elementNumber);
    }
    private void OnMouseExit()
    {
        gMScript.iGotExited(elementNumber);
    }
    public void changeColor()
    {
        if (isBlack)
        {
            this.GetComponent<SpriteRenderer>().sprite = redBall;
            isBlack = false;
        }
        else if (!permanentlyActivated)
        {
            this.GetComponent<SpriteRenderer>().sprite = blackBall;
            isBlack = true;
        }
    }
}

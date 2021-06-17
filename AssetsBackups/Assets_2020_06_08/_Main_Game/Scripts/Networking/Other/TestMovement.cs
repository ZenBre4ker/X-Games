using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    bool moveToCursor = false;
    private void Update()
    {
        if (moveToCursor)
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 mouseposition= Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
                transform.localPosition = mouseposition;
            } else
            {
                moveToCursor = false;
            }
        }
    }
    private void OnMouseDown()
    {
        moveToCursor = true;
    }
}

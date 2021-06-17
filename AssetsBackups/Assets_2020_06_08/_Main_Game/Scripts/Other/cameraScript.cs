using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cameraScript : MonoBehaviour
{
    public float cameraSize=6f;
    public CanvasScaler canvasScaler;

    private bool isPortrait;
    private bool firstCorrection;
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        firstCorrection = true;
        isPortrait = false;
        mainCamera = this.GetComponent<Camera>();
        if (Camera.main.aspect > 1)
        {
            isPortrait = true;
            canvasScaler.matchWidthOrHeight = 1f;
            mainCamera.orthographicSize = cameraSize / (16f / 9);
        }
        else
        {
            isPortrait = false;
            canvasScaler.matchWidthOrHeight = 0f;
            mainCamera.orthographicSize = cameraSize;
        }
        firstCorrection = false;
    }

    // Update is called once per frame
    void Update()
    {
        manageCamera();
    }
    void manageCamera()
    {
        if (Camera.main.aspect > 1 && !isPortrait)
        {
            isPortrait = true;
            canvasScaler.matchWidthOrHeight = 1f;
            mainCamera.orthographicSize = cameraSize / (16f / 9);
        }
        else if (Camera.main.aspect <= 1 && isPortrait)
        {
            isPortrait = false;
            canvasScaler.matchWidthOrHeight = 0f;
            mainCamera.orthographicSize = cameraSize * (9f / 16) / (Camera.main.aspect);
        }

    }
}

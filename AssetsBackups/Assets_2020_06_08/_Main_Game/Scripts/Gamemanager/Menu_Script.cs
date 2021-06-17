using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum infoIdentifier { hostSelection}

public class Menu_Script : MonoBehaviour
{
    public Buttonclick buttonClicks;
    public NetworkManager networkManager;

    public delegate void Buttonclick(sceneNames name);
    public additionalInfo sceneInfos;

    public void buttonClicked(sceneNamesObject scene)
    {
        buttonClicks(scene.sceneEnum);
    }

}



public class additionalInfo
{
    public infoIdentifier myIdentifier= 0;
    public additionalInfo(infoIdentifier _myIdentifier)
    {
        myIdentifier = _myIdentifier;
    }

    #region hostSelection
    public bool isHost=false;
    public bool hostIsSelected=false;
    public string joinIp;
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public enum infoIdentifier { hostSelection,DotsNBoxes}

public class Menu_Script : MonoBehaviour
{
    public Buttonclick buttonClicks;
    public NetworkManager networkManager { get; set; }

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
    public additionalInfo (additionalInfo clone)
    {
        myIdentifier = clone.myIdentifier;
        switch (myIdentifier)
        {
            case infoIdentifier.hostSelection:
                hostIsSelected = clone.hostIsSelected;
                joinIp = clone.joinIp;
                break;

            case infoIdentifier.DotsNBoxes:
                localPlayers = clone.localPlayers;
                networkPlayers = clone.networkPlayers;
                AIPlayers = clone.AIPlayers;
                numberOfPlayers = clone.numberOfPlayers;
                gridPointsWidth = clone.gridPointsWidth;
                gridPointsHeight = clone.gridPointsHeight;
                break;

        }
    }
    public void DebugInfos()
    {
        UnityEngine.Debug.Log($"Number of Players {numberOfPlayers}");
          UnityEngine.Debug.Log($"Local Players {localPlayers}, networkPlayers {networkPlayers}, AIPlayers {AIPlayers}");
        UnityEngine.Debug.Log($"gridPointsWidth {gridPointsWidth}, gridPointsHeight {gridPointsHeight}");

    }
    public void writePacket(Packet _packet)
    {
        _packet.Write(localPlayers);
        _packet.Write(numberOfPlayers);
        _packet.Write(gridPointsWidth);
        _packet.Write(gridPointsHeight);
    }

    public void readPacket(Packet _packet)
    {
        localPlayers = _packet.ReadInt();
        numberOfPlayers = _packet.ReadInt();
        gridPointsWidth = _packet.ReadInt();
        gridPointsHeight = _packet.ReadInt();
    }
    #region hostSelection
    public bool hostIsSelected=false;
    public string joinIp;
    #endregion

    #region hostSelection
    public int numberOfPlayers;
    public int localPlayers;
    public int networkPlayers;
    public int AIPlayers;
    public int gridPointsWidth;
    public int gridPointsHeight;
    #endregion
}

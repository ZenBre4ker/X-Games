using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using Networking;
public enum sceneNames {Introduction,Main_Menu,Multiplayer_Lobby, 
                        Game_Lobby, Multiplayer_Host, Multiplayer_Join, 
                        DotsNBoxes, DotsNBoxesSetup
                        };
public class MenuManager : MonoBehaviour
{
    public GameObject gameCanvas;
    public GameObject[] prefabListOfScenes;
    public GameObject[] ListOfNextScenes;
    public int numberOfScenes { get; private set; }

    public GameObject[] listOfInstantiatedScenes;
    public GameObject prevScene;
    public GameObject currentScene;
    public Menu_Script currentMenuScript;

    private NetworkManager networkManager;
    private bool packetOverride = false;

    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;

        numberOfScenes = prefabListOfScenes.Length;

        currentScene = listOfInstantiatedScenes[0];
        listOfInstantiatedScenes = new GameObject[numberOfScenes];
        listOfInstantiatedScenes[0] = currentScene;
        currentMenuScript = currentScene.GetComponent<Menu_Script>();
    }
    void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        networkManager.packetHandlers.Add((int)PacketsNum.sceneChange, handleSceneChangePacket);
        changeScene(sceneNames.Introduction);
    }

    void changeScene(sceneNames scene)
    {
        NetworkManager _nM = networkManager;
        if (_nM.myNetworkInfo.isSingleplayer || _nM.myNetworkInfo.isHost || packetOverride)
        {
            if (_nM.myNetworkInfo.isHost && !_nM.myNetworkInfo.isSingleplayer)
            {
                sendSceneChangePacket((int)scene);
                Debug.Log($"Packet is send to change to scene {scene}.");
            }
            if (packetOverride)
            {
                Debug.Log($"PacketOverride active and laoding scene {scene}.");
            }
            actualSceneChange(scene);
        }
    }

    void actualSceneChange(sceneNames scene)
    {
        additionalInfo _tempMenuInfos = currentMenuScript.sceneInfos;
        //Checking infos for next Scene as "scene" will contain the next one
        switch (scene)
        {
            case sceneNames.Multiplayer_Join:
                Debug.Log(currentMenuScript.sceneInfos.hostIsSelected);
                if (currentMenuScript.sceneInfos.hostIsSelected)
                {
                    networkManager.joinLobbyHost(currentMenuScript.sceneInfos.joinIp);
                    goto default;
                }
                break;

            case sceneNames.Game_Lobby:
                goto default;

            case sceneNames.DotsNBoxes:
                goto default;

            default:
                int sceneInt = (int)scene;
                if (currentScene != null)
                {
                    currentScene.SetActive(false);
                    currentMenuScript.buttonClicks = null;
                }
                if (listOfInstantiatedScenes[sceneInt] != null)
                {
                    currentScene = listOfInstantiatedScenes[sceneInt];
                    currentScene.SetActive(true);
                }
                else
                {
                    currentScene = Instantiate(prefabListOfScenes[sceneInt], gameCanvas.transform);
                    listOfInstantiatedScenes[sceneInt] = currentScene;
                }
                currentMenuScript = currentScene.GetComponent<Menu_Script>();
                currentMenuScript.buttonClicks = changeScene; 
                currentMenuScript.networkManager = networkManager;
                break;
        }

        //Assigning stuff as Manager after loading the next scene

        switch (scene)
        {
            case sceneNames.Multiplayer_Lobby:
                //assign peerJoined/Left Methods to Lobby_Script
                Multiplayer_Lobby lobby = currentScene.GetComponent<Multiplayer_Lobby>();
                networkManager.peerJoined = lobby.hostJoinedLobby;
                networkManager.peerLeft = lobby.hostLeftLobby;
                networkManager.StartLobby();
                break;

            case sceneNames.Multiplayer_Host:
                //start Hosting
                networkManager.StartLobbyHost();
                break;

            case sceneNames.DotsNBoxesSetup:
                currentMenuScript.sceneInfos = new additionalInfo(infoIdentifier.DotsNBoxes);
                break;

            case sceneNames.DotsNBoxes:
                currentMenuScript.sceneInfos = new additionalInfo(_tempMenuInfos);
                break;

            default:
                break;
        }
    }
    void handleSceneChangePacket(int _myId, Packet _packet)
    {
        int sceneNumber = _packet.ReadInt();
        Debug.Log($"Packet wants to change to scene {sceneNumber}.");
        if (!networkManager.myNetworkInfo.isHost)
        {
            Debug.Log($"Is not host, changing to scene {sceneNumber}.");
            packetOverride = true;
            changeScene((sceneNames) sceneNumber);
            packetOverride = false;
        }
    } 
    void sendSceneChangePacket(int sceneNumber)
    {
        using (Packet _packet = new Packet((int)PacketsNum.sceneChange))
        {
            _packet.Write(sceneNumber);
            networkManager.sendTCPPacket(_packet);
        }
    }
}

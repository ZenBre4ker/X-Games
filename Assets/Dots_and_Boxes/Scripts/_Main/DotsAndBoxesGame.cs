using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsAndBoxes;

public class DotsAndBoxesGame : MonoBehaviour
{
    private Menu_Script menu_Script;
    private GameObjectLibrary goLib;
    private NetworkManager networkManager;

    private VariablesContainer vars;
    private PointGridManager pointGrid;
    private PlayerManager players;
    private additionalInfo mssi;


    private InputState input;
    private int[] clickedPoints;
    private bool anotherTurn;

    GameObject currentLine;
    GameObject currentSquare;
    bool currentLineInstantiated;
    bool updateGameBehavior;

    public void initializeGame()
    {
        //get Menu_Script-Variables

       vars.initialize(mssi.gridPointsWidth, mssi.gridPointsHeight, 4f, 4f);
        vars.setNumberOfPlayers(networkManager.myNetworkInfo.myId, mssi.localPlayers, mssi.networkPlayers, mssi.numberOfPlayers);

        pointGrid.initGrid(ref vars);
        players.initialize(ref vars);

        updateGameBehavior = true;

        input = new InputState();
        clickedPoints = new int[2] { -1, -1 };
        anotherTurn = false;

        vars.GameState = gameStates.First_Selection;

        currentLineInstantiated = false;
    }
    void Start()
    {
        goLib = GetComponent<GameObjectLibrary>();
        menu_Script = goLib.menu_Script;
        mssi = menu_Script.sceneInfos;
        networkManager = menu_Script.networkManager; 

        //TODO: Fix that
        networkManager.packetHandlers.Add((int)PacketsNum.dotsNBoxesStart, retrieveInitializationInfo);

        vars = GetComponent<VariablesContainer>();
        pointGrid = GetComponent<PointGridManager>();
        players = GetComponent<PlayerManager>();
        players.setupForNetwork();
        updateGameBehavior = false;

        if (!networkManager.myNetworkInfo.isSingleplayer)
        {
            if (networkManager.myNetworkInfo.isHost)
            {
                sendInitializationInfo();
                initializeGame();
            } else
            {
                //TODO: Safehandling of packets that have no key in the dictionary yet
                //networkManager.packetHandlers.Add((int)PacketsNum.dotsNBoxesStart, retrieveInitializationInfo);
            }
        } else
        {
            initializeGame();
        }
    }
    private void sendInitializationInfo()
    {
        using (Packet _packet = new Packet((int)PacketsNum.dotsNBoxesStart))
        {
            mssi.writePacket(_packet);
            networkManager.sendTCPPacket(_packet);
        }
    }

    private void retrieveInitializationInfo(int _myId, Packet _packet)
    {
        mssi.readPacket(_packet);
        initializeGame();
    }
    // Update is called once per frame
    void Update()
    {
        if (updateGameBehavior)
        {
            input = players.handledInput();
            Draw(input);
            updateGameState(input);
        }
        
    }

    void Draw(InputState input)
    {
        if (input.drawLine)
        {
            if (!currentLineInstantiated)
            {
                currentLine = GameObject.Instantiate(vars.line);
                currentLine.transform.SetParent(this.transform);
                currentLineInstantiated = true;
            }
            PointGridManager.drawLine(currentLine, input.drawFromCoordinates, input.drawToCoordinates);
        }
    }
    

    void updateGameState(InputState input){
        bool inputCheckedOut=checkInput(input);
        switch (vars.GameState)
        {
            case gameStates.Default:
                break;

            case gameStates.First_Selection:
                if (inputCheckedOut)
                {
                    vars.GameState = gameStates.Second_Selection;
                    clickedPoints[0] = input.firstElementNumber;
                }
                else if (input.firstPointIsChosen)
                {
                    resetSelection(false);
                }
                break;

            case gameStates.Second_Selection:
                if(inputCheckedOut)
                {
                    vars.GameState = gameStates.CheckSelection;
                    clickedPoints[1] = input.secondElementNumber;
                } else if (input.secondPointIsChosen)
                {
                    resetSelection(false);
                }
                break;

            case gameStates.CheckSelection:
                if (inputCheckedOut)
                {
                    vars.GameState = gameStates.NextPlayer;
                } else
                {
                    resetSelection(inputCheckedOut);
                }
                break;

            case gameStates.NextPlayer:
                updateFields();
                switchPlayers();
                break;
        }
    }

    bool checkInput(InputState input)
    {

        bool checkedOut = false;
        int elementNumber;
        int secondElementNumber;
        switch (vars.GameState)
        {
            case gameStates.First_Selection:
                elementNumber = input.firstElementNumber;
                if (input.firstPointIsChosen
                    && PointGridManager.isPointAvailable(elementNumber, ref vars)
                    )
                {
                    checkedOut = true;
                }
                break;

            case gameStates.Second_Selection:
                elementNumber = input.secondElementNumber;
                if (input.secondPointIsChosen
                    && PointGridManager.isPointAvailable(elementNumber, ref vars)
                    )
                {
                    checkedOut = true;
                }
                break;

            case gameStates.CheckSelection:
                elementNumber = input.firstElementNumber;
                secondElementNumber = input.secondElementNumber;
                int lineNumber = PointGridManager.getLineNumberOfElements(elementNumber, secondElementNumber, ref vars);
                if (vars.LineStates[lineNumber]<1 && PointGridManager.canLineBeDrawn(elementNumber, secondElementNumber, ref vars))
                {
                    checkedOut = true;
                }
                break;

            default:
                break;
        }
        return checkedOut;
    }

    void resetSelection(bool keepLine)
    {
        if (currentLineInstantiated && !keepLine)
        {
            Destroy(currentLine);
        }
        currentLineInstantiated = false;
        players.resetInputState();
        vars.GameState = gameStates.First_Selection;
    }
    void updateFields()
    {
        int count, element;
        int lineNumber = PointGridManager.getLineNumberOfElements(clickedPoints[0], clickedPoints[1], ref vars);

        //Update Points
        for(int i = 0; i < 2; i++)
        {
            element = clickedPoints[i];
            count = vars.PointNumbers[element] += 1;
            if (count >= vars.MaxPointNumbers[element])
            {
                vars.PointColliderScripts[element].ChangeAppearance();
            }
        }
        //Update Lines
        vars.LineStates[lineNumber] += 1;
        vars.LineGameObjects[lineNumber] = currentLine;

        //Update Squares
        updateSquares(lineNumber);
    }
    void updateSquares(int lineNumber)
    {
        int squareNumber, squareState;
        for (int i = 0; i < 2; i++)
        {
            squareNumber = vars.LineToSquarePointer[lineNumber, i];
            if (squareNumber >= 0)
            {
                squareState = vars.SquareStates[squareNumber] += 1;
                if (squareState >= 4)
                {
                    players.addPoints(1);
                    anotherTurn = true;
                    currentSquare = GameObject.Instantiate(vars.square);
                    PointGridManager.drawSquare(squareNumber, currentSquare, this.gameObject, ref vars);
                    vars.SquareGameObjects[squareNumber] = currentSquare;
                }
            }

        }
    }
    void switchPlayers()
    {
        resetSelection(true);
        if (!anotherTurn)
        {
            players.switchToNextPlayer();
        }
        anotherTurn = false;
    }

}

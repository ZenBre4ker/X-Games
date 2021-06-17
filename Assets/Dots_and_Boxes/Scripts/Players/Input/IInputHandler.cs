using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DotsAndBoxes;
public interface IInputHandler
{
    InputState handleInput(gameStates gameState);
    //void assignDelegates(ref clickedObjectNumber clickedDotEvent, ref clickedObjectNumber enteredDotEvent, ref clickedObjectNumber exitedDotEvent );
    void assignDelegates( ref clickedObjectNumber enteredDotEvent, ref clickedObjectNumber exitedDotEvent);
    void resetInputState();
}

public class InputState
{

    public bool firstPointIsChosen;
    public bool secondPointIsChosen;

    public int firstElementNumber;
    public int secondElementNumber;

    public Vector2 drawFromCoordinates;
    public Vector2 drawToCoordinates;

    public bool drawLine;

    public InputState()
    {
        resetState();
    }

    public void resetState()
    {

        firstPointIsChosen = false;
        secondPointIsChosen = false;

        firstElementNumber = -1;
        secondElementNumber = -1;

        drawFromCoordinates = new Vector2();
        drawToCoordinates = new Vector2();

        drawLine = false;

    }

    public void writePacket(Packet _packet)
    {
        _packet.Write(firstPointIsChosen);
        _packet.Write(secondPointIsChosen);

        _packet.Write(firstElementNumber);
        _packet.Write(secondElementNumber);

        _packet.Write(drawFromCoordinates);
        _packet.Write(drawToCoordinates);

        _packet.Write(drawLine);
    }

    public void readPacket(Packet _packet)
    {
        firstPointIsChosen = _packet.ReadBool();
        secondPointIsChosen = _packet.ReadBool();

        firstElementNumber = _packet.ReadInt();
        secondElementNumber = _packet.ReadInt();

        drawFromCoordinates = _packet.ReadVector2();
        drawToCoordinates = _packet.ReadVector2();

        drawLine = _packet.ReadBool();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Number of a sent packet
public enum PacketsNum
{
    welcome = 1,
    udpTest,
    sceneChange,
    dotsNBoxesStart,
    dotsNBoxes,
    transformInitialize,
    transformTest,
    playerDisconnected
}
public class Constants : MonoBehaviour
{
    public const int TICKS_PER_SEC = 64;
    public const int MS_PER_TICK = 1000 / TICKS_PER_SEC;
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnMainGame : MonoBehaviour
{
    public GameObject UIParent;
    public GameObject GamePrefab;

    private Menu_Script menu_Script;
    private GameObject mainGame;

    private void Start()
    {
        startGame();
    }

    private void OnDisable()
    {
        if (mainGame != null)
        {
            Destroy(mainGame);
        }
    }

    private void OnEnable()
    {
        startGame();
    }

    private void startGame()
    {
        menu_Script = GetComponent<Menu_Script>();
        if (mainGame == null)
        {
            mainGame = Instantiate(GamePrefab);
            mainGame.GetComponent<GameObjectLibrary>().UIParent = UIParent;
            mainGame.GetComponent<GameObjectLibrary>().menu_Script = menu_Script;
        }
    }
}

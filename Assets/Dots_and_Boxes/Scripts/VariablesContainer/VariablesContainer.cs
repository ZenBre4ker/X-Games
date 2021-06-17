using UnityEngine;

namespace DotsAndBoxes
{
    public delegate void clickedObjectNumber(int number);
    public enum gameStates { Default, First_Selection, Second_Selection, CheckSelection, NextPlayer };
    public enum inputTypes { Mouse, Touch, Keyboard, NetworkPlayer, AI};
    public partial class VariablesContainer : MonoBehaviour
    {
        //private variables, that can be seen in editor
        [Header("Player Options")]
        [SerializeField]
        private int localPlayers = 1;
        [SerializeField]
        private int networkPlayers = 0;
        [SerializeField]
        private int AIPlayers = 1;
        [SerializeField]
        private inputTypes[] playerInputTypes={inputTypes.Mouse, inputTypes.AI };

        [Header("Number of Points in X and Y Direction")]
        [SerializeField]
        private int gridPointsWidth = 5;
        [SerializeField]
        private int gridPointsHeight = 5;

        [Header("Actual Size in X and Y Direction (Floats)")]
        [SerializeField]
        private float gridWidth = 4f;
        [SerializeField]
        private float gridHeight = 4f;

        [Header("Current Game State we're in")]
        [SerializeField]
        private gameStates gameState;

        [Header("Available Prefabs")]
        public GameObject touchSpheres;
        public GameObject line, square;
        public Sprite looseSprite, connectedSprite;

        //private Variables only available to the class
        private int numberOfPlayers;
        private GameObject[] pointGameObjects;
        private int[] pointNumbers;
        private int[] maxPointNumbers;
        private int[,] lineToSquarePointer;
        private int[,] squareToLinesPointer;
        private PointColliderEvents[] pointColliderScripts;
        private int[] gridSize;//Width x Height

        //public Accessors of the private variables, that can be accessed outside of this class
        public int NumberOfPlayers { get { return numberOfPlayers; } }
        public int NumberOfLocalPlayers { get { return localPlayers; } }
        public int NumberOfNetworkPlayers { get { return networkPlayers; } }
        public int NumberOfAIPlayers { get { return AIPlayers; } }
        public inputTypes[] PlayerInputTypes { get { return playerInputTypes; } }
        public int GridPointsWidth { get { return gridPointsWidth; } }
        public int GridPointsHeight { get { return gridPointsHeight; } }
        public float GridWidth { get { return gridWidth; } }
        public float GridHeight { get { return gridHeight; } }

        //Variables that should be changeable by other classes
        public int[] LineStates { get; set; }
        public GameObject[] LineGameObjects { get; set; }
        public int[] SquareStates { get; set; }
        public GameObject[] SquareGameObjects { get; set; }
        public gameStates GameState { get { return gameState; } set { gameState = value; } }

        //Readonly variables outside of this class after initialization
        public GameObject[] PointGameObjects { get { return pointGameObjects; } }
        public int[] PointNumbers { get { return pointNumbers; } }
        public int[] MaxPointNumbers { get { return maxPointNumbers; } }
        public int[,] LineToSquarePointer { get { return lineToSquarePointer; } }
        public int[,] SquareToLinesPointer { get { return squareToLinesPointer; } }
        public PointColliderEvents[] PointColliderScripts { get { return pointColliderScripts; } }
        public int[] GridSize { get { return gridSize; } } //Width x Height
    }
}

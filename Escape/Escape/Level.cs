//Author: Victoria Mak
//File Name: Level.cs
//Project Name: Escape
//Creation Date: May 9, 2023
//Modified Date: June 12, 2023
//Description: The Level class holds the methods and properties to update and draw the gameplay for each level.

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Animation2D;
using Helper;

namespace Escape
{
    class Level
    {
        //Store the random number generator
        private Random rng = new Random();

        //Store the points earned per second
        private const int TIME_POINTS_PER_SEC = 2;
        private const int MAX_TIME = 300;

        //Store the costs for travelling on the tile with the item on it
        public static readonly float[] ITEM_COSTS = { 100000f, 100000f, 100000f, 1f, 5f, 10f, 1f, 1f, 100000f, 1f, 1f };

        //Store the node where the key is found and the player starting location
        private readonly Vector2[] KEY_LOC = { new Vector2(1, 33), new Vector2(3, 33), new Vector2(10, 28), new Vector2(13, 2), new Vector2(1, 1) };
        private static readonly Vector2[] START_GRID_LOCS = { new Vector2(1, 0), new Vector2(10, 0), new Vector2(1, 13), new Vector2(15, 24), new Vector2(4, 24) };

        //Store the exit direction
        private readonly int[] EXIT_DIR = { Game1.RIGHT, Game1.UP, Game1.DOWN, Game1.RIGHT, Game1.LEFT };

        //Store the max number of ghosts to spawn per level
        private readonly int[] NUM_GHOSTS = { 1, 2, 2, 3, 3 };

        //Store the carpet background, the pixel image, and the font for the stats
        private Texture2D carpetBg;
        private Texture2D pixelImg;
        private SpriteFont font;

        //Store the item images
        public Texture2D[] itemImgs = new Texture2D[8];

        //Store the unlock animation
        private Animation unlockAnim;

        //Store the player and the inventory
        private Player player;
        private Inventory inventory;

        //Store the list of bullets, zombies, and ghosts
        private List<Bullet> playerBullets = new List<Bullet>();
        private List<Zombie> zombies = new List<Zombie>();
        private List<Ghost> ghosts = new List<Ghost>();

        //Store whether the player is exiting
        private bool isExiting = false;

        //Store the escape timer
        private Timer escapeTimer;

        //Store the level, number of zombies kiled and the score
        private int levelNum;
        private int zombiesKilled;
        private int score;

        //Store the messages displaying the level, timer and score
        private string levelMsg;
        private string timerMsg;
        private string scoreMsg;

        //Store the location of the messages displaying the level, timer and score
        private Vector2 levelMsgLoc;
        private Vector2 timerMsgLoc;
        private Vector2 scoreMsgLoc;

        //Store the stats bar rectangle
        private Rectangle statsBarRec;

        //Store the node map and a map containing the objects on each node
        public static Node[,] nodeMap = new Node[Game1.NUM_ROWS, Game1.NUM_COLS];
        private int[,,] map =
        {
            {
                {1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 },
                {10,3 ,10,10,10,6 ,10,10,10,10,10,10,10,1 ,1 ,0 ,2 ,2 ,2 ,1 ,1 ,1 ,10,10,10,10,10,10,10,5 ,0 ,10,10,5 ,0 },
                {0 ,0 ,10,10,10,10,10,10,10,9 ,10,4 ,10,10,10,0 ,10,10,10,10,10,10,10,10,10,10,10,10,10,10,0 ,10,10,10,0 },
                {0 ,0 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,10,10,4 ,10,10,10,0 ,10,10,10,10,10,10,10,10,10,10,10,10,10,10,0 ,10,10,10,0 },
                {0 ,0 ,10,10,10,10,10,10,10,10,10,4 ,4 ,4 ,4 ,0 ,10,10,10,10,1 ,1 ,1 ,1 ,1 ,0 ,10,10,10,10,0 ,10,10,10,0 },
                {0 ,0 ,10,10,0 ,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,5 ,10,10,0 ,10,0 ,10,10,10,10,0 ,10,10,10,0 },
                {0 ,0 ,10,10,0 ,10,0 ,10,9 ,10,10,10,0 ,10,10,10,10,10,10,10,5 ,10,10,0 ,10,0 ,10,10,6 ,10,0 ,10,10,10,0 },
                {0 ,0 ,10,10,0 ,10,0 ,10,10,10,10,10,0 ,10,10,10,10,10,10,10,5 ,10,10,0 ,10,0 ,10,10,10,10,5 ,5 ,5 ,10,0 },
                {0 ,4 ,10,10,0 ,1 ,0 ,1 ,1 ,1 ,1 ,1 ,0 ,1 ,1 ,1 ,1 ,1 ,10,10,5 ,10,10,0 ,10,0 ,10,10,10,10,5 ,10,2 ,10,0 },
                {0 ,4 ,10,10,0 ,5 ,2 ,10,10,10,5 ,10,0 ,10,10,10,10,10,10,10,5 ,10,10,10,10,0 ,10,9 ,10,10,10,10,10,10,0 },
                {0 ,4 ,10,10,0 ,10,10,10,10,10,5 ,10,0 ,10,10,10,10,10,10,10,3 ,10,10,10,10,0 ,10,10,10,10,10,6 ,10,10,0 },
                {0 ,10,10,10,0 ,10,10,10,10,10,5 ,10,0 ,2 ,2 ,2 ,2 ,2 ,2 ,2 ,3 ,10,10,1 ,10,0 ,10,10,10,10,10,10,10,10,0 },
                {0 ,10,10,10,10,10,10,10,10,10,5 ,10,10,10,10,10,10,10,10,10,3 ,10,10,10,10,0 ,4 ,4 ,4 ,4 ,4 ,5 ,5 ,5 ,0 },
                {0 ,4 ,10,10,10,10,6 ,10,10,10,5 ,10,10,10,10,10,10,10,10,10,2 ,10,10,10,10,0 ,10,10,10,10,10,10,10,10,0 },
                {0 ,10,10,10,10,10,0 ,10,10,10,10,10,10,10,10,10,6 ,10,10,10,10,0 ,10,10,10,10,10,10,10,10,10,2 ,10,10,0 },
                {0 ,10,10,10,10,10,0 ,10,10,10,10,10,10,10,10,10,10,10,10,10,10,0 ,10,10,10,0 ,10,10,10,10,10,2 ,9 ,10,8 },
                {1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 }
            },
            {
                {1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,8 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 },
                {10,10,10,10,10,10,10,10,10,0 ,10,10,10,10,10,10,10,10,10,3 ,10,10,10,10,10,10,0 ,10,10,10,10,10,10,10,6 },
                {10,1 ,1 ,1 ,1 ,1 ,1 ,10,10,0 ,1 ,1 ,10,0 ,10,10,10,0 ,10,3 ,10,1 ,1 ,1 ,1 ,10,0 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,10},
                {10,10,10,10,10,10,2 ,10,10,10,10,6 ,10,0 ,10,10,10,0 ,10,3 ,10,0 ,9 ,9 ,0 ,10,0 ,10,10,10,10,10,10,4 ,10},
                {10,10,10,5 ,5 ,9 ,2 ,10,0 ,10,0 ,10,10,0 ,10,10,10,0 ,10,3 ,10,0 ,10,10,0 ,10,0 ,10,10,10,2 ,3 ,10,4 ,10},
                {10,10,10,10,10,10,2 ,10,0 ,10,0 ,10,10,0 ,3 ,1 ,3 ,0 ,10,4 ,10,0 ,10,10,0 ,10,0 ,10,10,10,2 ,9 ,6 ,4 ,10},
                {10,1 ,1 ,1 ,1 ,1 ,1 ,10,0 ,10,0 ,4 ,4 ,4 ,9 ,9 ,9 ,0 ,10,5 ,10,0 ,5 ,5 ,0 ,10,0 ,10,10,10,2 ,5 ,5 ,4 ,10},
                {6 ,10,10,10,10,10,10,10,0 ,10,0 ,10,10,0 ,5 ,2 ,2 ,2 ,10,4 ,10,10,10,10,10,10,0 ,10,10,10,10,10,10,10,10},
                {10,1 ,1 ,1 ,1 ,1 ,1 ,10,0 ,10,0 ,10,10,10,10,6 ,10,10,10,5 ,10,0 ,1 ,1 ,0 ,10,10,1 ,1 ,1 ,1 ,10,0 ,10,10},
                {10,10,10,4 ,10,10,0 ,6 ,0 ,6 ,0 ,10,10,10,10,1 ,10,0 ,10,4 ,10,0 ,6 ,6 ,0 ,10,10,0 ,6 ,9 ,0 ,10,0 ,10,10},
                {10,10,10,4 ,10,6 ,0 ,1 ,1 ,1 ,1 ,1 ,1 ,10,10,1 ,10,0 ,10,5 ,10,0 ,9 ,10,0 ,10,10,0 ,10,10,0 ,10,0 ,10,10},
                {10,10,10,4 ,10,10,0 ,10,10,10,10,10,10,10,10,1 ,10,0 ,10,4 ,10,0 ,10,10,0 ,10,10,0 ,10,10,0 ,10,0 ,10,6 },
                {10,1 ,1 ,1 ,1 ,1 ,1 ,10,10,10,10,10,10,10,10,1 ,10,0 ,10,5 ,10,0 ,4 ,4 ,0 ,10,10,0 ,10,10,0 ,10,0 ,10,6 },
                {10,10,10,10,10,10,3 ,10,10,10,2 ,2 ,2 ,4 ,4 ,1 ,10,0 ,10,4 ,10,10,10,10,10,10,10,10,10,10,10,10,0 ,10,4 },
                {6 ,10,10,10,10,2 ,3 ,10,10,10,1 ,9 ,1 ,9 ,1 ,10,10,0 ,10,5 ,10,10,10,10,2 ,2 ,2 ,2 ,1 ,1 ,1 ,1 ,1 ,5 ,5 },
                {10,10,10,10,10,2 ,3 ,10,10,10,10,10,10,10,10,10,10,0 ,10,4 ,10,10,10,10,10,10,10,9 ,10,10,10,10,9 ,5 ,5 },
                {1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1}
            },
            {
                {1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 },
                {10,2 ,6 ,10,10,2 ,10,10,6 ,10,10,5 ,5 ,10,10,6 ,0 ,5 ,5 ,5 ,5 ,10,10,6 ,9 ,10,10,2 ,10,10,10,10,10,10,10},
                {10,2 ,10,2 ,10,2 ,10,2 ,2 ,2 ,2 ,10,10,10,10,10,0 ,5 ,5 ,5 ,10,10,2 ,9 ,10,2 ,10,2 ,10,2 ,2 ,2 ,2 ,2 ,10},
                {10,2 ,10,2 ,10,2 ,10,2 ,9 ,9 ,2 ,10,10,2 ,10,10,0 ,5 ,5 ,10,10,2 ,10,10,2 ,10,10,2 ,10,2 ,10,10,10,2 ,10},
                {10,2 ,10,2 ,10,2 ,10,2 ,9 ,9 ,2 ,10,2 ,2 ,2 ,10,0 ,5 ,10,10,2 ,10,10,2 ,10,10,10,2 ,10,2 ,10,2 ,10,2 ,10},
                {10,10,10,2 ,10,2 ,10,2 ,9 ,9 ,2 ,10,10,2 ,10,10,0 ,10,10,2 ,10,10,2 ,10,10,10,10,2 ,10,2 ,10,2 ,10,2 ,10},
                {10,10,10,10,10,2 ,10,2 ,2 ,10,2 ,2 ,2 ,2 ,2 ,2 ,0 ,2 ,2 ,10,10,2 ,10,10,6 ,10,10,2 ,10,2 ,9 ,2 ,10,2 ,10},
                {4 ,2 ,10,10,10,2 ,10,10,10,10,2 ,10,10,10,10,10,10,10,10,6 ,2 ,10,10,10,10,10,10,2 ,10,2 ,2 ,2 ,10,2 ,10},
                {10,2 ,10,2 ,10,2 ,4 ,4 ,4 ,4 ,2 ,10,10,10,10,6 ,0 ,10,2 ,2 ,10,10,5 ,2 ,2 ,2 ,10,2 ,10,10,10,10,10,2 ,10},
                {4 ,2 ,10,2 ,10,2 ,10,2 ,10,10,0 ,10,2 ,2 ,2 ,2 ,0 ,9 ,10,10,10,10,5 ,10,9 ,2 ,10,2 ,2 ,2 ,2 ,2 ,2 ,2 ,10},
                {10,2 ,10,2 ,10,2 ,10,2 ,10,10,0 ,10,10,10,10,10,10,10,10,10,4 ,10,5 ,10,10,2 ,10,10,4 ,10,10,10,10,10,10},
                {10,10,10,2 ,10,2 ,10,2 ,10,10,0 ,2 ,2 ,2 ,2 ,2 ,0 ,10,10,10,4 ,10,5 ,10,10,2 ,10,10,5 ,10,9 ,10,10,10,10},
                {10,10,10,10,10,2 ,10,2 ,10,10,0 ,10,10,10,10,10,0 ,10,2 ,10,4 ,10,5 ,10,10,2 ,10,10,2 ,10,6 ,6 ,10,10,10},
                {10,2 ,10,10,10,2 ,10,2 ,10,1 ,1 ,1 ,1 ,10,10,10,0 ,10,2 ,10,4 ,10,5 ,10,10,2 ,10,10,2 ,10,10,10,10,10,10},
                {10,2 ,10,2 ,10,2 ,10,2 ,10,10,10,10,10,10,10,10,0 ,10,2 ,10,4 ,10,10,10,10,2 ,10,10,5 ,1 ,1 ,1 ,1 ,1 ,10},
                {10,10,10,10,10,2 ,10,10,10,10,10,10,10,10,10,10,0 ,4 ,2 ,10,10,10,10,9 ,10,2 ,6 ,2 ,4 ,10,10,10,10,10,10},
                {1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,8 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 }
            },
            {
                {1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 },
                {10,2 ,10,10,10,3 ,10,10,10,10,10,10,10,10,6 ,0 ,10,10,10,5 ,10,10,10,3 ,10,10,10,10,0 ,10,10,10,10,10,10},
                {10,2 ,10,10,10,10,3 ,10,10,5 ,5 ,5 ,5 ,10,9 ,0 ,10,4 ,10,5 ,10,2 ,10,3 ,10,0 ,10,10,0 ,10,10,10,10,10,0 },
                {10,2 ,10,3 ,3 ,10,10,3 ,10,10,5 ,5 ,5 ,10,6 ,0 ,10,4 ,10,5 ,10,2 ,10,3 ,10,0 ,10,10,0 ,10,10,10,10,10,8 },
                {10,2 ,10,10,3 ,3 ,10,10,3 ,10,10,5 ,5 ,10,10,0 ,10,4 ,10,10,10,2 ,10,10,10,0 ,10,10,10,10,10,10,10,5 ,0 },
                {10,2 ,10,10,10,3 ,3 ,10,10,3 ,10,10,5 ,10,10,0 ,10,4 ,10,5 ,10,2 ,10,3 ,10,0 ,10,10,0 ,10,10,10,10,10,10},
                {10,2 ,10,10,10,10,3 ,3 ,10,10,3 ,10,10,10,10,0 ,10,4 ,10,5 ,10,2 ,10,3 ,10,0 ,10,10,0 ,10,10,10,10,10,10},
                {10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,5 ,10,10,10,3 ,10,10,10,9 ,0 ,10,10,10,10,10,10},
                {1 ,10,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,10,1 ,10,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 },
                {10,10,10,10,10,10,10,10,10,10,10,10,10,10,9 ,10,10,10,10,10,10,10,10,10,10,10,10,10,0 ,10,10,10,10,10,10},
                {10,10,10,10,10,10,10,10,10,4 ,10,10,10,10,10,0 ,10,6 ,10,10,10,10,9 ,10,10,10,6 ,10,0 ,10,2 ,2 ,2 ,2 ,10},
                {10,10,10,10,10,10,10,10,10,4 ,10,6 ,10,10,10,0 ,10,10,10,10,1 ,1 ,1 ,1 ,1 ,10,10,10,0 ,10,2 ,10,10,2 ,10},
                {10,5 ,5 ,5 ,10,5 ,5 ,5 ,10,4 ,10,10,10,10,10,0 ,10,10,10,10,0 ,10,9 ,10,2 ,10,10,10,10,10,2 ,9 ,9 ,2 ,10},
                {10,5 ,5 ,5 ,10,5 ,5 ,5 ,10,4 ,4 ,4 ,4 ,10,10,0 ,10,10,10,10,1 ,1 ,1 ,1 ,1 ,10,10,10,0 ,10,2 ,10,10,2 ,10},
                {9 ,5 ,5 ,5 ,10,5 ,5 ,5 ,10,10,10,10,10,10,10,0 ,10,6 ,10,10,10,10,9 ,10,10,10,6 ,10,0 ,10,2 ,2 ,2 ,2 ,10},
                {9 ,9 ,10,10,10,10,10,10,10,10,10,10,10,10,6 ,0 ,10,10,10,10,10,10,10,10,10,10,10,10,0 ,10,10,10,10,10,10},
                {1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 }
            },
            {
                {1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 },
                {9 ,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,9 },
                {10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
                {1 ,1 ,1 ,1 ,10,10,1 ,1 ,1 ,10,10,1 ,1 ,1 ,10,10,10,10,0 ,10,10,10,10,0 ,1 ,1 ,10,10,1 ,1 ,1 ,1 ,10,10,10},
                {0 ,10,10,10,10,0 ,10,10,10,10,0 ,10,10,10,10,10,10,0 ,10,0 ,10,10,10,2 ,10,9 ,0 ,10,0 ,10,10,10,10,10,10},
                {0 ,10,10,10,10,0 ,10,10,10,10,0 ,10,10,10,10,10,0 ,10,10,10,0 ,10,10,0 ,10,10,0 ,10,0 ,10,10,10,10,10,10},
                {0 ,1 ,1 ,1 ,10,10,1 ,1 ,10,10,0 ,10,10,10,10,0 ,10,10,10,10,10,0 ,10,0 ,1 ,1 ,10,10,0 ,1 ,1 ,1 ,10,10,10},
                {0 ,10,10,10,10,10,10,10,0 ,10,0 ,10,10,10,10,0 ,1 ,1 ,1 ,1 ,1 ,0 ,10,0 ,10,10,10,10,0 ,10,10,10,10,10,10},
                {0 ,10,10,10,10,10,10,10,0 ,10,0 ,10,10,10,10,0 ,10,10,10,10,10,0 ,10,0 ,10,10,10,10,0 ,10,10,10,10,10,10},
                {1 ,1 ,1 ,1 ,10,1 ,1 ,1 ,10,10,10,1 ,1 ,1 ,10,0 ,10,10,10,10,10,0 ,10,0 ,10,10,10,10,0 ,1 ,1 ,1 ,10,10,10},
                {10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
                {10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
                {10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
                {0 ,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
                {8 ,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,0 },
                {0 ,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,0 },
                {1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 ,1 }
            }
        };

        public Level(int levelNum, int health, Texture2D carpetBg, Texture2D[] itemImgs, Texture2D[] regZombieImgs, Texture2D[] smartZombieImgs, Texture2D[] ghostImgs, Texture2D[] playerImgs, Texture2D[] bulletImgs, Texture2D pixelImg, SpriteFont font, Animation unlockAnim, Vector2[] regZombieLocs, Vector2[] smartZombieLocs, SoundEffect shootSnd, SoundEffect impactSnd)
        {
            //Store 2 random numbers
            int num1 = 0;
            int num2 = 0;

            //Set the font
            this.font = font;

            //Set the item, pixel and carpet images
            this.itemImgs = itemImgs;
            this.pixelImg = pixelImg;
            this.carpetBg = carpetBg;

            //Set the level number, zombies killed and score
            this.levelNum = levelNum;
            zombiesKilled = 0;
            score = 0;

            //Set the unlock animation
            this.unlockAnim = unlockAnim;

            //Create the nodes in each row
            for (int i = 0; i < Game1.NUM_ROWS; i++)
            {
                //Create the nodes in each column of each row
                for (int j = 0; j < Game1.NUM_COLS; j++)
                {
                    //Create the node
                    nodeMap[i, j] = new Node(i, j, map[levelNum - 1, i, j], itemImgs);
                }
            }

            //Set the adjacent nodes of the nodes in each row
            for (int i = 0; i < Game1.NUM_ROWS; i++)
            {
                //Set the adjacent nodes of the nodes in each column
                for (int j = 0; j < Game1.NUM_COLS; j++)
                {
                    //Set the adjacent nodes of the node
                    nodeMap[i, j].SetAdjacent(nodeMap);
                }
            }

            //Set the inventory and generate a player at the starting location
            inventory = new Inventory(pixelImg, itemImgs, font, health);
            player = new Player(playerImgs, bulletImgs, nodeMap[(int)START_GRID_LOCS[levelNum - 1].X, (int)START_GRID_LOCS[levelNum - 1].Y], inventory, shootSnd, impactSnd);
            nodeMap[(int)START_GRID_LOCS[levelNum - 1].X, (int)START_GRID_LOCS[levelNum - 1].Y].SetPlayer(player);

            //Generate the regular zombies
            for (int i = 0; i < regZombieLocs.Length; i++)
            {
                //Add a new regular zombie to the list of zombies and it to the current node that it is on
                zombies.Add(new RegZombie(regZombieImgs, nodeMap[(int)regZombieLocs[i].X, (int)regZombieLocs[i].Y]));
                nodeMap[(int)regZombieLocs[i].X, (int)regZombieLocs[i].Y].GetZombiesOnSpace().Add(zombies[zombies.Count - 1]);
            }

            //Generate the smart zombies
            for (int i = 0; i < smartZombieLocs.Length; i++)
            {
                //Add a new smart zombie to the list of zombies and it to the current node that it is on
                zombies.Add(new SmartZombie(smartZombieImgs, nodeMap[(int)smartZombieLocs[i].X, (int)smartZombieLocs[i].Y]));
                nodeMap[(int)smartZombieLocs[i].X, (int)smartZombieLocs[i].Y].GetZombiesOnSpace().Add(zombies[zombies.Count - 1]);
            }

            //Generate the ghosts
            for (int i = 0; i < NUM_GHOSTS[levelNum - 1]; i++)
            {
                //Randomize 2 numbers
                num1 = rng.Next(0, Game1.NUM_ROWS);
                num2 = rng.Next(0, Game1.NUM_COLS);
                
                //Add a new ghost to the list of ghosts and add it to the current node that it is on
                ghosts.Add(new Ghost(ghostImgs, nodeMap[num1, num2], nodeMap));
                ghosts[i].GetCurNode().GetGhostsOnSpace().Add(ghosts[i]);
            }

            //Calculate the h cost of all nodes
            CalcHCost();

            //Set the node that has the key
            nodeMap[(int)KEY_LOC[levelNum - 1].X, (int)KEY_LOC[levelNum - 1].Y].SetHasKey(true);

            //Set the escape timer
            escapeTimer = new Timer(Timer.INFINITE_TIMER, true);

            //Set the stats bar rectangle
            statsBarRec = new Rectangle(0, Game1.SCREEN_HEIGHT - Game1.STATS_BAR_HEIGHT, Game1.SCREEN_WIDTH, Game1.STATS_BAR_HEIGHT);

            //Set the messages displaying the level, time, and score
            levelMsg = "Level " + levelNum;
            timerMsg = "Time: " + escapeTimer.GetTimePassedAsString(Timer.FORMAT_MIN_SEC_MIL);
            scoreMsg = "Score: 0";

            //Set the location of the messages displaying the stats
            levelMsgLoc = new Vector2(Game1.SCREEN_WIDTH * 3 / 4, Game1.SCREEN_HEIGHT - Game1.STATS_BAR_HEIGHT);
            timerMsgLoc = new Vector2(Game1.SCREEN_WIDTH * 3 / 4, levelMsgLoc.Y + font.MeasureString(levelMsg).Y + Game1.SPACER_3);
            scoreMsgLoc = new Vector2(Game1.SCREEN_WIDTH * 3 / 4, timerMsgLoc.Y + font.MeasureString(timerMsg).Y + Game1.SPACER_3);
        }

        //Pre: None
        //Post: Returns an integer equal or greater than 0 representing the score
        //Desc: Returns the score of the current level
        public int GetScore()
        {
            //Return the score
            return score;
        }

        //Pre: None
        //Post: Returns a double equal or greater than 0 representing the time playing
        //Desc: Returns the time of the current level
        public double GetTimeInSec()
        {
            //Return the time rounded to 2 decimal places
            return Math.Round(escapeTimer.GetTimePassed() / 1000, 2);
        }

        //Pre: None
        //Post: Returns the display time of the timer as a string
        //Desc: Returns the display time
        public string GetTimeDisplay()
        {
            //Return the display time
            return escapeTimer.GetTimePassedAsString(Timer.FORMAT_MIN_SEC_MIL);
        }

        //Pre: None
        //Post: Returns an integer between 0 and the max level inclusive as the level number
        //Desc: Returns the level number of the current level
        public int GetLevelNum()
        {
            //Return the level number
            return levelNum;
        }

        //Pre: None
        //Post: Returns an integer between 0 and the max health inclusive as the health of the player
        //Desc: Returns the health in the current level
        public int GetHealth()
        {
            //Return the health
            return inventory.GetHealth();
        }

        //Pre: None
        //Post: Returns an integer equal or greater than 0 representing the number of zombies killed in the current level
        //Desc: Returns the number of zombies killed
        public int GetZombiesKilled()
        {
            //Return the number of zombies kiled
            return zombiesKilled;
        }

        //Pre: None
        //Post: None
        //Desc: Sets all the h costs of all the nodes based on its location from the player
        public void CalcHCost()
        {
            //Set the h costs if the player's node is not null
            if (player.GetCurNode() != null)
            {
                //Set the h costs of nodes in each row
                for (int i = 0; i < nodeMap.GetLength(0); i++)
                {
                    //Set the h costs of nodes in each column
                    for (int j = 0; j < nodeMap.GetLength(1); j++)
                    {
                        //Set the h cost of the node
                        nodeMap[i, j].SetH(Math.Abs(i - player.GetCurNode().GetRow()) + Math.Abs(j - player.GetCurNode().GetCol()));
                    }
                }
            }
        }

        //Pre: gameTime is the GameTime while kb is the current keyboard state and prevKb is the previous keyboard state in the previous update
        //Post: Returns an integer representing whether the level is over and whether the player won
        //Desc: Updates the level and returns an integer representing the status of the game
        public int UpdateAndLevelIsOver(GameTime gameTime, KeyboardState kb, KeyboardState prevKb)
        {
            //Update the timer and the timer message
            escapeTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            timerMsg = "Time: " + escapeTimer.GetTimePassedAsString(Timer.FORMAT_MIN_SEC_MIL);

            //Update the player depending on if it is exiting or still in the game
            if (isExiting)
            {
                //Update the unlock animation
                unlockAnim.Update(gameTime);

                //Update the player's node if it is not null or end the level if the lock animation stopped
                if (player.GetCurNode() != null)
                {
                    //Update the player's node and have the player exit in the corresponding direction
                    UpdatePlayerNode();
                    player.Exit(EXIT_DIR[levelNum - 1], gameTime);
                }
                else if (!unlockAnim.isAnimating)
                {
                    //Increase the score based on the time points and return the win status
                    score += (int)(MAX_TIME - escapeTimer.GetTimePassed() / 1000) * TIME_POINTS_PER_SEC;
                    return Game1.LEVEL_OVER_WIN;
                }
            }
            else
            {
                //Update the player
                UpdatePlayer(gameTime, kb, prevKb);
            }

            //Update all the bullets, zombies, and ghosts
            UpdateBullets(gameTime);
            UpdateMonsters(gameTime);
            UpdateGhosts(gameTime);

            //End the game if the player has no more health
            if (inventory.GetHealth() == 0)
            {
                //Return the integer representing a fail
                return Game1.LEVEL_OVER_FAIL;
            }

            //Return a negative integer to represent the game not over
            return -1;
        }

        //Pre: spriteBatch is a SpriteBatch for drawing the game's graphics
        //Post: None
        //Desc: Draws the graphics of the level
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the background carpet
            spriteBatch.Draw(carpetBg, new Rectangle(0, 0, Game1.SCREEN_WIDTH, Game1.SCREEN_HEIGHT), Color.White * 0.3f);

            //Draw each row of nodes
            for (int i = 0; i < nodeMap.GetLength(0); i++)
            {
                //Draw the node in each column
                for (int j = 0; j < nodeMap.GetLength(1); j++)
                {
                    //Draw the node
                    nodeMap[i, j].Draw(spriteBatch);
                }
            }

            //Draw each active bullet
            for (int i = 0; i < playerBullets.Count; i++)
            {
                //Draw the bullet
                playerBullets[i].Draw(spriteBatch);
            }

            //Draw each ghost in the list of ghosts
            for (int i = 0; i < ghosts.Count; i++)
            {
                //Draw the ghost
                ghosts[i].Draw(spriteBatch);
            }

            //Draw the stats bar, the inventory, and the stats messages
            spriteBatch.Draw(pixelImg, statsBarRec, Color.Black);
            inventory.Draw(spriteBatch);
            spriteBatch.DrawString(font, levelMsg, levelMsgLoc, Color.White);
            spriteBatch.DrawString(font, timerMsg, timerMsgLoc, Color.White);
            spriteBatch.DrawString(font, scoreMsg, scoreMsgLoc, Color.White);

            //Draw the unlock animation if it is animating
            if (unlockAnim.isAnimating)
            {
                //Draw the unlock animation
                unlockAnim.Draw(spriteBatch, Color.White, SpriteEffects.None);
            }
        }

        //Pre: gameTime is the GameTime of the game while kb is the current keyboard state and prevKb is the previous keyboard state of the previous update
        //Post: None
        //Desc: Updates the player
        public void UpdatePlayer(GameTime gameTime, KeyboardState kb, KeyboardState prevKb)
        {
            //Store a new bullet if the player shoots
            Bullet newBullet;

            //Move the player and update its node
            player.Move(kb, gameTime, nodeMap);
            UpdatePlayerNode();

            //Pickup an object based on the keys pressed
            player.PickupObject(kb, prevKb);

            //Allow the player to drop an item or unlock. If the player is unlocking, set its state to exiting
            if (player.DropItemOrUnlock(kb, prevKb))
            {
                //Set the player as exiting and play the unlock animation
                isExiting = true;
                unlockAnim.isAnimating = true;
            }

            //Allow the player to shoot if the space is pressed
            if (kb.IsKeyDown(Keys.Space) && prevKb.IsKeyUp(Keys.Space))
            {
                //Set the new bullet to a new bullet
                newBullet = player.Shoot();

                //Add the bullet to the list if the new bullet is not null
                if (newBullet != null)
                {
                    //Add the bullet to the list of bullets
                    playerBullets.Add(newBullet);
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Updates the node of the player by moving it to the next node in its direction
        public void UpdatePlayerNode()
        {
            //Store and set the next node of the player
            Node nextNode = player.GetCurNode().GetAdj(player.GetDir());

            //Move the player to the next node if it is not null
            if (nextNode != null)
            {
                //Update the player's current node if its center is in the rectangle of the next node
                if (nextNode.GetRec().Contains(player.GetRec().Center))
                {
                    //Remove the player from the current node, set the player's new current node, set the player on the next node, and recalculate the h cost of all nodes
                    player.GetCurNode().SetPlayer(null);
                    player.SetCurNode(nextNode);
                    player.GetCurNode().SetPlayer(player);
                    CalcHCost();
                }
            }
        }

        //Pre: gameTime is the GameTime for the game
        //Post: None
        //Desc: Updates all the active bullets in the list of bullets
        public void UpdateBullets(GameTime gameTime)
        {
            //Store the next node and the zombie that the bullet could potentially hit
            Node nextNode;
            Zombie hitZombie;

            //Update all player bullets
            for (int i = 0; i < playerBullets.Count; i++)
            {
                //Move each player bullet
                playerBullets[i].Move(gameTime);

                //Set the next node of the bullet
                nextNode = playerBullets[i].GetCurNode().GetAdj(playerBullets[i].GetDir());

                //Update the bullet's node if the next node is not null
                if (nextNode != null)
                {
                    //Move to the next node if the center of the bullet is in the rectangle of the next node
                    if (nextNode.GetRec().Contains(playerBullets[i].GetBulletPoint()))
                    {
                        //Remove the bullet from the current node, set its node to the next node, and add the bullet to the next node
                        playerBullets[i].GetCurNode().GetBulletsOnSpace().Remove(playerBullets[i]);
                        playerBullets[i].SetCurNode(nextNode);
                        playerBullets[i].GetCurNode().GetBulletsOnSpace().Add(playerBullets[i]);
                    }
                }

                //Remove the bullet if it is off screen
                if (playerBullets[i].GetOffScreen())
                {
                    //Remove the bullet from its node and remove the bullet from the list of bullets
                    playerBullets[i].GetCurNode().GetBulletsOnSpace().Remove(playerBullets[i]);
                    playerBullets.RemoveAt(i);
                    i--;
                }
                else if (playerBullets[i].CheckCollisionWithWall())
                {
                    //Remove the bullet from its node and from the list of bullets
                    playerBullets[i].GetCurNode().GetBulletsOnSpace().Remove(playerBullets[i]);
                    playerBullets.RemoveAt(i);
                    i--;
                }
                else
                {
                    //Set the hit zombie
                    hitZombie = playerBullets[i].CheckCollisionWithZombie();

                    //Reduce the health of the zombie and remove the bullet if a zombie is hit
                    if (hitZombie != null)
                    {
                        //Remove the bullet from its node and from the list of bullets
                        playerBullets[i].GetCurNode().GetBulletsOnSpace().Remove(playerBullets[i]);
                        playerBullets.RemoveAt(i);

                        //Remove the zombie if it has no more health
                        if (hitZombie.ReduceHealthAndDeterminedIsKilled())
                        {
                            //Increase the score and zombies killed and update the score message
                            zombiesKilled++;
                            score += hitZombie.GetReward();
                            scoreMsg = "Score: " + score;

                            //Remove the zombie from its node and from the list of zombies
                            hitZombie.GetCurNode().GetZombiesOnSpace().Remove(hitZombie);
                            zombies.Remove(hitZombie);
                        }
                    }
                }
            }
        }

        //Pre: gameTime is the GameTime for the game
        //Post: None
        //Desc: Updates all the active zombies in the list of zombies
        public void UpdateMonsters(GameTime gameTime)
        {
            //Store the next node of the zombie
            Node nextNode;

            //Update all the zombies in the list
            for (int i = 0; i < zombies.Count; i++)
            {
                //Update each zombie
                zombies[i].Update(gameTime, nodeMap, player);

                //Set the next node of the zombie
                nextNode = zombies[i].GetCurNode().GetAdj(zombies[i].GetDir());

                //Update the zombie's node to its next node if there is a next node in its direction
                if (nextNode != null)
                {
                    //Update the current node of the zombie if its center is in the rectangle of the next node
                    if (nextNode.GetRec().Contains(zombies[i].GetRec().Center))
                    {
                        //Remove the zombie from its current node, set its node to the next node, and add the zombie to the next node
                        zombies[i].GetCurNode().GetZombiesOnSpace().Remove(zombies[i]);
                        zombies[i].SetCurNode(nextNode);
                        zombies[i].GetCurNode().GetZombiesOnSpace().Add(zombies[i]);
                    }
                }
            }
        }

        //Pre: gameTime is the GameTime for the game
        //Post: None
        //Desc: Updates all the active ghosts in the list of ghosts
        private void UpdateGhosts(GameTime gameTime)
        {
            //Store the next node of the ghost
            Node nextNode;

            //Update all the ghosts in the list
            for (int i = 0; i < ghosts.Count; i++)
            {
                //Update each ghost
                ghosts[i].Update(gameTime, nodeMap, player);

                //Set the next node of the ghost
                nextNode = ghosts[i].GetCurNode().GetAdj(ghosts[i].GetDir());

                //Update the ghost's node to its next node if there is a next node in its direction
                if (nextNode != null)
                {
                    //Update the current node of the ghost if its center is in the rectangle of the next node
                    if (nextNode.GetRec().Contains(ghosts[i].GetRec().Center))
                    {
                        //Remove the ghost from its current node, set its node to the next node, and add the ghost to the next node
                        ghosts[i].GetCurNode().GetGhostsOnSpace().Remove(ghosts[i]);
                        ghosts[i].SetCurNode(nextNode);
                        ghosts[i].GetCurNode().GetGhostsOnSpace().Add(ghosts[i]);
                    }
                }
            }
        }
    }
}

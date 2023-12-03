//Author: Victoria Mak
//File Name: Game1.cs
//Project Name: Escape
//Creation Date: May 9, 2023
//Modified Date: June 12, 2023
//Description: Play the game, Escape the House! In this game, you go through 5 different rooms (5 levels) and try to find the key that allows you to enter the next room.

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;
using Helper;

namespace Escape
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //Store the number of levels and the maximum number of users to save at once
        public const int NUM_LEVELS = 5;
        private const int MAX_USERS = 3;

        //Store the hovering button states
        public const int NOT_HOVER = 0;
        public const int HOVER = 1;

        //Store the UI layout
        private const int BTN_SPACER = 30;
        public const int SPACER_1 = 40;
        public const int SPACER_2 = 15;
        public const int SPACER_3 = 5;
        private const int TOP_SPACER = 100;
        private const int USER_REC_WIDTH = 300;
        private const int USER_REC_HEIGHT = 450;
        private const int HOVER_DISPLACEMENT = 2;

        //Store the game states
        private const int MENU = 0;
        private const int PREGAME = 1;
        private const int PLAY = 2;
        public const int LEVEL_OVER_FAIL = 3;
        public const int LEVEL_OVER_WIN = 4;
        private const int ENDGAME = 5;
        private const int LEADERBOARD = 6;
        private const int INSTRUCTIONS = 7;

        //Store the pregame states
        private const int SELECT_USER = 0;
        private const int CREATE_NEW_USER = 1;

        //Store the number of rows and columns int the rooms and the stats bar height
        public const int NUM_ROWS = 17;
        public const int NUM_COLS = 35;
        public const int STATS_BAR_HEIGHT = 80;

        //Store the screen width and height
        public const int SCREEN_WIDTH = Node.SIDE_LENGTH * NUM_COLS;
        public const int SCREEN_HEIGHT = Node.SIDE_LENGTH * NUM_ROWS + STATS_BAR_HEIGHT;

        //Store the selecting user rectangles
        public static readonly Rectangle[] USER_CHOICE_RECS = { new Rectangle(SPACER_1, TOP_SPACER, USER_REC_WIDTH, USER_REC_HEIGHT),
                                                                new Rectangle(SPACER_1 * 2 + USER_REC_WIDTH, TOP_SPACER, USER_REC_WIDTH, USER_REC_HEIGHT),
                                                                new Rectangle(SPACER_1 * 3 + USER_REC_WIDTH * 2, TOP_SPACER, USER_REC_WIDTH, USER_REC_HEIGHT) };

        //Store the directions 
        public const int DOWN = 0;
        public const int UP = 1;
        public const int LEFT = 2;
        public const int RIGHT = 3;

        //Store the number of bullets in a gun
        public const int NUM_BULLETS_PER_GUN = 5;

        //Store the item type
        public const int WALL_V = 0;
        public const int WALL_H = 1;
        public const int BARRICADE_OPEN = 2;
        public const int BARRICADE_FOLDED = 3;
        public const int CHAIR = 4;
        public const int BOX = 5;
        public const int GUN = 6;
        public const int KEY = 7;
        public const int LOCK = 8;
        public const int HEART = 9;
        public const int BLANK = 10;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Store the current and previous mouse and keyboard states
        private KeyboardState prevKb;
        private MouseState prevMouse;
        private MouseState mouse;
        private KeyboardState kb;

        //Store the fonts
        private SpriteFont titleFont;
        private SpriteFont instrFont;
        private SpriteFont boldFont;

        //Store the files for reading and writing to save scores
        private StreamWriter outFile;
        private StreamReader inFile;

        //Store the starting monster locations
        private Vector2[][] regZombieLocs = new Vector2[5][];
        private Vector2[][] smartZombieLocs = new Vector2[5][];

        //Store the buttons in all game states except the play state
        private Texture2D[] menuBtns = new Texture2D[2];
        private Texture2D[] playBtns = new Texture2D[2];
        private Texture2D[] leaderboardBtns = new Texture2D[2];
        private Texture2D[] exitBtns = new Texture2D[2];
        private Texture2D[] nextRoomBtns = new Texture2D[2];
        private Texture2D[] contProgressBtns = new Texture2D[2];
        private Texture2D[] newUserBtns = new Texture2D[2];
        private Texture2D[] deleteProgressBtns = new Texture2D[2];
        private Texture2D[] tryAgainBtns = new Texture2D[2];
        private Texture2D[] nextBtns = new Texture2D[2];
        private Texture2D[] instrBtns = new Texture2D[2];

        //Store whether the buttons are being hovered over
        private bool hoverMenuBtn = false;
        private bool hoverPlayBtn = false;
        private bool hoverInstrBtn = false;
        private bool hoverLeaderboardBtn = false;
        private bool hoverExitBtn = false;
        private bool hoverNextRoomBtn = false;
        private bool hoverTryAgainBtn = false;

        //Store the button rectangles
        private Rectangle[] menuBtnRecs1 = new Rectangle[2];
        private Rectangle[] menuBtnRecs2 = new Rectangle[2];
        private Rectangle[] playBtnRecs = new Rectangle[2];
        private Rectangle[] leaderBoardBtnRecs = new Rectangle[2];
        private Rectangle[] exitBtnRecs = new Rectangle[2];
        private Rectangle[] nextRoomBtnRecs = new Rectangle[2];
        private Rectangle[][] contProgressBtnRecs = new Rectangle[3][];
        private Rectangle[][] deleteProgressBtnRecs = new Rectangle[3][];
        private Rectangle[] tryAgainBtnRecs = new Rectangle[2];
        private Rectangle[] instrBtnRecs = new Rectangle[2];

        //Store the background images
        private Texture2D menuBg;
        private Texture2D endGameBg;
        private Texture2D corridorImg;
        private Texture2D frameImg;
        private Texture2D leaderboardBg;

        //Store the carpet background images
        private Texture2D[] carpetBgs = new Texture2D[NUM_LEVELS];

        //Store the game images
        private Texture2D[] regZombieImgs = new Texture2D[4];
        private Texture2D[] smartZombieImgs = new Texture2D[4];
        private Texture2D[] ghostImgs = new Texture2D[4];
        private Texture2D[] playerImgs = new Texture2D[4];
        private Texture2D[] bulletImgs = new Texture2D[4];
        private Texture2D[] itemImgs = new Texture2D[10];

        //Store the blank pixel image
        private Texture2D pixelImg;

        //Store the typing name rectangle
        private Rectangle typeNameRec;

        //Store the messages to be displayed after each level
        private string levelMsg = "YOU ESCAPED ROOM 1!";
        private string timeMsg = "Time: ";
        private string scoreMsg = "Scores: ";
        private string totScoreMsg = "Total Score: ";
        private string newLevelHsMsg = "New high score for level ";

        //Store the messages to be displayed after all levels are completed
        private string finalEscapeMsg = "YOU ESCAPED THE HOUSE!";
        private string[] eGScoreMsgs = new string[] { "Level 1 Score: ", "Level 2 Score: ", "Level 3 Score: ", "Level 4 Score: ", "Level 5 Score: " };
        private string[] eGTimeMsgs = new string[] { "Level 1 Time: ", "Level 2 Time: ", "Level 3 Time: ", "Level 4 Time: ", "Level 5 Time: " };
        private string eGFinalScoreMsg = "Final Score: ";
        private string eGZombiesKilledMsg = "Zombies Killed: ";

        //Store the leaderboard title, labels, scores, and names
        private string lBTitle = "Leaderboard";
        private string[] lBLevelMsgs = { "Level 1 Highscore: ", "Level 2 Highscore: ", "Level 3 Highscore: ", "Level 4 Highscore: ", "Level 5 Highscore: " };
        private string[] lBNames = new string[] { "", "", "", "", "" };
        private int[] highScores = new int[] { 0, 0, 0, 0, 0 };

        //Store the instructions 
        private string instructionsTitle = "HOW TO PLAY";
        private string[] instructionMsgs = { "Objective: Escape the house by finding the key to unlock the lock",
                                             "Use the arrow keys to move the player",
                                             "To collect items, press the F key",
                                             "To drop items, press the D key",
                                             "To shoot bullets, press SPACE",
                                             "Zombies and ghosts harm you. Avoid them",
                                             "The key can be hidden under boxes or chairs; pick them up to reveal the key",
                                             "To unlock the lock, press D facing the lock"
                                           };

        //Store the locations for the messages that are displayed after each level
        private Vector2 levelMsgLoc;
        private Vector2 timeMsgLoc;
        private Vector2 scoreMsgLoc;
        private Vector2 totScoreMsgLoc;
        private Vector2 newLevelHsLoc;

        //Store the locations for the messages that are displayed after all levels are completed
        private Vector2 finalEscapeMsgLoc;
        private Vector2[] scoreMsgLocs = new Vector2[NUM_LEVELS];
        private Vector2[] eGTimeMsgLocs = new Vector2[NUM_LEVELS];
        private Vector2 eGTotScoreMsgLoc;
        private Vector2 eGZombiesKilledMsgLoc;

        //Store the locations for the messages that are displayed in the leaderboard state
        private Vector2 lBTitleLoc;
        private Vector2[] lbLabelLocs = new Vector2[NUM_LEVELS];
        private Vector2[] lBNameLocs = new Vector2[NUM_LEVELS];
        private Vector2[] lBScoreMsgLocs = new Vector2[NUM_LEVELS];

        //Store the location for the instructions messages
        private Vector2[] instrMsgLocs = new Vector2[8];

        //Store the level
        private Level level;

        //Store the title and its location in the menu
        private string title = "Escape the House";
        private Vector2 titleLoc;

        //store the error messages and their locations
        private string saveStatsErrorMsg = "";
        private string readFileErrorMsg = "";
        private Vector2 saveErrorMsgLoc;
        private Vector2 readFileErrorMsgLoc;

        //Store the game and pregame state
        private int gameState = MENU;
        private int pregameState = SELECT_USER;

        //Store the full screen and frame rectangle
        private Rectangle fullScreenRec;
        private Rectangle frameRec;

        //Store the unlocking animation
        private Animation unlockAnim;

        //Store the users and the current user
        private User[] users = new User[MAX_USERS];
        private User curUser;

        //Store the previous health of the user from the beginning of the level
        private int prevHealth = Inventory.MAX_HEALTH;

        //Store the bullet sounds and background music
        private SoundEffect impactSnd;
        private SoundEffect shootSnd;
        private Song gameMusic;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //Set the game screen size
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;

            //Set the mouse as visible
            IsMouseVisible = true;

            //Apply the changes to the game graphics
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //Load the sound effects and music
            shootSnd = Content.Load<SoundEffect>("Audio/ShootSnd");
            impactSnd = Content.Load<SoundEffect>("Audio/ImpactSnd");
            gameMusic = Content.Load<Song>("Audio/TerrorMusic");

            //Load the font
            titleFont = Content.Load<SpriteFont>("Fonts/TitleFont");
            instrFont = Content.Load<SpriteFont>("Fonts/InstrFont");
            boldFont = Content.Load<SpriteFont>("Fonts/BoldFont");

            //Load the background images
            menuBg = Content.Load<Texture2D>("Backgrounds/HauntedHouseBg");
            corridorImg = Content.Load<Texture2D>("Backgrounds/LevelOverBg");
            frameImg = Content.Load<Texture2D>("Sprites/FailPopup");
            endGameBg = Content.Load<Texture2D>("Backgrounds/EndGameBg");
            leaderboardBg = Content.Load<Texture2D>("Backgrounds/LeaderboardBg");
            carpetBgs[0] = Content.Load<Texture2D>("Backgrounds/CarpetImg1");
            carpetBgs[1] = Content.Load<Texture2D>("Backgrounds/CarpetImg2");
            carpetBgs[2] = Content.Load<Texture2D>("Backgrounds/CarpetImg3");
            carpetBgs[3] = Content.Load<Texture2D>("Backgrounds/CarpetImg4");
            carpetBgs[4] = Content.Load<Texture2D>("Backgrounds/CarpetImg5");

            //Load the regular zombie images
            regZombieImgs[DOWN] = Content.Load<Texture2D>("Sprites/ZombieWalkFrontImg");
            regZombieImgs[UP] = Content.Load<Texture2D>("Sprites/ZombieWalkBackImg");
            regZombieImgs[LEFT] = Content.Load<Texture2D>("Sprites/ZombieWalkLeftImg");
            regZombieImgs[RIGHT] = Content.Load<Texture2D>("Sprites/ZombieWalkRightImg");

            //Load the smart zombie images
            smartZombieImgs[DOWN] = Content.Load<Texture2D>("Sprites/SmartZombFront");
            smartZombieImgs[UP] = Content.Load<Texture2D>("Sprites/SmartZombBack");
            smartZombieImgs[LEFT] = Content.Load<Texture2D>("Sprites/SmartZombLeft");
            smartZombieImgs[RIGHT] = Content.Load<Texture2D>("Sprites/SmartZombRight");
            
            //Load the ghost images
            ghostImgs[DOWN] = Content.Load<Texture2D>("Sprites/GhostFwd");
            ghostImgs[UP] = Content.Load<Texture2D>("Sprites/GhostBack");
            ghostImgs[LEFT] = Content.Load<Texture2D>("Sprites/GhostLeft");
            ghostImgs[RIGHT] = Content.Load<Texture2D>("Sprites/GhostRight");

            //Load the item images
            itemImgs[WALL_H] = Content.Load<Texture2D>("Barriers/BrickHoriz");
            itemImgs[WALL_V] = Content.Load<Texture2D>("Barriers/BrickVert");
            itemImgs[BARRICADE_OPEN] = Content.Load<Texture2D>("Barriers/Barricade");
            itemImgs[BARRICADE_FOLDED] = Content.Load<Texture2D>("Barriers/BarricadeFolded");
            itemImgs[BOX] = Content.Load<Texture2D>("Barriers/Box");
            itemImgs[CHAIR] = Content.Load<Texture2D>("Barriers/Chair");
            itemImgs[GUN] = Content.Load<Texture2D>("Sprites/Gun");
            itemImgs[KEY] = Content.Load<Texture2D>("Sprites/Key");
            itemImgs[HEART] = Content.Load<Texture2D>("Sprites/Heart");
            itemImgs[LOCK] = Content.Load<Texture2D>("Sprites/Lock");

            //Load the player spritesheet images
            playerImgs[DOWN] = Content.Load<Texture2D>("Sprites/PlayerFwdWalk");
            playerImgs[UP] = Content.Load<Texture2D>("Sprites/PlayerBackWalk");
            playerImgs[LEFT] = Content.Load<Texture2D>("Sprites/PlayerLeftWalk");
            playerImgs[RIGHT] = Content.Load<Texture2D>("Sprites/PlayerRightWalk");

            //Load the bullet images
            bulletImgs[DOWN] = Content.Load<Texture2D>("Sprites/BulletDown");
            bulletImgs[UP] = Content.Load<Texture2D>("Sprites/BulletUp");
            bulletImgs[RIGHT] = Content.Load<Texture2D>("Sprites/BulletRight");
            bulletImgs[LEFT] = Content.Load<Texture2D>("Sprites/BulletLeft");

            pixelImg = Content.Load<Texture2D>("Backgrounds/BlankPixel");


            //Load the button images for when they are hovered over or not hovered over
            for (int i = 0; i < menuBtns.Length; i++)
            {
                //Load each button image
                menuBtns[i] = Content.Load<Texture2D>("Buttons/MenuButton" + (i + 1));
                playBtns[i] = Content.Load<Texture2D>("Buttons/PlayButton" + (i + 1));
                leaderboardBtns[i] = Content.Load<Texture2D>("Buttons/LeaderboardButton" + (i + 1));
                exitBtns[i] = Content.Load<Texture2D>("Buttons/ExitButton" + (i + 1));
                nextRoomBtns[i] = Content.Load<Texture2D>("Buttons/NextRoomButton" + (i + 1));
                contProgressBtns[i] = Content.Load<Texture2D>("Buttons/ContProgressButton" + (i + 1));
                deleteProgressBtns[i] = Content.Load<Texture2D>("Buttons/DeleteProgressBtn" + (i + 1));
                newUserBtns[i] = Content.Load<Texture2D>("Buttons/NewUserButton" + (i + 1));
                tryAgainBtns[i] = Content.Load<Texture2D>("Buttons/TryAgainBtn" + (i + 1));
                nextBtns[i] = Content.Load<Texture2D>("Buttons/NextBtn" + (i + 1));
                instrBtns[i] = Content.Load<Texture2D>("Buttons/InstrBtn" + (i + 1));
            }

            //Set the location of the messages displayed after each level
            levelMsgLoc = new Vector2(SCREEN_WIDTH / 2, frameRec.Y + 3 * SPACER_1);
            timeMsgLoc = new Vector2(SCREEN_WIDTH / 2, levelMsgLoc.Y + titleFont.MeasureString(levelMsg).Y + SPACER_3);
            scoreMsgLoc = new Vector2(SCREEN_WIDTH / 2, timeMsgLoc.Y + instrFont.MeasureString(levelMsg).Y + SPACER_3);
            totScoreMsgLoc = new Vector2(SCREEN_WIDTH / 2, scoreMsgLoc.Y + instrFont.MeasureString(levelMsg).Y + SPACER_3);
            newLevelHsLoc = new Vector2(SCREEN_WIDTH / 2 - boldFont.MeasureString(newLevelHsMsg).X / 2, levelMsgLoc.Y - SPACER_1 - boldFont.MeasureString("|").Y);

            //Set the location of the error message
            saveErrorMsgLoc = new Vector2(0, SCREEN_HEIGHT - SPACER_1);
            readFileErrorMsgLoc = new Vector2(0, SPACER_3);

            //Set the location of the messages shown in the end game state
            for (int i = 0; i < NUM_LEVELS; i++)
            {
                //Set the location of the score messages and the time messages
                scoreMsgLocs[i] = new Vector2(4 * SPACER_1, finalEscapeMsgLoc.Y + titleFont.MeasureString(finalEscapeMsg).Y + (i + 2) * SPACER_2 + i * instrFont.MeasureString(eGScoreMsgs[i]).Y);
                eGTimeMsgLocs[i] = new Vector2(SCREEN_WIDTH / 2 - 2 * SPACER_1, scoreMsgLocs[i].Y);
            }

            //Set the location of the scores and names in the leaderboard state
            for (int i = 0; i < NUM_LEVELS; i++)
            {
                //Set the location of the scores and names
                lbLabelLocs[i] = new Vector2(scoreMsgLocs[i].X, scoreMsgLocs[i].Y + SPACER_1);
                lBScoreMsgLocs[i] = new Vector2(SCREEN_WIDTH / 2 - 3 * SPACER_1, lbLabelLocs[i].Y);
                lBNameLocs[i] = new Vector2(SCREEN_WIDTH / 2 + 2 * SPACER_1, lbLabelLocs[i].Y);
            }

            //Set the location of the instruction messages
            for (int i = 0; i < instructionMsgs.Length; i++)
            {
                //Set the location for each line of instruction
                instrMsgLocs[i] = new Vector2(scoreMsgLocs[0].X, titleLoc.Y + titleFont.MeasureString(instructionsTitle).Y + SPACER_1 + i * (instrFont.MeasureString(instructionMsgs[0]).Y + SPACER_3));
            }

            //Set the location of the total score and zombies killed message location
            eGTotScoreMsgLoc = new Vector2(scoreMsgLocs[NUM_LEVELS - 1].X, scoreMsgLocs[NUM_LEVELS - 1].Y + SPACER_2 + instrFont.MeasureString(eGScoreMsgs[NUM_LEVELS - 1]).Y);
            eGZombiesKilledMsgLoc = new Vector2(eGTimeMsgLocs[NUM_LEVELS - 1].X, eGTotScoreMsgLoc.Y);

            //Store the unlock animation
            unlockAnim = new Animation(Content.Load<Texture2D>("Sprites/UnlockAnim"), 4, 1, 4, 0, 0, Animation.ANIMATE_ONCE, 20, new Vector2(SCREEN_WIDTH / 2 - 240, SCREEN_HEIGHT / 2 - 240), 1f, false);

            //Set the button rectangles in the menu
            exitBtnRecs[NOT_HOVER] = new Rectangle(SCREEN_WIDTH - exitBtns[0].Width - BTN_SPACER, SCREEN_HEIGHT - exitBtns[0].Height - BTN_SPACER, exitBtns[0].Width, exitBtns[0].Height);
            exitBtnRecs[HOVER] = new Rectangle(SCREEN_WIDTH - exitBtns[0].Width - BTN_SPACER, exitBtnRecs[0].Y - HOVER_DISPLACEMENT, exitBtns[0].Width, exitBtns[0].Height);
            leaderBoardBtnRecs[NOT_HOVER] = new Rectangle(exitBtnRecs[0].X, exitBtnRecs[0].Y - exitBtns[0].Height - BTN_SPACER, exitBtns[0].Width, exitBtns[0].Height);
            leaderBoardBtnRecs[HOVER] = new Rectangle(exitBtnRecs[0].X, leaderBoardBtnRecs[0].Y - HOVER_DISPLACEMENT, exitBtns[0].Width, exitBtns[0].Height);
            playBtnRecs[NOT_HOVER] = new Rectangle(exitBtnRecs[0].X, leaderBoardBtnRecs[0].Y - exitBtns[0].Height - BTN_SPACER, exitBtns[0].Width, exitBtns[0].Height);
            playBtnRecs[HOVER] = new Rectangle(exitBtnRecs[0].X, playBtnRecs[0].Y - HOVER_DISPLACEMENT, exitBtns[0].Width, exitBtns[0].Height);
            instrBtnRecs[NOT_HOVER] = new Rectangle(exitBtnRecs[0].X, playBtnRecs[0].Y - exitBtns[0].Height - BTN_SPACER, exitBtns[0].Width, exitBtns[0].Height);
            instrBtnRecs[HOVER] = new Rectangle(exitBtnRecs[0].X, instrBtnRecs[0].Y - HOVER_DISPLACEMENT, exitBtns[0].Width, exitBtns[0].Height);

            //Set the continue progress button rectangles in the pregame state
            contProgressBtnRecs[0] = new Rectangle[2];
            contProgressBtnRecs[1] = new Rectangle[2];
            contProgressBtnRecs[2] = new Rectangle[2];
            contProgressBtnRecs[0][NOT_HOVER] = new Rectangle(USER_CHOICE_RECS[0].X + (USER_CHOICE_RECS[0].Width - newUserBtns[0].Width) / 2, USER_CHOICE_RECS[0].Bottom - SPACER_1 - newUserBtns[0].Height, newUserBtns[0].Width, newUserBtns[0].Height);
            contProgressBtnRecs[1][NOT_HOVER] = new Rectangle(USER_CHOICE_RECS[1].X + (USER_CHOICE_RECS[0].Width - newUserBtns[0].Width) / 2, USER_CHOICE_RECS[0].Bottom - SPACER_1 - newUserBtns[0].Height, newUserBtns[0].Width, newUserBtns[0].Height);
            contProgressBtnRecs[2][NOT_HOVER] = new Rectangle(USER_CHOICE_RECS[2].X + (USER_CHOICE_RECS[0].Width - newUserBtns[0].Width) / 2, USER_CHOICE_RECS[0].Bottom - SPACER_1 - newUserBtns[0].Height, newUserBtns[0].Width, newUserBtns[0].Height);
            contProgressBtnRecs[0][HOVER] = new Rectangle(USER_CHOICE_RECS[0].X + (USER_CHOICE_RECS[0].Width - newUserBtns[0].Width) / 2, USER_CHOICE_RECS[0].Bottom - SPACER_1 - newUserBtns[0].Height - HOVER_DISPLACEMENT, newUserBtns[0].Width, newUserBtns[0].Height);
            contProgressBtnRecs[1][HOVER] = new Rectangle(USER_CHOICE_RECS[1].X + (USER_CHOICE_RECS[0].Width - newUserBtns[0].Width) / 2, USER_CHOICE_RECS[0].Bottom - SPACER_1 - newUserBtns[0].Height - HOVER_DISPLACEMENT, newUserBtns[0].Width, newUserBtns[0].Height);
            contProgressBtnRecs[2][HOVER] = new Rectangle(USER_CHOICE_RECS[2].X + (USER_CHOICE_RECS[0].Width - newUserBtns[0].Width) / 2, USER_CHOICE_RECS[0].Bottom - SPACER_1 - newUserBtns[0].Height - HOVER_DISPLACEMENT, newUserBtns[0].Width, newUserBtns[0].Height);

            //Set the delete progress button rectangles in the pregame state
            deleteProgressBtnRecs[0] = new Rectangle[2];
            deleteProgressBtnRecs[1] = new Rectangle[2];
            deleteProgressBtnRecs[2] = new Rectangle[2];
            deleteProgressBtnRecs[0][NOT_HOVER] = new Rectangle(USER_CHOICE_RECS[0].X + (USER_CHOICE_RECS[0].Width - newUserBtns[0].Width) / 2, contProgressBtnRecs[0][0].Y - SPACER_2 - newUserBtns[0].Height, newUserBtns[0].Width, newUserBtns[0].Height);
            deleteProgressBtnRecs[1][NOT_HOVER] = new Rectangle(USER_CHOICE_RECS[1].X + (USER_CHOICE_RECS[0].Width - newUserBtns[0].Width) / 2, contProgressBtnRecs[0][0].Y - SPACER_2 - newUserBtns[0].Height, newUserBtns[0].Width, newUserBtns[0].Height);
            deleteProgressBtnRecs[2][NOT_HOVER] = new Rectangle(USER_CHOICE_RECS[2].X + (USER_CHOICE_RECS[0].Width - newUserBtns[0].Width) / 2, contProgressBtnRecs[0][0].Y - SPACER_2 - newUserBtns[0].Height, newUserBtns[0].Width, newUserBtns[0].Height);
            deleteProgressBtnRecs[0][HOVER] = new Rectangle(USER_CHOICE_RECS[0].X + (USER_CHOICE_RECS[0].Width - newUserBtns[0].Width) / 2, contProgressBtnRecs[0][0].Y - SPACER_2 - newUserBtns[0].Height - HOVER_DISPLACEMENT, newUserBtns[0].Width, newUserBtns[0].Height);
            deleteProgressBtnRecs[1][HOVER] = new Rectangle(USER_CHOICE_RECS[1].X + (USER_CHOICE_RECS[0].Width - newUserBtns[0].Width) / 2, contProgressBtnRecs[0][0].Y - SPACER_2 - newUserBtns[0].Height - HOVER_DISPLACEMENT, newUserBtns[0].Width, newUserBtns[0].Height);
            deleteProgressBtnRecs[2][HOVER] = new Rectangle(USER_CHOICE_RECS[2].X + (USER_CHOICE_RECS[0].Width - newUserBtns[0].Width) / 2, contProgressBtnRecs[0][0].Y - SPACER_2 - newUserBtns[0].Height - HOVER_DISPLACEMENT, newUserBtns[0].Width, newUserBtns[0].Height);

            //Set the rectangles for the menu, try again, and next room buttons
            menuBtnRecs1[NOT_HOVER] = new Rectangle(SCREEN_WIDTH / 2 - menuBtns[NOT_HOVER].Width - SPACER_3, (int)(totScoreMsgLoc.Y + instrFont.MeasureString(totScoreMsg).Y + SPACER_2), menuBtns[NOT_HOVER].Width, menuBtns[NOT_HOVER].Height);
            menuBtnRecs1[HOVER] = new Rectangle(SCREEN_WIDTH / 2 - menuBtns[NOT_HOVER].Width - SPACER_3, menuBtnRecs1[NOT_HOVER].Y - HOVER_DISPLACEMENT, menuBtns[NOT_HOVER].Width, menuBtns[NOT_HOVER].Height);
            menuBtnRecs2[NOT_HOVER] = new Rectangle((int)scoreMsgLocs[0].X, (int)(totScoreMsgLoc.Y + instrFont.MeasureString(totScoreMsg).Y + 2 * SPACER_1), menuBtns[NOT_HOVER].Width, menuBtns[NOT_HOVER].Height);
            menuBtnRecs2[HOVER] = new Rectangle((int)scoreMsgLocs[0].X, menuBtnRecs2[NOT_HOVER].Y - HOVER_DISPLACEMENT, menuBtns[NOT_HOVER].Width, menuBtns[NOT_HOVER].Height);
            tryAgainBtnRecs[NOT_HOVER] = new Rectangle(SCREEN_WIDTH / 2 + SPACER_3, (int)(totScoreMsgLoc.Y + instrFont.MeasureString(totScoreMsg).Y + SPACER_2), menuBtns[NOT_HOVER].Width, menuBtns[NOT_HOVER].Height);
            tryAgainBtnRecs[HOVER] = new Rectangle(SCREEN_WIDTH / 2 + SPACER_3, menuBtnRecs1[NOT_HOVER].Y - HOVER_DISPLACEMENT, menuBtns[NOT_HOVER].Width, menuBtns[NOT_HOVER].Height);
            nextRoomBtnRecs[NOT_HOVER] = new Rectangle(SCREEN_WIDTH / 2 - nextRoomBtns[NOT_HOVER].Width / 2, menuBtnRecs1[NOT_HOVER].Bottom + SPACER_3, nextRoomBtns[NOT_HOVER].Width, nextRoomBtns[NOT_HOVER].Height);
            nextRoomBtnRecs[HOVER] = new Rectangle(SCREEN_WIDTH / 2 - nextRoomBtns[NOT_HOVER].Width / 2, nextRoomBtnRecs[NOT_HOVER].Y - 2, nextRoomBtns[NOT_HOVER].Width, nextRoomBtns[NOT_HOVER].Height);

            //Set the title locations in the menu, end game, and leaderboard
            titleLoc = new Vector2(SCREEN_WIDTH / 2 - titleFont.MeasureString(title).X / 2, 2 * SPACER_1);
            finalEscapeMsgLoc = new Vector2(SCREEN_WIDTH / 2 - titleFont.MeasureString(finalEscapeMsg).X / 2, SPACER_1);
            lBTitleLoc = new Vector2(SCREEN_WIDTH / 2 - titleFont.MeasureString(lBTitle).X / 2, SPACER_2);

            //Set the rectangles of the background images and the frame image
            fullScreenRec = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
            frameRec = new Rectangle(SCREEN_WIDTH / 2 - frameImg.Width / 2, SCREEN_HEIGHT / 2 - frameImg.Height / 2, frameImg.Width, frameImg.Height);

            //Set the starting locations for the regular zombies
            regZombieLocs[0] = new Vector2[] { new Vector2(1, 25), new Vector2(10, 13) };
            regZombieLocs[1] = new Vector2[] { new Vector2(1, 0), new Vector2(11, 7), new Vector2(1, 30) };
            regZombieLocs[2] = new Vector2[] { };
            regZombieLocs[3] = new Vector2[] { new Vector2(7, 15), new Vector2(3, 26), new Vector2(14, 16) };
            regZombieLocs[4] = new Vector2[2] { new Vector2(1, 25), new Vector2(10, 13) };

            //Set the starting locations for the smart zombies
            smartZombieLocs[0] = new Vector2[] { new Vector2(1, 31) };
            smartZombieLocs[1] = new Vector2[] { new Vector2(3, 23), new Vector2(15, 24) };
            smartZombieLocs[2] = new Vector2[] { new Vector2(14, 2), new Vector2(1, 23), new Vector2(15, 23), new Vector2(8, 34), new Vector2(1, 0), new Vector2(8, 17) };
            smartZombieLocs[3] = new Vector2[] { new Vector2(12, 21), new Vector2(4, 32), new Vector2(13, 32), new Vector2(1, 17) };
            smartZombieLocs[4] = new Vector2[] { new Vector2(2, 17), new Vector2(14, 13), new Vector2(14, 22), new Vector2(15, 33), new Vector2(15, 1) };

            //Read in the scores
            ReadUserInfo();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //Get the current and previous states of the mouse and keyboard
            prevKb = kb;
            prevMouse = mouse;
            mouse = Mouse.GetState();
            kb = Keyboard.GetState();

            //Update the game depending on the state
            switch (gameState)
            {
                case MENU:
                    //Update the menu
                    UpdateMenu();
                    break;

                case PREGAME:
                    //Update the pre game state
                    UpdatePreGame();
                    break;

                case PLAY:
                    //Update the game state
                    UpdatePlay(gameTime);
                    break;

                case LEVEL_OVER_FAIL:
                    //Update the stats state
                    UpdateLevelOverFail();
                    break;

                case LEVEL_OVER_WIN:
                    //Update the stats state
                    UpdateLevelOverWin();
                    break;

                case ENDGAME:
                    //Update the end game state
                    UpdateEndGame();
                    break;

                case LEADERBOARD:
                    //Update the shop state
                    UpdateLeaderboard();
                    break;

                case INSTRUCTIONS:
                    //Update the shop state
                    UpdateInstructions();
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //Draw the game based on the game state
            switch (gameState)
            {
                case MENU:
                    //Update the menu
                    DrawMenu();
                    break;

                case PREGAME:
                    //Update the pre game state
                    DrawPreGame();
                    break;

                case PLAY:
                    //Update the game state
                    DrawPlay();
                    break;

                case LEVEL_OVER_FAIL:
                    //Update the stats state
                    DrawLevelOverFail();
                    break;

                case LEVEL_OVER_WIN:
                    //Update the stats state
                    DrawLevelOverWin();
                    break;

                case ENDGAME:
                    //Update the end game state
                    DrawEndGame();
                    break;

                case LEADERBOARD:
                    //Update the shop state
                    DrawLeaderboard();
                    break;

                case INSTRUCTIONS:
                    //Update the shop state
                    DrawInstructions();
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        //Pre: none
        //Post: none
        //Desc: Save the stats by writing the values to the stats file
        private void SaveProfilesAndScores()
        {
            try
            {
                //Open the stats file
                outFile = File.CreateText("Scores.txt");

                //Write the user info from each profile
                for (int i = 0; i < MAX_USERS; i++)
                {
                    //Write the info if the profile is created
                    if (users[i].GetIsCreated())
                    {
                        //Write the name, level, zombies killed, and health
                        outFile.WriteLine(users[i].GetName());
                        outFile.WriteLine(users[i].GetLevel());
                        outFile.WriteLine(users[i].GetZombiesKilled());
                        outFile.WriteLine(users[i].GetHealth());

                        //Write the time for each level
                        for (int j = 0; j < NUM_LEVELS; j++)
                        {
                            //Write the times for each level
                            if (j < NUM_LEVELS - 1)
                            {
                                //Write the time for levels before the last level with a comma
                                outFile.Write(users[i].GetTimes()[j] + ",");
                            }
                            else
                            {
                                //Write the time for the last level
                                outFile.WriteLine(users[i].GetTimes()[j]);
                            }
                        }

                        //Write the scores for each level
                        for (int j = 0; j < NUM_LEVELS; j++)
                        {
                            //Write the times for each level
                            if (j < NUM_LEVELS - 1)
                            {
                                //Write the time for levels before the last level with a comma
                                outFile.Write(users[i].GetScores()[j] + ",");
                            }
                            else
                            {
                                //Write the time for the last level
                                outFile.WriteLine(users[i].GetScores()[j]);
                            }
                        }
                    }
                    else
                    {
                        //Write a period to represent the profile being uncreated
                        outFile.WriteLine(".");
                    }
                }

                //Write the high scores
                for (int i = 0; i < NUM_LEVELS; i++)
                {
                    //Write the high score values
                    if (i < NUM_LEVELS - 1)
                    {
                        //Write the value with a comma if the level is less than the last level
                        outFile.Write(highScores[i] + ",");
                    }
                    else
                    {
                        //Write the last level's high score
                        outFile.WriteLine(highScores[i]);
                    }
                }

                //Write the names that achieved the high score
                for (int i = 0; i < NUM_LEVELS; i++)
                {
                    //Write the names
                    if (i < NUM_LEVELS - 1)
                    {
                        //Write the name with a comma if the level is less than the last level
                        outFile.Write(lBNames[i] + ",");
                    }
                    else
                    {
                        //Write the last level's high score name
                        outFile.Write(lBNames[i]);
                    }
                }
                //Set the error message to nothing
                saveStatsErrorMsg = "";
            }
            catch (FileNotFoundException fnf)
            {
                //Set the eror message and center it
                saveStatsErrorMsg = "ERROR SAVING STATS: File was not found.";
                saveErrorMsgLoc.X = (SCREEN_WIDTH - instrFont.MeasureString(saveStatsErrorMsg).X) / 2;
            }
            catch (IndexOutOfRangeException ore)
            {
                //Set the eror message and center it
                saveStatsErrorMsg = "ERROR SAVING STATS: The game tried reading past an array in the stats file.";
                saveErrorMsgLoc.X = (SCREEN_WIDTH - instrFont.MeasureString(saveStatsErrorMsg).X) / 2;
            }
            catch (Exception e)
            {
                //Set the error message and center it
                saveStatsErrorMsg = "ERROR SAVING STATS: " + e.Message;
                saveErrorMsgLoc.X = (SCREEN_WIDTH - instrFont.MeasureString(saveStatsErrorMsg).X) / 2;
            }
            finally
            {
                //Close the stats file if it is not empty
                if (outFile != null)
                {
                    //Close the file
                    outFile.Close();
                }
            }
        }

        //Pre: none
        //Post: none
        //Desc: Read in the stats by reading in the values from the stats file
        private void ReadUserInfo()
        {
            //Store the array of data, name, level, times, scores, zombies killed, and health to be read
            string[] data = new string[1];
            string name = "";
            int level = 1;
            double[] times = new double[NUM_LEVELS];
            int[] scores = new int[NUM_LEVELS];
            int zombiesKilled = 0;
            int health = Inventory.MAX_HEALTH;

            try
            {
                //Open the stats file
                inFile = File.OpenText("Scores.txt");

                //Read each user's data
                for (int i = 0; i < MAX_USERS; i++)
                {
                    try
                    {
                        //Read the first line and set it as the data
                        data[0] = inFile.ReadLine();

                        //Set the user as uncreated if the data is a period
                        if (data[0][0] == '.')
                        {
                            //Create an uncreated user profile
                            users[i] = new User(i, newUserBtns, contProgressBtns, deleteProgressBtns, contProgressBtnRecs[i], deleteProgressBtnRecs[i], instrFont);
                        }
                        else
                        {
                            //Set the name, level, zombies killed, and health
                            name = data[0];
                            level = Convert.ToInt32(inFile.ReadLine());
                            zombiesKilled = Convert.ToInt32(inFile.ReadLine());
                            health = Convert.ToInt32(inFile.ReadLine());

                            //Set the array of data as the next line split up by commas
                            data = inFile.ReadLine().Split(',');

                            //Set the times as each value in the array of data
                            for (int j = 0; j < data.Length; j++)
                            {
                                //Set each time
                                times[j] = Convert.ToDouble(data[j]);
                            }

                            //Set the array of data as the next line split up by commas
                            data = inFile.ReadLine().Split(',');

                            //Set the scores as each value in the array of data
                            for (int j = 0; j < data.Length; j++)
                            {
                                //Set each score
                                scores[j] = Convert.ToInt32(data[j]);
                            }

                            //Create a new user based on the data retrieved
                            users[i] = new User(name, i, instrFont, level, health, times, scores, zombiesKilled, newUserBtns, contProgressBtns, deleteProgressBtns, contProgressBtnRecs[i], deleteProgressBtnRecs[i]);
                        }
                    }
                    catch (Exception e)
                    {
                        //Set the error message and center it
                        readFileErrorMsg = "There is an error reading the saved profile." + scores[0] + " " + scores[1] + " " + scores[2] + " " + scores[3] + " " + times[4];
                        readFileErrorMsgLoc.X = (SCREEN_WIDTH - instrFont.MeasureString(readFileErrorMsg).X) / 2;

                        //Create a blank profile
                        users[i] = new User(i, newUserBtns, contProgressBtns, deleteProgressBtns, contProgressBtnRecs[i], deleteProgressBtnRecs[i], instrFont);
                    }
                }

                //Set the array of data as the next line split up by commas
                data = inFile.ReadLine().Split(',');

                //Set the high scores as each value in the array of data
                for (int j = 0; j < data.Length; j++)
                {
                    //Set each high score value
                    highScores[j] = Convert.ToInt32(data[j]);
                }

                //Set the array of names that achieved each high score as the next line split up by commas
                lBNames = inFile.ReadLine().Split(',');
            }
            catch (FileNotFoundException fnf)
            {
                //Set the eror message and center it
                readFileErrorMsg = "ERROR: File was not found.";
                readFileErrorMsgLoc.X = (SCREEN_WIDTH - instrFont.MeasureString(readFileErrorMsg).X) / 2;
            }
            catch (EndOfStreamException eos)
            {
                //Set the eror message and center it
                readFileErrorMsg = "ERROR: The game tried reading past the stats file.";
                readFileErrorMsgLoc.X = (SCREEN_WIDTH - instrFont.MeasureString(readFileErrorMsg).X) / 2;
            }
            catch (IndexOutOfRangeException ore)
            {
                //Set the eror message and center it
                readFileErrorMsg = "ERROR: The game tried reading past an array in the stats file.";
                readFileErrorMsgLoc.X = (SCREEN_WIDTH - instrFont.MeasureString(readFileErrorMsg).X) / 2;
            }
            catch (FormatException fe)
            {
                //Set the eror message and center it
                readFileErrorMsg = "ERROR: The game tried converting invalid data.";
                readFileErrorMsgLoc.X = (SCREEN_WIDTH - instrFont.MeasureString(readFileErrorMsg).X) / 2;
            }
            catch (Exception e)
            {
                //Set the error message to the default error
                readFileErrorMsg = "ERROR: " + e.Message;
                readFileErrorMsgLoc.X = (SCREEN_WIDTH - instrFont.MeasureString(readFileErrorMsg).X) / 2;
            }
            finally
            {
                //Close the file if it is not null
                if (inFile != null)
                {
                    //Close the stats file
                    inFile.Close();
                }
            }
        }

        //Pre: None
        //Post: none
        //Desc: Update the menu state
        private void UpdateMenu()
        {
            //Set the whether the buttons are hovered over
            hoverPlayBtn = playBtnRecs[NOT_HOVER].Contains(mouse.Position);
            hoverLeaderboardBtn = leaderBoardBtnRecs[NOT_HOVER].Contains(mouse.Position);
            hoverExitBtn = exitBtnRecs[NOT_HOVER].Contains(mouse.Position);
            hoverInstrBtn = instrBtnRecs[NOT_HOVER].Contains(mouse.Position);

            //Change the game state depending on the button clicked
            if (hoverPlayBtn && mouse.LeftButton == ButtonState.Pressed)
            {
                //Play the impact sound
                impactSnd.CreateInstance().Play();

                //Set the state to pregame
                gameState = PREGAME;
                pregameState = SELECT_USER;

                //Update the profiles by reading the profiles and scores
                ReadUserInfo();

            }
            else if (hoverLeaderboardBtn && mouse.LeftButton == ButtonState.Pressed)
            {
                //Play the impact sound
                impactSnd.CreateInstance().Play();

                //Set the state to leaderboard
                gameState = LEADERBOARD;
            }
            else if (hoverExitBtn && mouse.LeftButton == ButtonState.Pressed)
            {
                //Exit the program
                Exit();
            }
            else if (hoverInstrBtn && mouse.LeftButton == ButtonState.Pressed)
            {
                //Play the impact sound
                impactSnd.CreateInstance().Play();

                //Set the state to instructions
                gameState = INSTRUCTIONS;
            }
        }

        //Pre: None
        //Post: none
        //Desc: Update the pregame state
        private void UpdatePreGame()
        {
            //Update the pregame depending on its state
            switch (pregameState)
            {
                case SELECT_USER:
                    //Detect the selection or deletion of the 3 user profiles
                    for (int i = 0; i < MAX_USERS; i++)
                    {
                        //Create a new user or select the user depending on if the select button is clicked
                        if (users[i].SelectBtnIsClicked(mouse, prevMouse))
                        {
                            //Play the impact sound
                            impactSnd.CreateInstance().Play();

                            //Select the user if the profile had been created before
                            if (users[i].GetIsCreated())
                            {
                                //Set the current user as the clicked user
                                curUser = users[i];

                                //Change the game state to endgame if all levels are completed
                                if (curUser.GetLevel() > NUM_LEVELS)
                                {
                                    //Set the game state to endgame
                                    gameState = ENDGAME;
                                    SetupEndGame();
                                }
                                else
                                {
                                    //Change the game state to play and create a new level
                                    gameState = PLAY;
                                    CreateNewLevel(curUser.GetLevel(), curUser.GetHealth());
                                }
                            }
                            else
                            {
                                //Change the pregame state to create a new user, and set the current user to the selected user profile
                                pregameState = CREATE_NEW_USER;
                                curUser = users[i];
                                users[i].SetName("");
                                typeNameRec = new Rectangle(USER_CHOICE_RECS[i].X + SPACER_3, USER_CHOICE_RECS[i].Y + SPACER_3, USER_CHOICE_RECS[i].Width - 2 * SPACER_3, (int)instrFont.MeasureString("|").Y + SPACER_3);
                                users[i].SetIsCreated(true);
                            }
                        }

                        //Delete the user profile if the delete button is clicked
                        if (users[i].DeleteBtnIsClicked(mouse, prevMouse))
                        {
                            //Play the impact sound
                            impactSnd.CreateInstance().Play();

                            //Delete the user profile and save the stats
                            users[i].DeleteInfo();
                            SaveProfilesAndScores();
                        }
                    }
                    break;

                case CREATE_NEW_USER:
                    //Set the name of the current user based on the keys typed
                    curUser.SetName(kb, prevKb);

                    //Change the gamestate to play or the pregame state back to select user depending on the button clicked
                    if (curUser.SelectBtnIsClicked(mouse, prevMouse))
                    {
                        //Play the impact sound
                        impactSnd.CreateInstance().Play();

                        //Set the gamestate to play, save the scores, and create a new level
                        gameState = PLAY;
                        SaveProfilesAndScores();
                        CreateNewLevel(1, curUser.GetHealth());
                    }
                    else if (curUser.DeleteBtnIsClicked(mouse, prevMouse))
                    {
                        //Play the impact sound
                        impactSnd.CreateInstance().Play();

                        //Reset the current user's name, set the profile as not created, and change the pregame state to select user
                        curUser.SetName("New Player");
                        curUser.SetIsCreated(false);
                        pregameState = SELECT_USER;
                    }
                    break;
            }
        }

        //Pre: gameTime is a GameTime containing the elapsed time between updates
        //Post: none
        //Desc: Update the play state
        private void UpdatePlay(GameTime gameTime)
        {
            //Store the level status and update the game
            int levelStatus = level.UpdateAndLevelIsOver(gameTime, kb, prevKb);

            //Play music if the music is not playing
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.PlayPosition >= gameMusic.Duration)
            {
                //Play the game song
                MediaPlayer.Play(gameMusic);
            }

            //Change the gamestate if the game is over
            if (levelStatus == LEVEL_OVER_FAIL)
            {
                //Change the game state to level over (fail)
                gameState = LEVEL_OVER_FAIL;

                //Increase the number of zombies killed in the user's profile
                curUser.IncreaseZombiesKilled(level.GetZombiesKilled());

                //Set the stats messages to be shown in the level over (fail) state
                levelMsg = "Level " + level.GetLevelNum() + " Failed";
                scoreMsg = "Score: " + level.GetScore();
                totScoreMsg = "Total Score: " + curUser.GetScores().Sum();
                timeMsg = "Time: " + level.GetTimeDisplay();

                //Set the location of the stats messages to be shown in the level over (fail) state
                levelMsgLoc.X = SCREEN_WIDTH / 2 - titleFont.MeasureString(levelMsg).X / 2;
                scoreMsgLoc.X = SCREEN_WIDTH / 2 - instrFont.MeasureString(scoreMsg).X / 2;
                totScoreMsgLoc.X = SCREEN_WIDTH / 2 - instrFont.MeasureString(totScoreMsg).X / 2;
                timeMsgLoc.X = SCREEN_WIDTH / 2 - instrFont.MeasureString(timeMsg).X / 2;
                
                //If the score is higher than the high score, set the new high score and display the new high score message
                if (level.GetScore() > highScores[level.GetLevelNum() - 1])
                {
                    //Set the high score and the name that achieved that high score
                    highScores[level.GetLevelNum() - 1] = level.GetScore();
                    lBNames[level.GetLevelNum() - 1] = curUser.GetName();
                }

                SaveProfilesAndScores();
            }
            else if (levelStatus == LEVEL_OVER_WIN)
            {
                //Change the game state to level over (win)
                gameState = LEVEL_OVER_WIN;

                //Set the previous health of the user at the beginning of the game
                prevHealth = curUser.GetHealth();

                //Increase the number of zombies killed in the user's profile and set the user's level
                curUser.IncreaseZombiesKilled(level.GetZombiesKilled());
                curUser.SetLevel(level.GetLevelNum() + 1);
                curUser.SetHealth(level.GetHealth());

                //Set the user's score the the level if it was higher than a previous attempt
                if (curUser.GetScores()[level.GetLevelNum() - 1] < level.GetScore())
                {
                    //Set the user's score and time
                    curUser.SetScore(level.GetLevelNum(), level.GetScore());
                    curUser.SetTime(level.GetLevelNum(), level.GetTimeInSec());
                }

                //Set the stats messages to be shown in the level over (win) state
                levelMsg = "Level " + level.GetLevelNum() + " Passed";
                scoreMsg = "Score: " + level.GetScore();
                totScoreMsg = "Total Score: " + curUser.GetScores().Sum();
                timeMsg = "Time: " + level.GetTimeDisplay();

                //Set the locations of the stats messages to be shown in the level over (win) state
                levelMsgLoc.X = SCREEN_WIDTH / 2 - titleFont.MeasureString(levelMsg).X / 2;
                scoreMsgLoc.X = SCREEN_WIDTH / 2 - instrFont.MeasureString(scoreMsg).X / 2;
                totScoreMsgLoc.X = SCREEN_WIDTH / 2 - instrFont.MeasureString(totScoreMsg).X / 2;
                timeMsgLoc.X = SCREEN_WIDTH / 2 - instrFont.MeasureString(timeMsg).X / 2;

                //If the score is higher than the high score, set the new high score and display the new high score message
                if (level.GetScore() > highScores[level.GetLevelNum() - 1])
                {
                    //Set the location of the new high score on the screen
                    newLevelHsLoc.X = SCREEN_WIDTH / 2 - boldFont.MeasureString(newLevelHsMsg).X / 2;

                    //Set the high score and the name that achieved that high score
                    highScores[level.GetLevelNum() - 1] = level.GetScore();
                    lBNames[level.GetLevelNum() - 1] = curUser.GetName();
                }
                else
                {
                    //Set the location of the new high score message off the screen
                    newLevelHsLoc.X = SCREEN_WIDTH;
                }
                
                SaveProfilesAndScores();
            }
        }

        //Pre: None
        //Post: none
        //Desc: Update the level over (win) state
        private void UpdateLevelOverWin()
        {
            //Set whether the buttons are hovered over
            hoverMenuBtn = menuBtnRecs1[NOT_HOVER].Contains(mouse.Position);
            hoverTryAgainBtn = tryAgainBtnRecs[NOT_HOVER].Contains(mouse.Position);
            hoverNextRoomBtn = nextRoomBtnRecs[NOT_HOVER].Contains(mouse.Position);

            //Change the game state depending on the button clicked
            if (hoverMenuBtn && mouse.LeftButton == ButtonState.Pressed)
            {
                //Play the impact sound
                impactSnd.CreateInstance().Play();

                //Change the game state to menu
                gameState = MENU;
            }
            else if (hoverTryAgainBtn && mouse.LeftButton == ButtonState.Pressed)
            {
                //Play the impact sound
                impactSnd.CreateInstance().Play();

                //Set the level to the same level, and change the game state to play
                CreateNewLevel(curUser.GetLevel() - 1, prevHealth);
                gameState = PLAY;
            }
            else if (hoverNextRoomBtn && mouse.LeftButton == ButtonState.Pressed)
            {
                //Play the impact sound
                impactSnd.CreateInstance().Play();

                //Set the state to end game
                if (curUser.GetLevel() > NUM_LEVELS)
                {
                    //Setup the end game and change the game state
                    gameState = ENDGAME;
                    SetupEndGame();
                }
                else
                {
                    //Change the gane state to play and create a new level
                    gameState = PLAY;
                    CreateNewLevel(curUser.GetLevel(), curUser.GetHealth());
                }
            }
        }

        //Pre: None
        //Post: none
        //Desc: Update the level over (fail) state
        private void UpdateLevelOverFail()
        {
            //Set whether the buttons are hovered over
            hoverMenuBtn = menuBtnRecs1[NOT_HOVER].Contains(mouse.Position);
            hoverTryAgainBtn = tryAgainBtnRecs[NOT_HOVER].Contains(mouse.Position);

            //Change the game state depending on the button clicked
            if (hoverMenuBtn && mouse.LeftButton == ButtonState.Pressed)
            {
                //Play the impact sound
                impactSnd.CreateInstance().Play();

                //Change the game state to menu
                gameState = MENU;
            }
            else if (hoverTryAgainBtn && mouse.LeftButton == ButtonState.Pressed)
            {
                //Play the impact sound
                impactSnd.CreateInstance().Play();

                //Change the game state to play and create a new level for the same level number
                gameState = PLAY;
                CreateNewLevel(level.GetLevelNum(), curUser.GetHealth());
            }
        }

        //Pre: None
        //Post: none
        //Desc: Update the endgame state
        private void UpdateEndGame()
        {
            //Set the hovering over the menu button
            hoverMenuBtn = menuBtnRecs2[NOT_HOVER].Contains(mouse.Position);

            //Change the game state depending if the button is clicked
            if (hoverMenuBtn && mouse.LeftButton == ButtonState.Pressed)
            {
                //Play the impact sound
                impactSnd.CreateInstance().Play();

                //Change the game state to menu
                gameState = MENU;
            }
        }

        //Pre: None
        //Post: none
        //Desc: Update the leaderboard state
        private void UpdateLeaderboard()
        {
            //Set the hovering over the menu button
            hoverMenuBtn = menuBtnRecs2[NOT_HOVER].Contains(mouse.Position);

            //Change the game state depending if the button is clicked
            if (hoverMenuBtn && mouse.LeftButton == ButtonState.Pressed)
            {
                //Play the impact sound
                impactSnd.CreateInstance().Play();

                //Change the game state to menu
                gameState = MENU;
            }
        }

        //Pre: None
        //Post: none
        //Desc: Update the instructions state
        private void UpdateInstructions()
        {
            //Set the hovering over the menu button
            hoverMenuBtn = menuBtnRecs2[NOT_HOVER].Contains(mouse.Position);

            //Change the game state depending if the button is clicked
            if (hoverMenuBtn && mouse.LeftButton == ButtonState.Pressed)
            {
                //Play the impact sound
                impactSnd.CreateInstance().Play();

                //Change the game state to menu
                gameState = MENU;
            }
        }

        //Pre: None
        //Post: none
        //Desc: Draw the menu state
        private void DrawMenu()
        {
            //Draw the background and the title
            spriteBatch.Draw(menuBg, fullScreenRec, Color.White);
            spriteBatch.DrawString(titleFont, title, titleLoc, Color.White);

            //Draw the buttons
            spriteBatch.Draw(instrBtns[Convert.ToInt32(hoverInstrBtn)], instrBtnRecs[Convert.ToInt32(hoverInstrBtn)], Color.White);
            spriteBatch.Draw(playBtns[Convert.ToInt32(hoverPlayBtn)], playBtnRecs[Convert.ToInt32(hoverPlayBtn)], Color.White);
            spriteBatch.Draw(leaderboardBtns[Convert.ToInt32(hoverLeaderboardBtn)], leaderBoardBtnRecs[Convert.ToInt32(hoverLeaderboardBtn)], Color.White);
            spriteBatch.Draw(exitBtns[Convert.ToInt32(hoverExitBtn)], exitBtnRecs[Convert.ToInt32(hoverExitBtn)], Color.White);
            spriteBatch.DrawString(instrFont, readFileErrorMsg, readFileErrorMsgLoc, Color.White);
        }

        //Pre: None
        //Post: none
        //Desc: Draw the pregame state
        private void DrawPreGame()
        {
            //Draw the background
            spriteBatch.Draw(menuBg, fullScreenRec, Color.White);

            //Draw the boxes for each user profile
            spriteBatch.Draw(pixelImg, USER_CHOICE_RECS[0], Color.White);
            spriteBatch.Draw(pixelImg, USER_CHOICE_RECS[1], Color.White);
            spriteBatch.Draw(pixelImg, USER_CHOICE_RECS[2], Color.White);

            //Draw the display in each user profile box depending on the pregame state
            switch (pregameState)
            {
                case SELECT_USER:
                    //Draw all the users' profiles
                    for (int i = 0; i < MAX_USERS; i++)
                    {
                        //Draw the profile of the user
                        users[i].DrawInfo(spriteBatch);
                    }
                    break;

                case CREATE_NEW_USER:
                    //Only draw the profile of the current user selected and the typing bar
                    spriteBatch.Draw(pixelImg, typeNameRec, Color.LightGray);
                    curUser.DrawInfo(spriteBatch);
                    break;
            }

            //Draw the error message
            spriteBatch.DrawString(instrFont, readFileErrorMsg, readFileErrorMsgLoc, Color.White);
        }

        //Pre: None
        //Post: none
        //Desc: Draw the play state
        private void DrawPlay()
        {
            //Draw the level
            level.Draw(spriteBatch);
        }

        //Pre: None
        //Post: none
        //Desc: Draw the level over (fail) state
        private void DrawLevelOverFail()
        {
            //Draw the frame image
            spriteBatch.Draw(frameImg, frameRec, Color.White);

            //Draw the level, time, score, and total score messages
            spriteBatch.DrawString(titleFont, levelMsg, levelMsgLoc, Color.Black);
            spriteBatch.DrawString(instrFont, timeMsg, timeMsgLoc, Color.Black);
            spriteBatch.DrawString(instrFont, scoreMsg, scoreMsgLoc, Color.Black);
            spriteBatch.DrawString(instrFont, totScoreMsg, totScoreMsgLoc, Color.Black);

            //Draw the buttons
            spriteBatch.Draw(menuBtns[Convert.ToInt32(hoverMenuBtn)], menuBtnRecs1[Convert.ToInt32(hoverMenuBtn)], Color.White);
            spriteBatch.Draw(tryAgainBtns[Convert.ToInt32(hoverTryAgainBtn)], tryAgainBtnRecs[Convert.ToInt32(hoverTryAgainBtn)], Color.White);
        }

        //Pre: None
        //Post: none
        //Desc: Draw the level over (win) state
        private void DrawLevelOverWin()
        {
            //Draw the background image
            spriteBatch.Draw(corridorImg, fullScreenRec, Color.White);

            //Draw the level, time, score, and total score messages
            spriteBatch.DrawString(titleFont, levelMsg, levelMsgLoc, Color.White);
            spriteBatch.DrawString(instrFont, timeMsg, timeMsgLoc, Color.White);
            spriteBatch.DrawString(instrFont, scoreMsg, scoreMsgLoc, Color.White);
            spriteBatch.DrawString(instrFont, totScoreMsg, totScoreMsgLoc, Color.White);

            //Draw the buttons
            spriteBatch.Draw(menuBtns[Convert.ToInt32(hoverMenuBtn)], menuBtnRecs1[Convert.ToInt32(hoverMenuBtn)], Color.White);
            spriteBatch.Draw(tryAgainBtns[Convert.ToInt32(hoverTryAgainBtn)], tryAgainBtnRecs[Convert.ToInt32(hoverTryAgainBtn)], Color.White);

            //Draw the next or next room button depending on the level
            if (level.GetLevelNum() == NUM_LEVELS)
            {
                //Draw the next button
                spriteBatch.Draw(nextBtns[Convert.ToInt32(hoverNextRoomBtn)], nextRoomBtnRecs[Convert.ToInt32(hoverNextRoomBtn)], Color.White);
            }
            else
            {
                //Draw the next room button
                spriteBatch.Draw(nextRoomBtns[Convert.ToInt32(hoverNextRoomBtn)], nextRoomBtnRecs[Convert.ToInt32(hoverNextRoomBtn)], Color.White);
            }

            //Draw the new high score message
            spriteBatch.DrawString(boldFont, newLevelHsMsg, newLevelHsLoc, Color.White);
        }

        //Pre: None
        //Post: none
        //Desc: Draw the endgame state
        private void DrawEndGame()
        {
            //Draw the background image and the title
            spriteBatch.Draw(endGameBg, fullScreenRec, Color.White);
            spriteBatch.DrawString(titleFont, finalEscapeMsg, finalEscapeMsgLoc, Color.White);

            //Draw the score and time messages for each level
            for (int i = 0; i < NUM_LEVELS; i++)
            {
                //Draw the score and time message
                spriteBatch.DrawString(instrFont, eGScoreMsgs[i], scoreMsgLocs[i], Color.White);
                spriteBatch.DrawString(instrFont, eGTimeMsgs[i], eGTimeMsgLocs[i], Color.White);
            }

            //Draw the final score and the zombies killed messages
            spriteBatch.DrawString(instrFont, eGFinalScoreMsg, eGTotScoreMsgLoc, Color.White);
            spriteBatch.DrawString(instrFont, eGZombiesKilledMsg, eGZombiesKilledMsgLoc, Color.White);

            //Draw the menu button
            spriteBatch.Draw(menuBtns[Convert.ToInt32(hoverMenuBtn)], menuBtnRecs2[Convert.ToInt32(hoverMenuBtn)], Color.White);
        }

        //Pre: None
        //Post: none
        //Desc: Draw the leaderboard state
        private void DrawLeaderboard()
        {
            //Draw the background image and the title
            spriteBatch.Draw(leaderboardBg, fullScreenRec, Color.White);
            spriteBatch.DrawString(titleFont, lBTitle, lBTitleLoc, Color.White);

            //Draw the score messages for each level high score
            for (int i = 0; i < NUM_LEVELS; i++)
            {
                //Draw the score label, the high score, and the name
                spriteBatch.DrawString(instrFont, lBLevelMsgs[i], lbLabelLocs[i], Color.White);
                spriteBatch.DrawString(instrFont, highScores[i] + "", lBScoreMsgLocs[i], Color.White);
                spriteBatch.DrawString(instrFont, lBNames[i], lBNameLocs[i], Color.White);
            }

            //Draw the menu button
            spriteBatch.Draw(menuBtns[Convert.ToInt32(hoverMenuBtn)], menuBtnRecs2[Convert.ToInt32(hoverMenuBtn)], Color.White);

            //Draw the save error message
            spriteBatch.DrawString(instrFont, saveStatsErrorMsg, saveErrorMsgLoc, Color.White);
        }

        //Pre: None
        //Post: none
        //Desc: Draw the instructions state
        private void DrawInstructions()
        {
            //Draw the istructions title
            spriteBatch.DrawString(titleFont, instructionsTitle, lBTitleLoc, Color.White);

            //Draw each line of instructions
            for (int i = 0; i < instructionMsgs.Length; i++)
            {
                //Draw the line of instructions
                spriteBatch.DrawString(instrFont, instructionMsgs[i], instrMsgLocs[i], Color.White);
            }

            //Draw the menu button
            spriteBatch.Draw(menuBtns[Convert.ToInt32(hoverMenuBtn)], menuBtnRecs2[Convert.ToInt32(hoverMenuBtn)], Color.White);
        }

        //Pre: None
        //Post: none
        //Desc: Set the messages to be shown in the end game screen
        private void SetupEndGame()
        {
            //Set the score and time messages for each level
            for (int j = 0; j < NUM_LEVELS; j++)
            {
                //Set the score and time messages
                eGScoreMsgs[j] = "Level " + (j + 1) + " Score: " + curUser.GetScores()[j];
                eGTimeMsgs[j] = "Level " + (j + 1) + " Time: " + curUser.GetTimes()[j];
            }

            //Set the zombies killed and the total score message
            eGZombiesKilledMsg = "Total Zombies Killed: " + curUser.GetZombiesKilled();
            eGFinalScoreMsg = "Final Score: " + curUser.GetScores().Sum();
        }


        //Pre: None
        //Post: none
        //Desc: Set the messages to be shown in the end game screen
        private void CreateNewLevel(int levelNum, int health)
        {
            //Set the unlock animation as not animating
            unlockAnim.isAnimating = false;

            //Create the new level
            level = new Level(levelNum, health, carpetBgs[levelNum - 1], itemImgs, regZombieImgs, smartZombieImgs, ghostImgs, playerImgs, bulletImgs, pixelImg, instrFont, unlockAnim, regZombieLocs[levelNum - 1], smartZombieLocs[levelNum - 1], shootSnd, impactSnd);
        }
    }
}

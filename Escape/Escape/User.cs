//Author: Victoria Mak
//File Name: User.cs
//Project Name: Escape
//Creation Date: May 9, 2023
//Modified Date: June 12, 2023
//Description: User is a class that stores the user information on their progress in the game so that they can continue their level after exiting the program.

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Animation2D;
using Helper;
using Microsoft.Xna.Framework.Audio;

namespace Escape
{
    class User
    {
        //Store the maximum length of a name
        private const int MAX_NAME_LENGTH = 15;

        //Store the position number of the user
        private int position;

        //Store the level, times, scores, zombies killed, and health of the user
        private int level;
        private double[] times;
        private int[] scores;
        private int zombiesKilled;
        private int health;

        //Store the name and messages to display the stats in the pregame state
        private string name;
        private string totalTimeMsg;
        private string levelMsg;
        private string scoreMsg;
        private string zombiesKilledMsg;
        private string healthMsg;

        //Store the font
        private SpriteFont font;

        //Store the location of the stat messages in the pregame state
        private Vector2 nameLoc;
        private Vector2 totalTimeLoc;
        private Vector2 levelLoc;
        private Vector2 scoreLoc;
        private Vector2 zombiesKilledLoc;
        private Vector2 healthLoc;

        //Store the button images
        private Texture2D[] contProgressBtns;
        private Texture2D[] newUserBtns;
        private Texture2D[] deleteProgressBtns;

        //Store the button rectangles
        private Rectangle[] selectBtnRecs;
        private Rectangle[] deleteProgressBtnRecs;

        //store the hovering over each type of button
        private bool hoverDeleteBtn;
        private bool hoverSelectBtn;

        //Store whether the profile is created
        private bool isCreated;

        public User(int position, Texture2D[] newUserBtns, Texture2D[] contProgressBtns, Texture2D[] deleteProgressBtns, Rectangle[] newUserBtnRecs, Rectangle[] deleteProgressBtnRecs, SpriteFont font)
        {
            //Set the profile as not created and the name as the default
            isCreated = false;
            name = "New Player";

            //Set the position and font
            this.position = position;
            this.font = font;

            //Set the stats
            level = 1;
            scores = new int[] { 0, 0, 0, 0, 0 };
            zombiesKilled = 0;
            times = new double[] { 0, 0, 0, 0, 0 };
            health = Inventory.MAX_HEALTH;

            //Set the button images
            this.newUserBtns = newUserBtns;
            this.contProgressBtns = contProgressBtns;
            this.deleteProgressBtns = deleteProgressBtns;

            //Set the button rectangles
            selectBtnRecs = newUserBtnRecs;
            this.deleteProgressBtnRecs = deleteProgressBtnRecs;

            //Set the stat messages
            totalTimeMsg = "Total Time: 0";
            levelMsg = "Level: 1";
            scoreMsg = "Total Score: 0";
            zombiesKilledMsg = "Zombies Killed: 0";
            healthMsg = "Health: " + Inventory.MAX_HEALTH;

            //Set the locations for the stat messages
            nameLoc = new Vector2(Game1.USER_CHOICE_RECS[position].Center.X - font.MeasureString(name).X / 2, Game1.USER_CHOICE_RECS[position].Y + Game1.SPACER_3);
            levelLoc = new Vector2(Game1.USER_CHOICE_RECS[position].X + Game1.SPACER_2, nameLoc.Y + font.MeasureString(name).Y + Game1.SPACER_3);
            totalTimeLoc = new Vector2(Game1.USER_CHOICE_RECS[position].X + Game1.SPACER_2, levelLoc.Y + font.MeasureString(name).Y + Game1.SPACER_3);
            scoreLoc = new Vector2(Game1.USER_CHOICE_RECS[position].X + Game1.SPACER_2, totalTimeLoc.Y + font.MeasureString(name).Y + Game1.SPACER_3);
            zombiesKilledLoc = new Vector2(Game1.USER_CHOICE_RECS[position].X + Game1.SPACER_2, scoreLoc.Y + font.MeasureString(name).Y + Game1.SPACER_3);
            healthLoc = new Vector2(Game1.USER_CHOICE_RECS[position].X + Game1.SPACER_2, zombiesKilledLoc.Y + font.MeasureString(name).Y + Game1.SPACER_3);
        }


        public User(string name, int position, SpriteFont font, int level, int health, double[] times, int[] scores, int zombiesKilled, Texture2D[] newUserBtns, Texture2D[] contProgressBtns, Texture2D[] deleteProgressBtns, Rectangle[] contProgressBtnRecs, Rectangle[] deleteProgressBtnRecs)
        {
            //Set the profile as created
            isCreated = true;

            //Set the level, position, font, name, and stats
            this.level = level;
            this.position = position;
            this.font = font;
            this.name = name;
            this.times = times;
            this.scores = scores;
            this.zombiesKilled = zombiesKilled;
            this.health = health;

            //Set the button images and rectangles
            this.newUserBtns = newUserBtns;
            this.contProgressBtns = contProgressBtns;
            this.deleteProgressBtns = deleteProgressBtns;
            selectBtnRecs = contProgressBtnRecs;
            this.deleteProgressBtnRecs = deleteProgressBtnRecs;

            //Set the stat messages
            totalTimeMsg = "Total Time: " + Math.Round(times.Sum());
            scoreMsg = "Total Score: " + scores.Sum();
            zombiesKilledMsg = "Zombies Killed: " + zombiesKilled;
            healthMsg = "Health: " + health;

            //Set the level message depending on if all levels are completed
            if (level > Game1.NUM_LEVELS)
            {
                //Set the level message as all levels completed
                levelMsg = "All levels completed";
            }
            else
            {
                //Set the level message to the current level
                levelMsg = "Level: " + level;
            }

            //Set the location of the stat messages
            nameLoc = new Vector2(Game1.USER_CHOICE_RECS[position].Center.X - font.MeasureString(name).X / 2, Game1.USER_CHOICE_RECS[position].Y + Game1.SPACER_3);
            levelLoc = new Vector2(Game1.USER_CHOICE_RECS[position].X + Game1.SPACER_2, nameLoc.Y + font.MeasureString(name).Y + Game1.SPACER_3);
            totalTimeLoc = new Vector2(Game1.USER_CHOICE_RECS[position].X + Game1.SPACER_2, levelLoc.Y + font.MeasureString(name).Y + Game1.SPACER_3);
            scoreLoc = new Vector2(Game1.USER_CHOICE_RECS[position].X + Game1.SPACER_2, totalTimeLoc.Y + font.MeasureString(name).Y + Game1.SPACER_3);
            zombiesKilledLoc = new Vector2(Game1.USER_CHOICE_RECS[position].X + Game1.SPACER_2, scoreLoc.Y + font.MeasureString(name).Y + Game1.SPACER_3);
            healthLoc = new Vector2(Game1.USER_CHOICE_RECS[position].X + Game1.SPACER_2, zombiesKilledLoc.Y + font.MeasureString(name).Y + Game1.SPACER_3);
        }

        //Pre: None
        //Post: Returns a positive integer, level
        //Desc: Returns the level of the user
        public int GetLevel()
        {
            //Return the level number
            return level;
        }

        //Pre: None
        //Post: Returns a string less than 15 characters long
        //Desc: Returns the name
        public string GetName()
        {
            //Return the name
            return name;
        }

        //Pre: None
        //Post: Returns a bool for whether the profile is created
        //Desc: Returns whether the profile is created
        public bool GetIsCreated()
        {
            //Return whether the profile is created
            return isCreated;
        }

        //Pre: isCreated is a bool for whether the profile has been created
        //Post: None
        //Desc: Sets whether the profile is created
        public void SetIsCreated(bool isCreated)
        {
            //Set whether the profile is created
            this.isCreated = isCreated;
        }

        //Pre: health is an integer between 0 and 10 inclusive
        //Post: None
        //Desc: Sets the health of the user
        public void SetHealth(int health)
        {
            //Set the health
            this.health = health;
        }

        //Pre: level is an integer from 1 to 1 more than the max level
        //Post: None
        //Desc: Sets the level of the user
        public void SetLevel(int level)
        {
            //Set the level
            this.level = level;
        }

        //Pre: level is an integer from 1 to the max level and score is a positive integer
        //Post: None
        //Desc: Sets the score at an index representing the level
        public void SetScore(int level, int score)
        {
            //Set the score at the level's index
            scores[level - 1] = score;
        }

        //Pre: level is an integer from 1 to the max level and time is a positive double
        //Post: None
        //Desc: Sets the time at an index representing the level
        public void SetTime(int level, double time)
        {
            //Set the time at the level's index
            times[level - 1] = time;
        }

        //Pre: name is a string
        //Post: None
        //Desc: Sets the name to the specified name
        public void SetName(string name)
        {
            //Set the name
            this.name = name;
        }

        //Pre: kb is the current keyboard state and prevKb is the preivous keyboard state from the previous update
        //Post: None
        //Desc: Sets the name by adding/removing a letter in the name as the user types
        public void SetName(KeyboardState kb, KeyboardState prevKb)
        {
            //Add or remove a letter based on each key pressed
            foreach (Keys i in kb.GetPressedKeys())
            {
                //Only change the name if the key was not pressed in the previous update
                if (prevKb.IsKeyUp(i))
                {
                    //Remove the last letter or add another letter in the name depending on the key pressed
                    if (i == Keys.Back)
                    {
                        //Remove the letter if the name has at least 1 letter
                        if (name.Length > 0)
                        {
                            //Remove the last letter 
                            name = name.Remove(name.Length - 1);
                        }
                    }
                    else if (name.Length < MAX_NAME_LENGTH)
                    {
                        //Add a letter depending on the key pressed
                        switch (i)
                        {
                            case Keys.A:
                                //Add an A
                                name += "A";
                                break;

                            case Keys.B:
                                //Add a B
                                name += "B";
                                break;

                            case Keys.C:
                                //Add a C
                                name += "C";
                                break;

                            case Keys.D:
                                //Add a D
                                name += "D";
                                break;

                            case Keys.E:
                                //Add a E
                                name += "E";
                                break;

                            case Keys.F:
                                //Add a F
                                name += "F";
                                break;

                            case Keys.G:
                                //Add a G
                                name += "G";
                                break;

                            case Keys.H:
                                //Add a H
                                name += "H";
                                break;

                            case Keys.I:
                                //Add a I
                                name += "I";
                                break;

                            case Keys.J:
                                //Add a J
                                name += "J";
                                break;

                            case Keys.K:
                                //Add A K
                                name += "K";
                                break;

                            case Keys.L:
                                //Add a L
                                name += "L";
                                break;

                            case Keys.M:
                                //Add a M
                                name += "M";
                                break;

                            case Keys.N:
                                //Add a N
                                name += "N";
                                break;

                            case Keys.O:
                                //Add a O
                                name += "O";
                                break;

                            case Keys.P:
                                //Add a P
                                name += "P";
                                break;

                            case Keys.Q:
                                //Add a Q
                                name += "Q";
                                break;

                            case Keys.R:
                                //Add a R
                                name += "R";
                                break;

                            case Keys.S:
                                //Add a S
                                name += "S";
                                break;

                            case Keys.T:
                                //Add a T
                                name += "T";
                                break;

                            case Keys.U:
                                //Add a U
                                name += "U";
                                break;

                            case Keys.V:
                                //Add a V
                                name += "V";
                                break;

                            case Keys.W:
                                //Add a W
                                name += "W";
                                break;

                            case Keys.X:
                                //Add a X
                                name += "X";
                                break;

                            case Keys.Y:
                                //Add a Y
                                name += "Y";
                                break;

                            case Keys.Z:
                                //Add a Z
                                name += "Z";
                                break;

                            default:
                                break;
                        }
                    }

                    //Recenter the location of the name
                    nameLoc.X = Game1.USER_CHOICE_RECS[position].Center.X - font.MeasureString(name).X / 2;
                }
            }
        }

        //Pre: None
        //Post: Returns an array of doubles for the times spent in each level
        //Desc: Gets the times used for each level
        public double[] GetTimes()
        {
            //Return the time for the level
            return times;
        }

        //Pre: None
        //Post: Returns an array of scores that are positive integers
        //Desc: Gets the array of scores for the user
        public int[] GetScores()
        {
            //Return the array of scores
            return scores;
        }

        //Pre: None
        //Post: returns an integer between 0 and the max health inclusive
        //Desc: Gets the health of the user
        public int GetHealth()
        {
            //Return the health
            return health;
        }

        //Pre: None
        //Post: returns a positive integer or 0 representing the number of zombies killed
        //Desc: Gets the total number of zombies killed by the player
        public int GetZombiesKilled()
        {
            //Return the zombies killed
            return zombiesKilled;
        }

        //Pre: increase is an integer representing the additional number of zombies killed 
        //Post: none
        //Desc: Increases the number of zombies killed
        public void IncreaseZombiesKilled(int increase)
        {
            //Increase the number of zombies killed by an increase
            zombiesKilled += increase;
        }

        //Pre: None
        //Post: None
        //Desc: Resets the user profile to an uncreated profile
        public void DeleteInfo()
        {
            //Set the name and reset the stats
            name = "New Player";
            scores = new int[] { 0, 0, 0, 0, 0 };
            level = 1;
            times = new double[] { 0, 0, 0, 0, 0 };
            zombiesKilled = 0;
            health = Inventory.MAX_HEALTH;

            //Set the stat messages
            totalTimeMsg = "Total Time: 0";
            levelMsg = "Level: 1";
            scoreMsg = "Score: 0";
            zombiesKilledMsg = "Zombies Killed: 0";
            healthMsg = "Health: " + Inventory.MAX_HEALTH;

            //Set the profile as uncreated
            isCreated = false;
        }

        //Pre: mouse is the current mouse state and prevMouse is the previous mouse state in the previous update
        //Post: returns a bool for wether the delete button is clicked
        //Desc: Sets whether the delete button is hovered over and returns whether the button is clicked
        public bool DeleteBtnIsClicked(MouseState mouse, MouseState prevMouse)
        {
            //Sets whether the delete button is hovered over and returns whether it is clicked
            hoverDeleteBtn = deleteProgressBtnRecs[Game1.NOT_HOVER].Contains(mouse.Position);
            return hoverDeleteBtn && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released;
        }

        //Pre: mouse is the current mouse state and prevMouse is the previous mouse state in the previous update
        //Post: returns a bool for wether the select button is clicked
        //Desc: Sets whether the select button is hovered over and returns whether the button is clicked
        public bool SelectBtnIsClicked(MouseState mouse, MouseState prevMouse)
        {
            //Sets whether the select button is hovered over and returns whether the button is clicked
            hoverSelectBtn = selectBtnRecs[Game1.NOT_HOVER].Contains(mouse.Position);
            return hoverSelectBtn && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released;
        }

        //Pre: spriteBatch is the SpriteBatch for drawing the game graphics
        //Post: none
        //Desc: Draws the user's profile including the buttons
        public void DrawInfo(SpriteBatch spriteBatch)
        {
            //Draw the stat messages
            spriteBatch.DrawString(font, name, nameLoc, Color.Black);
            spriteBatch.DrawString(font, scoreMsg, scoreLoc, Color.Black);
            spriteBatch.DrawString(font, totalTimeMsg, totalTimeLoc, Color.Black);
            spriteBatch.DrawString(font, healthMsg, healthLoc, Color.Black);
            spriteBatch.DrawString(font, zombiesKilledMsg, zombiesKilledLoc, Color.Black);
            spriteBatch.DrawString(font, levelMsg, levelLoc, Color.Black);

            //Draw the continue progress or new user button depending on if the profile is created
            if (isCreated)
            {
                //Draw the continue progress button and the delete button
                spriteBatch.Draw(contProgressBtns[Convert.ToInt32(hoverSelectBtn)], selectBtnRecs[Convert.ToInt32(hoverSelectBtn)], Color.White);
                spriteBatch.Draw(deleteProgressBtns[Convert.ToInt32(hoverDeleteBtn)], deleteProgressBtnRecs[Convert.ToInt32(hoverDeleteBtn)], Color.White);
            }
            else
            {
                //Draw the new user button
                spriteBatch.Draw(newUserBtns[Convert.ToInt32(hoverSelectBtn)], selectBtnRecs[Convert.ToInt32(hoverSelectBtn)], Color.White);
            }
        }
    }
}

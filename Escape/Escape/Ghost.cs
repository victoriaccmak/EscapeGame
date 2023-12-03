//Author: Victoria Mak
//File Name: Ghost.cs
//Project Name: Escape
//Creation Date: May 9, 2023
//Modified Date: June 12, 2023
//Description: Ghost is a subclass of the Monster class. It can pass through walls and does not get hurt from bullets but still harms the player.

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Animation2D;
using Helper;

namespace Escape
{
    class Ghost : Monster
    {
        //Store the target node
        private Node targetNode;

        //Store the random number generator
        private Random rng = new Random();

        public Ghost(Texture2D[] imgs, Node curNode, Node[,] nodeMap) : base(curNode, 60f, 1, 1000, 0.5f, imgs[Game1.DOWN])
        {
            //Set the ghost animation
            anims[Game1.UP] = new Animation(imgs[Game1.UP], 6, 1, 6, 0, 0, Animation.ANIMATE_FOREVER, 12, pos, 1f, true);
            anims[Game1.DOWN] = new Animation(imgs[Game1.DOWN], 6, 1, 6, 0, 0, Animation.ANIMATE_FOREVER, 12, pos, 1f, true);
            anims[Game1.LEFT] = new Animation(imgs[Game1.LEFT], 6, 1, 6, 0, 0, Animation.ANIMATE_FOREVER, 12, pos, 1f, true);
            anims[Game1.RIGHT] = new Animation(imgs[Game1.RIGHT], 6, 1, 6, 0, 0, Animation.ANIMATE_FOREVER, 12, pos, 1f, true);


            //Randomize the next target node
            RandomizeNextNode(nodeMap);
        }

        //Pre: nodeMap is a 2D array of the nodes in the game
        //Post: None
        //Desc: Randomizes the next node for the ghost to go to
        private void RandomizeNextNode(Node[,] nodeMap)
        {
            //Randomize the target node if the current node is the target node
            while (curNode == targetNode || targetNode == null)
            {
                //Randomize the direction
                dir = rng.Next(0, 4);

                //Randomize the next target node based on the direction
                switch (dir)
                {
                    case Game1.UP:
                        //Set the target node above or at the current node
                        targetNode = nodeMap[new Random().Next(0, curNode.GetRow()), curNode.GetCol()];
                        break;

                    case Game1.DOWN:
                        //Set the target node below or at the current node
                        targetNode = nodeMap[new Random().Next(curNode.GetRow(), Game1.NUM_ROWS), curNode.GetCol()];
                        break;

                    case Game1.LEFT:
                        //Set the target node left or at the current node
                        targetNode = nodeMap[curNode.GetRow(), new Random().Next(0, curNode.GetCol())];
                        break;

                    case Game1.RIGHT:
                        //Set the target node right or at the current node
                        targetNode = nodeMap[curNode.GetRow(), new Random().Next(curNode.GetCol(), Game1.NUM_COLS)];
                        break;
                }
            }
        }

        //Pre: gameTime is the GameTime for the game, player is the Player, and nodeMap is the map of all the nodes in the world
        //Post: None
        //Desc: Updates the ghost
        public override void Update(GameTime gameTime, Node[,] nodeMap, Player player)
        {
            //Update the animation
            anims[dir].Update(gameTime);

            //Randomize the next target node if it is already at the target node
            if (curNode == targetNode)
            {
                //Randomize the next node
                RandomizeNextNode(nodeMap);
            }

            //Attack the player if possible and walk
            AttackIfPossible(player, gameTime);
            Walk(gameTime);
        }

        //Pre: gameTime is the GameTime for the game
        //Post: None
        //Desc: Moves the ghost toward its direction
        protected override void Walk(GameTime gameTime)
        {
            switch (dir)
            {
                case Game1.UP:
                    //Move the ghost up
                    pos.Y = Math.Max(0, pos.Y - (float)(maxSpeed * gameTime.ElapsedGameTime.TotalSeconds));
                    pos.X = curNode.GetRec().X;
                    break;

                case Game1.DOWN:
                    //Move the ghost down
                    pos.Y = Math.Min(Game1.SCREEN_HEIGHT - Game1.STATS_BAR_HEIGHT - GetRec().Height, pos.Y + (float)(maxSpeed * gameTime.ElapsedGameTime.TotalSeconds));
                    pos.X = curNode.GetRec().X;
                    break;

                case Game1.LEFT:
                    //Move the ghost left
                    pos.X = Math.Max(0, pos.X - (float)(maxSpeed * gameTime.ElapsedGameTime.TotalSeconds));
                    pos.Y = curNode.GetRec().Bottom - GetRec().Height;
                    break;

                case Game1.RIGHT:
                    //Move the ghost right
                    pos.X = Math.Min(Game1.SCREEN_WIDTH - GetRec().Width, pos.X + (float)(maxSpeed * gameTime.ElapsedGameTime.TotalSeconds));
                    pos.Y = curNode.GetRec().Bottom - GetRec().Height;
                    break;
            }

            //Moves the animations
            anims[Game1.LEFT].destRec.Location = pos.ToPoint();
            anims[Game1.RIGHT].destRec.Location = pos.ToPoint();
            anims[Game1.UP].destRec.Location = pos.ToPoint();
            anims[Game1.DOWN].destRec.Location = pos.ToPoint();
        }
    }
}

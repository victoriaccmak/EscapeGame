//Author: Victoria Mak
//File Name: SmartZombie.cs
//Project Name: Escape
//Creation Date: May 9, 2023
//Modified Date: June 12, 2023
//Description: SmartZombie is a subclass of Zombie. It is a monster that makes its appearance by jump-scaring the player and chasing the player until it has no path.

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

namespace Escape
{
    class SmartZombie : Zombie
    {
        //store the jump scare radius
        private const int JUMP_SCARE_RADIUS = 4;

        public SmartZombie(Texture2D[] walkImgs, Node curNode) : base(walkImgs, curNode, HIDING, 5, 3, 40, 80f)
        {
        }

        //Pre: gameTime is the GameTime for the game, player is the Player, and nodeMap is the map of all the nodes in the game
        //Post: None
        //Desc: Updates the zombie
        public override void Update(GameTime gameTime, Node[,] nodeMap, Player player)
        {
            //Pick up a heart if possible
            PickupHeart();

            //Update the zombie based on its state
            switch (state)
            {
                case HIDING:
                    //Update the hiding state
                    UpdateHiding(player, nodeMap);
                    break;

                case WANDERING:
                    //Determine the visibility and update the visibility
                    DetermineVisibility(player, nodeMap);
                    UpdateVisibility();

                    //Update the wandering state and find a path to the player
                    Wander(gameTime);
                    chasingPath = FindPath(nodeMap, player.GetCurNode());

                    //Chase the player if there is a path
                    if (chasingPath.Count > 0)
                    {
                        //Change the state to chasing
                        state = CHASING;
                    }
                    break;

                case CHASING:
                    //Determine the visibility, update the visibility, and update the chasing state
                    DetermineVisibility(player, nodeMap);
                    UpdateVisibility();
                    UpdateChasing(gameTime, player);

                    //Change the state to wandering if there is no path to the player
                    if (chasingPath.Count == 0)
                    {
                        //Change the state to wandering
                        state = WANDERING;
                    }
                    break;
            }

            //Attack the player if possible
            AttackIfPossible(player, gameTime);
        }

        //Pre: player is the Player, and nodeMap is the map of all the nodes in the game
        //Post: None
        //Desc: Updates the zombie
        private void UpdateHiding(Player player, Node[,] nodeMap)
        {
            //Only update the hiding state if the player's node is not null
            if (player.GetCurNode() != null)
            {
                //Only determine if the zombie is visible if the player is on the same row or column as the zombie within the jump scare radius
                if (player.GetCurNode().GetRow() == curNode.GetRow() && Math.Abs(player.GetCurNode().GetCol() - curNode.GetCol()) <= JUMP_SCARE_RADIUS)
                {
                    //Set the visibility to true
                    isVisible = true;

                    //Determine if the node in each column between the player and the zombie hass a wall
                    for (int colDiff = 1; colDiff < Math.Abs(player.GetCurNode().GetCol() - GetCurNode().GetCol()); colDiff++)
                    {
                        //Set the column to check
                        int curCol = curNode.GetCol() + Math.Sign(player.GetCurNode().GetCol() - curNode.GetCol()) * colDiff;

                        //Set the visibility as false if there is a wall on the node to check
                        if (nodeMap[curNode.GetRow(), curCol].GetItemOnSpace() == Game1.WALL_H || nodeMap[curNode.GetRow(), curCol].GetItemOnSpace() == Game1.WALL_V)
                        {
                            //Set the visibility to false
                            isVisible = false;
                            break;
                        }
                    }
                }
                else if (player.GetCurNode().GetCol() == curNode.GetCol() && Math.Abs(player.GetCurNode().GetRow() - curNode.GetRow()) <= JUMP_SCARE_RADIUS)
                {
                    //Set the visibility to true
                    isVisible = true;

                    //Determine if the node in each row between the player and the zombie hass a wall
                    for (int rowDiff = 1; rowDiff < Math.Abs(player.GetCurNode().GetRow() - GetCurNode().GetRow()); rowDiff++)
                    {
                        //Set the row to check
                        int curRow = curNode.GetRow() + Math.Sign(player.GetCurNode().GetRow() - curNode.GetRow()) * rowDiff;

                        //Set the visibility as false if there is a wall on the node to check
                        if (nodeMap[curRow, curNode.GetCol()].GetItemOnSpace() == Game1.WALL_H || nodeMap[curRow, curNode.GetCol()].GetItemOnSpace() == Game1.WALL_V)
                        {
                            //Set the visibility to false
                            isVisible = false;
                            break;
                        }
                    }
                }

                //Update the visibility to the max if the zombie is visible
                if (isVisible)
                {
                    //Set the visibility to the maximum and set the state to chasing
                    visibility = 1f;
                    state = CHASING;
                }
            }
        }
        
        //Pre: None
        //Post: None
        //Desc: Allows the zombie to pick up the heart if it is on its space
        private void PickupHeart()
        {
            //Set the current node to have no gun on it if the space has a gun
            if (curNode.GetItemOnSpace() == Game1.HEART)
            {
                //Set the current node to have nothing on it
                curNode.SetItemOnSpace(Game1.BLANK);
                health++;
            }
        }
    }
}

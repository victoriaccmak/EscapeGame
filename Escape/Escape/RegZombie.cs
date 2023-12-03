//Author: Victoria Mak
//File Name: RegZombie.cs
//Project Name: Escape
//Creation Date: May 9, 2023
//Modified Date: June 12, 2023
//Description: RegZombie is the subclass of Zombie and is a Monster that mostly wanders in the game unless it is really close to the player.

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
    class RegZombie : Zombie
    {
        //Store the max chasing path
        private const int MAX_CHASE_PATH = 6;

        public RegZombie(Texture2D[] walkImgs, Node curNode) : base(walkImgs, curNode, CHASING, 3, 2, 20, 60f)
        {
        }

        //Pre: gameTime is the GameTime for the game, player is the Player, and nodeMap is the map of all the nodes in the game
        //Post: None
        //Desc: Updates the zombie
        public override void Update(GameTime gameTime, Node[,] nodeMap, Player player)
        {
            //Determine if the zombie is visible and update its visibility
            DetermineVisibility(player, nodeMap);
            UpdateVisibility();

            //Pick up a gun if possible
            PickupGun();

            //Update the zombie based on its state
            switch (state)
            {
                case WANDERING:
                    //Udate the wandering state and find a path
                    Wander(gameTime);
                    chasingPath = FindPath(nodeMap, player.GetCurNode());

                    //Change the state to chasing if the chasing path is less than the maximum chasing path length
                    if (chasingPath.Count != 0 && chasingPath.Count < MAX_CHASE_PATH)
                    {
                        //Set the state to chasing
                        state = CHASING;
                    }
                    break;

                case CHASING:
                    //Update the chasing state
                    UpdateChasing(gameTime, player);

                    //Change the state to wandering if the chasing path is greater than the max chasing path
                    if (chasingPath.Count > MAX_CHASE_PATH || chasingPath.Count == 0)
                    {
                        //Change the state to wandering
                        state = WANDERING;
                    }
                    break;
            }

            //Attack the player if possible
            AttackIfPossible(player, gameTime);
        }

        //Pre: None
        //Post: None
        //Desc: Allows the zombie to pick up the gun if it is on its space
        private void PickupGun()
        {
            //Set the current node to have no gun on it if the space has a gun
            if (curNode.GetItemOnSpace() == Game1.GUN)
            {
                //Set the current node to have nothing on it
                curNode.SetItemOnSpace(Game1.BLANK);
            }
        }
    }
}
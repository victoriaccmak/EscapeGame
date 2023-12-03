//Author: Victoria Mak
//File Name: Monster.cs
//Project Name: Escape
//Creation Date: May 9, 2023
//Modified Date: June 12, 2023
//Description: Monster is the base class for all monsters that can harm the player in the game. The child classes of Monster include Zombie and Ghost.

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
    class Monster
    {
        //Store the speed
        protected float maxSpeed;

        //Store the position, current node, and direction
        protected Vector2 pos;
        protected Node curNode;
        protected int dir;

        //Store the animation
        protected Animation[] anims = new Animation[4];

        //Store the attack timer
        private Timer attackTimer;

        //Store the damage
        private int damage;

        //Store the visibility 
        protected float visibility;

        public Monster(Node curNode, float maxSpeed, int damage, int attackTime, float visibility, Texture2D img)
        {
            //Set the current node, max speed, damage, and visibility
            this.curNode = curNode;
            this.maxSpeed = maxSpeed;
            this.damage = damage;
            this.visibility = visibility;

            //Set the position
            pos = new Vector2(curNode.GetRec().X, curNode.GetRec().Bottom - img.Height);

            //Set the attack timer
            attackTimer = new Timer(500, true);
        }

        //Pre: None
        //Post: Returns a node
        //Desc: Returns the current node of the monster
        public Node GetCurNode()
        {
            //Return the current node
            return curNode;
        }

        //Pre: curNode is a Node where the monster will be added to
        //Post: None
        //Desc: Sets the current node of the monster
        public void SetCurNode(Node curNode)
        {
            //Set the current node to the node
            this.curNode = curNode;
        }

        //Pre: None
        //Post: Returns a rectangle
        //Desc: Returns the rectangle of the monster
        public Rectangle GetRec()
        {
            //Return the rectangle of the monster in its current direction
            return anims[dir].destRec;
        }

        //Pre: None
        //Post: Returns an int from 0 to 3 representing the direction 
        //Desc: Returns the direction of the monster
        public int GetDir()
        {
            //Return the direction
            return dir;
        }

        //Pre: gameTime is the GameTime for the game, player is the Player
        //Post: None
        //Desc: Determine if the monster intersects the player and attack the player if possible
        protected void AttackIfPossible(Player player, GameTime gameTime)
        {
            //Only attack if the player is on a node
            if (player.GetCurNode() != null)
            {
                //Update the attack timer if the monster and player are on the same node or if the player is directly in front of the monster
                if (curNode == player.GetCurNode() || (curNode.GetAdj(dir) != null && curNode.GetAdj(dir) == player.GetCurNode()))
                {
                    //Update the attack timer
                    attackTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                    //Reduce the player's health if the attack timer is done
                    if (attackTimer.IsFinished())
                    {
                        //Reset the attack timer and reduce the player's health
                        attackTimer.ResetTimer(true);
                        player.GetInventory().ReduceHealth(damage);
                    }
                }
                else
                {
                    //Reset the attack timer
                    attackTimer.ResetTimer(true);
                }
            }
            else
            {
                //Reset the attack timer
                attackTimer.ResetTimer(true);
            }
        }

        //Pre: gameTime is the GameTime for the game
        //Post: None
        //Desc: Moves the monster toward its direction
        protected virtual void Walk(GameTime gameTime)
        {

        }

        //Pre: gameTime is the GameTime for the game, player is the Player, and nodeMap is the map of all the nodes in the world
        //Post: None
        //Desc: Updates the monster
        public virtual void Update(GameTime gameTime, Node[,] nodeMap, Player player)
        {

        }

        //Pre: spriteBatch is a SpriteBatch for drawing the game graphics
        //Post: None
        //Desc: Draw the monster
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the animation
            anims[dir].Draw(spriteBatch, Color.White * visibility, SpriteEffects.None);
        }
    }
}

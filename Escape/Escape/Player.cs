//Author: Victoria Mak
//File Name: Player.cs
//Project Name: Escape
//Creation Date: May 9, 2023
//Modified Date: June 12, 2023
//Description: Player is the class that defines the player in the game for the user to control.

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
using Microsoft.Xna.Framework.Audio;
using Animation2D;

namespace Escape
{
    class Player
    {
        //Store the animation
        private Animation[] anims = new Animation[4];

        //Store the bullet images
        private Texture2D[] bulletImgs;

        //Store the max speed, position, current node, and direction
        private const float MAX_SPEED = 100;
        private Vector2 pos;
        private Node curNode;
        private int dir;

        //Store the bullet sound effects
        private SoundEffect shootSnd;
        private SoundEffect impactSnd;

        //Store the inventory
        private Inventory inventory;

        public Player(Texture2D[] imgs, Texture2D[] bulletImgs, Node curNode, Inventory inventory, SoundEffect shootSnd, SoundEffect impactSnd)
        {
            //Set the current node, bullet images, sound effects and inventory
            this.curNode = curNode;
            this.bulletImgs = bulletImgs;
            this.shootSnd = shootSnd;
            this.impactSnd = impactSnd;
            this.inventory = inventory;

            //Set the position 
            pos = new Vector2(curNode.GetRec().X, curNode.GetRec().Bottom - imgs[Game1.DOWN].Height);

            //Set the animations
            anims[Game1.DOWN] = new Animation(imgs[Game1.DOWN], 4, 1, 4, 1, 2, Animation.ANIMATE_FOREVER, 10, pos, 1f, true);
            anims[Game1.UP] = new Animation(imgs[Game1.UP], 4, 1, 4, 1, 2, Animation.ANIMATE_FOREVER, 10, pos, 1f, true);
            anims[Game1.LEFT] = new Animation(imgs[Game1.LEFT], 4, 1, 4, 1, 2, Animation.ANIMATE_FOREVER, 10, pos, 1f, true);
            anims[Game1.RIGHT] = new Animation(imgs[Game1.RIGHT], 4, 1, 4, 1, 2, Animation.ANIMATE_FOREVER, 10, pos, 1f, true);

            //Set the direction
            dir = Game1.DOWN;
        }

        //Pre: None
        //Post: Returns a rectangle
        //Desc: Returns the rectangle of the player
        public Rectangle GetRec()
        {
            //Return the rectangle of the animation
            return anims[dir].destRec;
        }

        //Pre: None
        //Post: Returns a node
        //Desc: Returns the current node of the player
        public Node GetCurNode()
        {
            //Return the current node
            return curNode;
        }

        //Pre: curNode is a Node where the player is going to be added to
        //Post: None
        //Desc: Sets the current node of the player
        public void SetCurNode(Node curNode)
        {
            //Set the current node
            this.curNode = curNode;
        }

        //Pre: None
        //Post: Returns an int from 0 to 3 representing the direction 
        //Desc: Returns the direction of the player
        public int GetDir()
        {
            //Return the direction of the player
            return dir;
        }

        //Pre: None
        //Post: Returns the inventory of the player
        //Desc: Returns the inventory of the player
        public Inventory GetInventory()
        {
            //Return the inventory
            return inventory;
        }

        //Pre: kb is the current keyboard state, gameTime is a GameTime, and nodeMap is a 2D array holding all the nodes in the game
        //Post: None
        //Desc: Moves the player based on the keyboard controls
        public void Move(KeyboardState kb, GameTime gameTime, Node[,] nodeMap)
        {
            //Move the player depending on the key pressed
            if (kb.IsKeyDown(Keys.Left))
            {
                //Set the direction to the left
                dir = Game1.LEFT;

                //Move the player if there is space to move
                if (!(GetRec().X <= 0 || (IsBlocked(curNode.GetAdj(dir)) && GetRec().X <= curNode.GetRec().X)))
                {
                    //Update the position
                    pos.X -= (float)(MAX_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
                    pos.Y = curNode.GetRec().Bottom - GetRec().Height;

                    //Update the animation
                    anims[dir].Update(gameTime);
                }
            }
            else if (kb.IsKeyDown(Keys.Right))
            {
                //Set the to the right
                dir = Game1.RIGHT;

                //Move the player if there is space to move
                if (!(GetRec().Right >= Game1.SCREEN_WIDTH || (IsBlocked(curNode.GetAdj(dir)) && GetRec().X >= curNode.GetRec().X)))
                {
                    //Update the position
                    pos.X += (float)(MAX_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
                    pos.Y = curNode.GetRec().Bottom - GetRec().Height;

                    //Update the animation
                    anims[dir].Update(gameTime);
                }
            }
            else if (kb.IsKeyDown(Keys.Up))
            {
                //Set the direction to up
                dir = Game1.UP;

                //Move the player if there is space to move
                if (!(GetRec().Bottom <= Node.SIDE_LENGTH || (IsBlocked(curNode.GetAdj(dir)) && GetRec().Bottom <= curNode.GetRec().Bottom)))
                {
                    //Update the position
                    pos.Y -= (float)(MAX_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
                    pos.X = curNode.GetRec().X;

                    //Update the animation
                    anims[dir].Update(gameTime);
                }
            }
            else if (kb.IsKeyDown(Keys.Down))
            {
                //Set the direction to down
                dir = Game1.DOWN;

                //Move the player if there is space to move
                if (!(GetRec().Bottom > Game1.SCREEN_HEIGHT || (IsBlocked(curNode.GetAdj(dir)) && GetRec().Bottom > curNode.GetRec().Bottom)))
                {
                    //Update the position
                    pos.Y += (float)(MAX_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
                    pos.X = curNode.GetRec().X;

                    //Update the animation
                    anims[dir].Update(gameTime);
                }
            }

            //Update the location of the animation
            anims[Game1.DOWN].destRec.Location = pos.ToPoint();
            anims[Game1.UP].destRec.Location = pos.ToPoint();
            anims[Game1.LEFT].destRec.Location = pos.ToPoint();
            anims[Game1.RIGHT].destRec.Location = pos.ToPoint();
        }

        //Pre: dir is the exiting direction specified for the level and gameTime is the GameTime
        //Post: None
        //Desc: Moves the player out of the screen
        public void Exit(int dir, GameTime gameTime)
        {
            //Move the player only in the direction of the exit
            switch (dir)
            {
                case Game1.UP:
                    //Move the player up
                    pos.Y -= (float)(MAX_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
                    pos.X = curNode.GetRec().X;

                    //Set the player's node to null when it is off the screen
                    if (GetRec().Bottom <= 0)
                    {
                        //Set the current node to null
                        curNode = null;
                    }
                    break;

                case Game1.DOWN:
                    //Move the player down
                    pos.Y += (float)(MAX_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
                    pos.X = curNode.GetRec().X;

                    //Set the player's node to null when it is off the screen
                    if (GetRec().Y >= Game1.SCREEN_HEIGHT)
                    {
                        //Set the current node to null
                        curNode = null;
                    }
                    break;

                case Game1.LEFT:
                    //Move the player left
                    pos.X -= (float)(MAX_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
                    pos.Y = curNode.GetRec().Bottom - GetRec().Height;

                    //Set the player's node to null when it is off the screen
                    if (GetRec().Right <= 0)
                    {
                        //Set the current node to null
                        curNode = null;
                    }
                    break;

                case Game1.RIGHT:
                    //Move the player right
                    pos.X += (float)(MAX_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
                    pos.Y = curNode.GetRec().Bottom - GetRec().Height;

                    //Set the player's node to null when it is off the screen
                    if (GetRec().X >= Game1.SCREEN_WIDTH)
                    {
                        //Set the current node to null
                        curNode = null;
                    }
                    break;
            }

            //Move all the animations to the location of the player
            anims[Game1.DOWN].destRec.Location = pos.ToPoint();
            anims[Game1.UP].destRec.Location = pos.ToPoint();
            anims[Game1.LEFT].destRec.Location = pos.ToPoint();
            anims[Game1.RIGHT].destRec.Location = pos.ToPoint();

        }

        //Pre: facingNode is the node that the player is facing
        //Post: Returns whether the facing node is blocked or null
        //Desc: Determines if the player can move to the next node
        private bool IsBlocked(Node facingNode)
        {
            //Return whether the facing node is null or has a blocked object on it
            return facingNode != null && (facingNode.GetItemOnSpace() == Game1.WALL_H || facingNode.GetItemOnSpace() == Game1.WALL_V || facingNode.GetItemOnSpace() == Game1.LOCK || facingNode.GetItemOnSpace() == Game1.BOX || facingNode.GetItemOnSpace() == Game1.CHAIR || facingNode.GetItemOnSpace() == Game1.BARRICADE_OPEN);
        }

        //Pre: kb is the current keyboard state and prevKb is the previous keyboard state in the previous update
        //Post: None
        //Desc: Picks up the object in front or on the current node of the player
        public void PickupObject(KeyboardState kb, KeyboardState prevKb)
        {
            //Pick up the object if F is pressed
            if (kb.IsKeyDown(Keys.F) && prevKb.IsKeyUp(Keys.F))
            {
                //Pick up the object on the current node if possible or pick up the object on the node that the player is facing
                if (curNode.GetItemOnSpace() == Game1.BARRICADE_FOLDED || curNode.GetItemOnSpace() == Game1.GUN || curNode.GetItemOnSpace() == Game1.KEY || curNode.GetItemOnSpace() == Game1.HEART)
                {
                    //Pick up the object on the current node
                    inventory.PickupItem(curNode);
                }
                else if (curNode.GetAdj(dir) != null)
                {
                    //Pick up the object on the facing node
                    inventory.PickupItem(curNode.GetAdj(dir));
                }
            }
        }

        //Pre: kb is the current keyboard state and prevKb is the previous keyboard state in the previous update
        //Post: Returns a bool for whether the player is unlocking the lock
        //Desc: Drops an item or a key and returns a bool for whether the player is unlocking to end the level
        public bool DropItemOrUnlock(KeyboardState kb, KeyboardState prevKb)
        {
            //Store the facing node and the item to be dropped
            Node facingNode = curNode.GetAdj(dir);
            int item;

            //Drop or unlock if D is pressed
            if (kb.IsKeyDown(Keys.D) && prevKb.IsKeyUp(Keys.D))
            {
                //Drop the item on the facing node if the facing node is not null and there is nothing on it
                if (facingNode != null && facingNode.GetZombiesOnSpace().Count == 0 && facingNode.GetItemOnSpace() == Game1.BLANK)
                {
                    //Store the item that is dropped
                    item = inventory.DropItem();

                    //If the item is a folded barricade, set it to an open barricade
                    if (item == Game1.BARRICADE_FOLDED)
                    {
                        //Set the item to an open barricade
                        item = Game1.BARRICADE_OPEN;
                    }

                    //Add the item to the facing node
                    facingNode.SetItemOnSpace(item);
                }
                else if (facingNode != null && facingNode.GetItemOnSpace() == Game1.LOCK)
                {
                    //Unlock the lock if the key is at the top of the inventory
                    if (inventory.Top() == Game1.KEY)
                    {
                        //Set the facing node as blank and return true for the player as unlocking
                        facingNode.SetItemOnSpace(Game1.BLANK);
                        return true;
                    }
                }
            }

            //Return false to represent that the player did not unlock
            return false;
        }

        //Pre: None
        //Post: Returns a bullet or null depending on whether a bullet was shot
        //Desc: Shoot a bullet by creating a new bullet facing away from the player
        public Bullet Shoot()
        {
            //Store the new bullet that may be shot
            Bullet newBullet = null;

            //Shoot the bullet if the facing node is not null or a wall and there are bullets to shoot
            if (curNode.GetAdj(dir) == null || (curNode.GetAdj(dir).GetItemOnSpace() != Game1.WALL_H && curNode.GetAdj(dir).GetItemOnSpace() != Game1.WALL_V && inventory.GetNumBullets() > 0))
            {
                //Set the new bullet, remove a bullet from the inventory, and set the current node to have the bullet
                newBullet = new Bullet(bulletImgs[dir], new Rectangle(GetRec().Center.X - bulletImgs[dir].Width / 2, GetRec().Center.Y - bulletImgs[dir].Height / 2, bulletImgs[dir].Width, bulletImgs[dir].Height), curNode, dir, shootSnd, impactSnd);
                inventory.RemoveBullet();
                curNode.GetBulletsOnSpace().Add(newBullet);
            }

            //Return the new bullet
            return newBullet;
        }

        //Pre: spriteBatch is a SpriteBatch for drawing the game graphics
        //Post: None
        //Desc: Draw the player
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the player animation
            anims[dir].Draw(spriteBatch, Color.White, SpriteEffects.None);
        }
    }
}

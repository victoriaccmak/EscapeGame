//Author: Victoria Mak
//File Name: Bullet.cs
//Project Name: Escape
//Creation Date: May 9, 2023
//Modified Date: June 12, 2023
//Description: Bullet is the class for the bullets that the player shoots in the game.

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
    class Bullet
    {
        //Store the constant speed of the bullet in pixels per second
        private const float MAX_SPEED = 400f;

        //Store the direction, current y speed, location, rectangle, current node and the location of the bullet point
        private int dir;
        private float speed;
        private Vector2 location;
        private Rectangle rec;
        private Node curNode;
        private Point bulletPoint;

        //Store the bullet image and impact sound
        private Texture2D img;
        private SoundEffect impactSnd;

        public Bullet(Texture2D img, Rectangle rec, Node curNode, int dir, SoundEffect shootSnd, SoundEffect impactSnd)
        {
            //Store and set the adjacent node as the next node beside the current node
            Node adjNode = curNode.GetAdj(dir);

            //Set the bullet image, rectangle, sound, direction, and current node
            this.img = img;
            this.rec = rec;
            this.impactSnd = impactSnd;
            this.dir = dir;

            //Set the current node
            this.curNode = curNode;

            //Set the location of the bullet based on the location of the rectangle
            location = new Vector2(rec.X, rec.Y);
            bulletPoint = new Point((int)location.X + img.Width, (int)location.Y + img.Height / 2);

            //Set the bullet's current node to the adjacent node if the adjacent node is not null
            if (adjNode != null)
            {
                //Set the current node to the adjacent node if the bullet point in the rectangle of the adjacent node
                if (adjNode.GetRec().Contains(bulletPoint))
                {
                    //Set the adjacent node as the current node
                    this.curNode = adjNode;
                }
            }

            //Play the shoot sound
            shootSnd.CreateInstance().Play();
        }

        //Pre: none
        //Post: returns a bool of whether the bullet is off screen or not
        //Desc: Determines whether the bullet is off the screen to be removed
        public bool GetOffScreen()
        {
            //Return whether or not the bullet is of screen
            return rec.Bottom < 0 || rec.Y > Game1.SCREEN_HEIGHT - Game1.STATS_BAR_HEIGHT || rec.Right < 0 || rec.X > Game1.SCREEN_WIDTH;
        }

        //Pre: None
        //Post: Returns a node
        //Desc: Returns the current node of the bullet
        public Node GetCurNode()
        {
            //Return the current node
            return curNode;
        }

        //Pre: None
        //Post: Returns the direction represented by an int from 0 to 3
        //Desc: Returns the direction of the bullet
        public int GetDir()
        {
            //Returns the direction
            return dir;
        }

        //Pre: curNode is a Node where the bullet is added to 
        //Post: None
        //Desc: Sets the current node of the bullet
        public void SetCurNode(Node curNode)
        {
            //Set the current node of the bullet
            this.curNode = curNode;
        }

        //Pre: gameTime is a GameTime and has the elapsed game time between updates
        //Post: none
        //Desc: Moves the bullet by update the speed, location, and bullet rectangle
        public void Move(GameTime gameTime)
        {
            //Set the current y speed
            speed = (float)(MAX_SPEED * gameTime.ElapsedGameTime.TotalSeconds);

            //Increase the location by the speed
            switch (dir)
            {
                case Game1.UP:
                    //Move the bullet location up and set the rectangle location
                    location.Y -= speed;
                    bulletPoint.Y = (int)location.Y + img.Height / 2;
                    rec.Y = (int)location.Y;
                    break;

                case Game1.DOWN:
                    //Move the bullet location down and set the rectangle location
                    location.Y += speed;
                    bulletPoint.Y = (int)location.Y + img.Height / 2;
                    rec.Y = (int)location.Y;
                    break;

                case Game1.LEFT:
                    //Move the bullet location left and set the rectangle location
                    location.X -= speed;
                    bulletPoint.X = (int)location.X + img.Width;
                    rec.X = (int)location.X;
                    break;

                case Game1.RIGHT:
                    //Move the bullet location right and set the rectangle location
                    location.X += speed;
                    bulletPoint.X = (int)location.X + img.Width;
                    rec.X = (int)location.X;
                    break;
            }
        }
        
        //Pre: None
        //Post: Returns a bool representing whether the bullet collided with the wall
        //Desc: Determines if the bullet collided with the wall
        public bool CheckCollisionWithWall()
        {
            //Return that the bullet has collided if its current node has a wall
            if (curNode.GetItemOnSpace() == Game1.WALL_H || curNode.GetItemOnSpace() == Game1.WALL_V)
            {
                //Play the impact sound and return that the bullet has collided
                impactSnd.CreateInstance();
                return true;
            }

            //Return that the bullet did not collide
            return false;
        }

        //Pre: None
        //Post: Returns a Zombie or null representing whether the bullet collided with a zombie or not
        //Desc: Determines if the bullet collided with a zombie
        public Zombie CheckCollisionWithZombie()
        {
            //Store the next node in the bullet's direction
            Node nextnode = curNode.GetAdj(dir);

            //Detect for a collision with the zombies on the current node
            foreach (Zombie i in curNode.GetZombiesOnSpace())
            {
                //Return the zombie if the zombie's rectangle has the bullet point in it
                if (i.GetRec().Contains(GetBulletPoint()))
                {
                    //Play the impact sound and return the zombie
                    impactSnd.CreateInstance();
                    return i;
                }
            }

            //Detect for a collision with the zombie in the next node if the next node is not null
            if (nextnode != null)
            {
                //Detect for a collision for every zombie on the next node
                foreach (Zombie i in nextnode.GetZombiesOnSpace())
                {
                    //Return the zombie if the zombie's rectangle has the bullet point in it
                    if (i.GetRec().Contains(GetBulletPoint()))
                    {
                        //Play the impact sound and return the zombie
                        //impactSnd.CreateInstance();
                        return i;
                    }
                }
            }

            //Return that no zombie has collided with the bullet
            return null;
        }

        //Pre: None
        //Post: Returns a Point representing the bullet point's location
        //Desc: Returns the bullet point's location
        public Point GetBulletPoint()
        {
            //Return the bullet point's location
            return bulletPoint;
        }

        //Pre: spriteBatch is a SpriteBatch
        //Post: none
        //Desc: Draws the bullet
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the bullet
            spriteBatch.Draw(img, rec, Color.White);
        }
    }
}

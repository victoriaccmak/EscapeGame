//Author: Victoria Mak
//File Name: Inventory.cs
//Project Name: Escape
//Creation Date: May 9, 2023
//Modified Date: June 12, 2023
//Description: The Inventory class is the inventory of the player and holds the items as a stack as well as bullets and health.

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
    class Inventory
    {
        //Store the inventory box side length, the max number of items, and the max health
        private const int INVENTORY_BOX_SIDE = 40;
        public const int MAX_ITEMS = 5;
        public const int MAX_HEALTH = 10;

        //Store the array of items, the number of bullets, the count of the number of items, and the health
        private int[] items = new int[MAX_ITEMS];
        private int count;
        private int numBullets;
        private int health;

        //Store the pixel image and the item images
        private Texture2D pixelImg;
        private Texture2D[] itemImgs;

        //Store the messages for the inventory label, number of bullets, and health
        private string inventoryMsg = "Inventory";
        private string bulletsMsg = "Bullets: ";
        private string healthMsg = "Health: ";

        //Store the font
        private SpriteFont font;

        //Store the location of the messages for the inventory label, bullets, and health
        private Vector2 inventoryMsgLoc;
        private Vector2 bulletsMsgLoc;
        private Vector2 healthMsgLoc;

        //Store the rectangles for the inventory boxes, the item rectangles, and the gun rectangle
        private Rectangle[] inventoryBoxRecs = new Rectangle[MAX_ITEMS];
        private Rectangle[] inventoryItemRecs = new Rectangle[MAX_ITEMS];
        private Rectangle gunRec;

        //Store the rectangle for the full and current health bar
        private Rectangle fullHealthBarRec;
        private Rectangle curHealthBarRec;

        public Inventory(Texture2D pixelImg, Texture2D[] itemImgs, SpriteFont font, int health)
        {
            //Set the health, pixel image, item images, font, and the count and number of bullets to 0
            this.health = health;
            this.pixelImg = pixelImg;
            this.itemImgs = itemImgs;
            this.font = font;
            count = 0;
            numBullets = 0;

            //Set the messages for the health and bullets
            bulletsMsg = "Bullets: " + 0;
            healthMsg = "Health: " + health;

            //Set the location of the inventory label
            inventoryMsgLoc = new Vector2(Game1.SPACER_3, Game1.SCREEN_HEIGHT - Game1.STATS_BAR_HEIGHT + Game1.SPACER_3);

            //Set the rectangles of the inventory box rectangles
            for (int i = 0; i < MAX_ITEMS; i++)
            {
                //set the rectangle for each inventory box
                inventoryBoxRecs[i] = new Rectangle((i + 1) * Game1.SPACER_3 + i * INVENTORY_BOX_SIDE, (int)(inventoryMsgLoc.Y + font.MeasureString(inventoryMsg).Y + Game1.SPACER_3), INVENTORY_BOX_SIDE, INVENTORY_BOX_SIDE);
            }

            //Set the location of the bullet message and the rectangle for the gun
            bulletsMsgLoc = new Vector2(inventoryBoxRecs[MAX_ITEMS - 1].Right, inventoryMsgLoc.Y);
            gunRec = new Rectangle(inventoryBoxRecs[MAX_ITEMS - 1].Right + Game1.SPACER_3, inventoryBoxRecs[0].Center.Y - itemImgs[Game1.GUN].Height / 2, itemImgs[Game1.GUN].Width, itemImgs[Game1.GUN].Height);

            //Set the location for the health message and the rectangles for health bars
            healthMsgLoc = new Vector2(Game1.SCREEN_WIDTH / 2, bulletsMsgLoc.Y);
            fullHealthBarRec = new Rectangle((int)healthMsgLoc.X, inventoryBoxRecs[0].Y, 200, 20);
            curHealthBarRec = new Rectangle((int)healthMsgLoc.X + 1, fullHealthBarRec.Y + 1, (fullHealthBarRec.Width - 2) * health / MAX_HEALTH, 18);
        }

        //Pre: None
        //Post: Returns the positive or 0 number of bullets
        //Desc: Returns the number of bullets
        public int GetNumBullets()
        {
            //Returns the number of bullets
            return numBullets;
        }

        //Pre: None
        //Post: Returns an integer from 0 to the max health of the player
        //Desc: Returns the health of the player
        public int GetHealth()
        {
            //Return the health
            return health;
        }

        //Pre: None
        //Post: Returns the type of object dropped as a positive integer
        //Desc: Drops the item by removing it from the inventory and returning the object
        public int DropItem()
        {
            //Only drop the item if there is an item to drop
            if (count > 0)
            {
                //Reduce the count of items and return the item
                count--;
                return items[count];
            }

            //Return a blank object
            return Game1.BLANK;
        }

        //Pre: None
        //Post: Returns the top object of the inventory as an integer
        //Desc: Returns the top object in the inventory
        public int Top()
        {
            //Return the top object if there is an object in the inventory
            if (!IsEmpty())
            {
                //Return the top item
                return items[count - 1];
            }

            //Return a negative integer
            return -1;
        }

        //Pre: None
        //Post: Returns an int for the size of the inventory
        //Desc: Returns the size of the inventory
        public int Size()
        {
            //Return the size of the inventory
            return count;
        }

        //Pre: None
        //Post: Returns a bool for whether the inventory is empth
        //Desc: Returns whether the inventory is empth
        public bool IsEmpty()
        {
            //Return whether the inventory is empty
            return count == 0;
        }

        //Pre: None
        //Post: None
        //Desc: Reduce the number of bullets
        public void RemoveBullet()
        {
            //Reduce the number of bullets and update the bullets message
            numBullets--;
            bulletsMsg = "Bullets: " + numBullets;
        }

        //Pre: damage is an integer that the health is to be reduced by
        //Post: None
        //Desc: Reduce the health of the player
        public void ReduceHealth(int damage)
        {
            //Reduce the health by the damage and update the health bar and the health message
            health = Math.Max(health - damage, 0);
            curHealthBarRec.Width = (fullHealthBarRec.Width - 2) * health / MAX_HEALTH;
            healthMsg = "Health: " + health;
        }

        //Pre: None
        //Post: None
        //Desc: Increase the health by 1
        public void IncreaseHealth()
        {
            //Increaset the health and update the health bar and message
            health = Math.Min(health + 1, MAX_HEALTH);
            curHealthBarRec.Width = (fullHealthBarRec.Width - 2) * health / MAX_HEALTH;
            healthMsg = "Health: " + health;
        }

        //Pre: Node is the node that the object is taken from
        //Post: None
        //Desc: Removes the item from the node and adds it to the inventory
        public void PickupItem(Node node)
        {
            //Increase the number of bullets, the health, or add an item to the inventory depending on the object picked up
            if (node.GetItemOnSpace() == Game1.GUN)
            {
                //Increase the number of bullets
                numBullets += Game1.NUM_BULLETS_PER_GUN;

                //Reveal the key underneath if the space has a key under the gun
                if (node.GetHasKey())
                {
                    //Set the node as having the key
                    node.SetItemOnSpace(Game1.KEY);
                }
                else
                {
                    //Set the node has having nothing
                    node.SetItemOnSpace(Game1.BLANK);
                }

                //Increase the number of bullets
                bulletsMsg = "Bullets: " + numBullets;
            }
            else if (node.GetItemOnSpace() == Game1.HEART)
            {
                //Increase the player's health
                IncreaseHealth();

                //Reveal the key underneath if the space has a key under the gun
                if (node.GetHasKey())
                {
                    //Set the node as having the key
                    node.SetItemOnSpace(Game1.KEY);
                }
                else
                {
                    //Set the node has having nothing
                    node.SetItemOnSpace(Game1.BLANK);
                }
            }
            else if (count < MAX_ITEMS && node.GetItemOnSpace() != Game1.BLANK && node.GetItemOnSpace() != Game1.WALL_V && node.GetItemOnSpace() != Game1.WALL_H && node.GetItemOnSpace() != Game1.LOCK)
            {
                //Add the item picked up to the inventory and increase the count in the inventory
                items[count] = node.GetItemOnSpace();
                count++;

                //Set the node as not having the key if the key was picked up
                if (items[count - 1] == Game1.KEY)
                {
                    //Set the node as not having the key
                    node.SetHasKey(false);
                }

                //Reveal the key underneath if the space has a key under the gun
                if (node.GetHasKey())
                {
                    //Set the node as having the key
                    node.SetItemOnSpace(Game1.KEY);
                }
                else
                {
                    //Set the node has having nothing
                    node.SetItemOnSpace(Game1.BLANK);
                }

                //Set the item rectangle to show the item in the inventory box
                inventoryItemRecs[count - 1] = new Rectangle(inventoryBoxRecs[count - 1].Center.X - itemImgs[items[count - 1]].Width / 2, inventoryBoxRecs[count - 1].Center.Y - itemImgs[items[count - 1]].Height / 2, itemImgs[items[count - 1]].Width, itemImgs[items[count - 1]].Height);
            }
        }

        //Pre: spriteBatch is the SpriteBatch for the drawing the game graphics
        //Post: None
        //Desc: Draws the inventory at the stats bar
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the inventory, bullet, and health messages
            spriteBatch.DrawString(font, inventoryMsg, inventoryMsgLoc, Color.White);
            spriteBatch.DrawString(font, bulletsMsg, bulletsMsgLoc, Color.White);
            spriteBatch.DrawString(font, healthMsg, healthMsgLoc, Color.White);

            //Draw the health bar
            spriteBatch.Draw(pixelImg, fullHealthBarRec, Color.Gray);
            spriteBatch.Draw(pixelImg, curHealthBarRec, Color.Red);

            //Draw each inventory box with the item on it
            for (int i = 0; i < MAX_ITEMS; i++)
            {
                //Draw each inventory box
                spriteBatch.Draw(pixelImg, inventoryBoxRecs[i], Color.LightGray);

                //Draw the item on the box if the box number is less than the count of items
                if (i < Size())
                {
                    //Draw the item in the box
                    spriteBatch.Draw(itemImgs[items[i]], inventoryItemRecs[i], Color.White);
                }
            }

            //Draw the gun icon
            spriteBatch.Draw(itemImgs[Game1.GUN], gunRec, Color.White);
        }
    }
}

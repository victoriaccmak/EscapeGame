//Author: Victoria Mak
//File Name: Node.cs
//Project Name: Escape
//Creation Date: May 9, 2023
//Modified Date: June 12, 2023
//Description: The Node is a space in the room, with many Nodes to form a grid. It can hold an item, a player, and monsters on it.

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
    class Node
    {
        //Store the side length of the node
        public const int SIDE_LENGTH = 32;

        //Store the row and column
        private int row;
        private int col;
        
        //Store the f, g, and h cost
        public float f = 0;
        public float g = 0;
        public float h = 0;

        //Store the parent node
        public Node parent = null;

        //Store the item, zombies, bullets, and ghosts on the space
        private int itemOnSpace;
        private List<Zombie> zombiesOnSpace = new List<Zombie>();
        private List<Bullet> bulletsOnSpace = new List<Bullet>();
        private List<Ghost> ghostsOnSpace = new List<Ghost>();

        //Store the player on the space
        private Player player = null;

        //Store the item images
        private Texture2D[] itemImgs;

        //Store the rectangle for the node and the item rectangle
        private Rectangle nodeRec;
        private Rectangle itemRec;

        //Store whether the node has the key
        private bool hasKey = false;

        //Store the adjacent nodes
        private Node[] adjNodes = new Node[4];

        public Node(int row, int col, int itemOnSpace, Texture2D[] itemImgs)
        {
            //Set the row, column, item on space, and the item images
            this.row = row;
            this.col = col;
            this.itemOnSpace = itemOnSpace;
            this.itemImgs = itemImgs;

            //Set the node rectangle
            nodeRec = new Rectangle(col * SIDE_LENGTH, row * SIDE_LENGTH, SIDE_LENGTH, SIDE_LENGTH);

            //Set the item rectangle if there is an item on the space
            if (itemOnSpace != Game1.BLANK)
            {
                //Set the item rectangle
                itemRec = new Rectangle(nodeRec.X, nodeRec.Bottom - itemImgs[itemOnSpace].Height, itemImgs[itemOnSpace].Width, itemImgs[itemOnSpace].Height);
            }
        }
        
        //Pre: None
        //Post: Returns a positive or 0 int for the row number
        //Desc: Returns the row number of the node
        public int GetRow()
        {
            //Return the row number
            return row;
        }

        //Pre: None
        //Post: Returns a positive or 0 int for the column number
        //Desc: Returns the column number of the node
        public int GetCol()
        {
            //Return the column number
            return col;
        }

        //Pre: None
        //Post: Returns the rectangle at the location of the node with the node dimensions
        //Desc: Returns the rectangle of the node
        public Rectangle GetRec()
        {
            //Return the rectangle of the node
            return nodeRec;
        }

        //Pre: None
        //Post: Returns the list of zombies
        //Desc: Returns the list of zombies on the node
        public List<Zombie> GetZombiesOnSpace()
        {
            //Return the list of zombies
            return zombiesOnSpace;
        }

        //Pre: None
        //Post: Returns the list of bullets
        //Desc: Returns the list of bullets on the node
        public List<Bullet> GetBulletsOnSpace()
        {
            //Return the list of bullets
            return bulletsOnSpace;
        }

        //Pre: None
        //Post: Returns a list of ghosts
        //Desc: Returns the list of ghosts on the node
        public List<Ghost> GetGhostsOnSpace()
        {
            //Return the list of ghosts
            return ghostsOnSpace;
        }

        //Pre: dir is an int representing the direction that the node is pointing to
        //Post: Returns the adjacent node in the specified direction
        //Desc: Returns the node directly beside the current node in the specified direction
        public Node GetAdj(int dir)
        {
            //Only return the adjacent node if the direction is valid
            if (dir >= 0 && dir < adjNodes.Length)
            {
                //Return the adjacent node
                return adjNodes[dir];
            }

            //Return no nodes
            return null;
        }

        //Pre: hasKey is a bool for whether the node would be set to have the key
        //Post: None
        //Desc: Sets the node as having the key
        public void SetHasKey(bool hasKey)
        {
            //Set whether the node has the key
            this.hasKey = hasKey;

            //Set the item to key if it is blank and the node has the key
            if (hasKey && itemOnSpace == Game1.BLANK)
            {
                //Set the item on the space to the key
                SetItemOnSpace(Game1.KEY);
            }
        }

        //Pre: None
        //Post: hasKey is a bool for whether the node has the key
        //Desc: Gets whether the node has the key
        public bool GetHasKey()
        {
            //Return whether the node has the key
            return hasKey;
        }

        //Pre: item in an int from 0 to 10 for the type of item to set on the node
        //Post: None
        //Desc: Sets the item on the node
        public void SetItemOnSpace(int item)
        {
            //Set the item on the space
            itemOnSpace = item;

            //Set the item rectangle if there is an item on the space
            if (item != Game1.BLANK)
            {
                //Set the item rectangle
                itemRec = new Rectangle(nodeRec.X, nodeRec.Bottom - itemImgs[itemOnSpace].Height, itemImgs[itemOnSpace].Width, itemImgs[itemOnSpace].Height);
            }
        }

        //Pre: None
        //Post: Returns an int for the type of item on the space
        //Desc: Gets the item on the node
        public int GetItemOnSpace()
        {
            //Return the item on the node
            return itemOnSpace;
        }

        //Pre: h is the h cost as a float
        //Post: none
        //Desc: Sets the h cost for the node for determining the shortest node path for the zombies
        public void SetH(float h)
        {
            //Set the h cost
            this.h = h;
        }

        //Pre: f is the f cost as a float
        //Post: none
        //Desc: Sets the f cost for the node for determining the shortest node path for the zombies
        public void SetF(float f)
        {
            //Set the f cost
            this.f = f;
        }

        //Pre: g is the g cost as a float
        //Post: none
        //Desc: Sets the g cost for the node for determining the shortest node path for the zombies
        public void SetG(float g)
        {
            //Set the g cost
            this.g = g;
        }

        //Pre: None
        //Post: returns the h cost of the node as a float
        //Desc: Gets the h cost for the node for determining the shortest node path for the zombies
        public float GetH()
        {
            //Return the h cost
            return h;
        }

        //Pre: None
        //Post: returns the g cost of the node as a float
        //Desc: Gets the g cost for the node for determining the shortest node path for the zombies
        public float GetG()
        {
            //Return the g cost
            return g;
        }

        //Pre: None
        //Post: returns the f cost of the node as a float
        //Desc: Gets the f cost for the node for determining the shortest node path for the zombies
        public float GetF()
        {
            //Return the f cost
            return f;
        }

        //Pre: parent is a Node that is adjacent to this node
        //Post: None
        //Desc: Sets the parent of this node
        public void SetParent(Node parent)
        {
            //Set the parent
            this.parent = parent;
        }

        //Pre: None
        //Post: returns a node adjacent to this node representing the parent
        //Desc: Gets the parent of this node
        public Node GetParent()
        {
            //Return the parent
            return parent;
        }

        //Pre: None
        //Post: returns the f cost of the node as a float
        //Desc: Gets the f cost for the node for determining the shortest node path for the zombies
        public void SetAdjacent(Node[,] map)
        {
            //Set the node above if the row is greater than 0
            if (row > 0)
            {
                //Set the adjacent node above
                adjNodes[Game1.UP] = map[row - 1, col];
            }
            else
            {
                //Set the adjacent node above as null
                adjNodes[Game1.UP] = null;
            }

            //Set the node to the left if the column is greater than 0
            if (col > 0)
            {
                //Set the adjacent node to the left
                adjNodes[Game1.LEFT] = map[row, col - 1];
            }
            else
            {
                //Set the adjacent node to the left as null
                adjNodes[Game1.LEFT] = null;
            }

            //Set the node to the right if the column is not the last column of the node map
            if (col + 1 < map.GetLength(1))
            {
                //Set the adjacent node to the right
                adjNodes[Game1.RIGHT] = map[row, col + 1];
            }
            else
            {
                //Set the adjacent node to the right as null
                adjNodes[Game1.RIGHT] = null;
            }

            //Set the node below if the row is not the last row of the node map
            if (row + 1 < map.GetLength(0))
            {
                //Set the adjacent node below
                adjNodes[Game1.DOWN] = map[row + 1, col];
            }
            else
            {
                //Set the adjacent node below as null
                adjNodes[Game1.DOWN] = null;
            }
        }

        //Pre: player is the Player in the game or null
        //Post: None
        //Desc: Sets the player on or off the node
        public void SetPlayer(Player player)
        {
            //Set the player on the node
            this.player = player;
        }

        //Pre: spriteBatch is a SpriteBatch for drawing the game graphics
        //Post: None
        //Desc: Draw the node with the items or monsters or player on the node
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the item if there is an item on the space
            if (itemOnSpace != Game1.BLANK)
            {
                //Draw the item on the node
                spriteBatch.Draw(itemImgs[itemOnSpace], itemRec, Color.White);
            }

            //Draw the player if there is a player on the node
            if (player != null)
            {
                //Draw the player
                player.Draw(spriteBatch);
            }

            //Draw each zombie on the space
            for (int i = 0; i < zombiesOnSpace.Count; i++)
            {
                //Draw the zombie
                zombiesOnSpace[i].Draw(spriteBatch);
            }
        }
    }
}
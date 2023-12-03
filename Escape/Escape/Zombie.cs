//Author: Victoria Mak
//File Name: Zombie.cs
//Project Name: Escape
//Creation Date: May 9, 2023
//Modified Date: June 12, 2023
//Description: Zombie is a base class of the SmartZombie and RegZombie. It is a subclass of the Monster class and has the properties and methods regarding the zombies.

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
    class Zombie : Monster
    {
        //Store the constant visibility radius, the rate of visibility change
        private const float VISIBLE_RADIUS = 20;
        private const float VISBILITY_CHANGE = 0.08f;

        //Store the possible states of the zombie
        protected const int HIDING = 0;
        protected const int WANDERING = 1;
        protected const int CHASING = 2;

        //Store the zombie's health and the reward for killing the zombie
        protected int health;
        private int reward;

        //Store the state of the zombie
        protected int state;

        //store the list of nodes for the path of the zombie
        protected List<Node> chasingPath = new List<Node>();

        //Store whether the zombie is visible 
        protected bool isVisible;

        public Zombie(Texture2D[] imgs, Node curNode, int state, int health, int damage, int reward, float maxSpeed) : base(curNode, maxSpeed, damage, 1500, 0f, imgs[Game1.DOWN])
        {
            //Set the monster as not visible 
            isVisible = false;

            //Set the state, health, and reward
            this.state = state;
            this.health = health;
            this.reward = reward;

            //Set the walk animation
            anims[Game1.DOWN] = new Animation(imgs[Game1.DOWN], 3, 1, 3, 0, 2, Animation.ANIMATE_FOREVER, 8, pos, 1f, true);
            anims[Game1.UP] = new Animation(imgs[Game1.UP], 3, 1, 3, 1, 2, Animation.ANIMATE_FOREVER, 8, pos, 1f, true);
            anims[Game1.LEFT] = new Animation(imgs[Game1.LEFT], 3, 1, 3, 1, 2, Animation.ANIMATE_FOREVER, 8, pos, 1f, true);
            anims[Game1.RIGHT] = new Animation(imgs[Game1.RIGHT], 3, 1, 3, 1, 2, Animation.ANIMATE_FOREVER, 8, pos, 1f, true);

            //Set the current direction of the zombie
            dir = Game1.LEFT;
        }

        //Pre: None
        //Post: Return an int for the score awarded for killing the zombie
        //Desc: Returns the reward amount for killing the zombie
        public int GetReward()
        {
            //Return the reward
            return reward;
        }

        //Pre: None
        //Post: Returns a bool for whether the zombie is killed
        //Desc: Reduces the health and returns whether the zombie is killed
        public bool ReduceHealthAndDeterminedIsKilled()
        {
            //Reduce the health of the zombie
            health--;

            //Return the zombie as kiled if the health is less than or equal to 0
            if (health <= 0)
            {
                //Return the zombie as killed
                return true;
            }

            //Return the zombie as still alive
            return false;
        }

        //Pre: gameTime is the GameTime for the game
        //Post: None
        //Desc: Updates the zombie by changing its direction if it hits a blocked path
        protected void Wander(GameTime gameTime)
        {
            //Store and set the facing node to the next node in its path
            Node facingNode = curNode.GetAdj(dir);

            //Move the zombie
            Walk(gameTime);

            //Change the direction of the zombie depending on its current direction
            switch (dir)
            {
                case Game1.UP:
                    //Change the direction if the zombie if its rectangle is partially in the next node and the next node is null or is blocked
                    if (GetRec().Bottom <= Node.SIDE_LENGTH || (IsBlocked(facingNode) && GetRec().Bottom <= curNode.GetRec().Bottom))
                    {
                        //Set the direction to the right
                        dir = Game1.RIGHT;
                    }
                    break;

                case Game1.DOWN:
                    //Change the direction if the zombie if its rectangle is partially in the next node and the next node is null or is blocked
                    if (GetRec().Bottom > Game1.SCREEN_HEIGHT || (IsBlocked(facingNode) && GetRec().Bottom > curNode.GetRec().Bottom))
                    {
                        //Set the direction to the left
                        dir = Game1.LEFT;
                    }
                    break;

                case Game1.LEFT:
                    //Change the direction if the zombie if its rectangle is partially in the next node and the next node is null or is blocked
                    if (GetRec().X <= 0 || (IsBlocked(facingNode) && GetRec().X <= curNode.GetRec().X))
                    {
                        //Set the direction to up
                        dir = Game1.UP;
                    }
                    break;

                case Game1.RIGHT:
                    //Change the direction if the zombie if its rectangle is partially in the next node and the next node is null or is blocked
                    if (GetRec().Right >= Game1.SCREEN_WIDTH || (IsBlocked(facingNode) && GetRec().X >= curNode.GetRec().X))
                    {
                        //Set the direction to down
                        dir = Game1.DOWN;
                    }
                    break;
            }
        }

        //Pre: facing node is a Node that the zombie is facing
        //Post: Returns whether the node is blocked or null
        //Desc: Returns whether the node that the zombie is facing is blocked
        private bool IsBlocked(Node facingNode)
        {
            //Return whether the facing node is blocked or null
            return facingNode != null && (facingNode.GetItemOnSpace() == Game1.WALL_H || facingNode.GetItemOnSpace() == Game1.WALL_V || facingNode.GetItemOnSpace() == Game1.LOCK || facingNode.GetItemOnSpace() == Game1.BOX || facingNode.GetItemOnSpace() == Game1.CHAIR || facingNode.GetItemOnSpace() == Game1.BARRICADE_OPEN || (facingNode.GetZombiesOnSpace().Count > 0));
        }

        //Pre: gameTime is the GameTime for the game
        //Post: None
        //Desc: Moves the zombie toward its direction
        protected override void Walk(GameTime gameTime)
        {
            //Updates the walk animation
            anims[dir].Update(gameTime);

            //Move the zombie based on its direction
            switch (dir)
            {
                case Game1.UP:
                    //Move the position of the zombie
                    pos.Y = Math.Max(0, pos.Y - (float)(maxSpeed * gameTime.ElapsedGameTime.TotalSeconds));
                    pos.X = curNode.GetRec().X;

                    //Move the zombie back if it is blocked by a zombie
                    if (curNode.GetAdj(dir) != null)
                    {
                        //Move the zombie back if the facing node has a zombie
                        if (curNode.GetAdj(dir).GetZombiesOnSpace().Count > 0)
                        {
                            //Move the zombie directly behind the zombie if the zombies are intersecting
                            if (curNode.GetAdj(dir).GetZombiesOnSpace()[0].GetRec().Y + Node.SIDE_LENGTH > pos.Y)
                            {
                                //Set the y position of the zombie with a minimum distance apart from the zombie above it
                                pos.Y = curNode.GetAdj(dir).GetZombiesOnSpace()[0].GetRec().Y + Node.SIDE_LENGTH;
                            }
                        }
                    }
                    break;

                case Game1.DOWN:
                    //Move the position of the zombie
                    pos.Y = Math.Min(Game1.SCREEN_HEIGHT - GetRec().Height, pos.Y + (float)(maxSpeed * gameTime.ElapsedGameTime.TotalSeconds));
                    pos.X = curNode.GetRec().X;

                    //Move the zombie back if it is blocked by a zombie
                    if (curNode.GetAdj(dir) != null)
                    {
                        //Move the zombie back if the facing node has a zombie
                        if (curNode.GetAdj(dir).GetZombiesOnSpace().Count > 0)
                        {
                            //Move the zombie directly behind the zombie if the zombies are intersecting
                            if (curNode.GetAdj(dir).GetZombiesOnSpace()[0].GetRec().Y - Node.SIDE_LENGTH < pos.Y)
                            {
                                //Set the y position of the zombie with a minimum distance apart from the zombie below it
                                pos.Y = curNode.GetAdj(dir).GetZombiesOnSpace()[0].GetRec().Y - Node.SIDE_LENGTH;
                            }
                        }
                    }
                    break;

                case Game1.LEFT:
                    //Move the position of the zombie
                    pos.X = Math.Max(0, pos.X - (float)(maxSpeed * gameTime.ElapsedGameTime.TotalSeconds));
                    pos.Y = curNode.GetRec().Bottom - GetRec().Height;

                    //Move the zombie back if it is blocked by a zombie
                    if (curNode.GetAdj(dir) != null)
                    {
                        //Move the zombie back if the facing node has a zombie
                        if (curNode.GetAdj(dir).GetZombiesOnSpace().Count > 0)
                        {
                            //Move the zombie directly behind the zombie if the zombies are intersecting
                            if (curNode.GetAdj(dir).GetZombiesOnSpace()[0].GetRec().X + Node.SIDE_LENGTH > pos.X)
                            {
                                //Set the y position of the zombie with a minimum distance apart from the zombie to its left
                                pos.X = curNode.GetAdj(dir).GetZombiesOnSpace()[0].GetRec().X + Node.SIDE_LENGTH;
                            }
                        }
                    }
                    break;

                case Game1.RIGHT:
                    //Move the position of the zombie
                    pos.X = Math.Min(Game1.SCREEN_WIDTH - GetRec().Width, pos.X + (float)(maxSpeed * gameTime.ElapsedGameTime.TotalSeconds));
                    pos.Y = curNode.GetRec().Bottom - GetRec().Height;

                    //Move the zombie back if it is blocked by a zombie
                    if (curNode.GetAdj(dir) != null)
                    {
                        //Move the zombie back if the facing node has a zombie
                        if (curNode.GetAdj(dir).GetZombiesOnSpace().Count > 0)
                        {
                            //Move the zombie directly behind the zombie if the zombies are intersecting
                            if (curNode.GetAdj(dir).GetZombiesOnSpace()[0].GetRec().X - Node.SIDE_LENGTH < pos.X)
                            {
                                //Set the y position of the zombie with a minimum distance apart from the zombie to its right
                                pos.X = curNode.GetAdj(dir).GetZombiesOnSpace()[0].GetRec().X - Node.SIDE_LENGTH;
                            }
                        }
                    }
                    break;
            }

            //move the walk animations to its position
            anims[Game1.LEFT].destRec.Location = pos.ToPoint();
            anims[Game1.RIGHT].destRec.Location = pos.ToPoint();
            anims[Game1.UP].destRec.Location = pos.ToPoint();
            anims[Game1.DOWN].destRec.Location = pos.ToPoint();
        }

        //Pre: None
        //Post: None
        //Desc: Updates the visibility of the zombie by increasing or decreasing the visibility
        protected void UpdateVisibility()
        {
            //Increase the visibility based on if the zombie is visible
            if (isVisible && visibility < 1f)
            {
                //Increase the visibility
                visibility = Math.Min(1f, VISBILITY_CHANGE + visibility);
            }
            else if (!isVisible && visibility > 0f)
            {
                //Decrease the visibility
                visibility = Math.Max(0f, visibility - VISBILITY_CHANGE);
            }
        }

        //Pre: gameTime is the GameTime for the game, player is the Player, and nodeMap is the map of all the nodes in the world
        //Post: None
        //Desc: Updates the zombie
        public override void Update(GameTime gameTime, Node[,] nodeMap, Player player)
        {
        }

        //Pre: gameTime is the GameTime for the game, player is the Player
        //Post: None
        //Desc: Updates the chasing state
        protected void UpdateChasing(GameTime gameTime, Player player)
        {
            //Find a path for chasing
            chasingPath = FindPath(Level.nodeMap, player.GetCurNode());

            //Chase the player along the chasing path if there is more than 1 node in the path
            if (chasingPath.Count > 1)
            {
                //Set the next node to the first node in the chasing path
                Node directNode = chasingPath[0];

                //Determine the direction towards the next node
                for (int i = 0; i < 4; i++)
                {
                    //Set the direction if the second node in the chasing path is equal to the adjacent node in the direction
                    if (chasingPath[1] == directNode.GetAdj(i))
                    {
                        //Set the direction of the adjacent node
                        dir = i;
                    }
                }

                //Move the zombie
                Walk(gameTime);

                //Remove the current node if the zombie rectangle is past the next node in the chasing path
                switch (dir)
                {
                    case Game1.LEFT:
                        //Remove the current node if the position is left of the next node in the path
                        if (pos.X <= chasingPath[1].GetRec().X)
                        {
                            //Remove the first node in the chasing path
                            chasingPath.RemoveAt(0);
                        }
                        break;

                    case Game1.RIGHT:
                        //Remove the current node if the position is right of the next node in the path
                        if (pos.X >= chasingPath[1].GetRec().X)
                        {
                            //Remove the first node in the chasing path
                            chasingPath.RemoveAt(0);
                        }
                        break;

                    case Game1.UP:
                        //Remove the current node if the position is above the next node in the path
                        if (pos.Y + GetRec().Height - Node.SIDE_LENGTH <= chasingPath[1].GetRec().Y)
                        {
                            //Remove the first node in the chasing path
                            chasingPath.RemoveAt(0);
                        }
                        break;

                    case Game1.DOWN:
                        //Remove the current node if the position is below the next node in the path
                        if (pos.Y + GetRec().Height - Node.SIDE_LENGTH >= chasingPath[1].GetRec().Y)
                        {
                            //Remove the first node in the chasing path
                            chasingPath.RemoveAt(0);
                        }
                        break;
                }
            }
        }

        //Pre: nodeMap is a 2D array containing all the nodes in the world and player is the Player
        //Post: None
        //Desc: Determine if the zombie is visible by the player
        protected void DetermineVisibility(Player player, Node[,] nodeMap)
        {
            //Store the change in the other dimension (row or column) based on an increase in the first dimension (row or column) and the current row and column for examination
            float slope;
            int curRow;
            int curCol;

            //Set the visibility to false
            isVisible = false;

            //Only determine visibility if the player's node is not null
            if (player.GetCurNode() != null)
            {
                //Determine if the zombie is within the visibility radius of the player
                if (Math.Sqrt(Math.Pow(player.GetCurNode().GetRow() - curNode.GetRow(), 2) + Math.Pow(player.GetCurNode().GetCol() - curNode.GetCol(), 2)) < VISIBLE_RADIUS)
                {
                    //Set the visibility to true
                    isVisible = true;

                    //Only determine if there is a wall blocking the sight if the player's and zombie's column is not the same
                    if (player.GetCurNode().GetCol() != curNode.GetCol())
                    {
                        //Set the slope
                        slope = Math.Abs((player.GetCurNode().GetRow() - curNode.GetRow()) / (float)(player.GetCurNode().GetCol() - curNode.GetCol()));

                        //Determine if there is a wall on the nodes that lie along the diagonal from the player to the zombie's current node
                        for (int colDiff = 1; colDiff < Math.Abs(player.GetCurNode().GetCol() - GetCurNode().GetCol()); colDiff++)
                        {
                            //Change the column by 1 and change the row based on the slope and the change in the column
                            curCol = curNode.GetCol() + Math.Sign(player.GetCurNode().GetCol() - curNode.GetCol()) * colDiff;
                            curRow = (int)Math.Round(curNode.GetRow() + Math.Sign(player.GetCurNode().GetRow() - curNode.GetRow()) * colDiff * slope);

                            //Set the visibility of the zombie if the node has a wall
                            if (nodeMap[curRow, curCol].GetItemOnSpace() == Game1.WALL_H || nodeMap[curRow, curCol].GetItemOnSpace() == Game1.WALL_V)
                            {
                                //Set the zombie as not visible
                                isVisible = false;
                                break;
                            }
                        }
                    }

                    //Only determine if there is a wall blocking the sight if the player's and zombie's row is not the same
                    if (player.GetCurNode().GetRow() != curNode.GetRow() && isVisible)
                    {
                        //Set the slope
                        slope = Math.Abs((player.GetCurNode().GetCol() - curNode.GetCol()) / (float)(player.GetCurNode().GetRow() - curNode.GetRow()));

                        //Determine if there is a wall on the nodes that lie along the diagonal from the player to the zombie's current node
                        for (int rowDiff = 1; rowDiff < Math.Abs(player.GetCurNode().GetRow() - GetCurNode().GetRow()); rowDiff++)
                        {
                            //Change the row by 1 and change the column based on the slope and the change in the row
                            curRow = curNode.GetRow() + Math.Sign(player.GetCurNode().GetRow() - curNode.GetRow()) * rowDiff;
                            curCol = (int)Math.Round(curNode.GetCol() + Math.Sign(player.GetCurNode().GetCol() - curNode.GetCol()) * rowDiff * slope);

                            //Set the visibility of the zombie if the node has a wall
                            if (nodeMap[curRow, curCol].GetItemOnSpace() == Game1.WALL_H || nodeMap[curRow, curCol].GetItemOnSpace() == Game1.WALL_V)
                            {
                                //Set the zombie as not visible
                                isVisible = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        //Pre: nodeMap is a 2D array containing all the nodes in the world and playerNode is the node that the player is on
        //Post: None
        //Desc: Determine if the zombie is visible by the player
        protected List<Node> FindPath(Node[,] map, Node playerNode)
        {
            //Store the resulting path and the open and closed list
            List<Node> result = new List<Node>();
            List<Node> open = new List<Node>();
            List<Node> closed = new List<Node>();

            //Store the minimum F cost
            float minF;

            //Store and set the current path node as nothing and store the next path node for if there is a path found
            Node pathNode = null;
            Node nextPathNode;

            //Set the G cost of the current node to 0 and the F cost to the sum of the G and H cost
            curNode.SetG(0);
            curNode.SetF(curNode.GetG() + curNode.GetH());

            //Set the parent of the current node to null
            curNode.SetParent(null);

            //Add the current node to the open list
            open.Add(curNode);

            //Loop through all adjacent nodes as long as there is a pathway
            while (true)
            {
                //Set the path node to null and
                pathNode = null;
                minF = 100000f;

                //Determine the node with the lowest F cost
                foreach (Node i in open)
                {
                    //Set the path node to the current node if it has the lower F cost
                    if (i.GetF() < minF)
                    {
                        //Set the minimum F cost and set the path node
                        minF = i.GetF();
                        pathNode = i;
                    }
                }

                //Add the path node to the closed list if it is not null
                if (pathNode != null)
                {
                    //Remove the path node from the open list and add it to the closed list
                    open.Remove(pathNode);
                    closed.Add(pathNode);

                    //Escape the loop if the path node is the payer node
                    if (pathNode == playerNode)
                    {
                        //Escape the loop
                        break;
                    }

                    //Add the adjacent nodes to the determine if they are an open path
                    for (int i = 0; i < 4; i++)
                    {
                        //Go through the adjacent node if it is not null
                        if (pathNode.GetAdj(i) != null)
                        {
                            //Go through the current adjacent node
                            GoThroughAdjNode(pathNode.GetAdj(i), pathNode, playerNode, open, closed);
                        }
                    }

                    //Escape the loop if there is no more nodes in the open list
                    if (open.Count == 0)
                    {
                        //Escape the loop
                        break;
                    }
                }
                else
                {
                    //Escape the loop
                    break;
                }
            }

            //Add the path to the resulting list if it contains the player node
            if (closed.Contains(playerNode))
            {
                //Set the next path node to the player's node and add the player's node
                nextPathNode = playerNode;
                result.Add(playerNode);

                //Add the parent of the next path node to the result while there is still a parent
                while (nextPathNode.GetParent() != null)
                {
                    //Add the parent of the next path node and set the next path node to the parent
                    result.Insert(0, nextPathNode.GetParent());
                    nextPathNode = nextPathNode.GetParent();
                }
            }

            //Return the resulting list
            return result;
        }

        //Pre: adjacent is the adjacent node of the curNode, curNode is the current node, playerCurNode is the player's node, and open and closed are lists of nodes
        //Post: None
        //Desc: Determine if the zombie is visible by the player
        private void GoThroughAdjNode(Node adjacent, Node curNode, Node playerCurNode, List<Node> open, List<Node> closed)
        {
            //Store the new G cost 
            float newG;

            //Only add the adjacent node to the open list if it does not contain a wall or a barricade or a zombie
            if (adjacent.GetItemOnSpace() != Game1.WALL_H && adjacent.GetItemOnSpace() != Game1.WALL_V && adjacent.GetItemOnSpace() != Game1.BARRICADE_OPEN && !closed.Contains(adjacent) && adjacent.GetZombiesOnSpace().Count == 0)
            {
                //Set the new G cost for the adjacent node
                newG = curNode.GetG() + Level.ITEM_COSTS[adjacent.GetItemOnSpace()];

                //Add the adjacent node to the open node if it not in the open list
                if (!open.Contains(adjacent))
                {
                    //Set the parent of the adjacent node to the current node and set the G and F cost 
                    adjacent.SetParent(curNode);
                    adjacent.SetG(newG);
                    adjacent.SetF(adjacent.GetG() + adjacent.GetH());

                    //Add the adjacent node to the open list
                    open.Add(adjacent);
                }
                else
                {
                    //Set the new G cost if its cost is less than its original G cost
                    if (newG < adjacent.GetG())
                    {
                        //Set the parent of the adjacent node and set its G and F costs
                        adjacent.SetParent(curNode);
                        adjacent.SetG(newG);
                        adjacent.SetF(adjacent.GetG() + adjacent.GetH());
                    }
                }
            }
        }
    }
}

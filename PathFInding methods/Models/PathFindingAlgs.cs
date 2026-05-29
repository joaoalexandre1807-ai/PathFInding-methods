using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using PathFInding_methods;

namespace PathFInding_methods.Models
{
    class PathFindingAlgs
    {



        public static List<AStarNode> AStar(int[] start, int[] end, bool[,] walls)
        {

            PriorityQueue<AStarNode, double> openSet = new PriorityQueue<AStarNode, double>();
            AStarNode[,] nodesGrid = new AStarNode[walls.GetLength(0), walls.GetLength(1)];
            openSet.Enqueue(new AStarNode(start, end, start, null), 0);

            while (openSet.Count > 0)
            {
                AStarNode currentNode = openSet.Dequeue();

                if (currentNode.X == end[0] && currentNode.Y == end[1])
                {
                    // Path found, reconstruct the path
                    List<AStarNode> path = new List<AStarNode>();
                    while (currentNode != null)
                    {
                        path.Add(currentNode);
                        currentNode = currentNode.Parent;
                    }
                    path.Reverse();
                    // Do something with the path
                    return path;
                    
                }

                //currentNode.add_Neighbor_Node(currentNode.X + 1, currentNode.Y, walls, nodesGrid, openSet, end);// right
                //currentNode.add_Neighbor_Node(currentNode.X - 1, currentNode.Y, walls, nodesGrid, openSet, end);// left
                //currentNode.add_Neighbor_Node(currentNode.X, currentNode.Y - 1, walls, nodesGrid, openSet, end);// up
                //currentNode.add_Neighbor_Node(currentNode.X, currentNode.Y + 1, walls, nodesGrid, openSet, end);// down
                //currentNode.add_Neighbor_Node(currentNode.X + 1, currentNode.Y - 1, walls, nodesGrid, openSet, end);// right up
                //currentNode.add_Neighbor_Node(currentNode.X + 1, currentNode.Y + 1, walls, nodesGrid, openSet, end);// right down
                //currentNode.add_Neighbor_Node(currentNode.X - 1, currentNode.Y - 1, walls, nodesGrid, openSet, end);// left up
                //currentNode.add_Neighbor_Node(currentNode.X - 1, currentNode.Y + 1, walls, nodesGrid, openSet, end);// left down


            }

            return null; // No path found
        }

        public static List<GreedyNode> Greedy(bool[,] walls_grid, int[] startPos, int[] endPos, int grid_Width, int grid_Height)
        {
            bool[,] visited = new bool[grid_Width, grid_Height];
            List<GreedyNode> path = new List<GreedyNode>();
            Stack<GreedyNode> possible_Nodes = new Stack<GreedyNode>();

            GreedyNode currentNode = new GreedyNode(startPos, Math.Sqrt(Math.Pow(Math.Abs(endPos[0] - startPos[0]), 2) + Math.Pow(Math.Abs(endPos[1] - startPos[1]), 2)));
            possible_Nodes.Push(currentNode);
            visited[currentNode.X, currentNode.Y] = true;

            while (possible_Nodes.Count > 0  &&  possible_Nodes.Peek().Distance > 0)
            {
                currentNode = possible_Nodes.Pop();
                currentNode.add_Adjacent_Nodes(walls_grid, visited, possible_Nodes, grid_Width, grid_Height, endPos);

            }

            currentNode = possible_Nodes.Peek();

            while (currentNode.Direction_From_Previous_Node != "Start")
            {
                path.Add(currentNode);
                switch (currentNode.Direction_From_Previous_Node)
                {
                    case "right":
                        currentNode = currentNode.Right_Node;
                        break;
                    case "left":
                        currentNode = currentNode.Left_Node;
                        break;
                    case "up":
                        currentNode = currentNode.Up_Node;
                        break;
                    case "down":
                        currentNode = currentNode.Down_Node;
                        break;
                }
            }
            path.Add(currentNode);
            return path;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace PathFInding_methods.Models
{
    internal class GreedyNode
    {

        public int X { get; set; }
        public int Y { get; set; }
        public double Distance { get; set; }

        public GreedyNode Right_Node { get; set; }
        public GreedyNode Left_Node { get; set; }
        public GreedyNode Up_Node { get; set; }
        public GreedyNode Down_Node { get; set; }
        public string Direction_From_Previous_Node { get; set; }

        List<GreedyNode> _adjacentNodes = new List<GreedyNode>();

        private double[] _nodesHeuristics = new double[4];
        public GreedyNode(int[] pos, double heuristic, string from="", GreedyNode sender=null)
        {
            X = pos[0];
            Y = pos[1];

            Distance = heuristic;
            
            switch (from)
            {
                case "right":
                    Direction_From_Previous_Node = "left";
                    Left_Node = sender;
                    break;
                case "left":
                    Direction_From_Previous_Node = "right";
                    Right_Node = sender;
                    break;
                case "up":
                    Direction_From_Previous_Node = "down";
                    Down_Node = sender;
                    break;
                case "down":
                    Direction_From_Previous_Node = "up";
                    Up_Node = sender;
                    break;
                default:
                    Direction_From_Previous_Node = "Start";
                    break;
            }
        }


        public void add_Adjacent_Nodes(bool[,] walls_grid, bool[,] visited, Stack<GreedyNode> possible_Nodes, int grid_Width, int grid_Height, int[] endPos)
        {
            // This function adds the adjacent nodes to the stack of possible nodes, but it adds the one with the lowest heuristic first, and it also checks if the node is already visited or not, and it also checks if the node is within the grid or not
            if (X + 1 < grid_Width && !visited[X + 1, Y] && !walls_grid[X + 1, Y]) // right
            {
                _nodesHeuristics[0] = Math.Sqrt(Math.Pow(Math.Abs(X + 1 - endPos[0]), 2) + Math.Pow(Math.Abs(Y - endPos[1]), 2));
                Right_Node = new GreedyNode(new int[] { X + 1, Y }, _nodesHeuristics[0], "right", this);
                visited[X + 1, Y] = true;
            }
            if (X - 1 >= 0 && !visited[X - 1, Y] && !walls_grid[X - 1, Y]) // left
            {
                _nodesHeuristics[1] = Math.Sqrt(Math.Pow(Math.Abs(X - 1 - endPos[0]), 2) + Math.Pow(Math.Abs(Y - endPos[1]), 2));
                Left_Node = new GreedyNode(new int[] { X - 1, Y }, _nodesHeuristics[1], "left", this);
                visited[X - 1, Y] = true;
            }
            if (Y - 1 >= 0 && !visited[X, Y - 1] && !walls_grid[X, Y - 1]) // up
            {
                _nodesHeuristics[2] = Math.Sqrt(Math.Pow(Math.Abs(X - endPos[0]), 2) + Math.Pow(Math.Abs(Y - 1 - endPos[1]), 2));
                Up_Node = new GreedyNode(new int[] { X, Y - 1 }, _nodesHeuristics[2], "up", this);
                visited[X, Y - 1] = true;
            }
            if (Y + 1 < grid_Height && !visited[X, Y + 1] && !walls_grid[X, Y + 1]) // down
            {
                _nodesHeuristics[3] = Math.Sqrt(Math.Pow(Math.Abs(X - endPos[0]), 2) + Math.Pow(Math.Abs(Y + 1 - endPos[1]), 2));
                Down_Node = new GreedyNode(new int[] { X, Y + 1 }, _nodesHeuristics[3], "down", this);
                visited[X, Y + 1] = true;
            }

            // Now we have the adjacent nodes, we need to add them to the stack of possible nodes, but we need to add the one with the lowest heuristic first, and we also need to check if the node is already visited or not, and we also need to check if the node is within the grid or not
            for (int i = 0; i < 4; i++) 
            {
                
                switch (i)
                {
                    case 0:
                        if (Right_Node != null  &&  Direction_From_Previous_Node != "right")
                            _adjacentNodes.Add(Right_Node);
                        break;
                    case 1:
                        if (Left_Node != null && Direction_From_Previous_Node != "left")
                            _adjacentNodes.Add(Left_Node);
                        break;
                    case 2:
                        if (Up_Node != null && Direction_From_Previous_Node != "up")
                            _adjacentNodes.Add(Up_Node);
                        break;
                    case 3:
                        if (Down_Node != null && Direction_From_Previous_Node != "down")
                            _adjacentNodes.Add(Down_Node);
                        break;
                }
            }

            while (_adjacentNodes.Count > 0) // add the adjacent nodes to the stack of possible nodes, but add the one with the lowest heuristic first
            {
                double max_Heuristic = double.MinValue;
                GreedyNode node_To_Add = null;
                foreach (var node in _adjacentNodes)
                {
                    if (node.Distance > max_Heuristic)
                    {
                        max_Heuristic = node.Distance;
                        node_To_Add = node;
                    }
                }
                possible_Nodes.Push(node_To_Add);// add the node with the lowest heuristic to the top of the stack of possible nodes
                _adjacentNodes.Remove(node_To_Add);
                AddNodeToCanvas(node_To_Add.X, node_To_Add.Y);
            }

        }

        private void AddNodeToCanvas(int x, int y)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.Drawing_Unsearched_Node(x, y);
            });


        }
    }
}

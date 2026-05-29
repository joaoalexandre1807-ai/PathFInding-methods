using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PathFInding_methods.Models
{
    internal class AStarNode
    {
        public double F_cost { get; set; }
        public double G_cost { get; set; }
        public double H_cost { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsOpen { get; set; }
        public bool IsClosed { get; set; }
        public AStarNode Parent { get; set; }

        public AStarNode(int[] pos, int[] end, int[] start, AStarNode from)
        {
            X = pos[0];
            Y = pos[1];
            if (from != null)
            {
                G_cost = Math.Floor(Math.Sqrt(Math.Pow(Math.Abs(start[0] - X), 2) + Math.Pow(Math.Abs(start[1] - Y), 2)) * 10);
                H_cost = Math.Floor(Math.Sqrt(Math.Pow(Math.Abs(end[0] - X), 2) + Math.Pow(Math.Abs(end[1] - Y), 2)) * 10);
                F_cost = G_cost + H_cost;
                IsOpen = true;
                Parent = from;
            }
            else
            {
                G_cost = 0;
                H_cost = Math.Floor(Math.Sqrt(Math.Pow(Math.Abs(end[0] - X), 2) + Math.Pow(Math.Abs(end[1] - Y), 2)) * 10);
                F_cost = G_cost + H_cost;
                IsOpen = true;
                Parent = null;
            }
        }

        public void add_Neighbor_Node(int x, int y, bool[,] wall_grid, AStarNode[,] nodes_grid, PriorityQueue<AStarNode, double> Open, int[] end, int[] start)
        {
            if (x < 0 || x >= wall_grid.GetLength(0) || y < 0 || y >= wall_grid.GetLength(1))
            {
                return;
            }

            if (wall_grid[x, y])
            {
                return;
            }

            if (nodes_grid[x, y] != null && nodes_grid[x, y].IsClosed)
            {
                return;
            }

            if (nodes_grid[x, y] == null)
            {
                nodes_grid[x, y] = new AStarNode(new int[] { x, y }, end, start, this);
                Open.Enqueue(nodes_grid[x, y], nodes_grid[x, y].F_cost);
                AddNodeToCanvas(x, y);
            }
            if (nodes_grid[x, y].G_cost > G_cost + Math.Sqrt(Math.Pow(Math.Abs(X - x), 2) + Math.Pow(Math.Abs(Y - y), 2)))
            {
                nodes_grid[x, y].G_cost = G_cost + Math.Sqrt(Math.Pow(Math.Abs(X - x), 2) + Math.Pow(Math.Abs(Y - y), 2));
                nodes_grid[x, y].F_cost = nodes_grid[x, y].G_cost + nodes_grid[x, y].H_cost;
                nodes_grid[x, y].Parent = this;

                AStarNode temp;
                double temp_double;
                Open.Remove(nodes_grid[x, y], out temp, out temp_double);

                Open.Enqueue(nodes_grid[x, y], nodes_grid[x, y].F_cost);

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

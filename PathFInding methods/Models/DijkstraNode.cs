using PathFInding_methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PathFInding_methods.Models
{
    class DijkstraNode 
    {
        public DijkstraNode(int x, int y) { 
            X = x; Y = y;
            prevNode = this;
            Direction_From_The_Previous_Node = "None";
            Distance_From_the_Start = 0;
        }
        public DijkstraNode(DijkstraNode prev, string dir)
        {
            if (dir == "Up") { X = prev.X; Y = prev.Y - 1; Direction_From_The_Previous_Node = "Down"; }
            else if (dir == "Down") { X = prev.X; Y = prev.Y + 1; Direction_From_The_Previous_Node = "Up"; }
            else if (dir == "Left") { X = prev.X - 1; Y = prev.Y; Direction_From_The_Previous_Node = "Right"; }
            else if (dir == "Right") { X = prev.X + 1; Y = prev.Y; Direction_From_The_Previous_Node = "Left"; }
            else { X = prev.X; Y = prev.Y; Direction_From_The_Previous_Node = "None"; }

            prevNode = prev;
            Distance_From_the_Start = prev.Distance_From_the_Start + 1;
        }
        public int X { get; set; }
        public int Y { get; set; }

        public int Distance_From_the_Start { get; set; }

        public string Direction_From_The_Previous_Node { get; set; }
        public DijkstraNode prevNode { get; set; }


        public void dijstraPathFinding(bool[,] walls_grid, bool[,] visited, int width, int height, Queue<DijkstraNode> unsearcheds)
        {

            // Check the four adjacent nodes and add them to the queue if they are valid and not visited
            // Sleep for 50 milliseconds to slow down the visualization

            if (X - 1 >= 0 && X - 1 < width && !walls_grid[X - 1, Y] && !visited[X - 1, Y]) { unsearcheds.Enqueue(new DijkstraNode(this, "Left")); visited[X - 1, Y] = true; AddNodeToCanvas(X - 1, Y); }
            if (X + 1 >= 0  &&  X + 1 < width  &&  !walls_grid[X + 1, Y]  &&  !visited[X + 1, Y]) { unsearcheds.Enqueue(new DijkstraNode(this, "Right")); visited[X + 1, Y] = true; AddNodeToCanvas(X + 1, Y); }
            if (Y - 1 >= 0  &&  Y - 1 < height  &&  !walls_grid[X, Y - 1]  &&  !visited[X, Y - 1]) { unsearcheds.Enqueue(new DijkstraNode(this, "Up")); visited[X, Y - 1] = true; AddNodeToCanvas(X, Y - 1); }
            if (Y + 1 >= 0  &&  Y + 1 < height  &&  !walls_grid[X, Y + 1]  &&  !visited[X, Y + 1]) { unsearcheds.Enqueue(new DijkstraNode(this, "Down")); visited[X, Y + 1] = true; AddNodeToCanvas(X, Y + 1); }
        }

        public void ReturnPath(List<DijkstraNode> path)
        {
            if (Direction_From_The_Previous_Node == "None")
            {
                path.Add(this);
            }
            else
            {
                path.Add(this);
                prevNode.ReturnPath(path);
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

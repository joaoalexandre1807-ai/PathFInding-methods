using PathFInding_methods.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace PathFInding_methods
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LoopSetUp();
        }

        public bool isDraggingStart = false;
        public bool isDraggingEnd = false;
        public bool isDrawingWall = false;
        public bool isDeletingWall = false;
        public string Mode = "NEditSE";
        public const int taille = 40;
        const int endAndStartSize1 = taille;
        const int endAndStartSize2 = taille*2;
        const int lastPosX = (799 / taille) * taille;
        const int lastPosY = (399 / taille) * taille;
        
        public bool[,] grid = new bool[lastPosX / taille + 1, lastPosY / taille + 1]; // Create a 2D array to represent the grid, where each cell can be either empty (false) or occupied by a wall (true)
        public int[] startPos_at_the_grid = new int[2] { 0, 0 }; // Create an array to store the position of the start point
        public int[] endPos_at_the_grid = new int[2] { lastPosX / taille, lastPosY / taille }; // Create an array to store the position of the end point

        //Dijkstra variables
        Queue<DijkstraNode> UnSearched_nodes;
        Queue<DijkstraNode> Searched_nodes;
        DijkstraNode StartNode;
        DijkstraNode CurrentNode;
        bool[,] visited_nodes;
        bool IsDijkstraRunning = false;


        // Greedy variables
        bool[,] visited;
        bool IsGreedyRunning = false;
        Stack<GreedyNode> possible_Nodes;
        GreedyNode currentNode;


        // AStar variables
        PriorityQueue<AStarNode, double> openSet;
        AStarNode[,] nodesGrid;
        bool IsAStarRunning = false;


        private void LoopSetUp()
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            Start.Margin = new Thickness(0, 0, 0, 0);
            End.Margin = new Thickness(lastPosX, lastPosY, lastPosX, lastPosY);
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            double mouseX = Mouse.GetPosition(MyCanvas).X;
            double mouseY = Mouse.GetPosition(MyCanvas).Y;
            int posX = (((int)mouseX) / taille) * taille; // Round the mouse position to the nearest multiple of 10
            int posY = (((int)mouseY) / taille) * taille;

            int viewBox_MouseX = int.Max(int.Min(posX, lastPosX), 0);
            int viewBox_MouseY = int.Max(int.Min(posY, lastPosY), 0);

            if (isDrawingWall  &&  Mode == "EditWalls")
            {
                 // Round the mouse position to the nearest multiple of 10
                if (Mouse.DirectlyOver != MyCanvas.Children) // Check if the mouse is not directly over an existing wall to prevent drawing on top of it
                {
                    System.Windows.Shapes.Rectangle wall = new System.Windows.Shapes.Rectangle
                    {
                        Fill = System.Windows.Media.Brushes.Black,
                        Width = taille,
                        Height = taille,
                        Margin = new Thickness(viewBox_MouseX, viewBox_MouseY, viewBox_MouseX, viewBox_MouseY) // Set the position of the wall based on the rounded mouse position
                    };
                    MyCanvas.Children.Add(wall);
                    grid[viewBox_MouseX/taille, viewBox_MouseY / taille] = true; // update the grid to mark the cell as occupied by a wall
                }
                
            }
            if (isDeletingWall  &&  Mode == "EditWalls")
            {
                // Loop through all the children of the canvas and check if any of them are rectangles (walls) that are close enough to the mouse cursor to be deleted
                for (int i = MyCanvas.Children.Count - 1; i >= 0; i--)
                {
                    if (MyCanvas.Children[i] is Rectangle wall)
                    {
                        double wallX = wall.Margin.Left;
                        double wallY = wall.Margin.Top;
                        // Check if the distance between the wall and the mouse cursor is less than a certain threshold (e.g., 10 pixels)
                        if (Math.Sqrt(Math.Pow(wallX - mouseX, 2) + Math.Pow(wallY - mouseY, 2)) < taille)
                        {
                            grid[(int)wallX / taille, (int)wallY / taille] = false; // Update the grid to mark the cell as empty
                            MyCanvas.Children.RemoveAt(i); // Remove the wall from the canvas
                            
                        }
                    }
                }
            }





















            // Dijkstra's algorithm loop implementation
            if (IsDijkstraRunning  &&  (CurrentNode.X != endPos_at_the_grid[0] || CurrentNode.Y != endPos_at_the_grid[1]))
            {
                CurrentNode.dijstraPathFinding(grid, visited_nodes, grid.GetLength(0), grid.GetLength(1), UnSearched_nodes);

                Searched_nodes.Enqueue(CurrentNode);
                Drawing_Unsearched_Node(CurrentNode.X, CurrentNode.Y);
                if (UnSearched_nodes.Count > 0)
                {
                    CurrentNode = UnSearched_nodes.Dequeue();
                }
                else
                {
                    CurrentNode = null;
                    IsDijkstraRunning = false;
                    Mode = "NGo";
                }

            }
            else if (IsDijkstraRunning)
            {
                List<DijkstraNode> Path = [];

                CurrentNode.ReturnPath(Path);

                Algoritm_Running(Path);

                CurrentNode = null;

                IsDijkstraRunning = false;
                Mode = "NGo";
            }










            // Greedy algorithm loop implementation
            if (IsGreedyRunning  &&  possible_Nodes.Count > 0 && possible_Nodes.Peek().Distance > 0)
            {
                currentNode = possible_Nodes.Pop();
                currentNode.add_Adjacent_Nodes(grid, visited, possible_Nodes, grid.GetLength(0), grid.GetLength(1), endPos_at_the_grid);
                Drawing_Unsearched_Node(currentNode.X, currentNode.Y);
                if (possible_Nodes.Count == 0) 
                {
                    IsGreedyRunning = false;
                    currentNode = null;
                    Mode = "NGo";
                }
            }
            else if (IsGreedyRunning  &&  possible_Nodes.Count > 0)
            {
                List<GreedyNode> Path = [];
                GreedyNode tempNode = possible_Nodes.Peek();
                while (tempNode.Direction_From_Previous_Node != "Start")
                {
                    Path.Add(tempNode);
                    switch (tempNode.Direction_From_Previous_Node)
                    {
                        case "right":
                            tempNode = tempNode.Right_Node;
                            break;
                        case "left":
                            tempNode = tempNode.Left_Node;
                            break;
                        case "up":
                            tempNode = tempNode.Up_Node;
                            break;
                        case "down":
                            tempNode = tempNode.Down_Node;
                            break;
                    }
                }
                Path.Add(tempNode);
                Algoritm_Running(Path);
                IsGreedyRunning = false;
                Mode = "NGo";
            }






            // AStar algorithm loop
            if (IsAStarRunning  &&  openSet.Count > 0) 
            {
                AStarNode currentNode = openSet.Dequeue();
                currentNode.IsClosed = true;

                if (currentNode.X == endPos_at_the_grid[0] && currentNode.Y == endPos_at_the_grid[1])
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
                    Algoritm_Running(path);
                    IsAStarRunning = false;
                    Mode = "NGo";
                }
                if (Mode == "NoGo")
                {

                    Drawing_Unsearched_Node(currentNode.X, currentNode.Y);
                    currentNode.add_Neighbor_Node(currentNode.X + 1, currentNode.Y, grid, nodesGrid, openSet, endPos_at_the_grid, startPos_at_the_grid);// right
                    currentNode.add_Neighbor_Node(currentNode.X - 1, currentNode.Y, grid, nodesGrid, openSet, endPos_at_the_grid, startPos_at_the_grid);// left
                    currentNode.add_Neighbor_Node(currentNode.X, currentNode.Y - 1, grid, nodesGrid, openSet, endPos_at_the_grid, startPos_at_the_grid);// up
                    currentNode.add_Neighbor_Node(currentNode.X, currentNode.Y + 1, grid, nodesGrid, openSet, endPos_at_the_grid, startPos_at_the_grid);// down
                }
            }
            else if (IsAStarRunning)
            {
                IsAStarRunning = false;
                Mode = "NGo";
            }
























            if (isDraggingStart) // If the start point is being dragged, update its position to follow the mouse cursor
            {
                Start.Width = endAndStartSize2;
                Start.Height = endAndStartSize2;
                Start.Margin = new Thickness(viewBox_MouseX, viewBox_MouseY, viewBox_MouseX, viewBox_MouseY);
            }
            else if (isDraggingEnd)
            {
                End.Width = endAndStartSize2;
                End.Height = endAndStartSize2;
                End.Margin = new Thickness(viewBox_MouseX, viewBox_MouseY, viewBox_MouseX, viewBox_MouseY);
            }
            else // If neither point is being dragged, reset their sizes to the default
            {
                Start.Width = endAndStartSize1;
                Start.Height = endAndStartSize1;
                End.Width = endAndStartSize1;
                End.Height = endAndStartSize1;
            }

            if (Keyboard.IsKeyDown(Key.T))
            {
                Debug_grid();
            }
            if (Keyboard.IsKeyDown(Key.Y))
            {
                Delete_debug_grid();
            }


            if (Mode == "Go" && CurrentNode == null) // Start the pathfinding algorithm when the "S" key is pressed and there is no current node being processed
            {
                delete_path();

                if (AlgoritmsTypes.SelectedIndex == 1)
                {
                    UnSearched_nodes = new Queue<DijkstraNode>();
                    Searched_nodes = new Queue<DijkstraNode>();
                    visited_nodes = new bool[grid.GetLength(0), grid.GetLength(1)];
                    // we will use a queue to store the nodes that we have not yet searched, and a queue to store the nodes that we have already searched
                    // we will also use a 2D array to keep track of which nodes we have visited, so that we don't visit the same node multiple times

                    StartNode = new DijkstraNode(startPos_at_the_grid[0], startPos_at_the_grid[1]);
                    visited_nodes[startPos_at_the_grid[0], startPos_at_the_grid[1]] = true;// we mark the starting node as visited, so that we don't visit it again
                                                                                           // we add the starting node to the queue of unsearched nodes, so that we can start searching from it

                    UnSearched_nodes.Enqueue(StartNode);

                    CurrentNode = UnSearched_nodes.Dequeue();
                    IsDijkstraRunning = true;
                }
                else if (AlgoritmsTypes.SelectedIndex == 2)
                {
                    visited = new bool[grid.GetLength(0), grid.GetLength(1)];
                    possible_Nodes = new Stack<GreedyNode>();

                    currentNode = new GreedyNode(startPos_at_the_grid, Math.Sqrt(Math.Pow(Math.Abs(endPos_at_the_grid[0] - startPos_at_the_grid[0]), 2) + Math.Pow(Math.Abs(endPos_at_the_grid[1] - startPos_at_the_grid[1]), 2)));
                    possible_Nodes.Push(currentNode);
                    visited[currentNode.X, currentNode.Y] = true;

                    IsGreedyRunning = true;
                }
                else if (AlgoritmsTypes.SelectedIndex == 0)
                {
                    openSet = new PriorityQueue<AStarNode, double>();
                    nodesGrid = new AStarNode[grid.GetLength(0), grid.GetLength(1)];
                    openSet.Enqueue(new AStarNode(startPos_at_the_grid, endPos_at_the_grid, startPos_at_the_grid, null), 0);
                    IsAStarRunning = true;
                }
                    Mode = "NoGo";
            }



            return;
            
        }

        private void Algoritm_Running(List<DijkstraNode> Path)
        {
            
       
            foreach (DijkstraNode node in Path)
            {
                System.Windows.Shapes.Rectangle pathPoint = new System.Windows.Shapes.Rectangle
                {
                    Fill = System.Windows.Media.Brushes.LightGreen,
                    Name = "pathPoint",
                    Width = taille,
                    Height = taille,
                    Margin = new Thickness(node.X * taille, node.Y * taille, 0, 0)
                };
                MyCanvas.Children.Add(pathPoint);
            }
        }

        private void Algoritm_Running(List<GreedyNode> Path)
        {
            foreach (GreedyNode node in Path)
            {
                System.Windows.Shapes.Rectangle pathPoint = new System.Windows.Shapes.Rectangle
                {
                    Fill = System.Windows.Media.Brushes.LightGreen,
                    Name = "pathPoint",
                    Width = taille,
                    Height = taille,
                    Margin = new Thickness(node.X * taille, node.Y * taille, 0, 0)
                };
                MyCanvas.Children.Add(pathPoint);
            }
        }

        private void Algoritm_Running(List<AStarNode> Path)
        {
            if (Path == null)
            {
                return;
            }
            foreach (AStarNode node in Path)
            {
                System.Windows.Shapes.Rectangle pathPoint = new System.Windows.Shapes.Rectangle
                {
                    Fill = System.Windows.Media.Brushes.LightGreen,
                    Name = "pathPoint",
                    Width = taille,
                    Height = taille,
                    Margin = new Thickness(node.X * taille, node.Y * taille, 0, 0)
                };
                MyCanvas.Children.Add(pathPoint);
            }
        }

        private void delete_path()
        {
            for (int i = MyCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (MyCanvas.Children[i] is Rectangle pathPoint &&  (pathPoint.Name == "pathPoint"  ||  pathPoint.Name == "unsearchedNode"))
                {
                    MyCanvas.Children.RemoveAt(i);
                }
            }
        }

        private void Delete_debug_grid()
        {
            for (int i = MyCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (MyCanvas.Children[i] is TextBlock textBlock && textBlock.Tag != null && textBlock.Tag.ToString() == "debug")
                {
                    MyCanvas.Children.RemoveAt(i);
                }
            }
        }

        private void Debug_grid()
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    MyCanvas.Children.Add(new TextBlock
                    {
                        Tag = "debug",
                        Text = grid[i, j] ? "1" : "0",
                        Margin = new Thickness(i * taille+(taille/2), j * taille, 0, 0),
                        Foreground = Brushes.White
                    });
                }
            }
        }

        private void Start_MouseLeftButton(object sender, MouseButtonEventArgs e)
        {
            if (!isDraggingEnd && Mode == "EditSE" && Mode != "NoGo")// If the end point is not being dragged, allow dragging the start point
            {
                isDraggingStart = !isDraggingStart;
                startPos_at_the_grid[0] = (int)Start.Margin.Left / taille; // Update the position of the start point in the grid based on its margin
                startPos_at_the_grid[1] = (int)Start.Margin.Top / taille;
            }
        }

        private void End_MouseLeftButton(object sender, MouseButtonEventArgs e)
        {
            if (!isDraggingStart  &&  Mode=="EditSE" && Mode != "NoGo")
            {
                isDraggingEnd = !isDraggingEnd;
                endPos_at_the_grid[0] = (int)End.Margin.Left / taille; // Update the position of the end point in the grid based on its margin
                endPos_at_the_grid[1] = (int)End.Margin.Top / taille;
            }
        }

        private void all_Buttons_lightBlue()// This method is used to reset the background color of all buttons to light blue
        {
            ButtonEditSE.Background = Brushes.LightBlue;
            ButtonEditWalls.Background = Brushes.LightBlue;
            ButtonGo.Background = Brushes.LightGreen;
        }


        private void EditSE_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mode != "EditSE"  &&  Mode != "NoGo")
            {
                Resetting_Buttons();
                ButtonEditSE.Background = Brushes.Gray;
                Mode = "EditSE";
            }
            
        }

        private void EditWalls_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mode != "EditWalls" && Mode != "NoGo")
            {
                Resetting_Buttons();
                ButtonEditWalls.Background = Brushes.Gray;
                Mode = "EditWalls";
            }
        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mode == "EditWalls") 
            {
                isDeletingWall = false;
                isDrawingWall = !isDrawingWall;
                if (isDrawingWall)
                {
                    ButtonEditWalls.Content = "Edit Walls: pencil";
                }
                else
                {
                    ButtonEditWalls.Content = "Edit Walls: none";
                }
            }
        }

        private void MyCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mode == "EditWalls")
            {
                isDrawingWall = false;
                isDeletingWall = !isDeletingWall;
                if (isDeletingWall)
                {
                    ButtonEditWalls.Content = "Edit Walls: rubber";
                }
                else
                {
                    ButtonEditWalls.Content = "Edit Walls: none";
                }

            }
        }

        private void AlgoritmsTypes_Selected(object sender, RoutedEventArgs e)
        {

        }



        public void Drawing_Unsearched_Node(int x, int y) 
        {
            System.Windows.Shapes.Rectangle unsearchedNode = new System.Windows.Shapes.Rectangle
            {
                Name = "unsearchedNode",
                Fill = System.Windows.Media.Brushes.Black,
                Opacity = 0.1,
                Width = taille,
                Height = taille,
                Margin = new Thickness(x * taille, y * taille, 0, 0)
            };

            MyCanvas.Children.Add(unsearchedNode);
        }

        private void ButtonGo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Mode != "Go")
            {
                Resetting_Buttons();
                ButtonGo.Background = Brushes.Gray;
                Mode = "Go";
            }
        }


        private void Resetting_Buttons()
        {
            isDeletingWall = false;
            isDrawingWall = false;
            ButtonEditWalls.Content = "Edit Walls: none";
            delete_path();
            all_Buttons_lightBlue();
        }
    }
}
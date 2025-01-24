using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }

    class Game
    {
        private int width = 20;
        private int height = 10;
        private int score;
        private bool gameOver;
        private int snakeX, snakeY;  // Current position of snake
        private List<int[]> snakeBody;  // List to store body parts of snake
        private int foodX, foodY;  // Position of food
        private Random random;  // To generate random food position
        private string direction = "RIGHT";  // Direction of snake
        private string prevDirection = "RIGHT";
        private int enemyX, enemyY;  // Position of enemy
        private int obstacleX, obstacleY;  // Position of obstacle
        private int speed;  // Snake speed control
        private const int defaultSpeed = 100;  // Default speed
        private const int increasedSpeed = 50;  // Increased speed

        public Game()
        {
            random = new Random();
            snakeBody = new List<int[]>();
            snakeX = width / 2;
            snakeY = height / 2;
            snakeBody.Add(new int[] { snakeX, snakeY });
            score = 0;
            gameOver = false;
            speed = defaultSpeed;
            GenerateFood();
            GenerateEnemy();
            GenerateObstacle();
        }

        public void Start()
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(width + 2, height + 3);  // Set console window size
            Console.SetBufferSize(width + 2, height + 3);  // Set console buffer size

            while (!gameOver)
            {
                Draw();
                Input();
                Logic();
                Thread.Sleep(speed); // Control the speed of the game
            }

            Console.SetCursorPosition(0, height + 1);
            Console.WriteLine("Game Over! Final Score: " + score);
        }

        private void Draw()
        {
            Console.Clear();

            // Draw the top border
            for (int i = 0; i < width + 2; i++)
                Console.Write("#");

            Console.WriteLine();

            // Draw the game area with snake, food, enemy, and obstacle
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == 0 || x == width - 1)
                    {
                        Console.Write("#"); // Draw walls
                    }
                    else if (y == snakeY && x == snakeX)
                    {
                        Console.Write("O"); // Draw snake's head
                    }
                    else if (snakeBody.Any(s => s[0] == x && s[1] == y))
                    {
                        Console.Write("o"); // Draw snake's body
                    }
                    else if (x == foodX && y == foodY)
                    {
                        Console.Write("F"); // Draw food
                    }
                    else if (x == enemyX && y == enemyY)
                    {
                        Console.Write("X"); // Draw enemy
                    }
                    else if (x == obstacleX && y == obstacleY)
                    {
                        Console.Write("H"); // Draw constant obstacle
                    }
                    else
                    {
                        Console.Write(" "); // Empty space
                    }
                }
                Console.WriteLine();
            }

            // Draw the bottom border
            for (int i = 0; i < width + 2; i++)
                Console.Write("#");

            Console.WriteLine();
            Console.SetCursorPosition(0, height + 1);
            Console.WriteLine("Score: " + score);
        }

        private void Input()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow && prevDirection != "DOWN")
                    direction = "UP";
                else if (key.Key == ConsoleKey.DownArrow && prevDirection != "UP")
                    direction = "DOWN";
                else if (key.Key == ConsoleKey.LeftArrow && prevDirection != "RIGHT")
                    direction = "LEFT";
                else if (key.Key == ConsoleKey.RightArrow && prevDirection != "LEFT")
                    direction = "RIGHT";
                else if (key.Key == ConsoleKey.Spacebar)  // Spacebar increases speed
                    speed = increasedSpeed;
            }
        }

        private void Logic()
        {
            prevDirection = direction;
            int prevX = snakeX;
            int prevY = snakeY;
            int[] prevSegment = new int[2];
            snakeBody.Insert(0, new int[] { snakeX, snakeY });

            // Update the snake's head position based on the direction
            switch (direction)
            {
                case "UP":
                    snakeY--;
                    break;
                case "DOWN":
                    snakeY++;
                    break;
                case "LEFT":
                    snakeX--;
                    break;
                case "RIGHT":
                    snakeX++;
                    break;
            }

            // Check if snake eats food
            if (snakeX == foodX && snakeY == foodY)
            {
                score++;
                GenerateFood();
            }
            else
            {
                snakeBody.RemoveAt(snakeBody.Count - 1); // Remove the last segment if no food was eaten
            }

            // Check if snake collides with enemy
            if (snakeX == enemyX && snakeY == enemyY)
            {
                snakeBody.Add(new int[] { prevX, prevY }); // Increase snake size by adding a new body part
                GenerateEnemy(); // Generate a new enemy
            }

            // Check for collisions with walls, self, or the constant obstacle
            if (snakeX < 1 || snakeX >= width - 1 || snakeY < 0 || snakeY >= height || snakeBody.Any(s => s[0] == snakeX && s[1] == snakeY) || (snakeX == obstacleX && snakeY == obstacleY))
            {
                gameOver = true;
            }
        }

        private void GenerateFood()
        {
            foodX = random.Next(1, width - 1);
            foodY = random.Next(0, height);
        }

        private void GenerateEnemy()
        {
            enemyX = random.Next(1, width - 1);
            enemyY = random.Next(0, height);
        }

        private void GenerateObstacle()
        {
            obstacleX = width / 2;
            obstacleY = height / 2;  // Fixed obstacle at center
        }
    }
}

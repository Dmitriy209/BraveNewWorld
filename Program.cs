using System;
using System.IO;
using System.Security.AccessControl;

namespace BraveNewWorld
{
    internal class Program
    {
        static void Main(string[] args)
        {
            char[,] map = ReadMap("map.txt");

            bool isRunning = true;

            int heroPositionX = 0;
            int heroPositionY = 0;

            Console.CursorVisible = false;

            FoundStartPosition(map, ref heroPositionX, ref heroPositionY);

            while (isRunning)
            {
                DrawMap(map);

                DrawHero(ref heroPositionX, ref heroPositionY);

                int nextHeroPositionX = 0;
                int nextHeroPositionY = 0;

                ConsoleKeyInfo pressedKey = Console.ReadKey();
                MoveHero(pressedKey, ref heroPositionX, ref heroPositionY, map, ref nextHeroPositionX, ref nextHeroPositionY);

                WriteMessage(map[nextHeroPositionX, nextHeroPositionY]);

                FinishTheGame(map[nextHeroPositionX, nextHeroPositionY], ref isRunning);
            }

            Console.WriteLine("Игра окончена.");
        }

        private static char[,] ReadMap(string path)
        {
            string[] file = File.ReadAllLines("map.txt");

            char[,] map = new char[GetMaxLengthOfLine(file), file.Length];

            for (int positionX = 0; positionX < map.GetLength(0); positionX++)
                for (int positionY = 0; positionY < map.GetLength(1); positionY++)
                    map[positionX, positionY] = file[positionY][positionX];

            return map;
        }

        private static void DrawMap(char[,] map)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(0, 0);

            for (int positionY = 0; positionY < map.GetLength(1); positionY++)
            {
                for (int positionX = 0; positionX < map.GetLength(0); positionX++)
                {
                    Console.Write(map[positionX, positionY]);
                }

                Console.WriteLine();
            }
        }

        private static int GetMaxLengthOfLine(string[] lines)
        {
            int maxLength = lines[0].Length;

            foreach (var line in lines)
                if (line.Length > maxLength)
                    maxLength = line.Length;

            return maxLength;
        }

        private static void FoundStartPosition(char[,] map, ref int heroPositionX, ref int heroPositionY)
        {
            char symbolStartPosition = 'S';

            int startPositionX = 1;
            int startPositionY = 1;

            int endCyclePositionX = map.GetLength(0) - 1;
            int endCyclePositionY = map.GetLength(1);

            bool haveStartPosition = false;

            for (int positionX = 1; positionX < endCyclePositionX; positionX++)
            {
                for (int positionY = 1; positionY < endCyclePositionY; positionY++)
                {
                    if (map[positionX, positionY] == symbolStartPosition)
                    {
                        heroPositionX = positionX;
                        heroPositionY = positionY;

                        haveStartPosition = true;

                        positionX = endCyclePositionX;
                        positionY = endCyclePositionY;
                    }
                }
            }

            if (haveStartPosition == false)
            {
                heroPositionX = startPositionX;
                heroPositionY = startPositionY;
            }
        }

        private static void MoveHero(ConsoleKeyInfo pressedKey, ref int heroPositionX, ref int heroPositionY, char[,] map, ref int nextHeroPositionX, ref int nextHeroPositionY)
        {
            const char River = '>';
            const char EndMap = '#';

            int[] direction = GetDirection(pressedKey);

            nextHeroPositionX = heroPositionX + direction[0];
            nextHeroPositionY = heroPositionY + direction[1];


            switch (map[nextHeroPositionX, nextHeroPositionY])
            {
                case River:
                    break;

                case EndMap:
                    break;

                default:
                    heroPositionX = nextHeroPositionX;
                    heroPositionY = nextHeroPositionY;
                    break;
            }
        }

        private static int[] GetDirection(ConsoleKeyInfo pressedKey)
        {
            int[] direction = { 0, 0 };

            ConsoleKey keyUpArrow = ConsoleKey.UpArrow;
            ConsoleKey keyDownArrow = ConsoleKey.DownArrow;
            ConsoleKey keyLeftArrow = ConsoleKey.LeftArrow;
            ConsoleKey keyRightArrow = ConsoleKey.RightArrow;

            if (pressedKey.Key == keyUpArrow)
                direction[1] -= 1;
            else if (pressedKey.Key == keyDownArrow)
                direction[1] = 1;
            else if (pressedKey.Key == keyLeftArrow)
                direction[0] -= 1;
            else if (pressedKey.Key == keyRightArrow)
                direction[0] = 1;

            return direction;
        }

        private static void DrawHero(ref int heroPositionX, ref int heroPositionY, string hero = "H", ConsoleColor heroColor = ConsoleColor.Green)
        {
            Console.ForegroundColor = heroColor;
            Console.SetCursorPosition(heroPositionX, heroPositionY);
            Console.Write(hero);
        }

        private static void WriteMessage(char symbol)
        {
            const char Field = ' ';
            const char RoadVertical = '/';
            const char RoadHorizontal = '_';
            const char StartPosition = 'S';
            const char City = 'C';
            const char Ally = 'A';
            const char Fishman = 'F';
            const char Trader = 'T';
            const char DeadMan = 'D';
            const char Enemy = 'E';
            const char BridgeLeft = ')';
            const char BridgeRight = '(';
            const char River = '>';
            const char EndMap = '#';

            string message = "";

            Console.Clear();

            switch (symbol)
            {
                case Field:
                    message = "Вы в поле. Тут ничего нет.";
                    break;

                case RoadVertical:
                    message = "Вы на дороге. Если пойти на север, то дойдёте до города.";
                    break;

                case RoadHorizontal:
                    message = "Вы на дороге. Если пойти на восток, то дойдёте до города.";
                    break;

                case StartPosition:
                    message = "Вам надо в город. Он на северо-востоке.";
                    break;

                case City:
                    message = "Вы дошли до города. На этом игра должна закончиться.";
                    break;

                case Ally:
                    message = "Вы смотрите на фермера. Фермер смотрит на вас. Тут должен быть диалог, но его нет.";
                    break;

                case Fishman:
                    message = "Вы смотрите на рыбака. Не стоит его отвлекать. Вы можете спугнуть рыбу.";
                    break;

                case Trader:
                    message = "Вы смотрите на торговца чем-то. Он подзывает вас. Но у вас нет денег. Да и кто будет в здравом уме торговать посреди поля!?";
                    break;

                case DeadMan:
                    message = "Вы видите человеческие остатки. Похоже рядом кто-то недружелюбный.";
                    break;

                case Enemy:
                    message = "Вы смотрите на тролля. Тролль смотрит на вас. Если хотите с ним подраться, то переходите по ссылке: https://github.com/Dmitriy209/BattleVsBoss.git";
                    break;

                case BridgeLeft:
                    message = "Вы в левой части моста, смотрите как река бежит на вас. Красиво.";
                    break;

                case BridgeRight:
                    message = "Вы в правой части моста, смотрите как река бежит от вас. Красиво. Рыбак вам машет рукой.";
                    break;

                case River:
                    message = "Вы смотрите на реку, там плавают рыбы.\n" +
                        "Где-то неподалёку должен быть мост.";
                    break;

                case EndMap:
                    message = "Тут не то стена, через которую вы не можете пройти, не то стены пещеры.";
                    break;
            }

            int posisionMessageX = 0;
            int posisionMessageY = 20;

            Console.SetCursorPosition(posisionMessageX, posisionMessageY);
            Console.WriteLine(message);
        }

        private static void FinishTheGame(char symbol, ref bool isRunning)
        {
            char symbolEndGame = 'C';

            if (symbol == symbolEndGame)
                isRunning = false;
        }
    }
}

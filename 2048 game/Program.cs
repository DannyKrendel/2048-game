using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Game
{
    class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;

            IMainUserInterface userInterface = new MainUserInterface();
            IGameLoop gameLoop = new GameLoop();
            IGame game = new Game(4, 4, 2);

            userInterface.StartGame(gameLoop, game);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Game
{
    class GameLoop : IGameLoop
    {
        // Launching a game
        public void Run(IGame game)
        {
            game.DisplayField();
            game.DisplayCells();
            game.DisplayStats();

            do
            {
                if (Console.KeyAvailable)
                {
                    game.HandleKey(Console.ReadKey(true));
                    game.DisplayCells();
                    game.DisplayStats();

                    bool isOver = game.IsOver();
                    bool isWon = game.IsWon();

                    if (isOver || isWon)
                    {
                        Console.SetCursorPosition(0, 11);
                        Console.WriteLine(isOver ? "You lost!" : "You won!");
                        Thread.Sleep(2000);
                        break;
                    }
                }
            } while (true);
        }
    }
}

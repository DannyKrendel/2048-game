using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public interface IMainUserInterface
    {
        void StartGame(IGameLoop gameLoop, IGame game);
    }

    public interface IGameLoop
    {
        void Run(IGame game);
    }

    public interface IGame
    {
        void Initialize();
        void DisplayField();
        void DisplayCells();
        void DisplayStats();
        void HandleKey(ConsoleKeyInfo cki);
        bool IsOver();
        bool IsWon();
    }
}

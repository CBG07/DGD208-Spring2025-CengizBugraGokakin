using System;
using System.Threading.Tasks;

namespace DGD208_spring2025_CengizBugraGokakin
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var game = new Game();
            await game.GameLoop();
        }
    }
} 
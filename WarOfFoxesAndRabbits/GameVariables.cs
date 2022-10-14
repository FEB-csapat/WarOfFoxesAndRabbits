using System;

namespace WarOfFoxesAndRabbits
{
    public static class GameVariables
    {
        public static readonly Random Random = new Random();

        public static readonly int CellsHorizontallyCount = 200;
        public static readonly int CellsVerticallyCount = 200;
        public static readonly int CellSize = 4;

        public static readonly int GraphRectSize = 3;

        public static readonly int WindowWidth = CellsVerticallyCount * CellSize + 400;
        public static readonly int WindowHeight = CellsVerticallyCount * CellSize;

        public static readonly string Title = "War of foxes and rabbits";

        public static readonly int GameCanvasWidth = CellsHorizontallyCount * CellSize;
    }
}

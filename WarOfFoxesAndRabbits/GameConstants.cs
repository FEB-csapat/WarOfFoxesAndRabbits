using System;

namespace WarOfFoxesAndRabbits
{
    public static class GameConstants
    {
        public static readonly Random Random = new Random();

        public const int CellsHorizontallyCount = 220;
        public const int CellsVerticallyCount = 220;
        public const int CellSize = 4;

        public const int GraphRectSize = 3;

        public const int WindowWidth = CellsVerticallyCount * CellSize + 400;
        public const int WindowHeight = CellsVerticallyCount * CellSize;

        public const string Title = "War of foxes and rabbits";

        public const int GameCanvasWidth = CellsHorizontallyCount * CellSize;

        public const float minWaterDepth = 0.55f;
    }
}

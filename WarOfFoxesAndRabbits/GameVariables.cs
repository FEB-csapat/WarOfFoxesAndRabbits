using System;

namespace WarOfFoxesAndRabbits
{
    public static class GameVariables
    {
        public static Random Random = new Random();

        public static int CellsHorizontallyCount = 200;
        public static int CellsVerticallyCount = 200;
        public static int CellSize = 4;


        public static int WindowWidth = CellsVerticallyCount * CellSize + 400;
        public static int WindowHeight = CellsVerticallyCount * CellSize;


        public static string Title = "War of foxes and rabbits";

        public static int GetGameCanvasWidth()
        {
            return CellsHorizontallyCount * CellSize;
        }

    }
}



namespace WarOfFoxesAndRabbits
{
    public class GameVariables
    {
        public static int CellsHorizontallyCount = 150;
        public static int CellsVerticallyCount = 150;
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

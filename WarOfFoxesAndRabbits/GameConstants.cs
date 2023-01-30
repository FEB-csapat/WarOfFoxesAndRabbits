using System;

namespace WarOfFoxesAndRabbits
{
    public static class GameConstants
    {
        public static readonly Random Random = new Random();

        public const int CELLS_HORIZONTALLY_COUNT = 210;
        public const int CELLS_VERTICALLY_COUNT = 210;
        public const int CELL_SIZE = 4;

        public const int GRAPH_RECT_SIZE = 3;

        public const int WINDOW_WIDTH = CELLS_VERTICALLY_COUNT * CELL_SIZE + 400;
        public const int WINDOW_HEIGHT = CELLS_VERTICALLY_COUNT * CELL_SIZE;

        public const string TITLE = "War of foxes and rabbits";

        public const int GAME_CANVAS_WIDTH = CELLS_HORIZONTALLY_COUNT * CELL_SIZE;

        public const float MIN_WATER_DEPTH = 0.55f;
    }
}

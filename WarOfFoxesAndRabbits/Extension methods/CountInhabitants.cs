namespace WarOfFoxesAndRabbits
{
    // Extension class
    public static class InhabintantCounter
    {
        public static int CountAnimals(this Cell[,] field)
        {
            int counter = 0;
            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].animal != null && field[x, y].animal is Animal)
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }

        public static int CountRabbits(this Cell[,] field)
        {
            int counter = 0;
            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].animal != null && field[x, y].animal.GetType() == typeof(Rabbit))
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }

        public static int CountFoxes(this Cell[,] field)
        {
            int counter = 0;
            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].animal != null && field[x, y].animal.GetType() == typeof(Fox))
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }
    }
}

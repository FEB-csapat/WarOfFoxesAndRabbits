namespace WarOfFoxesAndRabbits
{
    public static class InhabintantCounter
    {
        public static int CountInhabitants(this Cell[,] field)
        {
            int counter = 0;
            for (int y = 0; y < GameVariables.CellsVerticallyCount; y++)
            {
                for (int x = 0; x < GameVariables.CellsHorizontallyCount; x++)
                {
                    if (field[x, y].inhabitant != null)
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
                    if (field[x, y].inhabitant != null && field[x, y].inhabitant.GetType() == typeof(Rabbit))
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
                    if (field[x, y].inhabitant != null && field[x, y].inhabitant.GetType() == typeof(Fox))
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }
    }
}

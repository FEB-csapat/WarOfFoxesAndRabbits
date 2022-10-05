using System;
using System.Collections.Generic;
using System.Linq;

namespace WarOfFoxesAndRabbits
{
    class RabbitHandler
    {
        static public Cell[,] Check(Cell[,] field, int x, int y)
        {

            if (!field[x, y].inhabitant.hasMoved)
            {
                // rules

                // TODO: If sate=4 cannot eat grass:2
                if (field[x, y].Grass >= 1 && field[x, y].inhabitant.Sate < 5)
                {
                    (field[x, y].inhabitant as Rabbit).Eat(field[x, y].Grass);
                    field[x, y].grassEaten();
                }


                // move 
                if (field[x, y].Grass == 0)
                {

                    List<Cell> surroundingCells = new List<Cell>();

                    for (int py = -1; py <= 1; py++)
                    {
                        for (int px = -1; px <= 1; px++)
                        {

                            if (y + py >= 0 && x + px >= 0
                            && y + py < GameVariables.cellsVerticallyCount && x + px < GameVariables.cellsHorizontallyCount
                            && (px != 0 && py != 0))
                            {
                                if (field[x + px, y + py].inhabitant == null)
                                {
                                    surroundingCells.Add(field[x + px, y + py]);
                                }
                            }

                        }
                    }

                    if (surroundingCells.Count != 0)
                    {
                        List<Cell> optionalCells = surroundingCells.Where(x => x.Grass == surroundingCells.Max(y => y.Grass)).ToList();

                        int ran = new Random().Next(0, optionalCells.Count);

                        optionalCells[ran].inhabitant = field[x, y].inhabitant;
                        field[x, y].inhabitant = null;
                        optionalCells[ran].inhabitant.hasMoved = true;
                    }
                }
            }
            field[x, y].Update();
            return field;
        }

        public void Die()
        {
            throw new NotImplementedException();
        }

        public void Eat()
        {
            throw new NotImplementedException();
        }

        public void Move()
        {
            throw new NotImplementedException();
        }

        public void Reproduce()
        {
            throw new NotImplementedException();
        }
    }

    // manages the game's rules
    class GameManager
    {
        // returns the modified field
        public static Cell[,] Update(Cell[,] field)
        {

            for (int y = 0; y < field.GetLength(0); y++)
            {
                for (int x = 0; x < field.GetLength(1); x++)
                {
                    if (field[x, y].inhabitant != null)
                    {
                        if (field[x, y].inhabitant is Rabbit)
                        {
                            field = RabbitHandler.Check(field, x, y);

                        }
                        else if (field[x, y].inhabitant is Fox)
                        {



                        }
                    }

                    if (field[x, y].inhabitant == null)
                    {
                        field[x, y].Grow();
                    }

                }
            }


            for (int y = 0; y < field.GetLength(0); y++)
            {
                for (int x = 0; x < field.GetLength(1); x++)
                {
                    if (field[x, y].inhabitant != null)
                        field[x, y].inhabitant.hasMoved = false;


                }
            }

            return field;
        }
    }
}

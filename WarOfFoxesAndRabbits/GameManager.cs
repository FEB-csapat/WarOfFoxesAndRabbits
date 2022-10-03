using System;

namespace WarOfFoxesAndRabbits
{
    class RabbitHandler
    {
        static public GrassCell[,] Check( ref GrassCell[,] field, int x, int y)
        {
            // rules
            
            if (field[x,y].Grass >= 1)
            {
                (field[x, y].inhabitant as Rabbit).Eat(field[x, y].Grass);
            }



            // at the end don't forget to Move the rabbit

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
        public static GrassCell[,] Update(GrassCell[,] field)
        {

            for (int y = 0; y < field.GetLength(0); y++)
            {
                for (int x = 0; x < field.GetLength(1); x++)
                {
                    if (field[x,y].inhabitant is Rabbit)
                    {
                        RabbitHandler.Check(ref field, x, y);


                        field[x, y].Update();
                    }
                }
            }
            return field;
        }
    }
}

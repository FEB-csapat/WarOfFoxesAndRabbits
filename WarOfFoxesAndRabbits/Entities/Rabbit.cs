﻿using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    class Rabbit : Animal
    {
        private Color color;
        public Rabbit()
        {
            sate = 3;
            age = 0;
            maxSate = 5;
            maxAge = 30;


          //  int c = GameVariables.Random.Next(170, 255);
          //  color = new Color(c, c, c);
        }

        public override Color Color => Color.White;

        public override void Eat(int amount)
        {
            if (sate < maxSate)
            {
                sate += amount;
            }
        }
    }
}

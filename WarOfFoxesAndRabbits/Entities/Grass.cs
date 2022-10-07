using Microsoft.Xna.Framework;
using System;

namespace WarOfFoxesAndRabbits
{
    public class Grass : Matter
    {
        private double stage = 0;
        public Grass()
        {
            stage = new Random().Next(0, 3);
        }

        public void Grow()
        {
            if (stage < 2)
            {
                stage += 0.02;
            }
        }
        public double Stage { get => stage; }

        public void grassEaten()
        {
            if (stage <= 1)
            {
                stage = 0;
            }
            else
            {
                stage--;
            }
        }

        public override Color Color
        {
            get
            {
                switch (Stage)
                {
                    case > 0 and < 1:
                        return new Color(130, 200, 0);
                    case >= 1 and < 2:
                        return new Color(111, 183, 0);
                    case >= 2:
                        return new Color(0, 183, 0);
                }
                return Color.Black;
            }
        }
    }
}

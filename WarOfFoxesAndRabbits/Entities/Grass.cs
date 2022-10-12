using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public class Grass : Matter
    {
        private double stage = 0;
        private double maxStage = 2;
        public Grass()
        {
            stage = GameVariables.Random.Next(0, 3);
        }

        public void Grow()
        {
            if (stage < maxStage)
            {
                stage += 0.06;
            }
            if (stage >= maxStage)
            {
                stage = maxStage;
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
                return mapColor();
                /*
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
                */
            }
        }

        // Maps the color to the stage of grass
        private Color mapColor()
        {
            double c = maxStage - stage;

            double p = c / maxStage;

            return new Color(((int)((162 - 36) * p + 36)), ((int)((186 - 112) * p + 112)), ((int)((57 - 31) * p + 31)));
        }
    }
}

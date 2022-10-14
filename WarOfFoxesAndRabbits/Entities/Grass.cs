using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public class Grass : Matter
    {
        public double Stage { get; private set; } = 0;
        private double MaxStage { get => 2; }

        public Grass()
        {
            Stage = GameVariables.Random.Next(0, 3);
        }

        public void Grow()
        {
            if (Stage < MaxStage)
            {
                Stage += 0.06;
            }
            if (Stage >= MaxStage)
            {
                Stage = MaxStage;
            }
        }
        
        public void GrassEaten()
        {
            if (Stage <= 1)
            {
                Stage = 0;
            }
            else
            {
                Stage--;
            }
        }

        public override Color Color
        {
            get
            {
                return MapColor();
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
        private Color MapColor()
        {
            double c = MaxStage - Stage;

            double p = c / MaxStage;

            return new Color(((int)((162 - 36) * p + 36)), ((int)((186 - 112) * p + 112)), ((int)((57 - 31) * p + 31)));
        }
    }
}

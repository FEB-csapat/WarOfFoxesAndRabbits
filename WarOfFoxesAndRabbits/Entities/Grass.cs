using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public class Grass : Matter
    {
        public double Stage { get; private set; } = 0;
        private double MaxStage { get => 2; }

        public Grass()
        {
            Stage = GameConstants.Random.Next(0, 3);
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

        public void GrassEaten(int amount)
        {
            if (Stage - amount < 0)
            {
                Stage = 0;
            }
            else
            {
                Stage -= amount;
            }
        }

        public override Color Color
        {
            get
            {
                return MapColor();
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

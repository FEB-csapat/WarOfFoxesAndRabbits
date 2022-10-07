using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfFoxesAndRabbits
{
    public abstract class Entity
    {

        public Entity()
        {

        }

        abstract public Color Color
        {
            get;
        }
    }

    public abstract class Matter: Entity
    {
        public Matter()
        {
        }

        
    }

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
                stage += 0.07;
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



    public class Water : Matter
    {
        public Water() : base()
        {

        }

        public override Color Color => Color.DeepSkyBlue;
    }

    public class Wall : Matter
    {
        public Wall() : base()
        {
           
        }

        public override Color Color => new Color(60, 60, 60);
    }
}

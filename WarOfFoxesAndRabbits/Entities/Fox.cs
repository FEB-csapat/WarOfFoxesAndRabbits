using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    class Fox : Animal
    {
        public Fox() : base()
        {
            Sate = 100;
            Age = 0;

            MaxSate = 100;
            MaxAge = 150;

            HasAte = true;
            HasMoved = true;
            HasProduced = true;
        }

        public override Color Color => Color.Red;

        public override bool CanBreed() => Sate >= 35;

        public override bool CanEat() => Sate < 60;
        public void Eat()
        {
            if (Sate < MaxSate)
            {
                Sate += 3;
            }
        }
    }
}

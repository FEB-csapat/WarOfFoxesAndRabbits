using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    class Fox : Animal
    {
        public Fox() : base()
        {
            Sate = 18;
            Age = 0;

            MaxSate = 30;
            MaxAge = 120;

            HasAte = true;
            HasMoved = true;
            HasProduced = true;
        }

        public override Color Color => Color.Red;

        public override bool CanBreed() => Sate >= 16;

        public override bool CanEat() => Sate < 25;
        public void Eat()
        {
            if (Sate < MaxSate)
            {
                Sate += 3;
            }
        }
    }
}

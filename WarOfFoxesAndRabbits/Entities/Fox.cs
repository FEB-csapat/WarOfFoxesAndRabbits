using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    class Fox : Animal
    {
        public Fox() : base()
        {
            InitialSate = 50;
            MaxSate = 100;
            MaxAge = 250;

            Initialize();
        }

        public override Color Color => Color.Red;

        public override bool CanBreed() => Sate >= InitialSate && Age >= 4;

        public override bool CanEat() => Sate < 80;
        public void Eat()
        {
            if (Sate < MaxSate)
            {
                Sate += 6;
            }
        }
    }
}

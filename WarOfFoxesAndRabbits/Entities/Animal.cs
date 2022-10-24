using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public abstract class Animal : IEntity
    {
        public int Sate { get; protected set; }
        protected int InitialSate { get; set; }
        protected int MaxSate { get; set; }

        public int Age { get; protected set; }
        protected int MaxAge { get; set; }

        public bool HasMoved { get; set; } = false;
        public bool HasProduced { get; set; } = false;
        public bool HasAte { get; set; } = false;
        public abstract Color Color { get; }

        protected void Initialize()
        {
            Age = 0;
            Sate = InitialSate;
        }

        public virtual bool IsDead() => Sate <= 0 || Age >= MaxAge;

        public abstract bool CanBreed();
        public abstract bool CanEat();

        public virtual void Update()
        {
            Sate--;
            Age++;
        }
    }
}

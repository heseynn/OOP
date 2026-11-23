using System;

namespace Lab4_GameAccount
{
    public abstract class GameAccount
    {
        private static int nextId = 1;
        public int Id { get; private set; }
        public string UserName { get; private set; }
        public int InitialRating { get; private set; }

        protected GameAccount(string userName, int initialRating = 100)
        {
            Rename(userName);
            if (initialRating < 1)
                throw new ArgumentException("Початковий рейтинг має бути не менше 1.");

            Id = nextId++;
            InitialRating = initialRating;
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ім'я не може бути порожнім.");
            UserName = name.Trim();
        }

        public abstract long CalculatePoints(int rating, bool isWin, int winStreak);
    }

    public class StandardAccount : GameAccount
    {
        public StandardAccount(string name, int initialRating = 100) : base(name, initialRating) { }

        public override long CalculatePoints(int rating, bool isWin, int winStreak)
        {
            return isWin ? rating : -rating;
        }
    }

    public class ProtectedAccount : GameAccount
    {
        public ProtectedAccount(string name, int initialRating = 100) : base(name, initialRating) { }

        public override long CalculatePoints(int rating, bool isWin, int winStreak)
        {
            return isWin ? rating : -(rating / 2);
        }
    }

    public class BonusAccount : GameAccount
    {
        public BonusAccount(string name, int initialRating = 100) : base(name, initialRating) { }

        public override long CalculatePoints(int rating, bool isWin, int winStreak)
        {
            if (isWin && rating > 0 && winStreak >= 3)
                return (long)rating + 10;
            return isWin ? rating : -rating;
        }
    }
}

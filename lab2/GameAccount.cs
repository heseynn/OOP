using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2_GameAccount
{
    public abstract class GameAccount
    {
        private int currentRating;
        private List<GameResult> allGames = new List<GameResult>();

        public string UserName { get; private set; }

        public int CurrentRating
        {
            get { return currentRating; }
            private set
            {
                if (value < 1)
                    currentRating = 1;
                else
                    currentRating = value;
            }
        }

        public int GamesCount
        {
            get { return allGames.Count; }
        }

        protected GameAccount(string userName, int initialRating = 100)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Ім'я гравця не може бути порожнім.");

            if (initialRating < 1)
                throw new ArgumentException("Початковий рейтинг має бути не менше 1.");

            UserName = userName;
            CurrentRating = initialRating;
        }

        protected abstract int CalculatePoints(int rating, bool isWin);

        public void WinGame(string opponentName, Game game)
        {
            AddResult(opponentName, game, true);
        }

        public void LoseGame(string opponentName, Game game)
        {
            AddResult(opponentName, game, false);
        }
        
        public void WinGame(GameAccount opponent, Game game)
        {
            if (opponent == null)
                throw new ArgumentNullException(nameof(opponent));

            if (opponent == this)
                throw new ArgumentException("Не можна грати самому проти себе.");

            ValidateGame(opponent.UserName, game);
            opponent.ValidateGame(UserName, game);

            WinGame(opponent.UserName, game);
            opponent.LoseGame(UserName, game);
        }

        private void ValidateGame(string opponentName, Game game)
        {
            if (string.IsNullOrWhiteSpace(opponentName))
                throw new ArgumentException("Ім'я опонента не може бути порожнім.");

            if (game == null)
                throw new ArgumentNullException(nameof(game));

            foreach (GameResult result in allGames)
            {
                if (result.GameIndex == game.GameIndex)
                    throw new ArgumentException("Ця гра вже записана в історії гравця.");
            }
        }

        private void AddResult(string opponentName, Game game, bool isWin)
        {
            ValidateGame(opponentName, game);

            int rating = game.CalculateRating(this);
            if (rating < 0)
                throw new ArgumentException("Рейтинг гри не може бути від'ємним.");

            int oldRating = CurrentRating;
            int points = CalculatePoints(rating, isWin);
            CurrentRating += points;
            
            int ratingChange = CurrentRating - oldRating;
            allGames.Add(new GameResult(game, opponentName, isWin, rating, ratingChange));
        }

        public string GetStats()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("Гравець: " + UserName);
            report.AppendLine("Рейтинг: " + CurrentRating + " | Ігор: " + GamesCount);
            report.AppendLine($"{"Індекс",-7} {"Тип гри",-14} {"Опонент",-12} {"Результат",-10} {"Ставка",-7} {"Зміна",-7}");

            if (GamesCount == 0)
                report.AppendLine("Історія ігор порожня.");

            foreach (GameResult game in allGames)
            {
                string result = game.IsWin ? "Перемога" : "Поразка";
                string change = game.RatingChange > 0
                    ? "+" + game.RatingChange : game.RatingChange.ToString();

                report.AppendLine($"{game.GameIndex,-7} {game.GameType,-14} {game.OpponentName,-12} {result,-10} {game.Rating,-7} {change,-7}");
            }

            return report.ToString();
        }
    }

    public class StandardAccount : GameAccount
    {
        public StandardAccount(string userName, int initialRating = 100)
            : base(userName, initialRating)
        {
        }

        protected override int CalculatePoints(int rating, bool isWin)
        {
            if (isWin)
                return rating;

            return -rating;
        }
    }

    public class ProtectedAccount : GameAccount
    {
        public ProtectedAccount(string userName, int initialRating = 100)
            : base(userName, initialRating)
        {
        }

        protected override int CalculatePoints(int rating, bool isWin)
        {
            if (isWin)
                return rating;
            
            return -(rating / 2);
        }
    }

    public class BonusAccount : GameAccount
    {
        private int winStreak;

        public BonusAccount(string userName, int initialRating = 100)
            : base(userName, initialRating)
        {
            winStreak = 0;
        }

        protected override int CalculatePoints(int rating, bool isWin)
        {
            if (rating == 0)
                return 0;

            if (isWin)
            {
                winStreak++;
                if (winStreak >= 3)
                    return rating + 10;

                return rating;
            }

            winStreak = 0;
            return -rating;
        }
    }
}

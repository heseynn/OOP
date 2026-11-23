using System;

namespace Lab4_GameAccount
{
    public abstract class Game
    {
        private static int nextGameIndex = 1001;

        public int GameIndex { get; private set; }
        public string GameType { get; private set; }
        protected int Rating { get; private set; }

        protected Game(int rating, string gameType)
        {
            if (rating < 0)
                throw new ArgumentException("Рейтинг гри не може бути від'ємним.");

            Rating = rating;
            GameType = gameType;
            GameIndex = nextGameIndex++;
        }
        
        public abstract int CalculateRating(GameAccount player);
    }

    public class StandardGame : Game
    {
        public StandardGame(int rating) : base(rating, "Звичайна")
        {
        }

        public override int CalculateRating(GameAccount player)
        {
            return Rating;
        }
    }

    public class TrainingGame : Game
    {
        public TrainingGame() : base(0, "Тренувальна")
        {
        }

        public override int CalculateRating(GameAccount player)
        {
            return 0;
        }
    }

    public class SinglePlayerRatingGame : Game
    {
        public GameAccount RatedPlayer { get; private set; }

        public SinglePlayerRatingGame(int rating, GameAccount ratedPlayer)
            : base(rating, "Одностороння")
        {
            if (ratedPlayer == null)
                throw new ArgumentNullException(nameof(ratedPlayer));

            RatedPlayer = ratedPlayer;
        }

        public override int CalculateRating(GameAccount player)
        {
            if (player == RatedPlayer)
                return Rating;

            return 0;
        }
    }

    public class GameFactory
    {
        public Game CreateStandardGame(int rating)
        {
            return new StandardGame(rating);
        }

        public Game CreateTrainingGame()
        {
            return new TrainingGame();
        }

        public Game CreateSinglePlayerRatingGame(int rating, GameAccount player)
        {
            return new SinglePlayerRatingGame(rating, player);
        }
    }
}

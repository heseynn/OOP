namespace Lab2_GameAccount
{
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

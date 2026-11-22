namespace Lab2_GameAccount
{
    public class GameResult
    {
        public int GameIndex { get; private set; }
        public string GameType { get; private set; }
        public string OpponentName { get; private set; }
        public bool IsWin { get; private set; }
        public int Rating { get; private set; }
        public int RatingChange { get; private set; }

        public GameResult(Game game, string opponentName, bool isWin,
            int rating, int ratingChange)
        {
            GameIndex = game.GameIndex;
            GameType = game.GameType;
            OpponentName = opponentName;
            IsWin = isWin;
            Rating = rating;
            RatingChange = ratingChange;
        }
    }
}

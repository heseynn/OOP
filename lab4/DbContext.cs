using System;
using System.Collections.Generic;

namespace Lab4_GameAccount
{

    public class GameResult
    {
        public Game Game { get; private set; }
        public int WinnerId { get; private set; }
        public int LoserId { get; private set; }
        public int Id { get { return Game.GameIndex; } }

        public GameResult(Game game, int winnerId, int loserId)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));
            if (winnerId == loserId)
                throw new ArgumentException("Не можна грати самому проти себе.");

            Game = game;
            WinnerId = winnerId;
            LoserId = loserId;
        }
    }

    public class DbContext
    {
        public List<GameAccount> Players { get; private set; }
        public List<GameResult> Games { get; private set; }

        public DbContext()
        {
            Players = new List<GameAccount>();
            Games = new List<GameResult>();

            GameAccount first = new StandardAccount("Maksym");
            GameAccount second = new ProtectedAccount("ne_Maksym");
            Players.Add(first);
            Players.Add(second);
            Games.Add(new GameResult(new StandardGame(20), first.Id, second.Id));
        }
    }
}

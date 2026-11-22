using System;
using System.Collections.Generic;
using System.Text;

namespace Lab3_GameAccount
{
    public interface IPlayerService
    {
        void CreateAccount(GameAccount player);
        List<GameAccount> ReadAccounts();
        GameAccount ReadAccountById(int id);
        void RenameAccount(int id, string name);
        void DeleteAccount(int id);
    }

    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository players;

        public PlayerService(IPlayerRepository players) { this.players = players; }

        public void CreateAccount(GameAccount player) { players.Create(player); }
        public List<GameAccount> ReadAccounts() { return players.ReadAll(); }
        public GameAccount ReadAccountById(int id) { return players.ReadById(id); }
        public void RenameAccount(int id, string name) { players.Update(id, name); }
        public void DeleteAccount(int id) { players.Delete(id); }
    }

    public interface IGameService
    {
        void CreateGame(Game game, int winnerId, int loserId);
        List<GameResult> ReadGames();
        List<GameResult> ReadPlayerGamesByPlayerId(int playerId);
        void ChangeWinner(int gameId);
        void DeleteGame(int gameId);
        string GetStats(int playerId);
    }

    public class GameService : IGameService
    {
        private readonly IGameRepository games;
        private readonly IPlayerRepository players;

        public GameService(IGameRepository games, IPlayerRepository players)
        {
            this.games = games;
            this.players = players;
        }

        public void CreateGame(Game game, int winnerId, int loserId)
        {
            games.Create(new GameResult(game, winnerId, loserId));
        }

        public List<GameResult> ReadGames() { return games.ReadAll(); }

        public List<GameResult> ReadPlayerGamesByPlayerId(int playerId)
        {
            players.ReadById(playerId);
            return games.ReadAll().FindAll(g => g.WinnerId == playerId || g.LoserId == playerId);
        }

        public void ChangeWinner(int gameId)
        {
            GameResult old = games.ReadById(gameId);
            games.Update(new GameResult(old.Game, old.LoserId, old.WinnerId));
        }

        public void DeleteGame(int gameId) { games.Delete(gameId); }

        public string GetStats(int playerId)
        {
            GameAccount player = players.ReadById(playerId);
            List<GameResult> history = ReadPlayerGamesByPlayerId(playerId);
            int rating = player.InitialRating;
            int winStreak = 0;
            StringBuilder report = new StringBuilder();
            report.AppendLine("ID гри | Тип | Опонент | Результат | Ставка | Зміна");
            
            foreach (GameResult match in history)
            {
                bool isWin = match.WinnerId == playerId;
                int stake = match.Game.CalculateRating(player);
                if (stake > 0)
                    winStreak = isWin ? winStreak + 1 : 0;

                int points = player.CalculatePoints(stake, isWin, winStreak);
                int oldRating = rating;
                rating = Math.Max(1, rating + points);
                int change = rating - oldRating;

                int opponentId = isWin ? match.LoserId : match.WinnerId;
                string opponent = players.ReadById(opponentId).UserName;
                string outcome = isWin ? "Перемога" : "Поразка";
                report.AppendLine($"{match.Id} | {match.Game.GameType} | {opponent} | {outcome} | {stake} | {change:+0;-0;0}");
            }

            if (history.Count == 0)
                report.AppendLine("Гравець ще не зіграв жодного матчу.");
            report.AppendLine($"Рейтинг: {rating} | Ігор: {history.Count}");
            return report.ToString();
        }
    }
}

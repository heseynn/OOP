using System;
using System.Collections.Generic;

namespace Lab3_GameAccount
{
    public interface IPlayerRepository
    {
        void Create(GameAccount player);
        GameAccount ReadById(int id);
        List<GameAccount> ReadAll();
        void Update(int id, string newName);
        void Delete(int id);
    }

    public class PlayerRepository : IPlayerRepository
    {
        private readonly DbContext db;

        public PlayerRepository(DbContext db) { this.db = db; }

        public void Create(GameAccount player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            if (db.Players.Exists(p => p.Id == player.Id))
                throw new ArgumentException("Гравець із таким ID вже є.");
            db.Players.Add(player);
        }

        public GameAccount ReadById(int id)
        {
            GameAccount player = db.Players.Find(p => p.Id == id);
            if (player == null)
                throw new ArgumentException("Гравця з таким ID не знайдено.");
            return player;
        }

        public List<GameAccount> ReadAll()
        {
            return new List<GameAccount>(db.Players);
        }

        public void Update(int id, string newName)
        {
            ReadById(id).Rename(newName);
        }

        public void Delete(int id)
        {
            GameAccount player = ReadById(id);
            if (db.Games.Exists(g => g.WinnerId == id || g.LoserId == id))
                throw new ArgumentException("У гравця є записані ігри.");
            db.Players.Remove(player);
        }
    }

    public interface IGameRepository
    {
        void Create(GameResult result);
        GameResult ReadById(int id);
        List<GameResult> ReadAll();
        void Update(GameResult result);
        void Delete(int id);
    }

    public class GameRepository : IGameRepository
    {
        private readonly DbContext db;

        public GameRepository(DbContext db) { this.db = db; }

        public void Create(GameResult result)
        {
            CheckPlayers(result);
            if (db.Games.Exists(g => g.Id == result.Id))
                throw new ArgumentException("Цю гру вже записано.");
            db.Games.Add(result);
        }

        public GameResult ReadById(int id)
        {
            GameResult result = db.Games.Find(g => g.Id == id);
            if (result == null)
                throw new ArgumentException("Гру з таким ID не знайдено.");
            return result;
        }

        public List<GameResult> ReadAll()
        {
            return new List<GameResult>(db.Games);
        }

        public void Update(GameResult result)
        {
            CheckPlayers(result);
            GameResult old = ReadById(result.Id);
            db.Games[db.Games.IndexOf(old)] = result;
        }

        public void Delete(int id)
        {
            db.Games.Remove(ReadById(id));
        }

        private void CheckPlayers(GameResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            if (!db.Players.Exists(p => p.Id == result.WinnerId) ||
                !db.Players.Exists(p => p.Id == result.LoserId))
                throw new ArgumentException("Обидва гравці мають бути в базі.");

            SinglePlayerRatingGame single = result.Game as SinglePlayerRatingGame;
            if (single != null && single.RatedPlayer.Id != result.WinnerId &&
                single.RatedPlayer.Id != result.LoserId)
                throw new ArgumentException("Гравець, для якого діє ставка, має брати участь у грі.");
        }
    }
}

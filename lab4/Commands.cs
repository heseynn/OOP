using System;
using System.IO;

namespace Lab4_GameAccount
{
    public interface ICommand
    {
        string GetDescription();
        void Execute();
    }

    static class ConsoleInput
    {
        public static string ReadText(string prompt)
        {
            Console.Write(prompt);
            string text = Console.ReadLine();
            if (text == null)
                throw new EndOfStreamException();
            return text.Trim();
        }

        public static int ReadNumber(string prompt, int min, int max)
        {
            string text = ReadText(prompt);
            if (!int.TryParse(text, out int value) || value < min || value > max)
                throw new ArgumentException($"Введіть ціле число від {min} до {max}.");
            return value;
        }

        public static GameAccount ReadPlayer(IPlayerService players, string prompt)
        {
            int id = ReadNumber(prompt, 1, int.MaxValue);
            return players.ReadAccountById(id);
        }
    }

    class ListPlayersCommand : ICommand
    {
        private readonly IPlayerService players;

        public ListPlayersCommand(IPlayerService players) { this.players = players; }

        public string GetDescription() { return "Показати всіх гравців"; }

        public void Execute()
        {
            Console.WriteLine("ID | Ім’я | Тип акаунту");
            foreach (GameAccount player in players.ReadAccounts())
                Console.WriteLine($"{player.Id} | {player.UserName} | {player.GetType().Name}");
        }
    }

    class AddPlayerCommand : ICommand
    {
        private readonly IPlayerService players;

        public AddPlayerCommand(IPlayerService players) { this.players = players; }

        public string GetDescription() { return "Додати гравця"; }

        public void Execute()
        {
            string name = ConsoleInput.ReadText("Ім’я нового гравця: ");
            int type = ConsoleInput.ReadNumber("Акаунт (1 — звичайний, 2 — захищений, 3 — бонусний): ", 1, 3);
            GameAccount player;
            if (type == 1)
                player = new StandardAccount(name);
            else if (type == 2)
                player = new ProtectedAccount(name);
            else
                player = new BonusAccount(name);

            players.CreateAccount(player);
            Console.WriteLine($"Додано гравця {player.UserName}, ID {player.Id}.");
        }
    }

    class PlayerStatsCommand : ICommand
    {
        private readonly IPlayerService players;
        private readonly IGameService games;

        public PlayerStatsCommand(IPlayerService players, IGameService games)
        {
            this.players = players;
            this.games = games;
        }

        public string GetDescription() { return "Показати статистику гравця"; }

        public void Execute()
        {
            GameAccount player = ConsoleInput.ReadPlayer(players, "ID гравця: ");
            Console.WriteLine("Статистика гравця " + player.UserName);
            Console.Write(games.GetStats(player.Id));
        }
    }

    class PlayGameCommand : ICommand
    {
        private readonly IPlayerService players;
        private readonly IGameService games;
        private readonly GameFactory factory = new GameFactory();

        public PlayGameCommand(IPlayerService players, IGameService games)
        {
            this.players = players;
            this.games = games;
        }

        public string GetDescription() { return "Зіграти гру"; }

        public void Execute()
        {
            GameAccount winner = ConsoleInput.ReadPlayer(players, "ID переможця: ");
            GameAccount loser = ConsoleInput.ReadPlayer(players, "ID гравця, який програв: ");
            if (winner.Id == loser.Id)
                throw new ArgumentException("Оберіть двох різних гравців.");

            int type = ConsoleInput.ReadNumber("Гра (1 — звичайна, 2 — тренувальна, 3 — одностороння): ", 1, 3);
            Game game;
            if (type == 2)
            {
                game = factory.CreateTrainingGame();
            }
            else
            {
                int rating = ConsoleInput.ReadNumber("На скільки балів граємо? ", 0, int.MaxValue);
                if (type == 1)
                    game = factory.CreateStandardGame(rating);
                else
                {
                    int choice = ConsoleInput.ReadNumber($"Хто грає на рейтинг (1 — {winner.UserName}, 2 — {loser.UserName})? ", 1, 2);
                    GameAccount ratedPlayer = choice == 1 ? winner : loser;
                    game = factory.CreateSinglePlayerRatingGame(rating, ratedPlayer);
                }
            }

            games.CreateGame(game, winner.Id, loser.Id);
            Console.WriteLine($"Гру №{game.GameIndex} записано. Переможець: {winner.UserName}.");
        }
    }

    class ListGamesCommand : ICommand
    {
        private readonly IPlayerService players;
        private readonly IGameService games;

        public ListGamesCommand(IPlayerService players, IGameService games)
        {
            this.players = players;
            this.games = games;
        }

        public string GetDescription() { return "Показати всі ігри"; }

        public void Execute()
        {
            Console.WriteLine("ID гри | Тип | Переможець | Гравець, який програв");
            foreach (GameResult match in games.ReadGames())
            {
                string winner = players.ReadAccountById(match.WinnerId).UserName;
                string loser = players.ReadAccountById(match.LoserId).UserName;
                Console.WriteLine($"{match.Id} | {match.Game.GameType} | {winner} | {loser}");
            }
        }
    }
}

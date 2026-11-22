using System;
using System.Text;

namespace Lab3_GameAccount
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            DbContext db = new DbContext();
            IPlayerRepository playerRepository = new PlayerRepository(db);
            IGameRepository gameRepository = new GameRepository(db);
            IPlayerService players = new PlayerService(playerRepository);
            IGameService games = new GameService(gameRepository, playerRepository);
            GameFactory factory = new GameFactory();
            
            GameAccount first = players.ReadAccounts()[0];
            GameAccount second = players.ReadAccounts()[1];
            GameAccount third = new BonusAccount("ne_ne_Maksym");
            players.CreateAccount(third);
            Console.WriteLine("Створено бонусний акаунт: " + third.UserName);

            Console.WriteLine("\nУсі гравці:");
            foreach (GameAccount player in players.ReadAccounts())
                Console.WriteLine($"{player.Id} | {player.UserName} | {player.GetType().Name}");
            
            games.CreateGame(factory.CreateTrainingGame(), second.Id, first.Id);
            games.CreateGame(factory.CreateStandardGame(10), third.Id, first.Id);
            games.CreateGame(factory.CreateStandardGame(10), third.Id, second.Id);
            games.CreateGame(factory.CreateStandardGame(10), third.Id, first.Id);
            games.CreateGame(factory.CreateSinglePlayerRatingGame(30, first), third.Id, first.Id);
            games.CreateGame(factory.CreateStandardGame(200), second.Id, first.Id);
            games.CreateGame(factory.CreateStandardGame(10), first.Id, third.Id);
            games.CreateGame(factory.CreateStandardGame(10), third.Id, first.Id);

            Console.WriteLine("\nУсі ігри (ID | Тип | Переможець | Суперник):");
            foreach (GameResult match in games.ReadGames())
            {
                string winner = players.ReadAccountById(match.WinnerId).UserName;
                string loser = players.ReadAccountById(match.LoserId).UserName;
                Console.WriteLine($"{match.Id} | {match.Game.GameType} | {winner} | {loser}");
            }

            foreach (GameAccount player in players.ReadAccounts())
            {
                Console.WriteLine("\nІсторія гравця " + player.UserName);
                Console.Write(games.GetStats(player.Id));
            }

            GameAccount guest = new StandardAccount("Guest");
            players.CreateAccount(guest);
            players.RenameAccount(guest.Id, "Гість");
            Game testGame = factory.CreateTrainingGame();
            games.CreateGame(testGame, guest.Id, first.Id);
            games.ChangeWinner(testGame.GameIndex);
            Console.WriteLine("\nПеревірка редагування. Історія гравця " + guest.UserName);
            Console.Write(games.GetStats(guest.Id));
            games.DeleteGame(testGame.GameIndex);
            players.DeleteAccount(guest.Id);
            Console.WriteLine($"Після видалення тестових записів: гравців — {players.ReadAccounts().Count}, ігор — {games.ReadGames().Count}.");
        }
    }
}

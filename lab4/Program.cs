using System;
using System.IO;
using System.Text;

namespace Lab4_GameAccount
{
    class Program
    {
        static void Main()
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            DbContext db = new DbContext();
            IPlayerRepository playerRepository = new PlayerRepository(db);
            IGameRepository gameRepository = new GameRepository(db);
            IPlayerService players = new PlayerService(playerRepository);
            IGameService games = new GameService(gameRepository, playerRepository);

            ICommand[] commands =
            {
                new ListPlayersCommand(players),
                new AddPlayerCommand(players),
                new PlayerStatsCommand(players, games),
                new PlayGameCommand(players, games),
                new ListGamesCommand(players, games)
            };

            while (true)
            {
                Console.WriteLine("\nМеню гри");
                for (int i = 0; i < commands.Length; i++)
                    Console.WriteLine($"{i + 1}. {commands[i].GetDescription()}");
                Console.WriteLine("0. Вийти");

                try
                {
                    int choice = ConsoleInput.ReadNumber("Номер команди: ", 0, commands.Length);
                    if (choice == 0)
                        return;
                    commands[choice - 1].Execute();
                }
                catch (ArgumentException error)
                {
                    Console.WriteLine("Помилка: " + error.Message);
                }
                catch (EndOfStreamException)
                {
                    return;
                }
            }
        }
    }
}

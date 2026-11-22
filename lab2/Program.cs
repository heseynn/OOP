using System;
using System.Text;

namespace Lab2_GameAccount
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            GameAccount player1 = new StandardAccount("Maksym");
            GameAccount player2 = new ProtectedAccount("ne_Maksym");
            GameAccount player3 = new BonusAccount("ne_ne_Maksym");
            GameFactory factory = new GameFactory();
            
            Game game1 = factory.CreateStandardGame(20);
            player1.WinGame(player2.UserName, game1);
            player2.LoseGame(player1.UserName, game1);
            
            Game game2 = factory.CreateTrainingGame();
            player2.WinGame(player1, game2);
            
            player3.WinGame(player1, factory.CreateStandardGame(10));
            player3.WinGame(player2, factory.CreateStandardGame(10));
            player3.WinGame(player1, factory.CreateStandardGame(10));
            
            Game game6 = factory.CreateSinglePlayerRatingGame(30, player1);
            player3.WinGame(player1, game6);
            
            player2.WinGame(player1, factory.CreateStandardGame(200));
            
            player1.WinGame(player3, factory.CreateStandardGame(10));
            player3.WinGame(player1, factory.CreateStandardGame(10));

            try
            {
                Console.WriteLine("Спроба створити гру зі ставкою -15:");
                factory.CreateStandardGame(-15);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Помилка: " + ex.Message + "\n");
            }

            Console.WriteLine(player1.GetStats());
            Console.WriteLine(player2.GetStats());
            Console.WriteLine(player3.GetStats());
        }
    }
}

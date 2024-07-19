using Microsoft.Data.Sqlite;
using System.Globalization;

namespace Habit_Tracker
{
    internal class Program
    {
        static string connectionString = @"Data Source=habit-Tracker.db";
        static void Main(string[] args)
        {
            
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS drinking_water(
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        Quantity INTEGER
                        )";


                tableCmd.ExecuteNonQuery();

                connection.Close();
            }

            GetUserInput();
        }

        public static void GetUserInput()
        {
            Console.Clear();
            bool closeApp = false;

            while (closeApp == false)
            {
                Console.WriteLine(@"                        Main Menu
                What would you like to do?
                Type 0 to Close Application.
                Type 1 to view all records.
                Type 2 to insert record.
                Type 3 to Delete record.
                Tye 4 to Update record.");
                Console.WriteLine("======================================\n");

                string command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye!\n");
                        closeApp = true;
                        Environment.Exit(0); //when l click 0 l exit the program
                        break;
                    case "1":
                            GetAllRecords();
                        break;
                        
                    case "2":
                        Insert();
                        break;
                     case "3":
                            Delete();
                            break;
                     case "4":
                            Update();
                            break; 
                    default:
                        Console.Clear();
                        Console.WriteLine("You have selected the wrong option. Please type a number from 0 to 4.\n");
                            break;
                            
                }
            }
        }

        private static void Update()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("\n\nPlease type Id of the record would like to update. Type 0 to return main menu");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {recordId})"; //for true, 0 for false
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n\n");
                    connection.Close();
                    Update();
                }

                string date = GetDateInput();

                int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice. (No decimals allowed!)\n\n");
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"UPDATE drinking_water SET date = '{date}', quantity = '{quantity}' WHERE id = '{recordId}'";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        private static void Delete()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("Please type the Id of the record you want to delete to Main Menu");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"DELETE FROM drinking_water WHERE Id = '{recordId}'";

                int rowCount = tableCmd.ExecuteNonQuery(); //check if record if exists

                if(rowCount == 0)
                {
                    Console.WriteLine($"\n\nRecord: {recordId} does not exist. \n\n");
                    Delete();
                }

            }

            Console.WriteLine($"\n\nRecord with Id{recordId} was deleted. \n\n");

            GetUserInput();

        }

        private static void GetAllRecords()
        {
            Console.Clear();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"SELECT * FROM drinking_water";

                List<DrinkingWater> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader(); //read the data

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(

                        new DrinkingWater
                        {
                            Id = reader.GetInt32(0), //the 0 is the number of the column
                            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")), //the one is the number of column
                            Quantity = reader.GetInt32(2) //the 2 is the number of the column, basically quantity is = to value of column 3
                        }); ;
                    }
                }
                else
                {
                    Console.WriteLine("No rows found");
                }

                connection.Close();

                Console.WriteLine("======================================\n");
                foreach (var dw in tableData)
                {
                    Console.WriteLine($"Id: {dw.Id}\nDate: {dw.Date.ToString("dd-MMM-yyyy")}\nQuantity: {dw.Quantity}\n\n");
                }
                Console.WriteLine("======================================\n");
                
            }

        }

        private static void Insert()
        {
            string date = GetDateInput();

            int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice. (No decimals allowed)\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"INSERT INTO drinking_water(date, quantity) VALUES('{date}', '{quantity}')";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }

            GetUserInput();
        }

        internal static int GetNumberInput(string message)
        {
            Console.WriteLine(message);

            string numberInput = Console.ReadLine();

            if (numberInput == "0") GetUserInput();

            while(!Int32.TryParse(numberInput, out _) || Convert.ToInt32(numberInput) < 0)
            {
                Console.WriteLine("\n\nInvalid number. Try again.\n\n");
                numberInput = Console.ReadLine();
            }

            int finalInput = Convert.ToInt32(numberInput);

            return finalInput;
        }

        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert a date (Format: dd-mm-yy). Type 0 to return to main menu!");

            string dateInput = Console.ReadLine();

            if (dateInput == "0") GetUserInput();

            while(!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("\n\nInvalid date. (Format: dd-mm-yy). Type 0 to return to main menu or try again:\n\n");
                dateInput = Console.ReadLine();
            }

            return dateInput;
        }
    }
}

using System.Data.SQLite;
using System.IO;

namespace Journal.Data
{
    public static class DatabaseManager 
    {
        private static string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Journal.db");
        private static string connectionString = $"Data Source = \"{dbPath}\"; Version = 3"; 

        public static void InitializeDatabase()
        {

            if (!File.Exists(dbPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
                SQLiteConnection.CreateFile(dbPath);

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open(); 

                    string usersTable = @"CREATE TABLE User (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT UNIQUE NOT NULL,
                            Password TEXT NOT NULL
                        );";

                    string notesTable = @"CREATE TABLE Note (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Title TEXT,
                            Content TEXT NOT NULL,
                            CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
                            UserId INTEGER NOT NULL,
                            FOREIGN KEY (UserId) REFERENCES User(Id) ON DELETE CASCADE 
                        );"; 

                    using (var command = new SQLiteCommand(usersTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (var command = new SQLiteCommand(notesTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

            }
        }

        public static SQLiteConnection GetConnection() 
        {
            return new SQLiteConnection(connectionString);
        }
    }
}
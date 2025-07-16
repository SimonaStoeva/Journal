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
            bool isNew = !File.Exists(dbPath);

            if (isNew)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
                SQLiteConnection.CreateFile(dbPath);
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (var pragma = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    pragma.ExecuteNonQuery();
                }
 
                string usersTable = @"CREATE TABLE IF NOT EXISTS User (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Username TEXT UNIQUE NOT NULL,
                                Password TEXT NOT NULL
                              );";
                using (var command = new SQLiteCommand(usersTable, connection))
                {
                    command.ExecuteNonQuery();
                }

        // Създаваме Note таблицата
                string notesTable = @"CREATE TABLE IF NOT EXISTS Note (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Title TEXT,
                                Content TEXT NOT NULL,
                                CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
                                UserId INTEGER NOT NULL,
                                FOREIGN KEY (UserId) REFERENCES User(Id) ON DELETE CASCADE
                              );";
                using (var command = new SQLiteCommand(notesTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                
                string categoryTable = @"CREATE TABLE IF NOT EXISTS Category (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Name TEXT NOT NULL,
                                    UserId INTEGER NOT NULL,
                                    FOREIGN KEY (UserId) REFERENCES User(Id) ON DELETE CASCADE
                                 );";
                using (var command = new SQLiteCommand(categoryTable, connection))
                {
                    command.ExecuteNonQuery();
                }


                bool hasCategoryId = false;
                string checkColumn = "PRAGMA table_info(Note)";
        
                using (var command = new SQLiteCommand(checkColumn, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(1) == "CategoryId")
                        {
                            hasCategoryId = true;
                            break;
                        }
                    }
                }

                if (!hasCategoryId)
                {
                    string dropNotesTable = @"DROP TABLE IF EXISTS Note;";
                    using (var command = new SQLiteCommand(dropNotesTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    
                    string createNoteTable = @"
                        CREATE TABLE Note (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Title TEXT,
                            Content TEXT NOT NULL,
                            CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
                            UserId INTEGER NOT NULL,
                            CategoryId INTEGER,
                            FOREIGN KEY (UserId) REFERENCES User(Id) ON DELETE CASCADE,
                            FOREIGN KEY (CategoryId) REFERENCES Category(Id) ON DELETE SET NULL
                        )";
                    
                    using (var command = new SQLiteCommand(createNoteTable, connection))
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
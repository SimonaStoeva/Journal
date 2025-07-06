using Journal.Data;
using Journal.Models;
using System.Data.SQLite;

namespace Journal.Services
{
    public class NoteService
    {
        public virtual bool AddNote(int userId, string title, string content) 
        {
            string query = "INSERT INTO Note (UserId, Title, Content) VALUES (@UserId, @Title, @Content)";

            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Content", content);

                    try
                    {
                        command.ExecuteNonQuery(); 
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }

        public static List<Note> GetNotesByUser(int userId)
        {
            List<Note> notes = new List<Note>();
            string query = "SELECT Id, Title, Content, CreatedAt FROM Note WHERE UserId = @UserId";

            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {  
                        while (reader.Read())
                        {   
                            notes.Add(new Note
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Content = reader.GetString(2),
                                CreatedAt = DateTime.Parse(reader.GetString(3)),
                                UserId = userId
                            });
                        }
                    }
                }
            }
            return notes;
        }

        public static bool EditNote(int noteId, string title, string content)
        {
            string query = "UPDATE Note SET Title = @Title, Content = @Content WHERE Id = @Id";

            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", noteId); 
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Content", content);

                    return command.ExecuteNonQuery() > 0; 
                }
            }
        }

        public static bool DeleteNote(int noteId)
        {
            string query = "DELETE FROM Note WHERE Id = @Id";

            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", noteId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public static List<Note> FilterByKeyword(int userId, string keyword)
        {
            List<Note> notes = new List<Note>();
            string query = "SELECT Id, Title, Content, CreatedAt FROM Note WHERE UserId = @UserId AND (Title LIKE @Keyword OR Content LIKE @Keyword)";

            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%"); 

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notes.Add(new Note
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1), 
                                Content = reader.GetString(2),
                                CreatedAt = DateTime.Parse(reader.GetString(3)),
                                UserId = userId
                            });
                        }
                    }
                }
            }
            return notes;
        }

        public static List<Note> FilterByDate(int userId, DateTime startDate, DateTime endDate) {
            List<Note> notes = new List<Note>();
            string query = "SELECT Id, Title, Content, CreatedAt FROM Note WHERE UserId = @UserId AND (CreatedAt BETWEEN @StartDate AND @EndDate)";

            using(var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                using(var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    using(var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notes.Add(new Note
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Content = reader.GetString(2),
                                CreatedAt = reader.GetDateTime(3),
                                UserId = userId
                            });
                        }
                    }
                }
            }
            return notes;
        }
    }
}

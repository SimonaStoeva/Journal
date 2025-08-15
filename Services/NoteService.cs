using Journal.Data;
using Journal.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Journal.Services
{
    public class NoteService
    {
        public virtual bool AddNote(int userId, string title, string content, int? categoryId = null) 
        {
            string query = "INSERT INTO Note (UserId, Title, Content, CategoryId) VALUES (@UserId, @Title, @Content, @CategoryId)";

            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Content", content);
                    command.Parameters.AddWithValue("@CategoryId", (object)categoryId ?? DBNull.Value);

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

        public static bool EditNote(int noteId, string title, string content, int? categoryId = null)
        {
            string query = "UPDATE Note SET Title = @Title, Content = @Content, CategoryId = @CategoryId WHERE Id = @Id";

            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", noteId);
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Content", content);
                    command.Parameters.AddWithValue("@CategoryId", (object)categoryId ?? DBNull.Value);

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

        public List<Note> FilterByKeyword(int userId, string keyword)
        {
            var notes = new List<Note>();
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
                            notes.Add(ReadNote(reader, userId));
                        }
                    }
                }
            }

            return notes;
        }

        public List<Note> FilterByDate(int userId, DateTime startDate, DateTime endDate) 
        {
            var notes = new List<Note>();
            string query = "SELECT Id, Title, Content, CreatedAt FROM Note WHERE UserId = @UserId AND (CreatedAt BETWEEN @StartDate AND @EndDate)";

            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notes.Add(ReadNote(reader, userId));
                        }
                    }
                }
            }

            return notes;
        }

        public List<Note> GetNotesByUser(int userId)
        {
            var notes = new List<Note>();
            string query = "SELECT Id, Title, Content, CreatedAt, CategoryId FROM Note WHERE UserId = @UserId";

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
                            notes.Add(ReadNote(reader, userId));
                        }
                    }
                }
            }

            return notes;
        }

        public List<Note> GetNotesByCategory(int userId, int categoryId)
        {
            var notes = new List<Note>();
            string query = "SELECT Id, Title, Content, CreatedAt, CategoryId FROM Note WHERE UserId = @userId AND CategoryId = @categoryId";

            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@categoryId", categoryId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notes.Add(ReadNote(reader, userId, includeCategory: true));
                        }
                    }
                }
            }

            return notes;
        }

        private Note ReadNote(SQLiteDataReader reader, int userId, bool includeCategory = false)
        {
            return new Note
            {
                Id = reader.GetInt32(0),
                Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                Content = reader.GetString(2),
                CreatedAt = reader.GetDateTime(3),
                CategoryId = includeCategory && !reader.IsDBNull(4) ? reader.GetInt32(4) : null,
                UserId = userId
            };
        }
    }
}

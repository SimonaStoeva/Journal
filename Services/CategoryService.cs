using Journal.Models;
using System.Data.SQLite;
using Journal.Data;

namespace Journal.Services
{
    public class CategoryService
    {
        public List<Category> GetCategoriesByUser(int userId)
        {
            var categories = new List<Category>();

            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT * FROM Category WHERE UserId = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(ReadCategory(reader));
                    }
                }
            }

            return categories;
        }
        

        public bool AddCategory(Category category)
        {
            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                var command = new SQLiteCommand("INSERT INTO Category (Name, UserId) VALUES (@Name, @UserId)", connection);
                command.Parameters.AddWithValue("@Name", category.Name);
                command.Parameters.AddWithValue("@UserId", category.UserId);

                return command.ExecuteNonQuery() > 0;
            }
        }
        

        private Category ReadCategory(SQLiteDataReader reader)
        {
            return new Category
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"].ToString(),
                UserId = Convert.ToInt32(reader["UserId"])
            };
        }
    }
}

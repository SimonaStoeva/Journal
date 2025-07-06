using Journal.Data;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace Journal.Services
{
    public class AuthenticationService
    {

        public virtual bool RegisterUser(string username, string password) 
        {
            string passwordHash = HashPassword(password);

            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO User (Username, Password) VALUES (@Username, @Password)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", passwordHash);
                    try
                    {
                        command.ExecuteNonQuery();
                        string selectQuery = "SELECT Username FROM User";
                        using (var selectCommand = new SQLiteCommand(selectQuery, connection))
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            string allUsers = "Users in database:\n";
                            int count = 0;

                            while (reader.Read())
                            {
                                allUsers += "- " + reader.GetString(0) + "\n";
                                count++;
                            }

                           
                        }
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        public static bool LoginUser(string username, string password)
        {
            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                string query = "SELECT Password FROM User WHERE Username = @Username"; 

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    var result = command.ExecuteScalar();
                    if (result == null) return false;

                    string storedHash = result.ToString();
                    return storedHash == HashPassword(password);
                }
            }
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        public static int Get_User_Id(string username)
        {
            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                string query = "SELECT Id FROM User WHERE Username = @Username";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(0); 
                        }
                    }
                }
            }
            return -1; 
        }
    }
}

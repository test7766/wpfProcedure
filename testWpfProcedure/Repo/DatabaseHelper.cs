using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testWpfProcedure.Repo
{
    public class DatabaseHelper
    {
        private readonly string connectionString;

        public DatabaseHelper()
        {
            //  строкa подключения к базе данных .
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }


        ///Get Single
        public User GetUserById(int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("spUser_Get", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = (int)reader["Id"],
                               // FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString()
                            };
                        }
                        else
                        {
                            return null; // Пользователь не найден
                        }
                    }
                }
            }
        }




        ///Get Collection
        public List<User> GetUsersByFirstName(string firstName)
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("spUsers_GetByFirstName", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FirstName", firstName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                Id = (int)reader["Id"],
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString()
                            };
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }



    }
}

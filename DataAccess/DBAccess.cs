using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask2.Models.Core;
using System.Data.SQLite;

namespace TestTask2.DataAccess
{
    class DBAccess
    {
        public static string ConnectionString = "Data Source=DataAccess\\TestMasterDB.db";

        public static List<Test> GetTests()
        {
            List<Test> allTests = new List<Test>();

            string sql = "SELECT Tests.* FROM Tests";

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(ConnectionString))
                {
                    SQLiteConnection.Open();

                    SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection);

                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Test t = new Test();

                        t.id = reader.GetInt32(0);
                        t.title = reader.GetString(1);

                        allTests.Add(t);
                    }

                    reader.Close();
                    SQLiteConnection.Close();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($" Ошибка соединения с базой данных: {ex.Message}");
            }

            return allTests;
        }

        public static bool DropTest(int id)
        {
            string sql = "DELETE FROM Tests WHERE id=@id";

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(ConnectionString))
                {
                    SQLiteConnection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                    }

                    SQLiteConnection.Close();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($" Ошибка соединения с базой данных: {ex.Message}");
                return false;
            }

            return true;
        }





    }
}

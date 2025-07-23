using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask2.Models;
using TestTask2.Models.Core;
using System.Data.SQLite;
using System.Windows.Markup;
using System.Security.Cryptography;
using System.Security.Policy;

namespace TestTask2.DataAccess
{
    class DBAccess
    {
        public DBAccess() {}

        public static string ConnectionString = "Data Source=DataAccess\\TestMasterDB.db";

        public static List<TestInfo> GetTests()
        {
            List<TestInfo> allTests = new List<TestInfo>();

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
                        TestInfo t = new TestInfo();

                        t.id = reader.GetInt32(0);
                        t.title = reader.GetString(1);

                        allTests.Add(t);
                    }

                    reader.Close();
                    SQLiteConnection.Close();
                }
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка соединения с базой данных при удалении теста: {ex.Message}");
                return false;
            }


            // ----------------- удаление связанных вопросов ------------------------
            sql = "DELETE FROM Questions WHERE testId=@testId";

            try
            {
                using (SQLiteConnection SQLiteConnection = new SQLiteConnection(ConnectionString))
                {
                    SQLiteConnection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, SQLiteConnection))
                    {
                        command.Parameters.AddWithValue("@testId", id);

                        command.ExecuteNonQuery();
                    }

                    SQLiteConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка соединения с базой данных при удалении вопросов: {ex.Message}");
                return false;
            }

            return true;
        }

        public static bool InsertNewTest(Test t)
        {
            string sql = "INSERT INTO Tests (title) VALUES (@title); SELECT last_insert_rowid();";
            int testID = 0;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", t.title);
                        testID = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            // ------------------------------сохранение списка вопросов в бд ---------------------

            sql = "INSERT INTO Questions (testId, questionText, answer1, answer2, answer3, answer4, correctAnswer) " +
                "VALUES (@testId, @questionText, @answer1, @answer2, @answer3, @answer4, @correctAnswer)";


            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            foreach (Question q in t.QuestionsList)
                            {
                                command.Parameters.AddWithValue("@testId", testID);
                                command.Parameters.AddWithValue("@questionText", q.questionText);
                                command.Parameters.AddWithValue("@answer1", (string.IsNullOrEmpty(q.answer1) ? "-" : (q.answer1)));
                                command.Parameters.AddWithValue("@answer2", (string.IsNullOrEmpty(q.answer2) ? "-" : (q.answer2)));
                                command.Parameters.AddWithValue("@answer3", (string.IsNullOrEmpty(q.answer3) ? "-" : (q.answer3)));
                                command.Parameters.AddWithValue("@answer4", (string.IsNullOrEmpty(q.answer4) ? "-" : (q.answer4)));
                                command.Parameters.AddWithValue("@correctAnswer", q.correctAnswer);

                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($" Ошибка соединения с базой данных при сохранении вопросов: {ex.Message}");
                        transaction.Rollback();
                        return false;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return true;
        }

        public static List<Question> LoadQuestions(int testID)
        {
            List<Question> questions = new List<Question>();

            string sql = "SELECT Questions.id, Questions.questionText,  Questions.answer1,  Questions.answer2,  Questions.answer3, Questions.answer4,  Questions.correctAnswer FROM Questions WHERE testId=@testId";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@testId", testID);

                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Question question = new Question();

                        question.id = reader.GetInt32(0);
                        question.questionText = reader.GetString(1);
                        question.answer1 = reader.GetString(2);
                        question.answer2 = reader.GetString(3);

                        // возможные пустые поля
                        if (!reader.IsDBNull(4))
                            question.answer3 = reader.GetString(4);
                        else
                            question.answer3 = "";

                        if (!reader.IsDBNull(5))
                            question.answer4 = reader.GetString(5);
                        else
                            question.answer4 = "";

                        question.correctAnswer = reader.GetString(6);

                        questions.Add(question);
                    }
                }
            }

            return questions;
        }

        public static bool UpdateChangedTest(Test t)
        {
            string sql = "UPDATE Questions SET questionText= @questionText,  " +
                "answer1=@answer1, answer2=@answer2, answer3=@answer3, answer4=@answer4,  " +
                "correctAnswer=@correctAnswer " +
                "WHERE id = @id ";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    try
                    {
                        foreach (Question q in t.QuestionsList)
                        {
                            command.Parameters.AddWithValue("@questionText", q.questionText);
                            command.Parameters.AddWithValue("@answer1", q.answer1);
                            command.Parameters.AddWithValue("@answer2", q.answer2);
                            command.Parameters.AddWithValue("@answer3", q.answer3);
                            command.Parameters.AddWithValue("@answer4", q.answer4);
                            command.Parameters.AddWithValue("@correctAnswer", q.correctAnswer);
                            command.Parameters.AddWithValue("@id", q.id);

                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                connection.Close();

                return true;
            }
        }
    }
}

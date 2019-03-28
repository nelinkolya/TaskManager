using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using TaskManager.entity;

namespace TaskManager
{
    public class TaskRepository
    {
        private static readonly SqlConnection _connection =
            new SqlConnection(ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString);

        private TaskRepository() { }

        public static List<Task> Get()
        {
            var taskList = new List<Task>();
            SqlCommand cmd = _connection.CreateCommand();

            cmd.CommandText = @"SELECT
                                    *
                                FROM
                                    Tasks";

            _connection.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Task task = new Task(reader.GetGuid(0));
                    
                    task.Name = reader.GetString(1);
                    task.Date = reader.GetDateTime(2);
                    task.IsActive = reader.GetBoolean(3);
                    task.Category = reader.GetInt32(4);
                    task.Description = reader.IsDBNull(5) ? "" : reader.GetString(5);

                    taskList.Add(task);
                }
            }

            _connection.Close();

            return taskList;
        }

        public static void Save(List<Task> tasks)
        {
            SqlCommand cmd = _connection.CreateCommand();
            bool flagExistence; //todo: rename to 'exist'

            foreach (Task task in tasks)
            {
                //todo: would it be better to use string interpolation ($"select * from tasks where id={task.Id}")
                cmd.CommandText = string.Format(@"SELECT 
                                                    * 
                                                FROM 
                                                    Tasks t 
                                                WHERE 
                                                    t.id = '{0}'", 
                                                    task.Id.ToString());

                _connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    flagExistence = reader.Read();
                }

                if (flagExistence)//If the recrod exists
                {
                    cmd.CommandText = String.Format(@"UPDATE 
                                                        Tasks 
                                                    SET 
                                                        Name = '{0}', 
                                                        Date = '{1}', 
                                                        ActivityStatus = '{2}', 
                                                        Category = '{3}', 
                                                        Description = '{4}' 
                                                    WHERE 
                                                        id = '{5}'",
                                                        task.Name,
                                                        task.Date.ToString("yyyy-MM-dd"),
                                                        task.IsActive,
                                                        1,
                                                        task.Description,
                                                        task.Id.ToString());

                    cmd.ExecuteNonQuery();
                }
                else//If the record does not exist
                {
                    cmd.CommandText = String.Format(@"INSERT INTO 
                                                        Tasks(Id, Name, Date, ActivityStatus, Category, Description) 
                                                    VALUES 
                                                        ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                                                        task.Id.ToString(),
                                                        task.Name,
                                                        task.Date.ToString("yyyy-MM-dd"),
                                                        task.IsActive,
                                                        1,
                                                        task.Description);

                    cmd.ExecuteNonQuery();
                }

                _connection.Close();
            }

        }
    }
}

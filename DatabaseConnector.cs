using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SysAdmin_Remider
{
    internal class DatabaseConnector
    {
        private const string DatabaseName = "sysadmin_reminder";
        private const string TasksTableName = "tasks";
        private const string ConnectionConfiguration = "datasource=127.0.0.1;port=3306;username=root;password=root;database=" + DatabaseName;

        public static bool hasConnection()
        {
            MySqlConnection connection = new MySqlConnection(ConnectionConfiguration);

            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch (Exception) 
            { 
                MessageBox.Show("Ошибка подключения к базе данных. Проверьте настройки соединения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; 
            }
        }

        public static long CountTasksQuery()
        {
            MySqlConnection connection = new MySqlConnection(ConnectionConfiguration);

            string query = "SELECT COUNT(1) FROM " + DatabaseName + "." + TasksTableName;
            MySqlCommand command = new MySqlCommand(query, connection);

            long amount = 0;

            try
            {
                connection.Open();

                amount = (long)command.ExecuteScalar();

                connection.Close();
                return amount;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подсчета количества всех заданий в базе данных.\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public static List<Task> GetTasksQuery()
        {
            List<Task> tasks = new List<Task>();

            MySqlConnection connection = new MySqlConnection(ConnectionConfiguration);

            string query = "SELECT * FROM " + DatabaseName + "." + TasksTableName + " WHERE status=@status";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@status", Status.Running);

            try
            {
                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Task task = new Task(long.Parse(reader.GetString(0)), reader.GetString(1), DateTime.Parse(reader.GetString(2)), reader.GetString(3));
                    tasks.Add(task);
                }

                reader.Close();
                connection.Close();
                return tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка выгрузки активных заданий из базы данных.\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static bool SaveTaskQuery(Task task)
        {
            long tasksAmount = CountTasksQuery();

            if (tasksAmount > -1)
            {
                MySqlConnection connection = new MySqlConnection(ConnectionConfiguration);

                string query = "INSERT INTO " + DatabaseName + "." + TasksTableName + " VALUES"
                    + "(@id, @description, @executionTime, @priority, @status)";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", ++tasksAmount);
                command.Parameters.AddWithValue("@description", task.Description);
                command.Parameters.AddWithValue("@executionTime", task.ExecutionTime);
                command.Parameters.AddWithValue("@priority", task.Priority.ToString());
                command.Parameters.AddWithValue("@status", Status.Running);
                command.CommandTimeout = 15;

                try
                {
                    connection.Open();

                    command.ExecuteNonQuery();

                    connection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка добавления задания в базу данных.\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return false;
        }

        public static bool UpdateTaskQuery(Task task, string status)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionConfiguration);

            string query = "UPDATE " + DatabaseName + "." + TasksTableName
                + " SET description=@description, execution_time=@executionTime, priority=@priority, status=@status"
                + " WHERE id=@id";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@description", task.Description);
            command.Parameters.AddWithValue("@executionTime", task.ExecutionTime);
            command.Parameters.AddWithValue("@priority", task.Priority.ToString());
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@id", task.Id);

            try
            {
                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления задания в базе данных.\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}

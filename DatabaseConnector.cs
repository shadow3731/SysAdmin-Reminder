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

        private static string TasksTableName = "tasks";
        private static string ConnectionConfiguration;

        public static void SetConnectionConfiguration()
        {
            ConnectionConfiguration = "datasource=" + Settings.UserSettings["Address"]
            + ";port=" + Settings.UserSettings["Port"]
            + ";username=" + Settings.UserSettings["DBUsername"]
            + ";password="+ Settings.UserSettings["DBPassword"]
            + ";database=" + Settings.UserSettings["DBName"]
            + ";charset=utf8mb4";
        }

        public static bool HasConnection()
        {
            MySqlConnection connection = new MySqlConnection(ConnectionConfiguration);

            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(Settings.LanguagePack["DBConnectionFailed"] + "\n" + ex.Message, Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; 
            }
        }

        public static void CreateTableIfNotExists()
        {
            MySqlConnection connection = new MySqlConnection(ConnectionConfiguration);

            string query1 = "ALTER DATABASE " + Settings.UserSettings["DBName"]
                + " CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci";
            MySqlCommand command1 = new MySqlCommand(query1, connection);

            string query2 = "CREATE TABLE IF NOT EXISTS "
                + Settings.UserSettings["DBName"] + "." + TasksTableName + "("
                + "id BIGINT COLLATE utf8mb4_general_ci, " 
                + "description TEXT COLLATE utf8mb4_general_ci, "
                + "execution_time DATETIME COLLATE utf8mb4_general_ci, " 
                + "priority VARCHAR(50) COLLATE utf8mb4_general_ci, "
                + "status VARCHAR(30) COLLATE utf8mb4_general_ci, PRIMARY KEY (id))";
            MySqlCommand command2 = new MySqlCommand(query2, connection);

            try
            {
                connection.Open();

                command1.ExecuteNonQuery();
                command2.ExecuteNonQuery();

                connection.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(Settings.LanguagePack["DBCreateTableFailed"] + "\n" + ex.Message, Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (NotSupportedException) { }
            catch (KeyNotFoundException) { }
        }

        public static long CountTasksQuery()
        {
            MySqlConnection connection = new MySqlConnection(ConnectionConfiguration);

            string query = "SELECT COUNT(1) FROM " + Settings.UserSettings["DBName"] + "." + TasksTableName;
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
                MessageBox.Show(Settings.LanguagePack["DBCountFailed"] + "\n" + ex.Message, Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public static List<Task> GetTasksQuery()
        {
            List<Task> tasks = new List<Task>();

            MySqlConnection connection = new MySqlConnection(ConnectionConfiguration);

            string query = "SELECT * FROM " + Settings.UserSettings["DBName"] + "." + TasksTableName;
            MySqlCommand command = new MySqlCommand(query, connection);

            try
            {
                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Task task = new Task(long.Parse(reader.GetString(0)), reader.GetString(1), DateTime.Parse(reader.GetString(2)), reader.GetString(3), reader.GetString(4));
                    tasks.Add(task);
                }

                reader.Close();
                connection.Close();
                return tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Settings.LanguagePack["DBUploadFailed"] + "\n" + ex.Message, Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static List<Task> GetTasksQuery(string status)
        {
            List<Task> tasks = new List<Task>();

            MySqlConnection connection = new MySqlConnection(ConnectionConfiguration);

            string query = "SELECT * FROM " + Settings.UserSettings["DBName"] + "." + TasksTableName + " WHERE status=@status";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@status", status);

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
                MessageBox.Show(Settings.LanguagePack["DBUploadActiveFailed"] + "\n" + ex.Message, Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static bool SaveTaskQuery(Task task)
        {
            long tasksAmount = CountTasksQuery();

            if (tasksAmount > -1)
            {
                MySqlConnection connection = new MySqlConnection(ConnectionConfiguration);

                string query = "INSERT INTO " + Settings.UserSettings["DBName"] + "." + TasksTableName + " VALUES"
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
                    MessageBox.Show(Settings.LanguagePack["DBSaveFailed"] + "\n" + ex.Message, Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return false;
        }

        public static bool UpdateTaskQuery(Task task, string status)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionConfiguration);

            string query = "UPDATE " + Settings.UserSettings["DBName"] + "." + TasksTableName
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
                MessageBox.Show(Settings.LanguagePack["DBUpdateFailed"] + "\n" + ex.Message, Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}

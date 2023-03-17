using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Immutable;

namespace SysAdmin_Remider
{
    internal class TaskPerformer
    {
        public static string DateTimeFormat { get; } = "dd.MM.yyyy HH:mm";
        private static DateTime temp;
        private static bool[] IsAlreadyCreated = { false, false, false, false };

        public static void LoadTasks(List<Task> incomingTasks, DataGridView dataGridView)
        {
            ImmutableList<Task> tasks = incomingTasks.ToImmutableList();

            for (int i = 0; i < tasks.Count; i++)
            {
                int row = dataGridView.Rows.Add();

                DataGridView_SetID(tasks[i].Id, dataGridView, row);
                DataGridView_SetDescription(tasks[i].Description, dataGridView, row);
                DataGridView_SetExecutionTime(tasks[i].ExecutionTime, dataGridView, row);
                DataGridView_SetPriority(tasks[i].Priority, dataGridView, row);
            }
        }

        public static bool SaveTask(Task task, DataGridView dataGridView, int row)
        {
            if (DatabaseConnector.SaveTaskQuery(task))
            {
                DataGridView_SetID(task.Id, dataGridView, row);
                DataGridView_SetDescription(task.Description, dataGridView, row);
                DataGridView_SetExecutionTime(task.ExecutionTime, dataGridView, row);
                DataGridView_SetPriority(task.Priority, dataGridView, row);

                return true;
            }

            return false;
        }

        public static bool IsChangableTask(object changableValue, int column)
        {
            switch (column)
            {
                case 2:
                    {
                        if (changableValue == null || changableValue.ToString().Length < 1)
                        {
                            MessageBox.Show("Описание задания не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    } 
                    break;
                case 3:
                    {
                        if (changableValue == null || !DateTime.TryParse(changableValue.ToString(), out temp))
                        {
                            MessageBox.Show("Неправильный формат даты и времени.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    } 
                    break;
                case 4:
                    {
                        if (changableValue == null || !Priority.All.Contains(changableValue))
                        {
                            MessageBox.Show("Нераспознанный приоритет.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                    break;

            }

            return true;
        }

        public static Task SetValueInTask(Task task, object changableValue, int column)
        {
            switch (column)
            {
                case 2:
                    {
                        task.Description = changableValue.ToString();
                    }
                    break;
                case 3:
                    {
                        task.ExecutionTime = Convert.ToDateTime(changableValue);
                    }
                    break;
                case 4:
                    {
                        task.Priority = changableValue;
                    }
                    break;
            }

            if (DatabaseConnector.UpdateTaskQuery(task, Status.Running))
            {
                return task;
            }
            else
            {
                return null;
            }
        }

        public static void RollbackValueInTask(Task task, DataGridView dataGridView, int row, int column)
        {
            switch (column)
            {
                case 2:
                    {
                        dataGridView.Rows[row].Cells[column].Value = task.Description;
                    }
                    break;
                case 3:
                    {
                        dataGridView.Rows[row].Cells[column].Value = task.ExecutionTime.ToString(DateTimeFormat);
                    }
                    break;
                case 4:
                    {
                        dataGridView.Rows[row].Cells[column].Value = task.Priority.ToString();
                    }
                    break;
            }
        }

        public static bool DeleteTask(Task task, string status)
        {
            return DatabaseConnector.UpdateTaskQuery(task, status);
        }

        private static void DataGridView_SetID(long id, DataGridView dataGridView, int row) 
        {
            if (!IsAlreadyCreated[0])
            {
                IsAlreadyCreated[0] = true;

                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Name = "Column2";
                column.HeaderText = "ID";
                column.ToolTipText = "Порядковый номер задания";
                column.Width = 60;
                column.ReadOnly = true;

                dataGridView.Columns.Add(column);
            }
            
            dataGridView.Rows[row].Cells[1].Value = id;
            dataGridView.Rows[row + 1].Cells[1].ReadOnly = true;
        }

        private static void DataGridView_SetDescription(string description, DataGridView dataGridView, int row)
        {
            if (!IsAlreadyCreated[1])
            {
                IsAlreadyCreated[1] = true;

                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Name = "Column3";
                column.HeaderText = "Задание";
                column.ToolTipText = "Описание задания";
                column.Width = 758;

                dataGridView.Columns.Add(column);
            }
            
            dataGridView.Rows[row].Cells[2].Value = description;
            dataGridView.Rows[row + 1].Cells[2].ReadOnly = true;
        }

        private static void DataGridView_SetExecutionTime(DateTime dateTime, DataGridView dataGridView, int row)
        {
            if (!IsAlreadyCreated[2])
            {
                IsAlreadyCreated[2] = true;

                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Name = "Column4";
                column.HeaderText = "Выполнить в";
                column.ToolTipText = "Дата и время выполнения задания";
                column.Width = 150;

                dataGridView.Columns.Add(column);
            }
            
            dataGridView.Rows[row].Cells[3].Value = dateTime.ToString(DateTimeFormat);
            dataGridView.Rows[row + 1].Cells[3].ReadOnly = true;
        }

        private static void DataGridView_SetPriority(object priority, DataGridView dataGridView, int row)
        {
            if (!IsAlreadyCreated[3])
            {
                IsAlreadyCreated[3] = true;

                DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
                column.Name = "Column5";
                column.HeaderText = "Приоритет";
                column.ToolTipText = "Важность задания";
                column.Width = 130;
                column.Items.AddRange(Priority.All);

                dataGridView.Columns.Add(column);
            }
            
            dataGridView.Rows[row].Cells[4].Value = priority.ToString();
            dataGridView.Rows[row + 1].Cells[4].ReadOnly = true;
        }
    }
}

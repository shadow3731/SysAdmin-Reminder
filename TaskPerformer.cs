using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Immutable;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SysAdmin_Remider
{
    internal class TaskPerformer
    {
        public static string DateTimeFormat { get; } = "dd.MM.yyyy HH:mm";

        public static void LoadTasks(List<Task> incomingTasks, DataGridView dataGridView, bool readOnly)
        {
            if (incomingTasks != null)
            {
                ImmutableList<Task> tasks = incomingTasks.ToImmutableList();

                for (int i = 0; i < tasks.Count; i++)
                {
                    int row = dataGridView.Rows.Add();

                    DataGridView_SetID(tasks[i].Id, dataGridView, row, readOnly);
                    DataGridView_SetDescription(tasks[i].Description, tasks[i].Status, dataGridView, row, readOnly);
                    DataGridView_SetExecutionTime(tasks[i].ExecutionTime, dataGridView, row, readOnly);
                    DataGridView_SetPriority(tasks[i].Priority, dataGridView, row, readOnly);
                }
            }
        }

        public static bool SaveTask(Task task, DataGridView dataGridView, bool readOnly)
        {
            if (DatabaseConnector.SaveTaskQuery(task))
            {
                int row = dataGridView.Rows.Add();

                DataGridView_SetID(task.Id, dataGridView, row, readOnly);
                DataGridView_SetDescription(task.Description, task.Status, dataGridView, row, readOnly);
                DataGridView_SetExecutionTime(task.ExecutionTime, dataGridView, row, readOnly);
                DataGridView_SetPriority(task.Priority, dataGridView, row, readOnly);

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
                            MessageBox.Show(Settings.LanguagePack["TaskWrongDescription"], Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    } 
                    break;
                case 3:
                    {
                        if (changableValue == null || !DateTime.TryParseExact(changableValue.ToString(), DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                        {
                            MessageBox.Show(Settings.LanguagePack["TaskWrongDatetime"], Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    } 
                    break;
                case 4:
                    {
                        if (changableValue == null)
                        {
                            bool isContained = false;
                            int i = 0, j = 0;

                            while (i < Priority.All.Length && !isContained)
                            {
                                while (j < Priority.All[0].Length)
                                {
                                    if (Priority.All[i][j].Contains(changableValue.ToString()))
                                    {
                                        isContained = true;
                                        break;
                                    }

                                    j++;
                                }

                                i++;
                            }

                            if (!isContained)
                            {
                                MessageBox.Show(Settings.LanguagePack["TaskWrongPriority"], Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }
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
                        task.ExecutionTime = DateTime.ParseExact(changableValue.ToString(), DateTimeFormat, CultureInfo.InvariantCulture);
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

        public static void DataGridView_CreateColumns(DataGridView dataGridView)
        {
            DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn();
            column1.Name = "Column2";
            column1.HeaderText = Settings.LanguagePack["ID"];
            column1.ToolTipText = Settings.LanguagePack["IDTip"];
            column1.Width = 60;
            column1.ReadOnly = true;

            DataGridViewTextBoxColumn column2 = new DataGridViewTextBoxColumn();
            column2.Name = "Column3";
            column2.HeaderText = Settings.LanguagePack["Description"];
            column2.ToolTipText = Settings.LanguagePack["DescriptionTip"];
            column2.Width = 758;

            DataGridViewTextBoxColumn column3 = new DataGridViewTextBoxColumn();
            column3.Name = "Column4";
            column3.HeaderText = Settings.LanguagePack["ProceedAt"];
            column3.ToolTipText = Settings.LanguagePack["ProceedAt"];
            column3.Width = 150;

            DataGridViewComboBoxColumn column4 = new DataGridViewComboBoxColumn();
            column4.Name = "Column5";
            column4.HeaderText = Settings.LanguagePack["Priority"];
            column4.ToolTipText = Settings.LanguagePack["PriorityTip"];
            column4.Width = 130;

            List<string> priorities = new List<string>(Priority.All.Length);
            for (int i = 0; i < Priority.All.Length; i++)
            {
                priorities.Add(Priority.All[i][Settings.LanguageCode]);
            }
            column4.Items.AddRange(priorities.ToArray());

            dataGridView.Columns.AddRange(column1, column2, column3, column4);
        }

        private static void DataGridView_SetID(long id, DataGridView dataGridView, int row, bool readOnly)
        {
            if (readOnly) { dataGridView.Rows[row].Cells[1].ReadOnly = true; }

            dataGridView.Rows[row].Cells[1].Value = id;
            dataGridView.Rows[row + 1].Cells[1].ReadOnly = true;
        }

        private static void DataGridView_SetDescription(string description, string status, DataGridView dataGridView, int row, bool readOnly)
        {
            if (readOnly) { dataGridView.Rows[row].Cells[2].ReadOnly = true; }

            if (status == null) { dataGridView.Rows[row].Cells[2].Value = description; }
            else { dataGridView.Rows[row].Cells[2].Value = "(" + status + ") " + description; }           
            dataGridView.Rows[row + 1].Cells[2].ReadOnly = true;
        }

        private static void DataGridView_SetExecutionTime(DateTime dateTime, DataGridView dataGridView, int row, bool readOnly)
        {
            if (readOnly) { dataGridView.Rows[row].Cells[3].ReadOnly = true; }

            dataGridView.Rows[row].Cells[3].Value = dateTime.ToString(DateTimeFormat);
            dataGridView.Rows[row + 1].Cells[3].ReadOnly = true;
        }

        private static void DataGridView_SetPriority(object priority, DataGridView dataGridView, int row, bool readOnly)
        {
            if (readOnly) { dataGridView.Rows[row].Cells[4].ReadOnly = true; }

            int i = 0;
            while (i < Priority.All.Length)
            {
                if (Priority.All[i].Contains(priority))
                {
                    break;
                }

                i++;
            }

            if (i != Priority.All.Length) { dataGridView.Rows[row].Cells[4].Value = Priority.All[i][Settings.LanguageCode]; }
            else dataGridView.Rows[row].Cells[4].Value = Priority.All[0][Settings.LanguageCode];

            dataGridView.Rows[row + 1].Cells[4].ReadOnly = true;
        }
    }
}

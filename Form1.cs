using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysAdmin_Remider
{
    public partial class Form1 : Form
    {
        private CultureInfo culture;
        private List<Task> tasks = new List<Task>();
        private List<List<System.Windows.Forms.Timer>> timers = new List<List<System.Windows.Forms.Timer>>(4);
        private TimerPerformer timerPerformer;
        int[] colorMills = { 3600000, 1200000, 0, -300000 };
        private long nextTaskId = 0;
        private int textLength = 0;

        public Form1()
        {
            InitializeComponent();

            culture = new CultureInfo("ru-RU");
            CultureInfo.CurrentCulture = culture;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel3.Text = ServiceMessage.ApplicationLaunched;

            timer2.Start();

            comboBox1.Items.AddRange(Priority.All);
            comboBox1.SelectedIndex = 0;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            dataGridView1.Width = this.ClientSize.Width - 24;
            dataGridView1.Height = this.ClientSize.Height - 183;

            groupBox1.Top = this.ClientSize.Height - 164;
            groupBox1.Width = this.ClientSize.Width - 24;

            richTextBox1.Width = this.ClientSize.Width - 192;

            label2.Left = this.ClientSize.Width - 181;

            dateTimePicker1.Left = this.ClientSize.Width - 178;

            label3.Left = this.ClientSize.Width - 181;

            comboBox1.Left = this.ClientSize.Width - 178;

            double k1 = 0.241;
            double k2 = 1 - ((this.MaximumSize.Width - 16 - this.ClientSize.Width) * 0.0315 / (this.MaximumSize.Width - this.MinimumSize.Width - 32));
            
            button1.Top = this.ClientSize.Height - 51;
            button1.Width = (int)Math.Round(this.ClientSize.Width * k1 * k2);

            button2.Left = button1.Left + button1.Width + 6;
            button2.Top = this.ClientSize.Height - 51;
            button2.Width = (int)Math.Round(this.ClientSize.Width * k1 * k2);

            button3.Left = button2.Left + button2.Width + 6;
            button3.Top = this.ClientSize.Height - 51;
            button3.Width = (int)Math.Round(this.ClientSize.Width * k1 * k2);

            button4.Left = button3.Left + button3.Width + 6;
            button4.Top = this.ClientSize.Height - 51;
            button4.Width = (int)Math.Round(this.ClientSize.Width * (k1 - 0.0015) * k2);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1 && textLength > 0)
            {
                button1_Click(sender, e);
            }
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            textLength = richTextBox1.Text.Length;

            if (e.KeyChar != '\b')
            {
                textLength++;
            }

            button1.Enabled = textLength > 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task task = new Task(++nextTaskId, richTextBox1.Text, dateTimePicker1.Value, comboBox1.SelectedItem);

            int newRowIndex = dataGridView1.Rows.Add();

            if (TaskPerformer.SaveTask(task, dataGridView1, newRowIndex))
            {
                for (int i = 0; i < timers.Count; i++)
                {
                    if (i < timers.Count - 1) { timers[i].Add(ConfigureTimer(task, colorMills[i], colorMills[i + 1], timers[i].Count, i)); }
                    else { timers[i].Add(ConfigureTimer(task, colorMills[i], null, timers[i].Count, i)); }
                }

                tasks.Add(task);
                
                textLength = 0;

                richTextBox1.Clear();
                dateTimePicker1.Value = DateTime.Now;
                comboBox1.SelectedIndex = 0;
                button1.Enabled = false;

                toolStripStatusLabel3.Text = ServiceMessage.TaskAdded;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                object value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                long id = (long)dataGridView1.Rows[e.RowIndex].Cells[1].Value;
                Task task = tasks.Find(x => x.Id == id);

                if (task != null)
                {
                    if (TaskPerformer.IsChangableTask(value, e.ColumnIndex))
                    {
                        Task updatedTask = TaskPerformer.SetValueInTask(task, value, e.ColumnIndex);

                        if (updatedTask != null)
                        {
                            int changingId = tasks.IndexOf(task);
                            tasks.Insert(changingId, updatedTask);

                            if (e.ColumnIndex == 3)
                            {
                                for (int i = 0; i < timers.Count; i++)
                                {
                                    if (timers[i][changingId] != null) { timers[i][changingId].Enabled = false; }

                                    if (i < timers.Count - 1) { timers[i][changingId] = ConfigureTimer(task, colorMills[i], colorMills[i + 1], changingId, i); }
                                    else { timers[i][changingId] = ConfigureTimer(task, colorMills[i], null, changingId, i); }
                                }
                            }

                            toolStripStatusLabel3.Text = "";
                            toolStripStatusLabel3.Text = ServiceMessage.TaskUpdated;
                        }
                    }
                    else
                    {
                        TaskPerformer.RollbackValueInTask(task, dataGridView1, e.RowIndex, e.ColumnIndex);

                        toolStripStatusLabel3.Text = "";
                        toolStripStatusLabel3.Text += ServiceMessage.TaskRollback;
                    }
                }
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex != dataGridView1.Rows.Count - 1 && e.Button == MouseButtons.Right)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                Point clientPoint = dataGridView1.PointToClient(Cursor.Position);
                contextMenuStrip1.Show(dataGridView1, clientPoint);
            }
        }

        private void FinishTaskMenuItem_Click(object sender, EventArgs e)
        {
            int rowId = dataGridView1.CurrentCell.RowIndex;

            long id = (long)dataGridView1.Rows[rowId].Cells[1].Value;
            Task task = tasks.Find(x => x.Id == id);
            int deletingId = tasks.IndexOf(task);

            if (TaskPerformer.DeleteTask(task, Status.Finished))
            {
                for (int i = 0; i < timers.Count; i++)
                {
                    timers[i].Remove(timers[i][deletingId]);
                }
                
                tasks.Remove(task);

                dataGridView1.Rows.RemoveAt(rowId);

                for (int i = deletingId; i < tasks.Count; i++)
                {
                    for (int j = 0; j < timers.Count; j++)
                    {
                        if (timers[j][i] != null) { timers[j][i].Enabled = false; }
                        
                        if (j < timers.Count - 1) { timers[j][i] = ConfigureTimer(tasks[i], colorMills[j], colorMills[j + 1], i, j); }
                        else { timers[j][i] = ConfigureTimer(tasks[i], colorMills[j], null, i, j); }
                    }
                }

                toolStripStatusLabel3.Text = ServiceMessage.TaskFinished;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = DateTime.Now.ToString("dddd", culture) + " " 
                + DateTime.Now.ToString(TaskPerformer.DateTimeFormat + ":ss");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Interval = 120000;

            if (DatabaseConnector.hasConnection())
            {
                toolStripStatusLabel2.Image = Properties.Resources.db_success_mark;
                toolStripStatusLabel2.Text = "Соединение с базой данных установлено.";

                dataGridView1.Rows.Clear();

                timers.Clear();

                List<Task> uploadedTasks = DatabaseConnector.GetTasksQuery();
                if (uploadedTasks != null && uploadedTasks.Count > 0)
                {
                    TaskPerformer.LoadTasks(uploadedTasks, dataGridView1);

                    tasks = uploadedTasks;

                    timers = ConfigureTimersAndColors(tasks);

                    toolStripStatusLabel3.Text = ServiceMessage.TasksLoaded;
                }

                long totalTasks = DatabaseConnector.CountTasksQuery();
                if (totalTasks > -1)
                {
                    nextTaskId = totalTasks;
                }
            }
            else
            {
                toolStripStatusLabel2.Image = Properties.Resources.db_error_mark;
                toolStripStatusLabel2.Text = "Соединение с базой данных не установлено.";
            }
        }

        private void timer_Tick_eventYellow(object sender, EventArgs e, int index)
        {
            System.Windows.Forms.Timer timer = (System.Windows.Forms.Timer)sender;
            timer.Enabled = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[1].Value != null && (long)row.Cells[1].Value == tasks[index].Id)
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;
                }
            }
        }

        private void timer_Tick_eventOrange(object sender, EventArgs e, int index)
        {
            System.Windows.Forms.Timer timer = (System.Windows.Forms.Timer)sender;
            timer.Enabled = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[1].Value != null && (long)row.Cells[1].Value == tasks[index].Id)
                {
                    row.DefaultCellStyle.BackColor = Color.Orange;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;
                }
            }
        }

        private void timer_Tick_eventRed(object sender, EventArgs e, int index)
        {
            System.Windows.Forms.Timer timer = (System.Windows.Forms.Timer)sender;
            timer.Enabled = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[1].Value != null && (long)row.Cells[1].Value == tasks[index].Id)
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.ForeColor = Color.White;
                    break;
                }
            }
        }

        private void timer_Tick_eventGray(object sender, EventArgs e, int index)
        {
            System.Windows.Forms.Timer timer = (System.Windows.Forms.Timer)sender;
            timer.Enabled = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[1].Value != null && (long)row.Cells[1].Value == tasks[index].Id)
                {
                    row.DefaultCellStyle.BackColor = Color.Gray;
                    row.DefaultCellStyle.ForeColor = Color.White;
                    break;
                }
            }
        }

        private List<List<System.Windows.Forms.Timer>> ConfigureTimersAndColors(List<Task> tasks)
        {
            List<List<System.Windows.Forms.Timer>> timers = new List<List<System.Windows.Forms.Timer>>(colorMills.Length);

            for (int i = 0; i < colorMills.Length; i++)
            {
                List<System.Windows.Forms.Timer> inner = new List<System.Windows.Forms.Timer>();
                timers.Add(inner);
            }

            for (int i = 0; i < tasks.Count; i++) 
            {
                for (int j = 0; j < timers.Count; j++)
                {
                    if (j < timers.Count - 1) { timers[j].Add(ConfigureTimer(tasks[i], colorMills[j], colorMills[j + 1], i, j)); }
                    else { timers[j].Add(ConfigureTimer(tasks[i], colorMills[j], null, i, j)); }
                }
            }

            return timers;
        }

        private System.Windows.Forms.Timer ConfigureTimer(Task task, int colorMills, int? nextColorMills, int timerIndex, int colorId)
        {
            TimeSpan interval = task.ExecutionTime - DateTime.Now;

            if (interval.TotalMilliseconds > colorMills)
            {
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = (int)interval.TotalMilliseconds - colorMills;

                switch (colorId)
                {
                    case 0: timer.Tick += (s, ev) => timer_Tick_eventYellow(s, ev, timerIndex); break;
                    case 1: timer.Tick += (s, ev) => timer_Tick_eventOrange(s, ev, timerIndex); break;
                    case 2: timer.Tick += (s, ev) => timer_Tick_eventRed(s, ev, timerIndex); break;
                    case 3: timer.Tick += (s, ev) => timer_Tick_eventGray(s, ev, timerIndex); break;
                }
                
                timer.Enabled = true;
                timer.Start();

                return timer;
            }
            else
            {
                if (nextColorMills != null)
                {
                    if (interval.TotalMilliseconds <= colorMills && interval.TotalMilliseconds > nextColorMills)
                    {
                        dataGridView1.Rows[timerIndex].DefaultCellStyle.BackColor = TimerPerformer.GetBackColor(colorId);
                        dataGridView1.Rows[timerIndex].DefaultCellStyle.ForeColor = TimerPerformer.GetForeColor(colorId);
                    }
                }
                else
                {
                    if (interval.TotalMilliseconds <= colorMills)
                    {
                        dataGridView1.Rows[timerIndex].DefaultCellStyle.BackColor = TimerPerformer.GetBackColor(colorId);
                        dataGridView1.Rows[timerIndex].DefaultCellStyle.ForeColor = TimerPerformer.GetForeColor(colorId);
                    }
                }

                return null;
            }
        }
    }
}

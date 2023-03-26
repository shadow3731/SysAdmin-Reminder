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
        private long nextTaskId = 0;
        private int textLength = 0;

        public Form1()
        {
            InitializeComponent();

            Settings.LoadSettings();

            switch (Settings.UserSettings["Language"])
            {
                case "English": culture = new CultureInfo("en-EN"); break;
                case "Русский": culture = new CultureInfo("ru-RU"); break;
                default: culture = new CultureInfo("en-EN"); break;
            }
            CultureInfo.CurrentCulture = culture;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel3.Text = ServiceMessage.ApplicationLaunched;

            if (Settings.UserSettings["LaunchingWindowSize"] != null)
            {
                if (Settings.UserSettings["LaunchingWindowSize"] == Settings.LanguagePack["LaunchingWindowSizeMax"])
                    Size = MaximumSize;
                else if (Settings.UserSettings["LaunchingWindowSize"] == Settings.LanguagePack["LaunchingWindowSizeMin"])
                    Size = MinimumSize;
            }

            groupBox1.Text = Settings.LanguagePack["AddNewTask"];

            label1.Text = Settings.LanguagePack["Task"];
            label2.Text = Settings.LanguagePack["ProceedAt"];
            label3.Text = Settings.LanguagePack["Priority"];

            List<string> priorities = new List<string>(Priority.All.Length);
            for (int i = 0; i < Priority.All.Length; i++)
            {
                priorities.Add(Priority.All[i][Settings.LanguageCode]);
            }
            comboBox1.Items.AddRange(priorities.ToArray());
            comboBox1.SelectedIndex = 0;

            button1.Text = Settings.LanguagePack["AddTask"];
            button2.Text = Settings.LanguagePack["ShowHistory"];
            button3.Text = Settings.LanguagePack["Settings"];
            button4.Text = Settings.LanguagePack["Quit"];
            button5.Text = Settings.LanguagePack["Restart"];

            timer2.Start();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            dataGridView1.Width = ClientSize.Width - 24;
            dataGridView1.Height = ClientSize.Height - 183;

            groupBox1.Top = ClientSize.Height - 164;
            groupBox1.Width = ClientSize.Width - 24;

            richTextBox1.Width = ClientSize.Width - 192;

            label2.Left = ClientSize.Width - 181;

            dateTimePicker1.Left = ClientSize.Width - 178;

            label3.Left = ClientSize.Width - 181;

            comboBox1.Left = ClientSize.Width - 178;

            double k1 = 0.1924;
            double k2 = 1 - ((MaximumSize.Width - 16 - ClientSize.Width) * 0.0343 / (MaximumSize.Width - MinimumSize.Width - 32));
            
            button1.Top = ClientSize.Height - 51;
            button1.Width = (int)Math.Round(ClientSize.Width * k1 * k2);

            button2.Left = button1.Left + button1.Width + 6;
            button2.Top = ClientSize.Height - 51;
            button2.Width = (int)Math.Round(ClientSize.Width * (k1 - 0.0015) * k2);

            button3.Left = button2.Left + button2.Width + 6;
            button3.Top = ClientSize.Height - 51;
            button3.Width = (int)Math.Round(ClientSize.Width * (k1 - 0.0015) * k2);

            button4.Left = button3.Left + button3.Width + 6;
            button4.Top = ClientSize.Height - 51;
            button4.Width = (int)Math.Round(ClientSize.Width * (k1 - 0.0015) * k2);

            button5.Left = button4.Left + button4.Width + 6;
            button5.Top = ClientSize.Height - 51;
            button5.Width = (int)Math.Round(ClientSize.Width * k1 * k2);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1 && textLength > 0) { button1_Click(sender, e); }

            if (e.KeyCode == Keys.F2) { button2_Click(sender, e); }

            if (e.KeyCode == Keys.F3) { button3_Click(sender, e); }

            if (e.KeyCode == Keys.F5) { button5_Click(sender, e); }
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
            toolStripStatusLabel3.Text = ServiceMessage.TaskSaving;

            Task task = new Task(++nextTaskId, richTextBox1.Text, dateTimePicker1.Value, comboBox1.SelectedItem);

            if (dataGridView1.DataSource == null) { TaskPerformer.DataGridView_CreateColumns(dataGridView1); }

            if (TaskPerformer.SaveTask(task, dataGridView1, false))
            {
                for (int i = 0; i < timers.Count; i++)
                {
                    if (i < timers.Count - 1) { timers[i].Add(ConfigureTimer(task, Settings.CollorMills[i], Settings.CollorMills[i + 1], timers[i].Count, i)); }
                    else { timers[i].Add(ConfigureTimer(task, Settings.CollorMills[i], null, timers[i].Count, i)); }
                }

                tasks.Add(task);

                textLength = 0;

                richTextBox1.Clear();
                dateTimePicker1.Value = DateTime.Now;
                comboBox1.SelectedIndex = 0;
                button1.Enabled = false;

                toolStripStatusLabel3.Text = ServiceMessage.TaskAdded;
            }
            else toolStripStatusLabel3.Text = ServiceMessage.TaskNotAdded;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 form = new Form3();
            form.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                toolStripStatusLabel3.Text = ServiceMessage.TaskSaving;

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
                            tasks[changingId] = updatedTask;

                            if (e.ColumnIndex == 3)
                            {
                                for (int i = 0; i < timers.Count; i++)
                                {
                                    if (timers[i][changingId] != null) { timers[i][changingId].Enabled = false; }

                                    if (i < timers.Count - 1) { timers[i][changingId] = ConfigureTimer(task, Settings.CollorMills[i], Settings.CollorMills[i + 1], changingId, i); }
                                    else { timers[i][changingId] = ConfigureTimer(task, Settings.CollorMills[i], null, changingId, i); }
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

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e) { }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                e.CellStyle.BackColor = Color.White;
                e.CellStyle.ForeColor = Color.Black;
            }
        }

        private void EndTaskMenuItem_Click(object sender, EventArgs e)
        {
            int rowId = dataGridView1.CurrentCell.RowIndex;

            long id = (long)dataGridView1.Rows[rowId].Cells[1].Value;
            Task task = tasks.Find(x => x.Id == id);
            int deletingId = tasks.IndexOf(task);

            string statusToSend = null;
            if (sender.ToString() == toolStripMenuItem1.Text) { statusToSend = Status.Finished; }
            else if (sender.ToString() == toolStripMenuItem2.Text) { statusToSend = Status.Cancelled; }

            if (TaskPerformer.DeleteTask(task, statusToSend))
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
                        
                        if (j < timers.Count - 1) { timers[j][i] = ConfigureTimer(tasks[i], Settings.CollorMills[j], Settings.CollorMills[j + 1], i, j); }
                        else { timers[j][i] = ConfigureTimer(tasks[i], Settings.CollorMills[j], null, i, j); }
                    }
                }

                if (sender.ToString() == toolStripMenuItem1.Text) { toolStripStatusLabel3.Text = ServiceMessage.TaskFinished; }
                else if (sender.ToString() == toolStripMenuItem2.Text) { toolStripStatusLabel3.Text = ServiceMessage.TaskCancelled; }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = DateTime.Now.ToString("dddd", culture) + " " 
                + DateTime.Now.ToString(TaskPerformer.DateTimeFormat + ":ss");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Interval = int.Parse(Settings.UserSettings["PeriodSyncrinizationTime"]) * 60000;

            if (DatabaseConnector.HasConnection())
            {
                toolStripStatusLabel2.Image = Properties.Resources.db_success_mark;
                toolStripStatusLabel2.Text = Settings.LanguagePack["DBStatusEstablished"];

                DatabaseConnector.CreateTableIfNotExists();

                dataGridView1.Rows.Clear();

                timers.Clear();

                List<Task> uploadedTasks = DatabaseConnector.GetTasksQuery(Status.Running);

                if (uploadedTasks != null && uploadedTasks.Count > 0)
                {
                    TaskPerformer.DataGridView_CreateColumns(dataGridView1);
                    TaskPerformer.LoadTasks(uploadedTasks, dataGridView1, false);

                    tasks = uploadedTasks;

                    timers = ConfigureTimersAndColors(tasks);

                    toolStripStatusLabel3.Text = ServiceMessage.TasksLoaded;
                }

                long totalTasks = DatabaseConnector.CountTasksQuery();
                if (totalTasks > -1) { nextTaskId = totalTasks; }
            }
            else
            {
                toolStripStatusLabel2.Image = Properties.Resources.db_error_mark;
                toolStripStatusLabel2.Text = Settings.LanguagePack["DBStatusAbsent"];
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
            List<List<System.Windows.Forms.Timer>> timers = new List<List<System.Windows.Forms.Timer>>(Settings.CollorMills.Length);

            for (int i = 0; i < Settings.CollorMills.Length; i++)
            {
                List<System.Windows.Forms.Timer> inner = new List<System.Windows.Forms.Timer>();
                timers.Add(inner);
            }

            for (int i = 0; i < tasks.Count; i++) 
            {
                for (int j = 0; j < timers.Count; j++)
                {
                    if (j < timers.Count - 1) { timers[j].Add(ConfigureTimer(tasks[i], Settings.CollorMills[j], Settings.CollorMills[j + 1], i, j)); }
                    else { timers[j].Add(ConfigureTimer(tasks[i], Settings.CollorMills[j], null, i, j)); }
                }
            }

            return timers;
        }

        private System.Windows.Forms.Timer ConfigureTimer(Task task, int colorMills, int? nextColorMills, int timerIndex, int colorId)
        {
            TimeSpan interval = task.ExecutionTime - DateTime.Now;
            
            if (interval.TotalMilliseconds > Settings.CollorMills[0])
            {
                dataGridView1.Rows[timerIndex].DefaultCellStyle.BackColor = TimerPerformer.GetBackColor(-1);
                dataGridView1.Rows[timerIndex].DefaultCellStyle.ForeColor = TimerPerformer.GetForeColor(-1);
            }

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
            
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysAdmin_Remider
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Text = Settings.LanguagePack["TasksHistory"];

            button1.Text = Settings.LanguagePack["CloseWindow"];

            List<Task> tasks = DatabaseConnector.GetTasksQuery();

            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            TaskPerformer.DataGridView_CreateColumns(dataGridView1);
            TaskPerformer.LoadTasks(tasks, dataGridView1, true);
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            dataGridView1.Width = this.ClientSize.Width - 25;
            dataGridView1.Height = this.ClientSize.Height - 53;

            button1.Left = this.ClientSize.Width - 281;
            button1.Top = this.ClientSize.Height - 34;
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                button1_Click(sender, e);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

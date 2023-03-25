using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysAdmin_Remider
{
    public partial class Form3 : Form
    {
        private string updateVersion;

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Text = Settings.LanguagePack["SettingsWindow"];

            tabControl1.TabPages[0].Text = Settings.LanguagePack["Interface"];
            tabControl1.TabPages[1].Text = Settings.LanguagePack["Database"];
            tabControl1.TabPages[2].Text = Settings.LanguagePack["Help"];

            label1.Text = Settings.LanguagePack["Address"];
            label2.Text = Settings.LanguagePack["Port"];
            label3.Text = Settings.LanguagePack["DBName"];
            label4.Text = Settings.LanguagePack["DBUsername"];
            label5.Text = Settings.LanguagePack["DBPassword"];
            label6.Text = Settings.LanguagePack["Language"];
            label7.Text = Settings.LanguagePack["LaunchingWindowSize"];
            label8.Text = Settings.LanguagePack["Yellow"];
            label9.Text = Settings.LanguagePack["Orange"];
            label10.Text = Settings.LanguagePack["Red"];
            label11.Text = Settings.LanguagePack["Gray"];

            if (Settings.LanguagePack["TimersActivizationNote"].Contains("\\n"))
            {
                int newLineIndex = Settings.LanguagePack["TimersActivizationNote"].IndexOf("\\n");
                label12.Text = Settings.LanguagePack["TimersActivizationNote"].Substring(0, newLineIndex)
                    + Environment.NewLine + Settings.LanguagePack["TimersActivizationNote"].Substring(newLineIndex + 2);
            }
            else
            {
                label12.Text = Settings.LanguagePack["TimersActivizationNote"];
            }

            if (Settings.LanguagePack["PeriodSyncrinizationTime"].Contains("\\n")) 
            {
                int newLineIndex = Settings.LanguagePack["PeriodSyncrinizationTime"].IndexOf("\\n");
                label13.Text = Settings.LanguagePack["PeriodSyncrinizationTime"].Substring(0, newLineIndex)
                    + Environment.NewLine + Settings.LanguagePack["PeriodSyncrinizationTime"].Substring(newLineIndex + 2);
            }
            else
            {
                label13.Text = Settings.LanguagePack["PeriodSyncrinizationTime"];
            }
            

            label14.Text = Settings.LanguagePack["ChangesNote"];
            label16.Text = Settings.LanguagePack["Version"] + Settings.LanguagePack["CurrentVersion"];

            linkLabel1.Text = Settings.LanguagePack["DeveloperPage"];
            linkLabel3.Text = Settings.LanguagePack["DBGuide"];
            linkLabel4.Text = Settings.LanguagePack["DBVideoguide"];

            groupBox1.Text = Settings.LanguagePack["ConnectionSettings"];
            groupBox2.Text = Settings.LanguagePack["TimersActivizationMoment"];
            groupBox3.Text = Settings.LanguagePack["AboutApplication"];
            groupBox4.Text = Settings.LanguagePack["DBHowToConnect"];

            comboBox1.Items.Add(Settings.LanguagePack["LanguageEN"]);
            comboBox1.Items.Add(Settings.LanguagePack["LanguageRU"]);
            comboBox1.SelectedIndex = comboBox1.Items.IndexOf(Settings.UserSettings["Language"]);

            comboBox2.Items.Add(Settings.LanguagePack["LaunchingWindowSizeMax"]);
            comboBox2.Items.Add(Settings.LanguagePack["LaunchingWindowSizeMin"]);
            int selectingIndex = comboBox2.Items.IndexOf(Settings.UserSettings["LaunchingWindowSize"]);
            if (selectingIndex != -1) comboBox2.SelectedIndex = selectingIndex;
            else comboBox2.SelectedIndex = 0;

            textBox1.Text = Settings.UserSettings["Address"];
            textBox2.Text = Settings.UserSettings["Port"];
            textBox3.Text = Settings.UserSettings["DBName"];
            textBox4.Text = Settings.UserSettings["DBUsername"];
            textBox5.Text = Settings.UserSettings["DBPassword"];
            textBox6.Text = Settings.UserSettings["Yellow"];
            textBox7.Text = Settings.UserSettings["Orange"];
            textBox8.Text = Settings.UserSettings["Red"];
            textBox9.Text = Settings.UserSettings["Gray"];
            textBox10.Text = Settings.UserSettings["PeriodSyncrinizationTime"];

            button1.Text = Settings.LanguagePack["OK"];
            button2.Text = Settings.LanguagePack["Cancel"];
            button3.Text = Settings.LanguagePack["Reset"];

            CheckForUpdates();
        }

        private void Form3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { button1_Click(sender, e); }

            if (e.KeyCode == Keys.Escape) { button2_Click(sender, e); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IsCorrectData())
            {
                Settings.UserSettings["Language"] = comboBox1.SelectedItem.ToString();
                Settings.UserSettings["LaunchingWindowSize"] = comboBox2.SelectedItem.ToString();
                Settings.UserSettings["Address"] = textBox1.Text;
                Settings.UserSettings["Port"] = textBox2.Text;
                Settings.UserSettings["DBName"] = textBox3.Text;
                Settings.UserSettings["DBUsername"] = textBox4.Text;
                Settings.UserSettings["DBPassword"] = textBox5.Text;
                Settings.UserSettings["Yellow"] = textBox6.Text;
                Settings.UserSettings["Orange"] = textBox7.Text;
                Settings.UserSettings["Red"] = textBox8.Text;
                Settings.UserSettings["Gray"] = textBox9.Text;
                Settings.UserSettings["PeriodSyncrinizationTime"] = textBox10.Text;

                Settings.SaveSettings();

                Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = comboBox1.Items.IndexOf(Settings.DefaultUserSettings["Language"]);
            comboBox2.SelectedIndex = comboBox2.Items.IndexOf(Settings.DefaultUserSettings["LaunchingWindowSize"]);

            textBox1.Text = Settings.DefaultUserSettings["Address"];
            textBox2.Text = Settings.DefaultUserSettings["Port"];
            textBox3.Text = Settings.DefaultUserSettings["DBName"];
            textBox4.Text = Settings.DefaultUserSettings["DBUsername"];
            textBox5.Text = Settings.DefaultUserSettings["DBPassword"];
            textBox6.Text = Settings.DefaultUserSettings["Yellow"];
            textBox7.Text = Settings.DefaultUserSettings["Orange"];
            textBox8.Text = Settings.DefaultUserSettings["Red"];
            textBox9.Text = Settings.DefaultUserSettings["Gray"];
            textBox10.Text = Settings.DefaultUserSettings["PeriodSyncrinizationTime"];
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.com/v1rus/");
        }

        private bool IsCorrectData()
        {
            if (comboBox1.SelectedItem == null) 
            {
                MessageBox.Show(Settings.LanguagePack["SettingsWrongLanguage"], Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; 
            }

            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show(Settings.LanguagePack["SettingsWrongStartSize"], Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!int.TryParse(textBox6.Text, out int value1) || !int.TryParse(textBox7.Text, out int value2) 
                || !int.TryParse(textBox8.Text, out int value3) || !int.TryParse(textBox9.Text, out int value4))
            {
                MessageBox.Show(Settings.LanguagePack["SettingsWrongTimerValue"], Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!int.TryParse(textBox10.Text, out int value5) || value5 < 1)
            {
                MessageBox.Show(Settings.LanguagePack["SettingsWrongSyncTime"], Settings.LanguagePack["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private async void CheckForUpdates()
        {
            using (var httpClient = new HttpClient())
            {
                string url = "https://raw.githubusercontent.com/shadow3731/SpringChatTest/master/LastVersion.txt";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                updateVersion = await response.Content.ReadAsStringAsync();
                
                if (updateVersion != null && updateVersion != Settings.LanguagePack["CurrentVersion"])
                {
                    linkLabel2.Text = Settings.LanguagePack["UpdateVersion"] + " " + updateVersion + ")";
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nirCharidy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        googleSheets Gsheets = new googleSheets();

        private async void Form1_Load(object sender, EventArgs e)
        {
            projectTimer();
            if (Properties.Settings.Default.Members == null)
                Properties.Settings.Default.Members = new List<List<string>>() { new List<string>() { "-" } };
            //Properties.Settings.Default.Menual =0;            
            //Properties.Settings.Default.menualString = "";
            Properties.Settings.Default.Save();
            frm2.label4.Text = Properties.Settings.Default.matrim1;
            frm2.label6.Text = Properties.Settings.Default.matrim2;
            frm2.label7.Text = Properties.Settings.Default.matrim3;
            frm2.label9.Text = Properties.Settings.Default.matrim4;
            frm2.label11.Text = Properties.Settings.Default.matrim5;
            richTextBox3.Text = Properties.Settings.Default.menualString;
            frm2.FormClosing += new FormClosingEventHandler(frm2_Closing);
            getSumFromSite();
            await Task.Delay(1500);
            //Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/עולים_קומה/משתמשים");
            synchronizer();
            checkBox1.Checked = true;

        }

        private async void Alert(string title, string message, bool confirm)
        {
            title = title.Replace(" ", "%20");
            message = message.Replace(" ", "%20");
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.pushalert.co/rest/v1/send"))
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", "api_key=4cfd7b98060a9d115ea6f6ff876163a5");
                        request.Content = new StringContent("title=" + title + "&message=" + message + "&icon=&url=about:blank", Encoding.UTF8, "application/x-www-form-urlencoded");
                        textBox1.Text = string.Empty;
                        var response = await httpClient.SendAsync(request);
                    }
                }
                if (confirm)
                    MessageBox.Show("הודעה נשלחה בהצלחה!");
            }
            catch (Exception x) { MessageBox.Show("שגיאה בשליחת ההודעה:" + Environment.NewLine + Environment.NewLine + x.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string message = " ";
            if (textBox1.Text == string.Empty)
                return;
            message = textBox1.Text;
            Alert(message, "-", true);
            //textBox1.Text = "";

        }

        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();

        DateTime endTime = new DateTime(2018, 11, 29, 22, 0, 0);
        private void projectTimer()
        {
            t.Interval = 500;
            t.Tick += new EventHandler(t_Tick);
            TimeSpan ts = endTime.Subtract(DateTime.Now);
            label22.Text = ts.ToString("d' : 'h' : 'm' : 's' '");
            t.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = endTime.Subtract(DateTime.Now);
            int days = ts.Days;
            int hours = ts.Hours;
            int minutes = ts.Minutes;
            int seconds = ts.Seconds;
            string Sminutes = minutes.ToString();
            string Sseconds = seconds.ToString();
            hours = days * 24 + hours;
            string Shours = hours.ToString();
            if (Shours.Length == 1)
                Shours = "0" + Shours;
            if (Sminutes.Length == 1)
                Sminutes = "0" + Sminutes;
            if (Sseconds.Length == 1)
                Sseconds = "0" + Sseconds;
            label22.Text = Shours + " : " + Sminutes + " : " + Sseconds;
        }

        private void masheu()
        {
            IList<IList<object>> values = Gsheets.getValues("dfgdf", "dfgfdg");
            if (values != null && values.Count > 0)
            {
                //    Console.WriteLine("Name, Major");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    //richTextBox1.Text += row[0].ToString() + " , " + row[1].ToString() + " , " + row[2].ToString() + " , " + row[4].ToString() + Environment.NewLine;
                }
            }
            else
            {
                MessageBox.Show("No data found.");
            }
        }

        int AdCo = 0;
        bool syncBusy = false;
        public IList<IList<object>> nowTitle;

        private async void synchronizer()
        {
            if (!syncPlease || syncBusy)
                return;
            if (!CheckForInternetConnection())
            {
                label15.Show();
                label15.Text = "אין חיבור לאינטרנט";
                return;
            }
            syncBusy = true;
            label15.Show();
            label15.Text = "מסנכרן עכשיו...";
            richTextBox2.Text = "מסנכרן עכשיו...";
            bool allGood = true;
            int totalSum = 0;
            titles = new List<string>() { "yyy" };
            backgroundWorker1.RunWorkerAsync();

            while (titles[0] == "yyy")
            {
                if (!syncPlease)
                { return; }
                await Task.Delay(1000);
            }
            List<List<string>> members = Properties.Settings.Default.Members;
            int place = 0;
            if (titles != null)
                for (int i = 0; i < titles.Count; i++)
                {

                    for (int o = 0; o < members.Count; o++)
                    {
                        if (members[o][0] == titles[i])
                        { place = o; goto here; }
                    }
                    members.Add(new List<string>() { titles[i], "0" });
                    place = members.Count - 1;
                here:;
                    nowTitle = null;

                    backgroundWorker2.RunWorkerAsync(argument: titles[i]);
                    while (nowTitle == null)
                    {
                        if (!syncPlease)
                            return;
                        await Task.Delay(1000);
                    }
                    IList<IList<object>> values = nowTitle;

                    if (values != null && values.Count > 0)
                    {
                        foreach (var row in values)
                        {
                            try
                            {
                                int sum = 0;
                                if (row.Count > 0)
                                    sum = Convert.ToInt32(row[0].ToString().Replace(",", ""));
                                sum = sum * 3;

                                if (sum > Convert.ToInt32(members[place][1]))
                                {
                                    Alert(titles[i] + " גייס עוד " + (sum - Convert.ToInt32(members[place][1])).ToString() + " ש\"ח!", "-", false);
                                    insertMatrim(titles[i] + " : " + (sum - Convert.ToInt32(members[place][1])).ToString() + " ש\"ח ");
                                }
                                members[place][1] = sum.ToString();
                                if (richTextBox2.Text == "מסנכרן עכשיו...")
                                    richTextBox2.Text = "";
                                updateDataRichText(members[place][0], members[place][1]);
                                totalSum += sum;
                            }
                            catch (Exception x) { /*MessageBox.Show("בעיה בסנכרון של " + titles[i] + Environment.NewLine + Environment.NewLine + x.Message); */}
                        }
                    }
                }

            //totalSum = totalSum + Properties.Settings.Default.Menual;
            //label2.Text = totalSum.ToString("N0");            
            ////label2.Text = Psikim(label2.Text);
            Properties.Settings.Default.Members = members;
            Properties.Settings.Default.Save();
            label15.Hide();
            syncBusy = false;
            syncRestarter();
        }

        private void updateDataRichText(string name, string sum)
        {
            richTextBox2.Text += Environment.NewLine + name + "  :  " + sum + Environment.NewLine;
            label21.Text = DateTime.Now.ToString();
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new System.Net.WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private string Psikim(string number)
        {
            number = number.Insert(number.Length - 3, ",");
            if (number.Length > 6)
                number = number.Insert(number.Length - 8, ",");
            return number;
        }

        private async void syncRestarter()
        {
            await Task.Delay(105000);
            synchronizer();
        }

        bool syncPlease = true;

        private void button2_Click(object sender, EventArgs e)
        {
            label15.Hide();
            label25.Show();
            syncPlease = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label15.Show();
            label25.Hide();
            syncPlease = true;
            synchronizer();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == string.Empty || comboBox1.Text == string.Empty)
            {
                MessageBox.Show("סכום התרומה וצורת הגביה הם שדות חובה");
                return;
            }
            try { int i = Convert.ToInt32(textBox4.Text); } catch { return; }

            richTextBox3.Text += Environment.NewLine + DateTime.Now.ToString() + " : " + Environment.NewLine + " סכום : " + textBox4.Text + Environment.NewLine + " תורם: " + textBox2.Text + Environment.NewLine + " . מתרים : " + textBox3.Text + Environment.NewLine + " . צורת הגביה : " + comboBox1.Text + " . " + Environment.NewLine + Environment.NewLine;
            Properties.Settings.Default.menualString = richTextBox3.Text;
            Properties.Settings.Default.Save();
            int sum = Convert.ToInt32(textBox4.Text);
            textBox2.Text = textBox3.Text = textBox4.Text = "";
            Properties.Settings.Default.Menual += sum;
            Properties.Settings.Default.Save();
            MessageBox.Show("בוצע בהצלחה!");
            int balance = Convert.ToInt32(label2.Text.Replace(",", ""));
            balance = balance + sum;
            label2.Text = balance.ToString("N0");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox5.Text == string.Empty || textBox7.Text == string.Empty)
            {
                MessageBox.Show("כל השדות הם שדות חובה");
                return;
            }
            try { int i = Convert.ToInt32(textBox5.Text); } catch { return; }
            richTextBox3.Text += Environment.NewLine + DateTime.Now.ToString() + " : " + Environment.NewLine + " סכום מוחסר : -" + textBox5.Text + Environment.NewLine + " שם המבצע: " + textBox7.Text;
            Properties.Settings.Default.menualString = richTextBox3.Text;
            Properties.Settings.Default.Save();
            int sum = 0;
            sum = Convert.ToInt32(textBox5.Text);
            textBox5.Text = textBox7.Text = "";
            Properties.Settings.Default.Menual -= sum;
            int balance = Convert.ToInt32(label2.Text.Replace(",", ""));
            balance = balance - sum;
            label2.Text = balance.ToString("N0");
            Properties.Settings.Default.Save();
            MessageBox.Show("בוצע בהצלחה!");
        }

        private void label2_TextChanged(object sender, EventArgs e)
        {

        }

        Form2 frm2 = new Form2();

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                frm2.Show();
                frm2.WindowState = FormWindowState.Maximized;
                return;
            }
            if (!checkBox1.Checked)
            {
                frm2.Hide();
                return;
            }
        }

        private void frm2_Closing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            frm2.Hide();
            checkBox1.Checked = false;
        }

        private void label22_TextChanged(object sender, EventArgs e)
        {
            frm2.label1.Text = label22.Text.Replace(" ", "");
        }

        private void label24_TextChanged(object sender, EventArgs e)
        {
            frm2.label3.Text = label24.Text;
        }

        public List<string> titles = new List<string>() { "" };

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> titless = Gsheets.sheetsNames();
            e.Result = titless;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            titles = (List<string>)e.Result;
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            IList<IList<object>> values = Gsheets.getValues((string)e.Argument, "B1");
            if (values == null)
                values = Gsheets.getValues((string)e.Argument, "C1");
            e.Result = values;
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            nowTitle = (IList<IList<object>>)e.Result;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://www.causematch.com/he/projects/yeshivat-nir/?gclid=CjwKCAiAlvnfBRA1EiwAVOEgfOH3M2T30lE2aySjUfCXFizzNU3QArnYafkk3QWsKdhRB8znVzzVChoCmhsQAvD_BwE");
        }

        private void getSumFromSite()
        {

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 15000;
            timer.Enabled = true;
            timer.Start();
            timer.Tick += (s, e) =>
            {
                try
                {
                    frm2.label5.Text = frm2.label8.Text = frm2.label10.Text = frm2.label12.Text = "";
                    string sum = webBrowser1.Document.GetElementById("cm-pda").OuterText;
                    frm2.label2.Text = sum;
                    foreach (HtmlElement h in webBrowser1.Document.GetElementsByTagName("span"))
                    {
                        if (h.GetAttribute("className") == "rdp-donor-name")
                        {
                            if (frm2.label5.Text == "")
                            { frm2.label5.Text = h.OuterText; goto end; }
                            if (frm2.label8.Text == "")
                            { frm2.label8.Text = h.OuterText; goto end; }
                            if (frm2.label10.Text == "")
                            { frm2.label10.Text = h.OuterText; goto end; }
                            if (frm2.label12.Text == "")
                            { frm2.label12.Text = h.OuterText; goto end; }
                            break;
                        }
                    end:;
                    }
                    int i = 1;
                    foreach (HtmlElement h in webBrowser1.Document.GetElementsByTagName("i"))
                    {
                        if (h.GetAttribute("className") == "rdp-donor-amount")
                        {
                            if (i == 1)
                            { frm2.label5.Text += " : " + h.OuterText; i = 2; goto endd; }
                            if (i == 2)
                            { frm2.label8.Text += " : " + h.OuterText; i = 3; goto endd; }
                            if (i == 3)
                            { frm2.label10.Text += " : " + h.OuterText; i = 4; goto endd; }
                            if (i == 4)
                            { frm2.label12.Text += " : " + h.OuterText; break; }
                        }
                    endd:;
                    }
                }
                catch { }
            };

        }

        private void insertMatrim(string name)
        {
            frm2.label11.Text = frm2.label9.Text;
            frm2.label9.Text = frm2.label7.Text;
            frm2.label7.Text = frm2.label6.Text;
            frm2.label6.Text = frm2.label4.Text;
            frm2.label4.Text = name;

            Properties.Settings.Default.matrim1 = frm2.label4.Text;
            Properties.Settings.Default.matrim2 = frm2.label6.Text;
            Properties.Settings.Default.matrim3 = frm2.label7.Text;
            Properties.Settings.Default.matrim4 = frm2.label9.Text;
            Properties.Settings.Default.matrim5 = frm2.label11.Text;
            Properties.Settings.Default.Save();
        }
    }
}

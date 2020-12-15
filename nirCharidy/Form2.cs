using System;
using System.Windows.Forms;

namespace nirCharidy
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void label2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string sum = label2.Text;
                sum = sum.Replace(",", "");
                sum = sum.Replace(" ", "");
                sum = sum.Replace("₪", "");
                double summ = Convert.ToInt32(sum);
                double ahuz = Math.Round((double)(summ / 30000), 2);
                //ahuz = Math.Round(ahuz, 2);
                label3.Text = ahuz.ToString();
                label3.Text = label3.Text + "%";
            }
            catch { }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }
    }
}

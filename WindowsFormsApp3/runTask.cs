using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class runTask : Form
    {
        public runTask()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtOpen.Text))

            {
                try
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = txtOpen.Text;
                    proc.Start();
                    // RefreshProcessList();
                   // this.Refresh();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message,"Message",MessageBoxButtons.OK,MessageBoxIcon.Error);
                       
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
                txtOpen.Text = openFileDialog.FileName;
            




        }

        private void runTask_Load(object sender, EventArgs e)
        {

        }
    }
}

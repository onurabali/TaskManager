using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
          //  GetWindowServices();
            
        }

     
        private void RefreshProcessList()
        {
            listView1.Items.Clear();
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                try
                {
                    string cpuUsage = GetCpuUsage(process).ToString("0.00");
                    string memoryUsage = GetMemoryUsage(process).ToString("0.00");

                    ListViewItem item = new ListViewItem(process.ProcessName);
                    item.SubItems.Add(cpuUsage);
                    item.SubItems.Add(memoryUsage);
                    listView1.Items.Add(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata: " + ex.Message);
                }
            }
        }
        private float GetCpuUsage(Process process)
        {
            return process.TotalProcessorTime.Ticks / (float)DateTime.Now.Subtract(process.StartTime).Ticks * 100 / Environment.ProcessorCount;
        }

        private float GetMemoryUsage(Process process)
        {
            float memoryUsage = 0;

            memoryUsage = process.WorkingSet64;

            long totalMemory = GetTotalMemory();
            return memoryUsage / (float)totalMemory * 100;
        }

        private long GetTotalMemory()
        {
            long totalMemory = 0;

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    totalMemory = Convert.ToInt64(obj["TotalPhysicalMemory"]);
                }
            }

            return totalMemory;
        }

        

/*
       void GetProcesses()
        {
            proc = Process.GetProcesses();
            listBox2.Items.Clear();
            foreach (Process p in proc)
            {
                listView1.Items.Add(p.ProcessName);
            }
        }
       */
        /*
        private void GetWindowServices()
        {
            // throw new NotImplementedException();
            ServiceController[] service;
            service = ServiceController.GetServices();
            listBox1.Items.Clear();
            for(int i=0; i<service.Length; i++)
            {
                listBox1.Items.Add(service[i].ServiceName);
            }

        }
        */
        Process[] proc;
        
       void GetAllProcess()
        {
            proc = Process.GetProcesses();
            listView1.Items.Clear();
            foreach(Process p in proc)
            {
                listView1.Items.Add(p.ProcessName);
            }
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
          //  GetAllProcess();
            listView1.View = View.Details;
            listView1.Columns.Add("İşlem Adı",200);
            listView1.Columns.Add("CPU Kullanımı % ",120);
            listView1.Columns.Add("Bellek Kullanımı % ",120);
            RefreshProcessList();
            listView2.View = View.Details;
            listView2.Columns.Add("Ad", 200);
            listView2.Columns.Add("PID", 120);
            listView2.Columns.Add("Durum", 120);
            RefreshServiceList();
            listView3.View = View.Details;
            listView3.Columns.Add("Kullanıcı Adı", 200);
            listView3.Columns.Add("CPU Kullanımı %", 120);
            listView3.Columns.Add("Bellek Kullanımı (%)", 120);
            RefreshUserList();

        }
        private void RefreshUserList()
        {
            listView3.Items.Clear();
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                try
                {
                    string userName = GetProcessOwner(process.Id);
                    string cpuUsage = GetCpuUsage(process).ToString("0.00");
                    string memoryUsage = GetMemoryUsage(process).ToString("0.00");

                    ListViewItem item = new ListViewItem(userName);
                    item.SubItems.Add(cpuUsage);
                    item.SubItems.Add(memoryUsage);
                    listView3.Items.Add(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata: " + ex.Message);
                }
            }
        }
        private string GetProcessOwner(int processId)
        {
            string query = $"Select * From Win32_Process Where ProcessID = {processId}";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));

                if (returnVal == 0)
                {
                    return argList[0];
                }
            }

            return "N/A";
        }
        private void RefreshServiceList()
        {
            listView2.Items.Clear();
            ServiceController[] services = ServiceController.GetServices();

            foreach (ServiceController service in services)
            {
                try
                {
                    string serviceName = service.ServiceName;
                    int processId = GetServiceProcessId(service);
                    string status = service.Status.ToString();

                    ListViewItem item = new ListViewItem(serviceName);
                    item.SubItems.Add(processId.ToString());
                    item.SubItems.Add(status);
                    listView2.Items.Add(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata: " + ex.Message);
                }
            }
        }

        private int GetServiceProcessId(ServiceController service)
        {
            ManagementObject wmiService = new ManagementObject($"Win32_Service.Name='{service.ServiceName}'");
            wmiService.Get();

            return Convert.ToInt32(wmiService["ProcessId"]);
        }

        

        private void upoıpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void çıkışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void herZamanÜstteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
        }

        private void kullanımdaSimgeDurumaKüçültToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void şimdiYenileToolStripMenuItem_Click(object sender, EventArgs e)
        {
          //  this.Refresh();
            RefreshProcessList();
            RefreshServiceList();
            RefreshUserList();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }
        /*
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                proc[listBox2.SelectedIndex].Kill();
                GetAllProcess();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */
        private void button1_Click(object sender, EventArgs e)
        {
            Process[] procs = Process.GetProcessesByName(listView1.SelectedItems.ToString());
            foreach (Process p in procs)
            {
                p.Kill();
            }
          /*  Process process = (Process)listView1.SelectedItems;
            process.Kill();*/
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void yeniGörevÇalıştırToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(runTask rnt=new runTask())
            {
                if (rnt.ShowDialog() == DialogResult.OK)
                    GetAllProcess();
                  
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void yüksekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Timer refreshTimer = new Timer();
            refreshTimer.Interval = 60000; // 60 saniye 
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshProcessList();
            RefreshServiceList();
            RefreshUserList();

        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Timer refreshTimer = new Timer();
            refreshTimer.Interval = 30000; // 30 saniye 
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private void yavaşToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Timer refreshTimer = new Timer();
            refreshTimer.Interval = 15000; // 15 saniye 
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }
    }
}

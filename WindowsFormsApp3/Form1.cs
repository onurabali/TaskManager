using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        private void Form1_Load(object sender, EventArgs e)
        {
            //  GetAllProcess();
            //  listView1.View = View.Details;
            listView1.Columns.Add("İşlem Adı", 300);
            listView1.Columns.Add("CPU Kullanımı % ", 150);
            listView1.Columns.Add("Bellek Kullanımı % ", 150);
            RefreshProcessList();
            //  listView2.View = View.Details;
            listView2.Columns.Add("Ad", 300);
            listView2.Columns.Add("PID", 150);
            listView2.Columns.Add("Durum", 150);
            RefreshServiceList();
            //   listView3.View = View.Details;
            listView3.Columns.Add("Kullanıcı Adı", 300);
            listView3.Columns.Add("CPU Kullanımı %", 150);
            listView3.Columns.Add("Bellek Kullanımı (%)", 150);
            RefreshUserList();

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
        private float GetMemoryUsage(Process process)
        {
            float memoryUsage = 0;

            memoryUsage = process.WorkingSet64;

            long totalMemory = GetTotalMemory();
            return memoryUsage / (float)totalMemory * 100;
        }


        
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
                    item.SubItems.Add(processId.ToString()); // sutun
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
           // this.Close(); sadece bu form için
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
        

        private void button1_Click(object sender, EventArgs e)
        {
            /* Process[] procs = Process.GetProcessesByName(listView1.SelectedItems.ToString());
             foreach (Process p in procs)
             {
                 p.Kill();
             }*/
            /*  Process process = (Process)listView1.SelectedItems;
              process.Kill();*/
            ListViewItem selectedItem = listView1.SelectedItems[0];
            string processName = selectedItem.Text;
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
            {
                process.Kill();
                RefreshProcessList();
            }


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
            refreshTimer.Tick += RefreshTimer_Tick; // tick olduğunda tiick altındaki işlemleri yapar.
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

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void durdurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView2.SelectedItems[0];
                string serviceName = selectedItem.Text;

                try
                {
                    ServiceController service = new ServiceController(serviceName);

                    if (service.Status != ServiceControllerStatus.Stopped)
                    {
                        service.Stop();
                     //   service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10)); 
                        RefreshServiceList(); 
                    }
                    else
                    {
                        MessageBox.Show("Hizmet zaten durdurulmuş.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hizmet durdurulamadı: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void çalıştırToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView2.SelectedItems[0];
                string serviceName = selectedItem.Text;

                try
                {
                    ServiceController service = new ServiceController(serviceName);

                    if (service.Status != ServiceControllerStatus.Running)
                    {
                        service.Start();
                     
                        RefreshServiceList(); 
                    }
                    else
                    {
                        MessageBox.Show("hizmet zaten çalışıyor","uyarı");
                     
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("hizmet başlatılamadı: ", "hata");
                }
            }
        }

        private void dosyaKonumuAçToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* {

                 OpenFileDialog file = new OpenFileDialog();

                 if (file.ShowDialog() == DialogResult.OK)
                 {
                     FileInfo fi = new FileInfo(file.FileName);
                     if (fi.Exists)
                     {
                         System.Diagnostics.Process.Start(file.FileName);
                     }
                     else //www.yazilimkodlama.com
                     {
                         //Hata
                     }
                 }
             }*/
            if(listView1.SelectedItems.Count >0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                string processname = selectedItem.Text;
                Process[] processes = Process.GetProcessesByName(processname);
                if (processes.Length > 0)
                {
                    string filepath = processes[0].MainModule.FileName;

                    if (!string.IsNullOrEmpty(filepath))
                    {
                        Process.Start("explorer.exe", $"/select,\"{filepath}\"");
                    }
                }
                else
                    MessageBox.Show("dosya konumu yok","hata");
            }
            
        }

        private void endTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem selectedItem = listView1.SelectedItems[0];
            string processName = selectedItem.Text;
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
            {
                process.Kill();
                RefreshProcessList();
            }
        }

        private void işlemlerToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}

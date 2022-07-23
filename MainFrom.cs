using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoEnvironment
{
    public partial class MainFrom : Form
    {
        public MainFrom()
        {
            InitializeComponent();
        }

        private void MainFrom_Load(object sender, EventArgs e)
        {
            string machineName = Environment.MachineName;
            string osVersionName = GetOsVersion(Environment.OSVersion.Version);
            string servicePack = Environment.OSVersion.ServicePack;
            osVersionName = osVersionName + " " + servicePack;
            string userName = Environment.UserName;
            string domainName = Environment.UserDomainName;
            string tickCount = (Environment.TickCount / 1000).ToString() + "s";
            string systemPageSize = (Environment.SystemPageSize / 1024).ToString() + "KB";
            string systemDir = Environment.SystemDirectory;
            string stackTrace = Environment.StackTrace;
            string processorCounter = Environment.ProcessorCount.ToString();
            string platform = Environment.OSVersion.Platform.ToString();
            string newLine = Environment.NewLine;
            bool is64Os = Environment.Is64BitOperatingSystem;
            bool is64Process = Environment.Is64BitProcess;
            
            string currDir = Environment.CurrentDirectory;
            string cmdLine = Environment.CommandLine;
            string[] drives = Environment.GetLogicalDrives();
            //long workingSet = (Environment.WorkingSet / 1024);
            this.lblMachineName.Text = machineName;
            this.lblOsVersion.Text = osVersionName;
            this.lblUserName.Text = userName;
            this.lblDomineName.Text = domainName;
            this.lblStartTime.Text = tickCount;
            this.lblPageSize.Text = systemPageSize;
            this.lblSystemDir.Text = systemDir;
            this.lblLogical.Text = string.Join(",", drives);
            this.lblProcesserCounter.Text = processorCounter;
            this.lblPlatform.Text = platform;
            this.lblNewLine.Text = newLine.ToString();
            this.lblSystemType.Text = is64Os ? "64bit" : "32bit";
            this.lblProcessType.Text = is64Process ? "64bit" : "32bit";
            this.lblCurDir.Text = currDir;
            this.lblCmdLine.Text = cmdLine;
            this.lblWorkSet.Text = GetPhisicalMemory().ToString()+"MB";
            //环境变量
            // HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment
            IDictionary dicMachine = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);
            this.rtbVaribles.AppendText(string.Format("{0}： {1}", "机器环境变量", newLine));
            foreach (string str in dicMachine.Keys) {
                string val = dicMachine[str].ToString();
                this.rtbVaribles.AppendText(string.Format("{0}： {1}{2}", str, val, newLine));
            }
            this.rtbVaribles.AppendText(string.Format("{0}{1}", ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", newLine));
            // 环境变量存储在 Windows 操作系统注册表的 HKEY_CURRENT_USER\Environment 项中，或从其中检索。
            IDictionary dicUser = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User);
            this.rtbVaribles.AppendText(string.Format("{0}： {1}", "用户环境变量", newLine));
            foreach (string str in dicUser.Keys)
            {
                string val = dicUser[str].ToString();
                this.rtbVaribles.AppendText(string.Format("{0}： {1}{2}", str, val, newLine));
            }
            this.rtbVaribles.AppendText(string.Format("{0}{1}", ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>", newLine));
            IDictionary dicProcess = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process);
            this.rtbVaribles.AppendText(string.Format("{0}： {1}", "进程环境变量", newLine));
            foreach (string str in dicProcess.Keys)
            {
                string val = dicProcess[str].ToString();
                this.rtbVaribles.AppendText(string.Format("{0}： {1}{2}", str, val, newLine));
            }
            //特殊目录 
            string[] names = Enum.GetNames(typeof(Environment.SpecialFolder));
            foreach (string name in names){

                Environment.SpecialFolder sf;
                if (Enum.TryParse<Environment.SpecialFolder>(name, out sf))
                {
                    string folder = Environment.GetFolderPath(sf);
                    this.rtbFolders.AppendText(string.Format("{0}： {1}{2}", name, folder, newLine));
                }
            }
            //获取其他硬件，软件信息
            GetPhicnalInfo();
        }

        private string GetOsVersion(Version ver) {
            string strClient = "";
            if (ver.Major == 5 && ver.Minor == 1)
            {
                strClient = "Win XP";
            }
            else if (ver.Major == 6 && ver.Minor == 0)
            {
                strClient = "Win Vista";
            }
            else if (ver.Major == 6 && ver.Minor == 1)
            {
                strClient = "Win 7";
            }
            else if (ver.Major == 5 && ver.Minor == 0)
            {
                strClient = "Win 2000";
            }
            else
            {
                strClient = "未知";
            }
            return strClient;
        }

        /// <summary>
        /// 获取系统内存大小
        /// </summary>
        /// <returns>内存大小（单位M）</returns>
        private int GetPhisicalMemory()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher();   //用于查询一些如系统信息的管理对象 
            searcher.Query = new SelectQuery("Win32_PhysicalMemory ", "", new string[] { "Capacity" });//设置查询条件 
            ManagementObjectCollection collection = searcher.Get();   //获取内存容量 
            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();

            long capacity = 0;
            while (em.MoveNext())
            {
                ManagementBaseObject baseObj = em.Current;
                if (baseObj.Properties["Capacity"].Value != null)
                {
                    try
                    {
                        capacity += long.Parse(baseObj.Properties["Capacity"].Value.ToString());
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
            return (int)(capacity / 1024 / 1024);
        }

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/aa394084(VS.85).aspx
        /// </summary>
        /// <returns></returns>
        private int GetPhicnalInfo() {
            ManagementClass osClass = new ManagementClass("Win32_Processor");//后面几种可以试一下，会有意外的收获//Win32_PhysicalMemory/Win32_Keyboard/Win32_ComputerSystem/Win32_OperatingSystem
            foreach (ManagementObject obj in osClass.GetInstances())
            {
                PropertyDataCollection pdc = obj.Properties;
                foreach (PropertyData pd in pdc) {
                    this.rtbOs.AppendText(string.Format("{0}： {1}{2}", pd.Name, pd.Value, "\r\n")); 
                }
            }
            return 0;
        }
    }
}

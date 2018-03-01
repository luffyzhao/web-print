using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Collections;
using System.Timers;

namespace luffy_print
{
    public partial class Start : Form
    {
        string serviceFilePath = $"{Application.StartupPath}\\print-server.exe";
        string serviceName = "PrintService";
        private static object loker = new object();
        public Start()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        private void Start_Load(object sender, EventArgs e)
        {
            
        }
        /// <summary>
        /// 开始服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startServiceClick(object sender, EventArgs e)
        {
            if (this.IsServiceExisted(serviceName))
            {
                if (this.IsServiceStart(serviceName))
                {
                    this.ServiceStop(serviceName);
                }
                else
                {
                    this.ServiceStart(serviceName);
                }
            }
            else
            {
                MessageBox.Show("请先安装服务");
            }
                
        }
        /// <summary>
        /// 安装服务 & 卸载服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void installClick(object sender, EventArgs e)
        {
            if (!this.IsServiceExisted(serviceName))
            {
                this.InstallService(serviceFilePath);
            }
            else
            {
                this.UninstallService(serviceFilePath);
            }
        }
        /// <summary>
        /// notifyIcon点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示    
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点
                this.Activate();
                //任务栏区显示图标
                this.ShowInTaskbar = true;
                //托盘区图标隐藏
                notifyIcon.Visible = false;
            }
        }


        /// <summary>
        /// 判断是否最小化,然后显示托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void F_Main_SizeChanged(object sender, EventArgs e)
        {
            //判断是否选择的是最小化按钮
            if (WindowState == FormWindowState.Minimized)
            {
                //隐藏任务栏区图标
                this.ShowInTaskbar = false;
                //图标显示在托盘区
                notifyIcon.Visible = true;
            }
        }
        /// <summary>
        /// 确认是否退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void F_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否确认退出程序？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // 关闭所有的线程
                this.Dispose();
                Close();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void about(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/luffyzhao");
        }

        /// <summary>
        /// 判断服务是否存在
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private bool IsServiceExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController sc in services)
            {
                if (sc.ServiceName.ToLower() == serviceName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 安装服务
        /// </summary>
        /// <param name="serviceFilePath"></param>
        private void InstallService(string serviceFilePath)
        {
            using (AssemblyInstaller installer = new AssemblyInstaller())
            {
                installer.UseNewContext = true;
                installer.Path = serviceFilePath;
                IDictionary savedState = new Hashtable();
                try
                {
                    installer.Install(savedState);
                    installer.Commit(savedState);
                }
                catch
                {
                    MessageBox.Show("服务已安装，请不要重复安装！");
                }                
            }
        }

        /// <summary>
        /// 卸载服务
        /// </summary>
        /// <param name="serviceFilePath"></param>
        private void UninstallService(string serviceFilePath)
        {
            using (AssemblyInstaller installer = new AssemblyInstaller())
            {
                installer.UseNewContext = true;
                installer.Path = serviceFilePath;
                installer.Uninstall(null);
            }
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceName"></param>
        private void ServiceStart(string serviceName)
        {
            using (ServiceController control = new ServiceController(serviceName))
            {
                if (control.Status == ServiceControllerStatus.Stopped)
                {
                    control.Start();
                }
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="serviceName"></param>
        private void ServiceStop(string serviceName)
        {
            using (ServiceController control = new ServiceController(serviceName))
            {
                if (control.Status == ServiceControllerStatus.Running)
                {
                    control.Stop();
                }
            }
        }

        /// <summary>    
        /// 判断某个Windows服务是否启动    
        /// </summary>    
        /// <returns></returns>    
        private bool IsServiceStart(string serviceName)
        {
            ServiceController psc = new ServiceController(serviceName);
            bool bStartStatus = false;
            try
            {
                if (!psc.Status.Equals(ServiceControllerStatus.Stopped))
                {
                    bStartStatus = true;
                }

                return bStartStatus;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (this.IsServiceExisted(serviceName))
            {
                this.安装ToolStripMenuItem.Text = "卸载服务";
                this.启动ToolStripMenuItem.Enabled = true;
                if (this.IsServiceStart(serviceName))
                {
                    this.启动ToolStripMenuItem.Text = "关闭服务";
                    this.安装ToolStripMenuItem.Enabled = false;
                    this.notifyIcon.Icon = Properties.Resources.startprint;

                }
                else
                {
                    this.启动ToolStripMenuItem.Text = "开启服务";
                    this.安装ToolStripMenuItem.Enabled = true;
                    this.notifyIcon.Icon = Properties.Resources.yesinstall;
                }
            }
            else
            {
                this.安装ToolStripMenuItem.Text = "安装服务";
                this.安装ToolStripMenuItem.Enabled = true;
                this.启动ToolStripMenuItem.Enabled = false;
                this.notifyIcon.Icon = Properties.Resources.noinstall;
            }

            
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamsSimulator
{
    public class TeamsTrayContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private Simulator simulator;

        private Icon runningIcon;
        private Icon stoppedIcon;
        public TeamsTrayContext()
        {
            simulator = new Simulator();

            runningIcon = new Icon("images/greenteamslogo.ico");
            stoppedIcon = new Icon("images/redteamslogo.ico");

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Open", null, Open_Click);
            contextMenu.Items.Add("Exit", null, Exit_Click);

            trayIcon = new NotifyIcon()
            {
                Icon = stoppedIcon,
                Text = "Teams Simulator - Not Activated",
                ContextMenuStrip = contextMenu,
                Visible = true,
            };

            trayIcon.DoubleClick += Open_Click;
            simulator.SimulationStateChanged += OnSimulationStateChanged;
        }
        private void Open_Click(object sender, EventArgs e)
        {
            if (!simulator.Visible)
                simulator.Show();
            simulator.WindowState = FormWindowState.Normal;
            simulator.BringToFront();
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            trayIcon.Visible = false;

            simulator.AllowClose = true;
            simulator.Close();

            Application.Exit();
        }
        private void OnSimulationStateChanged(bool isRunning)
        {
            if (trayIcon == null) return;

            if (isRunning)
            {
                trayIcon.Icon = runningIcon;
                trayIcon.Text = "Teams Simulator - Running";
                trayIcon.ShowBalloonTip(1000, "Teams Simulator", "Simulation Started", ToolTipIcon.Info);
            }
            else
            {
                trayIcon.Icon = stoppedIcon;
                trayIcon.Text = "Teams Simulator - Stopped";
                trayIcon.ShowBalloonTip(1000, "Teams Simulator", "Simulation Stopped", ToolTipIcon.Info);
            }
        }

    }
}

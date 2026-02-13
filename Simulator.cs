using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamsSimulator
{
    public partial class Simulator : Form
    {
        public bool AllowClose { get; set; } = false;
        public event Action<bool> SimulationStateChanged;
        
       
        private CancellationTokenSource cancellationTokenSource;
        private NotifyIcon trayIcon;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(
        byte bVk,
        byte bScan,
        uint dwFlags,
        UIntPtr dwExtraInfo);
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public int type;
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        const int INPUT_MOUSE = 0;
        const uint MOUSEEVENTF_MOVE = 0x0001;

        private const byte VK_SHIFT = 0x10;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        public Simulator()
        {
            InitializeComponent();
           

            trayIcon = new NotifyIcon();
            trayIcon.Visible = true;
            trayIcon.Text = "Teams Simulator - Not Activated";

            trayIcon.DoubleClick += (s, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            };
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!AllowClose && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }

            base.OnFormClosing(e);
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (cancellationTokenSource != null) return;

            btnStart.Enabled = false;
            btnStop.Enabled = true;

            this.Hide();
           

            SimulationStateChanged?.Invoke(true);

            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            try
            {
                await Task.Run(() => RunSimulationAsync(token), token);
            }
            catch (TaskCanceledException)
            {
                // Expected when stopping
            }
            finally
            {
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;

                btnStart.Enabled = true;
                btnStop.Enabled = false;

                SimulationStateChanged?.Invoke(false);
               
            }
        }
        private async Task RunSimulationAsync(CancellationToken token)
        {
            var random = new Random();

            while (!token.IsCancellationRequested)
            {
                var pos = Cursor.Position;

                for (int i = 0; i < 5; i++)
                {
                    if (token.IsCancellationRequested)
                        return;
                    MoveMouse(1, 0);
                    MoveMouse(-1, 0);

                    await Task.Delay(1000, token);

                    SimulateShiftKeyPress();
                }

                int delaySeconds = random.Next(50, 91);
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), token);

                if (token.IsCancellationRequested)
                    break;

                SimulateShiftKeyPress();
                MoveMouse(1, 0);
                MoveMouse(-1, 0);
            }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
            this.Hide();
        }

        private void MoveMouse(int dx, int dy)
        {
            INPUT[] input = new INPUT[1];

            input[0].type = INPUT_MOUSE;
            input[0].mi.dx = dx;
            input[0].mi.dy = dy;
            input[0].mi.dwFlags = MOUSEEVENTF_MOVE;

            SendInput(1, input, Marshal.SizeOf(typeof(INPUT)));
        }
        private void SimulateShiftKeyPress()
        {
            // Key down
            keybd_event(VK_SHIFT, 0, 0, UIntPtr.Zero);

            // Small delay to simulate natural press
            Thread.Sleep(50);

            // Key up
            keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
        private void TeamsSimulator_Load(object sender, EventArgs e)
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }
    }
}

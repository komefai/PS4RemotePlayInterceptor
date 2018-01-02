using PS4RemotePlayInterceptor;
using System;
using System.Windows.Forms;

namespace PS4RemotePlayInterceptorDemo
{
    public partial class Main : Form
    {
        private bool IsInjected { get; set; }
        private int PID { get; set; }

        public Main()
        {
            InitializeComponent();

            // Setup callback to interceptor
            Interceptor.InjectionMode = InjectionMode.Compatibility;
            Interceptor.Callback = new InterceptionDelegate(OnReceiveData);
        }

        private static void OnReceiveData(ref DualShockState state)
        {
            /* -- Modify the controller state here -- */

            // Force press X
            state.Cross = true;

            // Force left analog upwards
            state.LY = 0;

            // Force left analog downwards
            // state.LY = 255;

            // Force left analog to center
            // state.LX = 128;
            // state.LY = 128;
        }

        private void UpdateUI()
        {
            injectButton.Text = IsInjected ? "Stop" : "Inject";
            pidLabel.Text = string.Format("Target PID: {0}", (PID < 0 ? "-" : PID.ToString()));
        }

        private void injectButton_Click(object sender, EventArgs e)
        {
            if (!IsInjected)
            {
                // Try to inject into PS4 Remote Play
                try
                {
                    PID = Interceptor.Inject();
                    IsInjected = true;
                }
                catch(InterceptorException ex)
                {
                    MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (ex.InnerException != null)
                    {
                        MessageBox.Show(ex.InnerException.Message + "\n\n" + ex.InnerException.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                // Remove injection from PS4 Remote Play
                Interceptor.StopInjection();

                PID = -1;
                IsInjected = false;
            }

            UpdateUI();
        }

        private void sendStartSignalButton_Click(object sender, EventArgs e)
        {
            Interceptor.SendStartSignal();
        }
    }
}

using PS4RemotePlayInterceptor;
using System;
using System.Windows.Forms;

namespace PS4RemotePlayInterceptorDemo
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private static void OnReceiveData(ref DualshockState state)
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

        private void injectButton_Click(object sender, EventArgs e)
        {
            // Inject into PS4 Remote Play
            Interceptor.Callback = new InterceptionDelegate(OnReceiveData);
            Interceptor.Inject();
        }
    }
}

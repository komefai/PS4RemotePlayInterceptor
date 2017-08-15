using PS4RemotePlayInterceptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PS4RemotePlayInterceptorConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Inject into PS4 Remote Play
            Interceptor.Callback = new InterceptionDelegate(OnReceiveData);
            Interceptor.Inject();

            Console.WriteLine("-- Press any key to exit");
            Console.ReadKey();
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
    }
}

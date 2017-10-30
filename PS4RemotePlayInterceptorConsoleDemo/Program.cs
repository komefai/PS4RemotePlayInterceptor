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
            // Setup callback to interceptor
            Interceptor.Callback = new InterceptionDelegate(OnReceiveData);
            // Emulate controller (BETA)
            Interceptor.EmulateController = true;

            // Start watchdog to automatically inject when possible
            Interceptor.Watchdog.Start();
            // Notify watchdog events
            Interceptor.Watchdog.OnInjectionSuccess = () => Console.WriteLine("Watchdog OnInjectionSuccess");
            Interceptor.Watchdog.OnInjectionFailure = () => Console.WriteLine("Watchdog OnInjectionFailure");

            // Or inject manually and handle exceptions yourself
            //Interceptor.Inject();

            Console.WriteLine("-- Press any key to exit");
            Console.ReadKey();
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
    }
}

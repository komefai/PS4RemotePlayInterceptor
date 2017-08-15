using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PS4RemotePlayInterceptor
{
    /// <summary>
    /// Provides an interface for communicating from the client (target) to the server (injector)
    /// </summary>
    class InjectionInterface : MarshalByRefObject
    {
        /// <summary>
        /// Called when the hook has been injected successsfully
        /// </summary>
        public void OnInjectionSuccess(int clientPID)
        {
            Console.WriteLine("OnInjectionSuccess {0}", clientPID);
        }

        /// <summary>
        /// Called to confirm that the IPC channel is still open / host application has not closed
        /// </summary>
        public void Ping()
        {
            //Console.WriteLine("Ping");
        }

        /// <summary>
        /// Report exception
        /// </summary>
        /// <param name="message"></param>
        public void ReportLog(string message)
        {
            Console.WriteLine("ReportLog {0}", message);
        }

        /// <summary>
        /// Report exception
        /// </summary>
        /// <param name="e"></param>
        public void ReportException(Exception e)
        {
            Console.WriteLine("ReportException {0}", e.Message);
        }


        /* Interface for hooks */

        public void OnCreateFile(string filename, string mode)
        {
            //Console.WriteLine("OnCreateFile {0} | {1}", filename, mode);
        }

        public void OnReadFile(string filename, ref byte[] inputReport)
        {
            //Console.WriteLine("OnReadFile {0}", filename);

            // Expect inputReport to be modified
            if (Interceptor.Callback != null)
            {
                // Parse the state
                var state = DualshockState.ParseFromDualshockRaw(inputReport);

                // Skip if state is invalid
                if (state == null)
                    return;

                // Expect it to be modified
                Interceptor.Callback(ref state);

                // Convert it back
                state.ConvertToDualshockRaw(ref inputReport);
            }
        }

        public void OnWriteFile(string filename, ref byte[] outputReport)
        {
            //Console.WriteLine("OnWriteFile {0}", filename);
        }
    }
}

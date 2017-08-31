using EasyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;

namespace PS4RemotePlayInterceptor
{
    public delegate void InterceptionDelegate(ref DualShockState state);

    public class Interceptor
    {
        // Constants
        private const string TARGET_PROCESS_NAME = "RemotePlay";
        private const string INJECT_DLL_NAME = "PS4RemotePlayInterceptor.dll";

        // EasyHook
        private static string _channelName = null;
        private static IpcServerChannel _ipcServer;
        private static bool _noGAC = false;

        public static InterceptionDelegate Callback { get; set; }

        public static void Inject()
        {
            // Setup remote hooking
            _ipcServer = RemoteHooking.IpcCreateServer<InjectionInterface>(ref _channelName, WellKnownObjectMode.Singleton);

            // Find by process name
            var processes = Process.GetProcessesByName(TARGET_PROCESS_NAME);
            foreach (var process in processes)
            {
                // Full path to our dll file
                string injectionLibrary = Path.Combine(Path.GetDirectoryName(typeof(InjectionInterface).Assembly.Location), INJECT_DLL_NAME);

                try
                {
                    // Inject dll into the process
                    RemoteHooking.Inject(
                        process.Id, // ID of process to inject into
                        (_noGAC ? InjectionOptions.DoNotRequireStrongName : InjectionOptions.Default), // if not using GAC allow assembly without strong name
                        injectionLibrary, // 32-bit version (the same because AnyCPU)
                        injectionLibrary, // 64-bit version (the same because AnyCPU)
                        _channelName
                    );

                    // Success
                    return;
                }
                catch (Exception ex)
                {
                    throw new InterceptorException(string.Format("Failed to inject to target: {0}", ex.Message), ex);
                }
            }

            throw new InterceptorException(string.Format("{0} not found in list of processes", TARGET_PROCESS_NAME));
        }
    }
}

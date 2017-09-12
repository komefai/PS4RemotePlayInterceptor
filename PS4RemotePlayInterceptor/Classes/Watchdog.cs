// PS4RemotePlayInterceptor (File: Classes/Watchdog.cs)
//
// Copyright (c) 2017 Komefai
//
// Visit http://komefai.com for more information
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace PS4RemotePlayInterceptor
{
    public class Watchdog
    {
        private DateTime m_LastPingTime;

        private Timer m_Timer = null;
        private Timer Timer
        {
            get
            {
                // Lazy instantiate timer
                if (m_Timer == null)
                {
                    m_Timer = new Timer(1000);
                    m_Timer.Elapsed += (sender, e) =>
                    {
                        if (m_LastPingTime == Interceptor.LastPingTime)
                        {
                            Interceptor.StopInjection();
                            Interceptor.Inject();
                        }

                        m_LastPingTime = Interceptor.LastPingTime;
                    };

                }
                return m_Timer;
            }
        }


        public void Start(int interval = 1000)
        {
            if (Interceptor.InjectionMode == InjectionMode.Compatibility)
                throw new InterceptorException("Watchdog is not supported in Compatibility mode");

            Timer.Interval = interval;
            Timer.Enabled = true;
        }

        public void Stop()
        {
            if (Interceptor.InjectionMode == InjectionMode.Compatibility)
                throw new InterceptorException("Watchdog is not supported in Compatibility mode");

            Timer.Enabled = false;
        }
    }
}

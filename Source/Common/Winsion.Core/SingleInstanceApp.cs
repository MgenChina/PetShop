using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Winsion.Core
{
    public class SingleInstanceApp
    {
        /// <summary>
        /// 根据 productName 判断唯一进程
        /// </summary>
        /// <param name="productName">必须和 exe 执行程序集中AssemblyInfo.cs文件的 [assembly: AssemblyProduct(#YourProductName#)] #YourProductName# 一致</param>
        public SingleInstanceApp(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new ArgumentOutOfRangeException("productName");
            }
            this.productName = productName;
            this.singleLockName = ProcessStarter.GetProcessSynLockName(productName);
        }

        public bool Check()
        {
            var currentProcess = Process.GetCurrentProcess();
            var list = Process.GetProcessesByName(currentProcess.ProcessName);
            var me = list.FirstOrDefault(p => p.MainModule != null
                && p.MainModule.FileVersionInfo != null
                && p.MainModule.FileVersionInfo.ProductName == productName
                && p.MainWindowHandle != IntPtr.Zero
                );
            if (me != null)
            {
                Trace.TraceInformation("SingleInstanceApp Start 进程检查，已经启动了同样的进程，currPath={0}", me.MainModule.FileName);
                OpenIcon(me.MainWindowHandle);
                SetForegroundWindow(me.MainWindowHandle);
                Environment.Exit(0);
                return false;
            }

            bool isCreatedNew;
            singleInstanceMutex = new Mutex(true, singleLockName, out isCreatedNew);
            if (!isCreatedNew)
            {
                Trace.TraceInformation("SingleInstanceApp Start 互斥锁检查 已经启动了同样的进程，互斥锁={0}", singleLockName);
                singleInstanceMutex.Dispose();
                singleInstanceMutex = null;
                Environment.Exit(0);
                return false;
            }

            return true;
        }

        [DllImport("user32")]
        private static extern int OpenIcon(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private Mutex singleInstanceMutex = null;
        private string productName = null;
        private readonly string singleLockName = null;      
    }
}

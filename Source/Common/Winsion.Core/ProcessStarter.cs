using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Events;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;


namespace Winsion.Core
{
    public class ProcessStarter : IProcessStarter
    {
        public ProcessStarter(string exePath)
            : this(exePath, null)
        {
        }

        public ProcessStarter(string exePath, string[] args)
            : this(true, exePath, args)
        {
        }

        public ProcessStarter(bool isBindMainProcess, string exePath, string[] args)
        {
            this.isBindMainProcess = isBindMainProcess;
            this.exePath = exePath;
            this.processSynLockName = GetProcessSynLockName(exePath);
            this.arguments = args;
        }

        public bool Run()
        {
            try
            {
                var isOk = Run(true);
                if (isOk)
                {
                    CheckMyProcess(ref myProcess);
                    OnServiceProcessKeeper();
                }
                return isOk;
            }
            catch (Exception ex)
            {
                log.Fatal("Run 方法启动服务进程出错", ex);
                throw;
            }
        }

        public void BeginRun()
        {
            var thd = new Thread(() =>
                  {
                      try
                      {
                          Run(false);
                      }
                      catch (Exception ex)
                      {
                          log.Fatal("BeginRun 方法启动服务进程出错", ex);
                      }
                  });
            thd.Name = "ProcessStarter.BeginRun";
            thd.IsBackground = true;
            thd.Priority = ThreadPriority.AboveNormal;
            thd.Start();
        }

        /// <summary>
        /// 要注意有可能BeginRun() 还没有执行完，就调用 EndRun()，但在实际使用中此处不会存在这种情况，所以不处理该逻辑。
        /// </summary>       
        public bool EndRun()
        {
            try
            {
                bool isOk = false;
                bool hasService = ExistProcess();
                if (hasService)
                {
                    isOk = true;
                }
                else
                {
                    //无论是BeginRun()还没启动完造成获取进程锁失败，还是根本没有启动过进程，此处在启动一次，后台进程有唯一性判断。
                    var proc = StartProcess();
                    //等待后台进程启动完成
                    if (CheckProcessCompleted(proc))
                    {
                        myProcess = proc;
                    }
                    if (isBindMainProcess && myProcess != null)
                    {
                        myProcess.Exited += new EventHandler(ServiceProcess_Exited);
                    }
                    isOk = myProcess != null;
                }
                if (isOk)
                {
                    CheckMyProcess(ref myProcess);
                    OnServiceProcessKeeper();
                }
                return isOk;
            }
            catch (Exception ex)
            {
                Trace.TraceError("ProcessStarter.EndRun Message={0},StackTrace={1},InnerException={2}", ex.Message, ex.StackTrace, ex.InnerException);
                log.Fatal("EndRun", ex);
                return false;
            }
        }

        public void Exit()
        {
            isEixt = true;
            try
            {
                IList<int> list;
                var proc = Process.GetCurrentProcess();
                IServiceWatcher service;
                using (var cf = ServiceWatcherHelper.ChannelFactory)
                using ((service = cf.CreateChannel()) as IDisposable)
                {
                    list = service.GetAllAttachedProcessId();
                    service.DetachProcess(proc.Id);
                }

                if (list != null && list.Any(p => p != proc.Id) == false)
                {
                    if (myProcess != null && myProcess.HasExited == false)
                    {
                        myProcess.Exited -= new EventHandler(ServiceProcess_Exited);
                        myProcess.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Fatal("GetExistServiceProcess", ex);
            }
        }

        public Process Process
        {
            get
            {
                return myProcess;
            }
        }

        public static string GetProcessSynLockName(string processName)
        {
            return processName + "_" + processSynLockNameSalt.Substring(0, 13);
        }

        private bool Run(bool isWait)
        {
            myProcess = GetExistServiceProcess();
            if (myProcess == null)
            {
                var proc = StartProcess();
                if (isWait)
                {
                    //等待直到启动进程成功
                    if (CheckProcessCompleted(proc))
                    {
                        myProcess = proc;
                    }
                }
                else
                {
                    myProcess = proc;
                }
            }
            if (isBindMainProcess && myProcess != null)
            {
                myProcess.Exited += new EventHandler(ServiceProcess_Exited);
            }
            return myProcess != null;
        }

        private Process GetExistServiceProcess()
        {
            Process proc = null;
            bool hasService = ExistProcess();
            if (hasService)
            {
                proc = GetServiceProcess();
            }
            return proc;
        }

        private static Process GetServiceProcess()
        {
            Process proc = null;
            IServiceWatcher service = null;
            try
            {
                using (var cf = ServiceWatcherHelper.ChannelFactory)
                using ((service = cf.CreateChannel()) as IDisposable)
                {
                    var id = service.GetServiceProcessId();
                    proc = Process.GetProcessById(id);
                    service.AttachedProcess(Process.GetCurrentProcess().Id);
                }
            }
            catch (Exception ex)
            {
                log.Fatal("GetServiceProcess", ex);
                proc = null;
            }
            return proc;
        }

        private Process StartProcess()
        {
            var process = new Process();
            process.EnableRaisingEvents = true;
            var temp = ConvertArgs();
            process.StartInfo.Arguments = temp;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = exePath;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();

            Trace.TraceInformation("ProcessStarter.StartProcess 启动参数{0}", temp);
            return process;
        }

        private string ConvertArgs()
        {
            var list = new List<string>();
            list.Add(Process.GetCurrentProcess().Id.ToString());
            list.Add(processSynLockName);
            if (this.arguments != null && this.arguments.Length > 0)
            {
                list.AddRange(this.arguments);
            }

            string temp = list.Aggregate((r, p) => { return r + " " + p; });
            return temp;
        }

        private bool CheckProcessCompleted(Process proc)
        {
            var mutex = WaitingStart();
            if (mutex == null)
            {
                log.Fatal("StartProcess 方法启动后台服务失败");
                if (proc.HasExited == false)
                {
                    proc.Kill();
                }
                return false;
            }
            else
            {
                try
                {
                    mutex.WaitOne(processWaitMills);
                }
                finally
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }
                return true;
            }
        }

        private Mutex WaitingStart()
        {
            Mutex mutex = null;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int count = 0;
            int sleepTimeout = 20;
            while (count++ < 100 && mutex == null)
            {
                try
                {
                    Thread.Sleep(sleepTimeout);
                    mutex = Mutex.OpenExisting(processSynLockName);
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                    if (sleepTimeout < 100)
                    {
                        sleepTimeout += 2;
                    }
                    mutex = null;
                    continue;
                }
                break;
            }
            sw.Stop();
            Trace.TraceInformation("Mutex.OpenExisting count={0},sleepTimeout={1},等待时间={2}", count, sleepTimeout, sw.ElapsedMilliseconds);
            return mutex;
        }

        /// <summary>
        /// 如果已经启动进程，就等待服务host完成。
        /// </summary>       
        private bool ExistProcess()
        {
            try
            {
                var mutex = Mutex.OpenExisting(processSynLockName);
                try
                {
                    mutex.WaitOne(processWaitMills);
                }
                finally
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }
                return true;
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                return false;
            }
        }

        private static void ServiceProcess_Exited(object sender, EventArgs e)
        {
            Trace.TraceError("ProcessStarter.ServiceProcess_Exited ,currentProcess={0}", Process.GetCurrentProcess().ProcessName);
            //因为有自动启动后台进程的机制，所以后台进程退出时，不用退出主进程
            //var currentProcess = Process.GetCurrentProcess();
            //if (currentProcess != null && currentProcess.HasExited == false)
            //{
            //    currentProcess.Kill();
            //}
        }

        /// <summary>
        /// 防止后台进程是别的进程启动的
        /// </summary>
        private bool CheckMyProcess(ref Process proc)
        {
            IServiceWatcher service = null;
            try
            {
                using (var cf = ServiceWatcherHelper.ChannelFactory)
                using ((service = cf.CreateChannel()) as IDisposable)
                {
                    var id = service.GetServiceProcessId();
                    if (proc == null || proc.Id != id)
                    {
                        if (proc != null)
                        {
                            proc.Exited -= new EventHandler(ServiceProcess_Exited);
                        }
                        proc = Process.GetProcessById(id);
                        if (isBindMainProcess && proc != null)
                        {
                            proc.Exited += new EventHandler(ServiceProcess_Exited);
                        }
                        service.AttachedProcess(Process.GetCurrentProcess().Id);
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("CheckMyProcess 检查后台进程", ex);
                return true;
            }
        }

        private void OnServiceProcessKeeper()
        {
            if (++keeperThreadCount > 20)
            {
                return;
            }
            if (myProcess != null)
            {
                var keeperThread = new Thread(ProcessKeeper);
                keeperThread.IsBackground = true;
                keeperThread.Name = "ProcessStarter.keeperThread";
                keeperThread.Start();
            }
            else
            {
                Trace.TraceError("ProcessStarter.OnServiceProcessKeeper 第{0}次启动后台进程，myProcess is null。", keeperThreadCount);
                log.ErrorFormat("OnServiceProcessKeeper 第{0}次启动后台进程，myProcess is null。", keeperThreadCount);
            }
        }

        private void ProcessKeeper(object state)
        {
            if (myProcess == null)
            {
                return;
            }
            var name = "";
            try
            {
                myProcess.WaitForExit();
                if (isEixt)
                {
                    return;
                }
                name = Process.GetCurrentProcess().ProcessName;
                Trace.TraceError("ProcessStarter.OnServiceProcessKeeper 后台进程退出，开始自动启动后台进程，这是第{0}次启动，currentProcess={1}。", keeperThreadCount, name);
                log.WarnFormat("OnServiceProcessKeeper 后台进程退出，开始自动启动后台进程，这是第{0}次启动，currentProcess={1}。", keeperThreadCount, name);

                //等待后台进程释放资源
                Thread.Sleep(500);
                bool isNew;
                //避免多个守护进程启动后台进程
                startServiceProcessMutex = new Mutex(true, startServiceProcessLockName, out isNew);
                Process proc = null;
                if (isNew)
                {
                    proc = StartProcess();
                    //有可能有多个守护进程启动后台进程，等待唯一一个后台进程启动成功。
                    Thread.Sleep(1000);
                    WaitNewProcess(proc);
                    //释放本次申请的进程锁，为下次使用准备。
                    startServiceProcessMutex.Dispose();
                    startServiceProcessMutex = null;
                }
                else
                {
                    startServiceProcessMutex.Dispose();
                    startServiceProcessMutex = null;
                    //有可能有多个守护进程启动后台进程，等待唯一一个后台进程启动成功。
                    Thread.Sleep(1200);
                    WaitNewProcess(proc);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("ProcessStarter.OnServiceProcessKeeper ex={0}, 第{1}次启动后台进程，currentProcess={2}。", ex.Message, keeperThreadCount, name);
                log.ErrorFormat("OnServiceProcessKeeper 第{0}次启动后台进程，currentProcess={1}。", ex, keeperThreadCount, name);
            }
        }

        private void WaitNewProcess(Process proc)
        {
            var mutex = WaitingStart();
            if (mutex != null)
            {
                myProcess = proc;
                if (isBindMainProcess && myProcess != null)
                {
                    myProcess.Exited += new EventHandler(ServiceProcess_Exited);
                }
                try
                {
                    mutex.WaitOne();
                }
                finally
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                    mutex = null;
                }
                CheckMyProcess(ref myProcess);
                OnServiceProcessKeeper();
            }
        }

        private Process myProcess = null;
        private string exePath = "";
        private string[] arguments = null;
        private bool isBindMainProcess = true;
        private readonly static ILogger<ProcessStarter> log = new Logger<ProcessStarter>();
        private const int processWaitMills = 1000 * 60 * 5;
        private readonly string processSynLockName = null;
        private const string processSynLockNameSalt = "78F23491-B3B3-472E-B3C7-D9F2644B39C4";
        private int keeperThreadCount = 0;
        private bool isEixt = false;
        private const string startServiceProcessLockName = "B3DF7948-9E8F-47DC-93A5-1E2416B613EE";
        private Mutex startServiceProcessMutex = null;
    }

    public abstract class ServiceWatcherHelper
    {
        public static ServiceHost HostWatcher(Type serviceType)
        {
            var watcherHost = new ServiceHost(serviceType, baseUri);
            watcherHost.AddServiceEndpoint(typeof(IServiceWatcher), GetBinding(), subUri);
            watcherHost.Open();
            return watcherHost;
        }

        public static bool IsOnline(ServiceHostType serviceHostType)
        {
            IServiceWatcher service = null;
            try
            {
                bool isOk = false;
                using (var cf = ChannelFactory)
                using ((service = cf.CreateChannel()) as IDisposable)
                {
                    isOk = service.CheckServiceOnline(serviceHostType);
                }
                return isOk;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ProcessStarter.IsOnline {0}", ex);
                log.Error("IsOnlone", ex);
                return false;
            }
        }

        public static void ServiceOnlineHello(ServiceHostType serviceHostType)
        {
            IServiceWatcher service = null;
            try
            {
                using (var cf = ChannelFactory)
                using ((service = cf.CreateChannel()) as IDisposable)
                {
                    service.ServiceOnlineHello(serviceHostType);
                }
            }
            catch (Exception ex)
            {
                log.Error("ServiceOnlineHello", ex);
            }
        }

        public static void ParentProcessLogout(int process)
        {
            IServiceWatcher service = null;
            try
            {
                using (var cf = ChannelFactory)
                using ((service = cf.CreateChannel()) as IDisposable)
                {
                    service.ParentProcessLogout(process);
                }
            }
            catch (Exception ex)
            {
                log.Error("ParentProcessLogout", ex);
            }
        }

        internal static ChannelFactory<IServiceWatcher> ChannelFactory
        {
            get
            {
                ChannelFactory<IServiceWatcher> cf = new ChannelFactory<IServiceWatcher>(GetBinding(),
                    new EndpointAddress(new Uri(baseUri, subUri)));
                return cf;
            }
        }

        private static Binding GetBinding()
        {
            return new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
        }

        private readonly static ILogger<ServiceWatcherHelper> log = new Logger<ServiceWatcherHelper>();
        private static readonly Uri baseUri = new Uri("net.pipe://" + addrSalt);
        private static readonly Uri subUri = new Uri("ServiceWatcher.svc", UriKind.Relative);
        private const string addrSalt = "57125279-6C6B-4161-B749-0993D733EC6C";
    }

    public interface IProcessStarter
    {
        bool Run();

        void BeginRun();

        bool EndRun();

        void Exit();

        Process Process { get; }
    }

    [ServiceContract]
    public interface IServiceWatcher
    {
        [OperationContract]
        void StartService(ServiceHostType serviceHostType);

        [OperationContract]
        bool CheckServiceOnline(ServiceHostType serviceHostType);

        [OperationContract]
        void ServiceOnlineHello(ServiceHostType serviceHostType);

        [OperationContract]
        void AttachedProcess(int processId);

        [OperationContract]
        void DetachProcess(int processId);

        [OperationContract]
        int GetServiceProcessId();

        [OperationContract]
        IList<int> GetAllAttachedProcessId();

        [OperationContract]
        void ParentProcessLogout(int processId);

    }

    public enum ServiceHostType
    {
        Business = 0,
        Streamer = 1,
        Adapter = 2,
        TRS = 3,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.Sockets;

namespace Winsion.Core
{
    public partial class Helper
    {
        public static byte[] StructToBytes(object obj)
        {
            int size = Marshal.SizeOf(obj);
            byte[] bytes = new byte[size];
            IntPtr structPtr = IntPtr.Zero;
            try
            {
                structPtr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(obj, structPtr, false);
                Marshal.Copy(structPtr, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(structPtr);
            }
        }

        public static object BytesToStruct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
            {
                return null;
            }
            IntPtr structPtr = IntPtr.Zero;
            try
            {
                structPtr = Marshal.AllocHGlobal(size);
                Marshal.Copy(bytes, 0, structPtr, size);
                var obj = Marshal.PtrToStructure(structPtr, type);
                return obj;
            }
            finally
            {
                Marshal.FreeHGlobal(structPtr);
            }
        }

        
        public class RemoteHostParam
        {
            public string hostAddr; // 需要探测的对端IP地址
            public int port;        // 端口号
            public bool IsConnectionSuccessful; // 用于返回，true表示连接成功
        };

        private class DetectConnectionParam
        {
            public TcpClient client;               // 临时的socket客户端对象
            public bool IsConnectionSuccessful;    // true表示已连接成功
            public ManualResetEvent timeoutEvent; // 用于超时的信号量
        }

        /// <summary>
        /// 探测指定的主机某端口是否可连接
        /// </summary>
        /// <param name="hostAddr">主机IP</param>
        /// <param name="port">端口</param>
        /// <param name="millisecond">最多等待的毫秒值, 小于等于0会强制为1000毫秒</param>
        /// <returns>true 表示可连接</returns>
        static public bool PingIsOnline(string hostAddr, int port, int millisecond)
        {
            DetectConnectionParam param = new DetectConnectionParam();
            param.IsConnectionSuccessful = false;
            param.timeoutEvent = new ManualResetEvent(false);
            param.client = new TcpClient();

            try
            {
                param.client.BeginConnect(hostAddr, port, new AsyncCallback(ConnectCallBackMethod), param);

                if (millisecond < 0)
                    millisecond = 1000;

                // 当发起异步连接后，就阻塞在此处，
                // 要么超时返回，要么被唤醒。
                param.timeoutEvent.WaitOne(millisecond);
            }
            catch (System.Exception)
            {
            }
            finally
            {
                param.client.Close();
            }

            return param.IsConnectionSuccessful;
        }

        private static void ConnectCallBackMethod(IAsyncResult asyncresult)
        {
            try
            {
                DetectConnectionParam param = asyncresult.AsyncState as DetectConnectionParam;
                if (param.client.Client != null)
                {
                    param.client.EndConnect(asyncresult);
                    param.IsConnectionSuccessful = true; // 置标志为连接成功
                    param.timeoutEvent.Set();            // 唤醒在等待该事件的信号
                }
            }
            catch (Exception)
            {
                // 不做任何事
            }

        }


        private class DetectMultiConnectionParam
        {
            public TcpClient client;               // 临时的socket客户端对象
            public RemoteHostParam state;          // 设备状态信息      
            static public ManualResetEvent timeoutEvent = new ManualResetEvent(false); // 用于超时的信号量
        }

        /// <summary>
        /// 探测指定的一批主机某端口是否可连接
        /// </summary>
        /// <param name="hosts">主机地址列表，内部的IsConnectionSuccessful用于反馈是否连接成功</param>
        /// <param name="millisecond">最多等待的毫秒值, 小于等于0会强制为1000毫秒</param>
        static public void PingIsOnline(ref List<RemoteHostParam> hosts, int millisecond)
        {
            if (millisecond < 0)
                millisecond = 1000;

            DetectMultiConnectionParam[] multiParam = new DetectMultiConnectionParam[hosts.Count];
            int i = 0;

            // 遍历地址列表，依次进行异步的connect
            foreach (var host in hosts)
            {
                multiParam[i] = new DetectMultiConnectionParam();
                multiParam[i].client = new TcpClient();
                multiParam[i].state = host;
                multiParam[i].state.IsConnectionSuccessful = false;

                try
                {
                    if (multiParam[i].state.hostAddr != null && multiParam[i].state.port > 0)
                        multiParam[i].client.BeginConnect(multiParam[i].state.hostAddr,
                                                          multiParam[i].state.port, 
                                                          new AsyncCallback(ConnectCallBackMethod2), 
                                                          multiParam[i]);
                }
                catch (System.Exception)
                {
                }
                finally
                {
                    i++;
                }
            }

            // 计算出超时的绝对时间，这里不考虑更改系统时间的情况。
            DateTime timeoutAbsTime = DateTime.Now.AddMilliseconds(millisecond);

            while (true)
            {
                // 等待信号，然后检查队列中的所有连接标记，
                // 只要仍有false的，则再次等待信号，直到超时。
                if (!DetectMultiConnectionParam.timeoutEvent.WaitOne(millisecond))
                    break;

                int notConnectCount = hosts.Where(p => p.IsConnectionSuccessful == false).Count();
                if (notConnectCount == 0)
                    break;

                if (timeoutAbsTime <= DateTime.Now)
                    break;
            }

            // 关闭临时开启的所有socket
            for (int j = 0; j < hosts.Count; j++)
            {
                if (multiParam[j].client != null)
                    multiParam[j].client.Close();
            }

            return;
        }


        private static void ConnectCallBackMethod2(IAsyncResult asyncresult)
        {
            try
            {
                DetectMultiConnectionParam param = asyncresult.AsyncState as DetectMultiConnectionParam;
                if (param.client.Client != null)
                {
                    param.client.EndConnect(asyncresult);
                    param.state.IsConnectionSuccessful = true;          // 置标志为连接成功
                    DetectMultiConnectionParam.timeoutEvent.Set();      // 唤醒在等待该事件的信号
                }
            }
            catch (Exception)
            {
                // 不做任何事
            }

            return;
        }




    }
}

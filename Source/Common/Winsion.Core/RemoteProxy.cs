using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Winsion.Core
{
    public class RemoteProxy : MarshalByRefObject
    {
        public static AppDomain Start(string assemblyFile, Action<Assembly> a)
        {
            AppDomainSetup info = new AppDomainSetup();
            info.ShadowCopyFiles = "false";
            info.ConfigurationFile = assemblyFile + ".config";
            var domain = Start(info, assemblyFile, a);
            return domain;
        }

        public static AppDomain Start(string assemblyFile, string configurationFile, Action<Assembly> a)
        {
            AppDomainSetup info = new AppDomainSetup();
            info.ShadowCopyFiles = "false";
            info.ConfigurationFile = string.IsNullOrEmpty(configurationFile) ? assemblyFile + ".config" : configurationFile;
            var domain = Start(info, assemblyFile, a);
            return domain;
        }

        public static AppDomain Start(AppDomainSetup info, string assemblyFile, Action<Assembly> a)
        {
            AppDomain appDomain = null;
            try
            {
                appDomain = AppDomain.CreateDomain(Path.GetFileName(assemblyFile), null, info);
                appDomain.SetData("APPBASE", Path.GetDirectoryName(assemblyFile));

                Type proxyType = typeof(RemoteProxy);
                // proxy runs *inside* new app domain;
                // there it can handle service hosting
                RemoteProxy proxy = (RemoteProxy)appDomain
                    .CreateInstanceAndUnwrap(
                        proxyType.Assembly.FullName,
                        proxyType.FullName);

                if (!proxy.Load(assemblyFile, a))
                {
                    AppDomain.Unload(appDomain);
                    appDomain = null;
                }

                return appDomain;
            }
            catch (Exception ex)
            {
                if (appDomain != null)
                {
                    AppDomain.Unload(appDomain);
                }
                throw ex;
            }
        }        

        public bool Load(string assemblyFile, Action<Assembly> a)
        {
            try
            {
                TraceDomain();
                AppDomain.CurrentDomain.AssemblyResolve += (s, args) =>
                {
                    AppDomain appDomain = AppDomain.CurrentDomain;
                    var asmList = appDomain.GetAssemblies();
                    Assembly loadedAssembly = asmList.FirstOrDefault(
                             asm => string.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase));
                    if (loadedAssembly != null)
                    {
                        return loadedAssembly;
                    }
                    AssemblyName assemblyName = new AssemblyName(args.Name);
                    string dependentAssemblyFilename = Path.Combine(appDomain.BaseDirectory, assemblyName.Name + ".dll");
                    if (File.Exists(dependentAssemblyFilename))
                    {
                        return Assembly.LoadFrom(dependentAssemblyFilename);
                    }
                    if (assemblyName.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase) && assemblyName.CultureInfo != null)
                    {
                        var resourceAssemblyFilename = Path.Combine(appDomain.BaseDirectory, assemblyName.CultureInfo.Name, assemblyName.Name + ".dll");
                        if (File.Exists(resourceAssemblyFilename))
                        {
                            return Assembly.LoadFrom(resourceAssemblyFilename);
                        }
                        else
                        {
                            Debug.WriteLine("AssemblyResolve 未能加载多语言程序集{0}", (object)args.Name);
                            return null;
                        }
                    }
                    Debug.WriteLine("AssemblyResolve 未能加载程序集{0}", (object)args.Name);
                    return null;
                };
                AssemblyName assemblyRef = new AssemblyName();
                assemblyRef.CodeBase = assemblyFile;
                Assembly assembly = Assembly.Load(assemblyRef);
                a(assembly);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void TraceDomain()
        {
            AppDomain appDomain = AppDomain.CurrentDomain;
            appDomain.UnhandledException += (sender, e) =>
            {
                ILogger<RemoteProxy> log = new Logger<RemoteProxy>();
                var ex = e.ExceptionObject as Exception;
                if (ex != null)
                {
                    log.FatalFormat("UnhandledException AppDomain.FriendlyName={0}", ex, ((AppDomain)sender).FriendlyName);
                }
                else
                {
                    log.FatalFormat("UnhandledException AppDomain.FriendlyName={0},\r\nMessage={1}", ((AppDomain)sender).FriendlyName, e.ExceptionObject);
                }
            };

            appDomain.DomainUnload += delegate(object sender, EventArgs e)
            {
                ILogger<RemoteProxy> log = new Logger<RemoteProxy>();
                log.ErrorFormat("DomainUnload AppDomain.FriendlyName={0}", ((AppDomain)sender).FriendlyName);
            };

            appDomain.ProcessExit += delegate(object sender, EventArgs e)
            {
                ILogger<RemoteProxy> log = new Logger<RemoteProxy>();
                log.ErrorFormat("ProcessExit AppDomain.FriendlyName={0}", ((AppDomain)sender).FriendlyName);
            };            
        }

    }

}

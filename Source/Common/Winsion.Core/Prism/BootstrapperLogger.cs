using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Logging;
using log4net;

namespace Winsion.Core.Prism
{
    public class BootstrapperLogger : ILoggerFacade
    {
        private readonly static log4net.ILog _log = LogManager.GetLogger("prism.lib.shell");

        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    _log.DebugFormat("message -> {0},priority -> {1}", message, priority);
                    break;
                case Category.Info:
                    _log.InfoFormat("message -> {0},priority -> {1}", message, priority);
                    break;
                case Category.Warn:
                    _log.WarnFormat("message -> {0},priority -> {1}", message, priority);
                    break;
                case Category.Exception:
                    _log.ErrorFormat("message -> {0},priority -> {1}", message, priority);
                    break;
                default:
                    _log.InfoFormat("message -> {0},priority -> {1}", message, priority);
                    break;
            }
        }
    }
}

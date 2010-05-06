using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
namespace Avro
{
    sealed class Logger
    {
        private readonly Type _Type;
#if(LOG4NET)
        private readonly log4net.ILog _Log;
#endif
        private const string LEVEL_DEBUG = "DEBUG";
        private const string LEVEL_ERROR = "ERROR";
        private const string LEVEL_INFO = "INFO";
        private const string LEVEL_WARN = "WARN";

        public Logger()
        {
            StackFrame frame = new StackFrame(1);
            MethodBase method = frame.GetMethod();
            _Type = method.DeclaringType;
#if(LOG4NET)
            _Log = log4net.LogManager.GetLogger(_Type);
#endif
        }
#if(LOG4NET)
        #region Log4Net Implementation

        public void Debug(object message, Exception exception)
        {
            _Log.Debug(message, exception);
        }

        public void Debug(object message)
        {
            _Log.Debug(message);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            _Log.DebugFormat(format, arg0, arg1, arg2);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            _Log.DebugFormat(format, arg0, arg1);
        }

        public void DebugFormat(string format, object arg0)
        {
            _Log.DebugFormat(format, arg0);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _Log.DebugFormat(format, args);
        }

        public void Error(object message, Exception exception)
        {
            _Log.Error(message, exception);
        }

        public void Error(object message)
        {
            _Log.Error(message);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            _Log.ErrorFormat(format, arg0, arg1, arg2);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            _Log.ErrorFormat(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0)
        {
            _Log.ErrorFormat(format, arg0);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _Log.ErrorFormat(format, args);
        }

        public void Info(object message, Exception exception)
        {
            _Log.Info(message, exception);
        }

        public void Info(object message)
        {
            _Log.Info(message);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            _Log.InfoFormat(format, arg0, arg1, arg2);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            _Log.InfoFormat(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0)
        {
            _Log.InfoFormat(format, arg0);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _Log.InfoFormat(format, args);
        }

        public bool IsDebugEnabled
        {
            get { return _Log.IsDebugEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return _Log.IsErrorEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return _Log.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return _Log.IsWarnEnabled; }
        }

        public void Warn(object message, Exception exception)
        {
            _Log.Warn(message, exception);
        }

        public void Warn(object message)
        {
            _Log.Warn(message);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            _Log.WarnFormat(format, arg0, arg1, arg2);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            _Log.WarnFormat(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0)
        {
            _Log.WarnFormat(format, arg0);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _Log.WarnFormat(format, args);
        }
        #endregion
#elif(LOGTOCONSOLE)
        #region Logging to Console



        public void Debug(object message, Exception exception)
        {
            Console.WriteLine("{0} {1} - {2} {3}", LEVEL_DEBUG, _Type.FullName, message, exception);
        }

        public void Debug(object message)
        {
            Console.WriteLine("{0} {1} - {2}", LEVEL_DEBUG, _Type.FullName, message);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.Write("{0} {1} - ", LEVEL_DEBUG, _Type.FullName);
            Console.WriteLine(format, arg0, arg1, arg2);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            Console.Write("{0} {1} - ", LEVEL_DEBUG, _Type.FullName);
            Console.WriteLine(format, arg0, arg1);
        }

        public void DebugFormat(string format, object arg0)
        {
            Console.Write("{0} {1} - ", LEVEL_DEBUG, _Type.FullName);
            Console.WriteLine(format, arg0);
        }

        public void DebugFormat(string format, params object[] args)
        {
            Console.Write("{0} {1} - ", LEVEL_DEBUG, _Type.FullName);
            Console.WriteLine(format, args);
        }

        public void Error(object message, Exception exception)
        {
            Console.WriteLine("{0} {1} - {2} {3}", LEVEL_ERROR, _Type.FullName, message, exception);
        }

        public void Error(object message)
        {
            Console.WriteLine("{0} {1} - {2}", LEVEL_ERROR, _Type.FullName, message);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.Write("{0} {1} - ", LEVEL_ERROR, _Type.FullName);
            Console.WriteLine(format, arg0, arg1, arg2);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            Console.Write("{0} {1} - ", LEVEL_ERROR, _Type.FullName);
            Console.WriteLine(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0)
        {
            Console.Write("{0} {1} - ", LEVEL_ERROR, _Type.FullName);
            Console.WriteLine(format, arg0);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Console.Write("{0} {1} - ", LEVEL_ERROR, _Type.FullName);
            Console.WriteLine(format, args);
        }

        public void Info(object message, Exception exception)
        {
            Console.WriteLine("{0} {1} - {2} {3}", LEVEL_INFO, _Type.FullName, message, exception);
        }

        public void Info(object message)
        {
            Console.WriteLine("{0} {1} - {2}", LEVEL_INFO, _Type.FullName, message);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.Write("{0} {1} - ", LEVEL_INFO, _Type.FullName);
            Console.WriteLine(format, arg0, arg1, arg2);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            Console.Write("{0} {1} - ", LEVEL_INFO, _Type.FullName);
            Console.WriteLine(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0)
        {
            Console.Write("{0} {1} - ", LEVEL_INFO, _Type.FullName);
            Console.WriteLine(format, arg0);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Console.Write("{0} {1} - ", LEVEL_INFO, _Type.FullName);
            Console.WriteLine(format, args);
        }

        public bool IsDebugEnabled
        {
            get { return true; }
        }

        public bool IsErrorEnabled
        {
            get { return true; }
        }

        public bool IsInfoEnabled
        {
            get { return true; }
        }

        public bool IsWarnEnabled
        {
            get { return true; }
        }

        public void Warn(object message, Exception exception)
        {
            Console.WriteLine("{0} {1} - {2} {3}", LEVEL_WARN, _Type.FullName, message, exception);
        }

        public void Warn(object message)
        {
            Console.WriteLine("{0} {1} - {2}", LEVEL_WARN, _Type.FullName, message);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.Write("{0} {1} - ", LEVEL_WARN, _Type.FullName);
            Console.WriteLine(format, arg0, arg1, arg2);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            Console.Write("{0} {1} - ", LEVEL_WARN, _Type.FullName);
            Console.WriteLine(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0)
        {
            Console.Write("{0} {1} - ", LEVEL_WARN, _Type.FullName);
            Console.WriteLine(format, arg0);
        }

        public void WarnFormat(string format, params object[] args)
        {
            Console.Write("{0} {1} - ", LEVEL_WARN, _Type.FullName);
            Console.WriteLine(format, args);
        }

        #endregion
#elif(LOG4NET_INTERNAL)

        static string DEFAULT_FORMAT = "{0}" + Environment.NewLine + "{1}";
        private string formatMessage(string level, string format, object[] args)
        {
            try
            {
                string renderedmessage = string.Format(format, args);
                string message = string.Concat(_Type, " ", level, ":", renderedmessage);
                return message;
            }
            catch (Exception ex)
            {
                return "Exception encountered during format" + Environment.NewLine + ex.ToString();
            }
        }
        private void debug(string format, params object[] args)
        {
            string message = formatMessage(LEVEL_DEBUG, format, args);
            LogLog.Debug(message);
        }
        private void error(string format, params object[] args)
        {
            string message = formatMessage(LEVEL_ERROR, format, args);
            LogLog.Error(message);
        }
        private void info(string format, params object[] args)
        {
            string message = formatMessage(LEVEL_INFO, format, args);
            LogLog.Debug(message);
        }
        private void warn(string format, params object[] args)
        {
            string message = formatMessage(LEVEL_WARN, format, args);
            LogLog.Warn(message);
        }
        public void Debug(object message, Exception exception)
        {
            debug(DEFAULT_FORMAT, message, exception);
        }

        public void Debug(object message)
        {
            debug("{0}", message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            debug(format, args); 
        }

        public void Error(object message, Exception exception)
        {
            error(DEFAULT_FORMAT, message, exception);
        }

        public void Error(object message)
        {
            error("{0}", message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            error(format, args);
        }

        public void Info(object message, Exception exception)
        {
            info(DEFAULT_FORMAT, message, exception);
        }

        public void Info(object message)
        {
            info("{0}", message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            info(format, args);
        }

        public bool IsDebugEnabled
        {
            get { return true; }
        }

        public bool IsErrorEnabled
        {
            get { return true; }
        }

        public bool IsInfoEnabled
        {
            get { return true; }
        }

        public bool IsWarnEnabled
        {
            get { return true; }
        }

        public void Warn(object message, Exception exception)
        {
            warn(DEFAULT_FORMAT, message, exception);
        }

        public void Warn(object message)
        {
            warn("{0}", message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            warn(format, args);
        }

#else
        #region Trace Logging

        public void Debug(object message, Exception exception)
        {
            if (!IsDebugEnabled) return;
            Trace.TraceInformation("{0} {1} - {2} {3}", LEVEL_DEBUG, _Type.FullName, message, exception);
        }

        public void Debug(object message)
        {
            if (!IsDebugEnabled) return;
            Trace.TraceInformation("{0} {1} - {2}", LEVEL_DEBUG, _Type.FullName, message);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            if (!IsDebugEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_DEBUG, _Type.FullName);
            builder.AppendFormat(format, arg0, arg1, arg2);
            Trace.TraceInformation(builder.ToString());
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            if (!IsDebugEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_DEBUG, _Type.FullName);
            builder.AppendFormat(format, arg0, arg1);
            Trace.TraceInformation(builder.ToString());
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (!IsDebugEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_DEBUG, _Type.FullName);
            builder.AppendFormat(format, args);
            Trace.TraceInformation(builder.ToString());
        }

        public void Error(object message, Exception exception)
        {
            if (!IsErrorEnabled) return;
            Trace.TraceError("{0} {1} - {2} {3}", LEVEL_ERROR, _Type.FullName, message, exception);
        }

        public void Error(object message)
        {
            if (!IsErrorEnabled) return;
            Trace.TraceError("{0} {1} - {2}", LEVEL_ERROR, _Type.FullName, message);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            if (!IsErrorEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_ERROR, _Type.FullName);
            builder.AppendFormat(format, arg0, arg1, arg2);
            Trace.TraceError(builder.ToString());
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            if (!IsErrorEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_ERROR, _Type.FullName);
            builder.AppendFormat(format, arg0, arg1);
            Trace.TraceError(builder.ToString());
        }

        public void ErrorFormat(string format, object arg0)
        {
            if (!IsErrorEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_ERROR, _Type.FullName);
            builder.AppendFormat(format, arg0);
            Trace.TraceError(builder.ToString());
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (!IsErrorEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_ERROR, _Type.FullName);
            builder.AppendFormat(format, args);
            Trace.TraceError(builder.ToString());
        }

        public void Info(object message, Exception exception)
        {
            if (!IsInfoEnabled) return;
            Trace.TraceInformation("{0} {1} - {2} {3}", LEVEL_INFO, _Type.FullName, message, exception);
        }

        public void Info(object message)
        {
            if (!IsInfoEnabled) return;
            Trace.TraceInformation("{0} {1} - {2}", LEVEL_INFO, _Type.FullName, message);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            if (!IsInfoEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_INFO, _Type.FullName);
            builder.AppendFormat(format, arg0, arg1, arg2);
            Trace.TraceInformation(builder.ToString());
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            if (!IsInfoEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_INFO, _Type.FullName);
            builder.AppendFormat(format, arg0, arg1);
            Trace.TraceInformation(builder.ToString());
        }

        public void InfoFormat(string format, object arg0)
        {
            if (!IsInfoEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_INFO, _Type.FullName);
            builder.AppendFormat(format, arg0);
            Trace.TraceInformation(builder.ToString());
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (!IsInfoEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_INFO, _Type.FullName);
            builder.AppendFormat(format, args);
            Trace.TraceInformation(builder.ToString());
        }

        public bool IsDebugEnabled
        {
            get
            {
#if(DEBUG)
                return true;
#else
                return false; 
#endif
            }
        }

        public bool IsErrorEnabled
        {
            get { return true; }
        }

        public bool IsInfoEnabled
        {
            get { return true; }
        }

        public bool IsWarnEnabled
        {
            get { return true; }
        }

        public void Warn(object message, Exception exception)
        {
            if (!IsWarnEnabled) return;
            Trace.TraceWarning("{0} {1} - {2} {3}", LEVEL_WARN, _Type.FullName, message, exception);
        }

        public void Warn(object message)
        {
            if (!IsWarnEnabled) return;
            Trace.TraceWarning("{0} {1} - {2}", LEVEL_WARN, _Type.FullName, message);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            if (!IsWarnEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_WARN, _Type.FullName);
            builder.AppendFormat(format, arg0, arg1, arg2);
            Trace.TraceWarning(builder.ToString());
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            if (!IsWarnEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_WARN, _Type.FullName);
            builder.AppendFormat(format, arg0, arg1);
            Trace.TraceWarning(builder.ToString());
        }

        public void WarnFormat(string format, object arg0)
        {
            if (!IsWarnEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_WARN, _Type.FullName);
            builder.AppendFormat(format, arg0);
            Trace.TraceWarning(builder.ToString());
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (!IsWarnEnabled) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {1} - ", LEVEL_WARN, _Type.FullName);
            builder.AppendFormat(format, args);
            Trace.TraceWarning(builder.ToString());
        }
        #endregion
#endif
    }
}

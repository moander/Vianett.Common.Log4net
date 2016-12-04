using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Core;
using log4net.Util;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using Vianett.Common;

/// <summary>
/// Finest      10000
/// Finer       20000
/// Fine        30000
///
/// Verbose     10000
/// Trace       20000
/// Debug       30000
/// Info        40000
/// Notice      50000
/// Warn        60000
/// Error       70000
/// Severe      80000
/// Critical    90000
/// Alert       100000
/// Fatal       110000
/// Emergency   120000
/// </summary>
public static class ILogExtensions
{
    private readonly static Type ThisDeclaringType = typeof(ILogExtensions);

    private static string SafeFormat(string format, params object[] args)
    {
        if (string.IsNullOrEmpty(format)) return "";
        try
        {
            return string.Format(format, args);
        }
        catch
        {
            return "";
        }
    }

    #region AsSelf
    public static void CustomAsSelf(this ILog log, Level level, object message, Exception exception = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        if (log != null && log.IsCustomEnabled(level))
        {
            log.Custom(level, SafeFormat("#{0} {1}() {2}", callerLineNumber, callerName, message), exception);
        }
    }

    #region RedirectToCustom (message, exception)
    public static void VerboseAsSelf(this ILog log, object message, Exception exception = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        CustomAsSelf(log, m_levelVerbose, message, exception, callerName, callerLineNumber);
    }

    public static void TraceAsSelf(this ILog log, object message, Exception exception = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        CustomAsSelf(log, m_levelTrace, message, exception, callerName, callerLineNumber);
    }

    public static void DebugAsSelf(this ILog log, object message, Exception exception = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        CustomAsSelf(log, Level.Debug, message, exception, callerName, callerLineNumber);
    }

    public static void InfoAsSelf(this ILog log, object message, Exception exception = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        CustomAsSelf(log, Level.Info, message, exception, callerName, callerLineNumber);
    }

    public static void WarnAsSelf(this ILog log, object message, Exception exception = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        CustomAsSelf(log, Level.Warn, message, exception, callerName, callerLineNumber);
    }

    public static void ErrorAsSelf(this ILog log, object message, Exception exception = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        CustomAsSelf(log, Level.Error, message, exception, callerName, callerLineNumber);
    }

    public static void FatalAsSelf(this ILog log, object message, Exception exception = null, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        CustomAsSelf(log, Level.Fatal, message, exception, callerName, callerLineNumber);
    }
    #endregion

    #region SmartException
    static void CustomAsSelf(this ILog log, Level level, Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        if (log == null || !log.IsCustomEnabled(level)) return;

        string o;

        if (exception == null)
        {
            o = "LogExtensions SmartErrorAsCustom (exception is null)";
        }

#if !FEATURE_CORECLR
        if (exception is ThreadAbortException || exception is ThreadInterruptedException)
        {
            o = exception.GetType().Name;
            exception = null;
        }
        else
        {
            o = exception.GetType().Name + ": " + safeGetExceptionMessage(exception);
        }
#else
            o = exception.GetType().Name + ": " + safeGetExceptionMessage(exception);
#endif

        if (level < Level.Error)
        {
            exception = null;
        }

        log.CustomAsSelf(level, o, exception, callerName, callerLineNumber);
    }

    static string safeGetExceptionMessage(Exception ex)
    {
        if (ex == null) return "";
        try
        {
            return ex.Message;
        }
        catch
        {
            return "(log sys err) ex.Message failed";
        }
    }

    public static void DebugAsSelf(this ILog log, Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        CustomAsSelf(log, Level.Debug, exception, callerName, callerLineNumber);
    }

    public static void WarnAsSelf(this ILog log, Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        CustomAsSelf(log, Level.Warn, exception, callerName, callerLineNumber);
    }

    public static void ErrorAsSelf(this ILog log, Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        CustomAsSelf(log, Level.Error, exception, callerName, callerLineNumber);
    }

    public static void FatalAsSelf(this ILog log, Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
    {
        CustomAsSelf(log, Level.Fatal, exception, callerName, callerLineNumber);
    }
    #endregion

    #endregion

    #region Custom

    public static bool IsCustomEnabled(this ILog log, Level level)
    {
        return log.Logger.IsEnabledFor(level);
    }

    public static void Custom(this ILog log, Level level, object message)
    {
        log.Logger.Log(ThisDeclaringType, level, message, null);
    }

    public static void Custom(this ILog log, Level level, object message, Exception exception)
    {
        log.Logger.Log(ThisDeclaringType, level, message, exception);
    }

    public static void CustomFormat(this ILog log, Level level, string format, params object[] args)
    {
        if (log.IsCustomEnabled(level))
        {
            log.Logger.Log(ThisDeclaringType, level, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
        }
    }

    public static void CustomFormat(this ILog log, Level level, string format, object arg0)
    {
        if (log.IsCustomEnabled(level))
        {
            log.Logger.Log(ThisDeclaringType, level, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0 }), null);
        }
    }

    public static void CustomFormat(this ILog log, Level level, string format, object arg0, object arg1)
    {
        if (log.IsCustomEnabled(level))
        {
            log.Logger.Log(ThisDeclaringType, level, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1 }), null);
        }
    }

    public static void CustomFormat(this ILog log, Level level, string format, object arg0, object arg1, object arg2)
    {
        if (log.IsCustomEnabled(level))
        {
            log.Logger.Log(ThisDeclaringType, level, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1, arg2 }), null);
        }
    }

    public static void CustomFormat(this ILog log, Level level, IFormatProvider provider, string format, params object[] args)
    {
        if (log.IsCustomEnabled(level))
        {
            log.Logger.Log(ThisDeclaringType, level, new SystemStringFormat(provider, format, args), null);
        }
    }
    #endregion

    #region Verbose

    private static Level m_levelVerbose = Level.Verbose;

    public static bool IsVerboseEnabled(this ILog log)
    {
        return log.Logger.IsEnabledFor(m_levelVerbose);
    }

    public static void Verbose(this ILog log, object message)
    {
        log.Logger.Log(ThisDeclaringType, m_levelVerbose, message, null);
    }

    public static void Verbose(this ILog log, object message, Exception exception)
    {
        log.Logger.Log(ThisDeclaringType, m_levelVerbose, message, exception);
    }

    public static void VerboseFormat(this ILog log, string format, params object[] args)
    {
        if (log.IsVerboseEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelVerbose, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
        }
    }

    public static void VerboseFormat(this ILog log, string format, object arg0)
    {
        if (log.IsVerboseEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelVerbose, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0 }), null);
        }
    }

    public static void VerboseFormat(this ILog log, string format, object arg0, object arg1)
    {
        if (log.IsVerboseEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelVerbose, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1 }), null);
        }
    }

    public static void VerboseFormat(this ILog log, string format, object arg0, object arg1, object arg2)
    {
        if (log.IsVerboseEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelVerbose, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1, arg2 }), null);
        }
    }

    public static void VerboseFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
    {
        if (log.IsVerboseEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelVerbose, new SystemStringFormat(provider, format, args), null);
        }
    }
    #endregion

    #region Trace

    private static Level m_levelTrace = Level.Trace;

    public static bool IsTraceEnabled(this ILog log)
    {
        return log.Logger.IsEnabledFor(m_levelTrace);
    }

    public static void Trace(this ILog log, object message)
    {
        log.Logger.Log(ThisDeclaringType, m_levelTrace, message, null);
    }

    public static void Trace(this ILog log, object message, Exception exception)
    {
        log.Logger.Log(ThisDeclaringType, m_levelTrace, message, exception);
    }

    public static void TraceFormat(this ILog log, string format, params object[] args)
    {
        if (log.IsTraceEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelTrace, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
        }
    }

    public static void TraceFormat(this ILog log, string format, object arg0)
    {
        if (log.IsTraceEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelTrace, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0 }), null);
        }
    }

    public static void TraceFormat(this ILog log, string format, object arg0, object arg1)
    {
        if (log.IsTraceEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelTrace, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1 }), null);
        }
    }

    public static void TraceFormat(this ILog log, string format, object arg0, object arg1, object arg2)
    {
        if (log.IsTraceEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelTrace, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1, arg2 }), null);
        }
    }

    public static void TraceFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
    {
        if (log.IsTraceEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelTrace, new SystemStringFormat(provider, format, args), null);
        }
    }
    #endregion

    #region Severe

    private static Level m_levelSevere = Level.Severe;

    public static bool IsSevereEnabled(this ILog log)
    {
        return log.Logger.IsEnabledFor(m_levelSevere);
    }

    public static void Severe(this ILog log, object message)
    {
        log.Logger.Log(ThisDeclaringType, m_levelSevere, message, null);
    }

    public static void Severe(this ILog log, object message, Exception exception)
    {
        log.Logger.Log(ThisDeclaringType, m_levelSevere, message, exception);
    }

    public static void SevereFormat(this ILog log, string format, params object[] args)
    {
        if (log.IsSevereEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelSevere, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
        }
    }

    public static void SevereFormat(this ILog log, string format, object arg0)
    {
        if (log.IsSevereEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelSevere, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0 }), null);
        }
    }

    public static void SevereFormat(this ILog log, string format, object arg0, object arg1)
    {
        if (log.IsSevereEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelSevere, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1 }), null);
        }
    }

    public static void SevereFormat(this ILog log, string format, object arg0, object arg1, object arg2)
    {
        if (log.IsSevereEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelSevere, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1, arg2 }), null);
        }
    }

    public static void SevereFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
    {
        if (log.IsSevereEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelSevere, new SystemStringFormat(provider, format, args), null);
        }
    }
    #endregion

    #region Notice

    private static Level m_levelNotice = Level.Notice;

    public static bool IsNoticeEnabled(this ILog log)
    {
        return log.Logger.IsEnabledFor(m_levelNotice);
    }

    public static void Notice(this ILog log, object message)
    {
        log.Logger.Log(ThisDeclaringType, m_levelNotice, message, null);
    }

    public static void Notice(this ILog log, object message, Exception exception)
    {
        log.Logger.Log(ThisDeclaringType, m_levelNotice, message, exception);
    }

    public static void NoticeFormat(this ILog log, string format, params object[] args)
    {
        if (log.IsNoticeEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelNotice, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
        }
    }

    public static void NoticeFormat(this ILog log, string format, object arg0)
    {
        if (log.IsNoticeEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelNotice, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0 }), null);
        }
    }

    public static void NoticeFormat(this ILog log, string format, object arg0, object arg1)
    {
        if (log.IsNoticeEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelNotice, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1 }), null);
        }
    }

    public static void NoticeFormat(this ILog log, string format, object arg0, object arg1, object arg2)
    {
        if (log.IsNoticeEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelNotice, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1, arg2 }), null);
        }
    }

    public static void NoticeFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
    {
        if (log.IsNoticeEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelNotice, new SystemStringFormat(provider, format, args), null);
        }
    }
    #endregion

    #region Finest

    private static Level m_levelFinest = Level.Finest;

    public static bool IsFinestEnabled(this ILog log)
    {
        return log.Logger.IsEnabledFor(m_levelFinest);
    }

    public static void Finest(this ILog log, object message)
    {
        log.Logger.Log(ThisDeclaringType, m_levelFinest, message, null);
    }

    public static void Finest(this ILog log, object message, Exception exception)
    {
        log.Logger.Log(ThisDeclaringType, m_levelFinest, message, exception);
    }

    public static void FinestFormat(this ILog log, string format, params object[] args)
    {
        if (log.IsFinestEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFinest, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
        }
    }

    public static void FinestFormat(this ILog log, string format, object arg0)
    {
        if (log.IsFinestEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFinest, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0 }), null);
        }
    }

    public static void FinestFormat(this ILog log, string format, object arg0, object arg1)
    {
        if (log.IsFinestEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFinest, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1 }), null);
        }
    }

    public static void FinestFormat(this ILog log, string format, object arg0, object arg1, object arg2)
    {
        if (log.IsFinestEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFinest, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1, arg2 }), null);
        }
    }

    public static void FinestFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
    {
        if (log.IsFinestEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFinest, new SystemStringFormat(provider, format, args), null);
        }
    }
    #endregion

    #region Finer

    private static Level m_levelFiner = Level.Finer;

    public static bool IsFinerEnabled(this ILog log)
    {
        return log.Logger.IsEnabledFor(m_levelFiner);
    }

    public static void Finer(this ILog log, object message)
    {
        log.Logger.Log(ThisDeclaringType, m_levelFiner, message, null);
    }

    public static void Finer(this ILog log, object message, Exception exception)
    {
        log.Logger.Log(ThisDeclaringType, m_levelFiner, message, exception);
    }

    public static void FinerFormat(this ILog log, string format, params object[] args)
    {
        if (log.IsFinerEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFiner, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
        }
    }

    public static void FinerFormat(this ILog log, string format, object arg0)
    {
        if (log.IsFinerEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFiner, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0 }), null);
        }
    }

    public static void FinerFormat(this ILog log, string format, object arg0, object arg1)
    {
        if (log.IsFinerEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFiner, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1 }), null);
        }
    }

    public static void FinerFormat(this ILog log, string format, object arg0, object arg1, object arg2)
    {
        if (log.IsFinerEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFiner, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1, arg2 }), null);
        }
    }

    public static void FinerFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
    {
        if (log.IsFinerEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFiner, new SystemStringFormat(provider, format, args), null);
        }
    }
    #endregion

    #region Fine

    private static Level m_levelFine = Level.Fine;

    public static bool IsFineEnabled(this ILog log)
    {
        return log.Logger.IsEnabledFor(m_levelFine);
    }

    public static void Fine(this ILog log, object message)
    {
        log.Logger.Log(ThisDeclaringType, m_levelFine, message, null);
    }

    public static void Fine(this ILog log, object message, Exception exception)
    {
        log.Logger.Log(ThisDeclaringType, m_levelFine, message, exception);
    }

    public static void FineFormat(this ILog log, string format, params object[] args)
    {
        if (log.IsFineEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFine, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
        }
    }

    public static void FineFormat(this ILog log, string format, object arg0)
    {
        if (log.IsFineEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFine, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0 }), null);
        }
    }

    public static void FineFormat(this ILog log, string format, object arg0, object arg1)
    {
        if (log.IsFineEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFine, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1 }), null);
        }
    }

    public static void FineFormat(this ILog log, string format, object arg0, object arg1, object arg2)
    {
        if (log.IsFineEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFine, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1, arg2 }), null);
        }
    }

    public static void FineFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
    {
        if (log.IsFineEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelFine, new SystemStringFormat(provider, format, args), null);
        }
    }
    #endregion

    #region Emergency

    private static Level m_levelEmergency = Level.Emergency;

    public static bool IsEmergencyEnabled(this ILog log)
    {
        return log.Logger.IsEnabledFor(m_levelEmergency);
    }

    public static void Emergency(this ILog log, object message)
    {
        log.Logger.Log(ThisDeclaringType, m_levelEmergency, message, null);
    }

    public static void Emergency(this ILog log, object message, Exception exception)
    {
        log.Logger.Log(ThisDeclaringType, m_levelEmergency, message, exception);
    }

    public static void EmergencyFormat(this ILog log, string format, params object[] args)
    {
        if (log.IsEmergencyEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelEmergency, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
        }
    }

    public static void EmergencyFormat(this ILog log, string format, object arg0)
    {
        if (log.IsEmergencyEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelEmergency, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0 }), null);
        }
    }

    public static void EmergencyFormat(this ILog log, string format, object arg0, object arg1)
    {
        if (log.IsEmergencyEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelEmergency, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1 }), null);
        }
    }

    public static void EmergencyFormat(this ILog log, string format, object arg0, object arg1, object arg2)
    {
        if (log.IsEmergencyEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelEmergency, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1, arg2 }), null);
        }
    }

    public static void EmergencyFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
    {
        if (log.IsEmergencyEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelEmergency, new SystemStringFormat(provider, format, args), null);
        }
    }
    #endregion

    #region Critical

    private static Level m_levelCritical = Level.Critical;

    public static bool IsCriticalEnabled(this ILog log)
    {
        return log.Logger.IsEnabledFor(m_levelCritical);
    }

    public static void Critical(this ILog log, object message)
    {
        log.Logger.Log(ThisDeclaringType, m_levelCritical, message, null);
    }

    public static void Critical(this ILog log, object message, Exception exception)
    {
        log.Logger.Log(ThisDeclaringType, m_levelCritical, message, exception);
    }

    public static void CriticalFormat(this ILog log, string format, params object[] args)
    {
        if (log.IsCriticalEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelCritical, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
        }
    }

    public static void CriticalFormat(this ILog log, string format, object arg0)
    {
        if (log.IsCriticalEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelCritical, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0 }), null);
        }
    }

    public static void CriticalFormat(this ILog log, string format, object arg0, object arg1)
    {
        if (log.IsCriticalEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelCritical, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1 }), null);
        }
    }

    public static void CriticalFormat(this ILog log, string format, object arg0, object arg1, object arg2)
    {
        if (log.IsCriticalEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelCritical, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1, arg2 }), null);
        }
    }

    public static void CriticalFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
    {
        if (log.IsCriticalEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelCritical, new SystemStringFormat(provider, format, args), null);
        }
    }
    #endregion

    #region Alert

    private static Level m_levelAlert = Level.Alert;

    public static bool IsAlertEnabled(this ILog log)
    {
        return log.Logger.IsEnabledFor(m_levelAlert);
    }

    public static void Alert(this ILog log, object message)
    {
        log.Logger.Log(ThisDeclaringType, m_levelAlert, message, null);
    }

    public static void Alert(this ILog log, object message, Exception exception)
    {
        log.Logger.Log(ThisDeclaringType, m_levelAlert, message, exception);
    }

    public static void AlertFormat(this ILog log, string format, params object[] args)
    {
        if (log.IsAlertEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelAlert, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
        }
    }

    public static void AlertFormat(this ILog log, string format, object arg0)
    {
        if (log.IsAlertEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelAlert, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0 }), null);
        }
    }

    public static void AlertFormat(this ILog log, string format, object arg0, object arg1)
    {
        if (log.IsAlertEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelAlert, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1 }), null);
        }
    }

    public static void AlertFormat(this ILog log, string format, object arg0, object arg1, object arg2)
    {
        if (log.IsAlertEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelAlert, new SystemStringFormat(CultureInfo.InvariantCulture, format, new object[] { arg0, arg1, arg2 }), null);
        }
    }

    public static void AlertFormat(this ILog log, IFormatProvider provider, string format, params object[] args)
    {
        if (log.IsAlertEnabled())
        {
            log.Logger.Log(ThisDeclaringType, m_levelAlert, new SystemStringFormat(provider, format, args), null);
        }
    }
    #endregion
}


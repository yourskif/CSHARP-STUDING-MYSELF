using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace ConsoleApp.Helpers
{
    /// <summary>
    /// Простий файловий логер + tee-вивід у консоль.
    /// </summary>
    public static class FileLogger
    {
        // SA1000: уникаємо target-typed new(); використовуємо new object()
        private static readonly object Sync = new object();

        private static string? logFilePath;
        private static TextWriter? originalConsoleOut;
        private static TextWriter? teeWriter;
        private static StreamWriter? fileWriter;
        private static bool processExitHooked;

        /// <summary>
        /// Повертає шлях до активного лог-файлу (або null, якщо ще не ініціалізовано).
        /// </summary>
        public static string? CurrentLogPath => logFilePath;

        /// <summary>
        /// Ініціалізує лог-файл у &lt;baseDir&gt;\logs\app-YYYYMMDD-HHMMSS.log і пише шапку.
        /// </summary>
        public static void Init(string? baseDir = null, string? fileName = null)
        {
            lock (Sync)
            {
                if (logFilePath is not null && fileWriter is not null)
                {
                    return;
                }

                string dir = baseDir ?? AppContext.BaseDirectory;
                string logsDir = Path.Combine(dir, "logs");
                Directory.CreateDirectory(logsDir);

                logFilePath = Path.Combine(
                    logsDir,
                    fileName ?? $"app-{DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture)}.log");

                // Один спільний стрім до файлу (Dispose у Shutdown)
                fileWriter = new StreamWriter(
                    new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    AutoFlush = true,
                    NewLine = Environment.NewLine,
                }; // SA1413: кома після останнього ініціалізатора

                AppendLineToLog(new string('=', 70));
                AppendLineToLog($"Log started at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}");
                AppendLineToLog(new string('=', 70));

                if (!processExitHooked)
                {
                    AppDomain.CurrentDomain.ProcessExit += (_, __) => Shutdown();
                    processExitHooked = true;
                }
            }
        }

        /// <summary>
        /// Увімкнути tee: увесь консольний вивід дублікатом іде в лог-файл.
        /// </summary>
        public static void AttachConsoleTee()
        {
            lock (Sync)
            {
                if (fileWriter is null)
                {
                    Init();
                }

                if (teeWriter is not null)
                {
                    return; // вже підключено
                }

                originalConsoleOut ??= Console.Out;

                // CA2000: гарантуємо Dispose на всіх шляхах
                NonDisposableTextWriter? consoleWrapped = null;
                try
                {
                    consoleWrapped = new NonDisposableTextWriter(originalConsoleOut);
                    teeWriter = new MultiTextWriter(consoleWrapped, fileWriter!);
                    // Власність передано в MultiTextWriter → щоб не подвійно не звільнити в finally
                    consoleWrapped = null;
                    Console.SetOut(teeWriter);
                }
                finally
                {
                    consoleWrapped?.Dispose();
                }
            }
        }

        /// <summary>
        /// Лог-секція.
        /// </summary>
        public static void Section(string title)
        {
            AppendLineToLog(new string('=', 70));
            AppendLineToLog(title);
            AppendLineToLog(new string('=', 70));
        }

        public static void Info(string message) =>
            AppendLineWithLevel("INFO ", message);

        public static void Warn(string message) =>
            AppendLineWithLevel("WARN ", message);

        public static void Error(string message) =>
            AppendLineWithLevel("ERROR", message);

        public static void Error(Exception ex, string? message = null)
        {
            string line = message is null ? ex.ToString() : $"{message}: {ex}";
            AppendLineWithLevel("ERROR", line);
        }

        /// <summary>
        /// Акуратно відключає tee і закриває файл.
        /// </summary>
        public static void Shutdown()
        {
            lock (Sync)
            {
                try
                {
                    if (teeWriter is not null && originalConsoleOut is not null)
                    {
                        Console.SetOut(originalConsoleOut);
                        teeWriter.Dispose(); // закриє обгорнутий consoleWriter (no-op) і файл через MultiTextWriter
                        teeWriter = null;
                    }
                }
                finally
                {
                    fileWriter?.Dispose();
                    fileWriter = null;
                }
            }
        }

        // ===== Внутрішня утиліта запису в лог-файл =====
        private static void AppendLineToLog(string value)
        {
            lock (Sync)
            {
                if (fileWriter is null)
                {
                    Init();
                }

                fileWriter!.WriteLine(value ?? string.Empty);
            }
        }

        private static void AppendLineWithLevel(string level5, string message)
        {
            string stamp = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            AppendLineToLog($"[{stamp}] {level5} {message}");
        }

        // ===== Внутрішні класи =====

        /// <summary>
        /// Записує в два TextWriter одночасно (tee).
        /// </summary>
        private sealed class MultiTextWriter : TextWriter
        {
            private readonly TextWriter consoleWriter;
            private readonly TextWriter fileLogWriter;

            public MultiTextWriter(TextWriter consoleWriter, TextWriter fileLogWriter)
            {
                this.consoleWriter = consoleWriter ?? throw new ArgumentNullException(nameof(consoleWriter));
                this.fileLogWriter = fileLogWriter ?? throw new ArgumentNullException(nameof(fileLogWriter));
            }

            public override Encoding Encoding => this.consoleWriter.Encoding;

            public override void Write(char value)
            {
                this.consoleWriter.Write(value);
                this.fileLogWriter.Write(value);
            }

            public override void Write(string? value)
            {
                this.consoleWriter.Write(value);
                this.fileLogWriter.Write(value);
            }

            public override void Write(char[] buffer, int index, int count)
            {
                this.consoleWriter.Write(buffer, index, count);
                this.fileLogWriter.Write(buffer, index, count);
            }

            public override void WriteLine(string? value)
            {
                this.consoleWriter.WriteLine(value);
                this.fileLogWriter.WriteLine(value);
            }

            public override void Flush()
            {
                this.consoleWriter.Flush();
                this.fileLogWriter.Flush();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    // Консольний врайтер — NonDisposableTextWriter (без закриття stdout)
                    this.consoleWriter.Dispose();
                    this.fileLogWriter.Dispose();
                }

                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Обгортка над TextWriter, у якої Dispose робить лише Flush (не закриває inner).
        /// Використовується, щоб не "закрити" реальний stdout.
        /// </summary>
        private sealed class NonDisposableTextWriter : TextWriter
        {
            private readonly TextWriter inner;

            public NonDisposableTextWriter(TextWriter inner)
            {
                this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            }

            public override Encoding Encoding => this.inner.Encoding;

            public override void Write(char value) => this.inner.Write(value);
            public override void Write(string? value) => this.inner.Write(value);
            public override void Write(char[] buffer, int index, int count) => this.inner.Write(buffer, index, count);
            public override void WriteLine(string? value) => this.inner.WriteLine(value);
            public override void Flush() => this.inner.Flush();

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    // Нам не належить stdout — лише flush.
                    this.inner.Flush();
                }

                // CA2215: все одно викликаємо базовий Dispose
                base.Dispose(disposing);
            }
        }
    }
}

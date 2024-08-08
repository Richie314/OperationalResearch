namespace OperationalResearch
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;
            Application.Run(new StartForm());
        }
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var r = MessageBox.Show(
                e.Exception.Message + Environment.NewLine + 
                e.Exception.StackTrace ?? string.Empty,
                "An exception caused the app to fail",
                MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Error);
            if (r == DialogResult.Abort)
            {
                Application.Exit();
            }
        }
    }
}
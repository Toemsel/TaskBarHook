using System;

namespace TaskbarHook
{
    public static class TaskBarFactory
    {
        private const string SHELLTRAY = "Shell_traywnd";
        private const string REBAR = "ReBarWindow32";
        private const string SYSPAGER = "SysPager";

        public static IntPtr ShellTrayHandle => User32.GetWindow(SHELLTRAY);

        public static IntPtr ReBarWindowHandle => User32.GetWindow(ShellTrayHandle, REBAR);

        public static IntPtr IconTrayHandle => User32.GetNextWindow(ReBarWindowHandle);

        public static IntPtr SysPagerHandle => User32.GetWindow(IconTrayHandle, SYSPAGER);

        public static IntPtr InfoSectionHandle => User32.GetFirstWindowChild(SysPagerHandle);

        public static IntPtr TaskBarHandle => User32.GetFirstWindowChild(User32.GetFirstWindowChild(ReBarWindowHandle));

        /// <summary>
        /// Tries to get the taskbar of the running windows system.
        /// (MSTaskListWClass)
        /// </summary>
        /// <returns>A Taskbar representing object for manipulation.</returns>
        /// <exception cref="PlatformNotSupportedException">No TaskBar could be obtained.</exception>
        public static Taskbar GetTaskbar()
        {
            if (Taskbar.Instance != null)
                return Taskbar.Instance;

            IntPtr taskbarHandle = TaskBarHandle;

            if (taskbarHandle == IntPtr.Zero)
                throw new PlatformNotSupportedException($"The TaskBar cound't be obtained.");

            Taskbar.CreateAndInitialize(taskbarHandle);
            return GetTaskbar();
        }
    }
}

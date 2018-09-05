using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskbarHook
{
    public class Taskbar
    {
        public Taskbar(IntPtr handle)
        {
            Handle = handle;

            Initialize();
        }

        private void Initialize()
        {
            Rectangle = User32.GetWindowRectangle(Handle);
        }

        public IntPtr Handle { get; private set; }

        public Rectangle Rectangle { get; private set; }

        public async Task<TaskbarElement> AddToTaskbar() => await AddToTaskbar(Process.GetCurrentProcess());

        public async Task<TaskbarElement> AddToTaskbar(Process process)
        {
            while (process.MainWindowHandle == IntPtr.Zero)
                await Task.Delay(6); //ToDo: Add timeout

            TaskbarElement taskbarElement = new TaskbarElement(process.MainWindowHandle);
            taskbarElement.SetParent(Handle);

            return taskbarElement;
        }
    }
}

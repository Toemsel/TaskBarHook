using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskbarHook
{
    public class Taskbar : IDisposable
    {
        private static WinEventDelegate taskbarResizeDelegate = new WinEventDelegate(TaskBarResizeEvent);

        private Taskbar() { }

        internal static void CreateAndInitialize(IntPtr taskbarHandle)
        {
            Instance = new Taskbar();
            Instance.Handle = taskbarHandle;
            Instance.Rectangle = User32.GetWindowRectangle(Instance.Handle);
            Instance.TaskbarResizeHandle = User32.RegisterWindowSizeChangeEvent(Instance.Handle, taskbarResizeDelegate);
        }

        internal static Taskbar Instance { get; private set; }

        public IntPtr Handle { get; private set; }

        public Rectangle Rectangle { get; private set; }

        public async Task<TaskbarElement> AddToTaskbar() => await AddToTaskbar(Process.GetCurrentProcess());

        public Action SizeChanged { get; set; }

        private IntPtr TaskbarResizeHandle { get; set; }

        public async Task<TaskbarElement> AddToTaskbar(Process process)
        {
            while (process.MainWindowHandle == IntPtr.Zero)
                await Task.Delay(6); //ToDo: Add timeout

            TaskbarElement taskbarElement = new TaskbarElement(process.MainWindowHandle);
            taskbarElement.SetParent(Handle);

            return taskbarElement;
        }

        private static void TaskBarResizeEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            var currentRectangle = User32.GetWindowRectangle(Instance.Handle);
            if (currentRectangle.Equals(Instance.Rectangle))
                return;

            Instance.Rectangle = currentRectangle;
            Instance?.SizeChanged?.Invoke();
        }

        public void Dispose() => User32.UnRegisterWindowSizeChangeEvent(TaskbarResizeHandle);
    }
}

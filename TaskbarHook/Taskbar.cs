using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskbarHook
{
    public class Taskbar : IDisposable
    {
        private static WinEventDelegate eventDelegate = new WinEventDelegate(TaskBarResizeEvent);

        private Taskbar() { }

        internal static void CreateAndInitialize(IntPtr handle)
        {
            Instance = new Taskbar();
            Instance.Handle = handle;
            Instance.Rectangle = User32.GetWindowRectangle(Instance.Handle);
            Instance.ResizeHandle = User32.RegisterWindowSizeChangeEvent(Instance.Handle, eventDelegate);
        }

        internal static Taskbar Instance { get; private set; }

        public IntPtr Handle { get; private set; }

        private IntPtr ResizeHandle { get; set; }

        public Rectangle Rectangle { get; private set; }

        public Action SizeChanged { get; set; }

        public async Task<TaskbarElement> AddToTaskbar() => await AddToTaskbar(Process.GetCurrentProcess());

        public async Task<TaskbarElement> AddToTaskbar(Process process)
        {
            while (process.MainWindowHandle == IntPtr.Zero)
                await Task.Delay(6); //ToDo: Add timeout

            TaskbarElement taskbarElement = new TaskbarElement(process.MainWindowHandle);
            taskbarElement.SetParent(Handle);

            return taskbarElement;
        }

        private static void TaskBarResizeEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) => Instance?.SizeChanged();

        public void Dispose() => User32.UnRegisterWindowSizeChangeEvent(ResizeHandle);
    }
}

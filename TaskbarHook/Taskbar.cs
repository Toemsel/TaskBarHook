using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskbarHook
{
    public class Taskbar : IDisposable
    {
        private static WinEventDelegate taskbarResizeDelegate = new WinEventDelegate(TaskBarResizeEvent);
        private static WinEventDelegate sysPageResizeDelegate = new WinEventDelegate(SysPageResizeEvent);

        private Taskbar() { }

        internal static void CreateAndInitialize(IntPtr taskbarHandle, IntPtr sysPageHandle)
        {
            Instance = new Taskbar();
            Instance.Handle = taskbarHandle;
            Instance.SysPageHandle = sysPageHandle;
            Instance.Rectangle = User32.GetWindowRectangle(Instance.Handle);
            Instance.SysPageRectangle = User32.GetWindowRectangle(Instance.SysPageHandle);
            Instance.TaskbarResizeHandle = User32.RegisterWindowSizeChangeEvent(Instance.Handle, taskbarResizeDelegate);
            Instance.SysPageResizeHandle = User32.RegisterWindowElementAdded(Instance.SysPageHandle, sysPageResizeDelegate);
        }

        internal static Taskbar Instance { get; private set; }

        public IntPtr Handle { get; private set; }

        private IntPtr SysPageHandle { get; set; }

        private IntPtr TaskbarResizeHandle { get; set; }

        private IntPtr SysPageResizeHandle { get; set; }

        public Rectangle Rectangle { get; private set; }

        private Rectangle SysPageRectangle { get; set; }

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

        private static void TaskBarResizeEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            Instance.Rectangle = User32.GetWindowRectangle(Instance.Handle);
            Instance?.SizeChanged?.Invoke();
        }

        private static void SysPageResizeEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            var currentIconTrayRectangle = User32.GetWindowRectangle(Instance.SysPageHandle);
            int width = currentIconTrayRectangle.Right - currentIconTrayRectangle.Left;
            int height = currentIconTrayRectangle.Bottom - currentIconTrayRectangle.Top;
            int cwidth = Instance.SysPageRectangle.Right - Instance.SysPageRectangle.Left;
            int cheight = Instance.SysPageRectangle.Bottom - Instance.SysPageRectangle.Top;

            if (width != cwidth || height != cheight)
            {
                Instance.SysPageRectangle = currentIconTrayRectangle;
                TaskBarResizeEvent(hWinEventHook, eventType, hwnd, idObject, idChild, dwEventThread, dwmsEventTime);
            }
        }

        public void Dispose()
        {
            User32.UnRegisterWindowSizeChangeEvent(TaskbarResizeHandle);
            User32.UnRegisterWindowSizeChangeEvent(SysPageResizeHandle);
        }
    }
}

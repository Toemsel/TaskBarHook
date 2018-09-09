using System;
using System.Runtime.InteropServices;

namespace TaskbarHook
{
    internal static class User32
    {
        //private const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;
        //private const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;
        private const uint WINEVENT_OUTOFCONTEXT = 0;

        #region DLLImports
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hwnd, out Rectangle lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        #endregion

        internal static bool SetWindowPosition(IntPtr handle, int x, int y) => SetWindowPos(handle, 0, x, y, 0, 0, 0x0001) != IntPtr.Zero;

        internal static IntPtr SetWindowParent(IntPtr parent, IntPtr child) => SetParent(child, parent);

        internal static bool HideWindow(IntPtr handle) => SetVisibilityState(handle, 0);

        internal static bool ShowWindow(IntPtr handle) => SetVisibilityState(handle, 5);

        internal static bool SetVisibilityState(IntPtr handle, int state) => ShowWindow(handle, state);

        internal static IntPtr RegisterWindowSizeChangeEvent(IntPtr handle, WinEventDelegate delegateCallback)
        {
            uint process, thread = 0;
            thread = GetWindowThreadProcessId(handle, out process);

            return SetWinEventHook(0x00001, 0x7FFFF, IntPtr.Zero, delegateCallback, process, thread, WINEVENT_OUTOFCONTEXT);
        }

        internal static IntPtr RegisterWindowElementAdded(IntPtr handle, WinEventDelegate delegateCallback)
        {
            uint process, thread = 0;
            thread = GetWindowThreadProcessId(handle, out process);

            return SetWinEventHook(0x8001, 0x8001, IntPtr.Zero, delegateCallback, process, thread, WINEVENT_OUTOFCONTEXT);
        }

        internal static bool UnRegisterWindowSizeChangeEvent(IntPtr hoockHandle) => UnhookWinEvent(hoockHandle);

        internal static IntPtr GetWindowParent(IntPtr handle) => GetParent(handle);

        internal static IntPtr GetWindow(string className) => GetWindow(IntPtr.Zero, className);

        internal static IntPtr GetWindow(IntPtr parentHandle, string className) => FindWindowEx(parentHandle, IntPtr.Zero, className, string.Empty);

        internal static IntPtr GetFirstWindowChild(IntPtr parentHandle) => GetWindow(parentHandle, 5);

        internal static IntPtr GetNextWindow(IntPtr handle) => GetWindow(handle, 2);

        internal static Rectangle GetWindowRectangle(IntPtr handle)
        {
            Rectangle rectangle = new Rectangle();
            GetWindowRect(handle, out rectangle);
            return rectangle;
        }
    }
}

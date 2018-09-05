using System;

namespace TaskbarHook
{
    public class TaskbarElement
    {
        public TaskbarElement(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; private set; }

        public IntPtr Parent { get; private set; } = IntPtr.Zero;

        public IntPtr OriginalParent { get; private set; } = IntPtr.Zero;

        public bool CanResetParent => OriginalParent != IntPtr.Zero;

        public bool Show() => User32.ShowWindow(Handle);

        public bool Hide() => User32.HideWindow(Handle);

        /// <summary>
        /// Sets the position of the process.
        /// (0, 0) indicates the top left corner of the taskbar.
        /// </summary>
        /// <param name="x">The X coordinate, relative to its parent.</param>
        /// <param name="y">The y coordinate, relative to its parent.</param>
        /// <param name="width">The width of the process.</param>
        /// <param name="height">The height of the process.</param>
        /// <returns><c>true</c> if the position adjustment was successfull, <c>false</c> otherwise.</returns>
        public bool SetPosition(int x, int y) => User32.SetWindowPosition(Handle, x, y);

        internal bool SetParent(IntPtr handle)
        {
            Parent = handle;
            OriginalParent = User32.SetWindowParent(handle, Handle);
            return OriginalParent != IntPtr.Zero;
        }

        public bool ResetParent()
        {
            if (!CanResetParent)
                throw new ApplicationException($"Could not reset the parent. No parent known!");

            var parentBackup = OriginalParent;
            var returnValue = SetParent(OriginalParent);
            OriginalParent = parentBackup;
            return returnValue;
        }
    }
}

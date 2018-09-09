using System.Runtime.InteropServices;

namespace TaskbarHook
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner

        public bool Equals(Rectangle rectangle) => Left == rectangle.Left &&
            Right == rectangle.Right &&
            Top == rectangle.Top &&
            Bottom == rectangle.Bottom;
    }
}

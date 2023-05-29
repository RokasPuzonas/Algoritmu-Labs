using Raylib_cs;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml;

namespace Lab4
{
    public class RaylibDrawContext
    {
        class DrawFrame
        {
            public float ox = 0;
            public float oy = 0;
            public float sx = 1;
            public float sy = 1;
            public DrawFrame(float ox, float oy, float sx, float sy)
            {
                this.ox = ox;
                this.oy = oy;
                this.sx = sx;
                this.sy = sy;
            }
        }

        public float ox = 0;
        public float oy = 0;
        public float sx = 1;
        public float sy = 1;

        Stack<DrawFrame> stack;

        public RaylibDrawContext()
        {
            stack = new Stack<DrawFrame>();
        }

        public void Translate(float offsetX, float offsetY)
        {
            ox += offsetX / sx;
            oy += offsetY / sy;
        }

        public void Scale(float scaleX, float scaleY)
        {
            sx *= scaleX;
            sy *= scaleY;
        }

        public void Push()
        {
            stack.Push(new DrawFrame(ox, oy, sx, sy));
        }

        public void Pop()
        {
            var frame = stack.Pop();
            ox = frame.ox;
            oy = frame.oy;
            sx = frame.sx;
            sy = frame.sy;
        }

        public Vector2 TransformToLocalSpace(Vector2 pos)
        {
            return new Vector2(
                (pos.X / sx) + ox,
                (pos.Y / sy) + oy
            );
        }

        public void DrawCircle(float centerX, float centerY, float radius, Color color)
        {
            int x = (int)((centerX - ox) * sx);
            int y = (int)((centerY - oy) * sy);
            float rh = radius * sx;
            float rv = radius * sy;
            Raylib.DrawEllipse(x, y, rh, rv, color);
        }

        public void DrawCircleLines(float centerX, float centerY, float radius, Color color)
        {
            int x = (int)((centerX - ox) * sx);
            int y = (int)((centerY - oy) * sy);
            float rh = radius * sx;
            float rv = radius * sy;
            Raylib.DrawEllipseLines(x, y, rh, rv, color);
        }

        public void DrawLine(float startPosX, float startPosY, float endPosX, float endPosY, Color color)
        {
            int x0 = (int)((startPosX - ox) * sx);
            int y0 = (int)((startPosY - oy) * sy);
            int x1 = (int)((endPosX - ox) * sx);
            int y1 = (int)((endPosY - oy) * sy);
            Raylib.DrawLine(x0, y0, x1, y1, color);
        }
    }
}

//  m o n O c l e
//
//   e n g i n e
//
//
//Copyright (c) 2012 - 2014 Matt Thorson
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Technolithic
{
    public class MyTexture
    {
        public Texture2D Texture { get; set; }
        public Rectangle ClipRect { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2 Size { get; private set; }
        public Vector2 Center { get; private set; }
        

        static public MyTexture FromFile(string filename)
        {
            var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
            fileStream.Close();

            return new MyTexture(texture);
        }

        public MyTexture(Texture2D texture)
        {
            Texture = texture;
            ClipRect = new Rectangle(0, 0, texture.Width, texture.Height);
            Width = ClipRect.Width;
            Height = ClipRect.Height;
            Size = new Vector2(Width, Height);
            SetUtil();
        }

        public MyTexture(MyTexture parent, int x, int y, int width, int height)
        {
            Texture = parent.Texture;
            ClipRect = parent.GetRelativeRect(x, y, width, height);
            Width = width;
            Height = height;
            Size = new Vector2(Width, Height);
            SetUtil();
        }

        public MyTexture(int width, int height, Color color)
        {
            Texture = new Texture2D(Engine.Instance.GraphicsDevice, width, height);
            var colors = new Color[width * height];
            for (int i = 0; i < width * height; i++)
            {
                colors[i] = color;
            }
            Texture.SetData(colors);

            ClipRect = new Rectangle(0, 0, width, height);
            Width = width;
            Height = height;
            Size = new Vector2(Width, Height);
            SetUtil();
        }

        private void SetUtil()
        {
            Center = new Vector2(Width, Height) * 0.5f;
        }

        public MyTexture GetSubtexture(int x, int y, int width, int height)
        {
            return new MyTexture(this, x, y, width, height);
        }

        public Color[] GetData()
        {
            Color[] data = new Color[ClipRect.Width * ClipRect.Height];

            Texture.GetData(0, ClipRect, data, 0, ClipRect.Width * ClipRect.Height);

            return data;
        }

        #region Helpers

        public Rectangle GetRelativeRect(Rectangle rect)
        {
            return GetRelativeRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        private Rectangle GetRelativeRect(int x, int y, int width, int height)
        {
            int atX = ClipRect.X + x;
            int atY = ClipRect.Y + y;

            int rX = MathHelper.Clamp(atX, ClipRect.Left, ClipRect.Right);
            int rY = MathHelper.Clamp(atY, ClipRect.Top, ClipRect.Bottom);
            int rW = Math.Max(0, Math.Min(atX + width, ClipRect.Right) - rX);
            int rH = Math.Max(0, Math.Min(atY + height, ClipRect.Bottom) - rY);

            return new Rectangle(rX, rY, rW, rH);
        }

        #endregion

        #region Draw

        public void Draw(Rectangle dest, Color color)
        {
#if DEBUG
            if (Texture.IsDisposed)
                throw new Exception("Texture2D Is Disposed");
#endif
            RenderManager.SpriteBatch.Draw(Texture, dest, ClipRect, color, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void Draw(Rectangle dest, Vector2 origin, Color color, float rotation, SpriteEffects effects)
        {
#if DEBUG
            if (Texture.IsDisposed)
                throw new Exception("Texture2D Is Disposed");
#endif
            RenderManager.SpriteBatch.Draw(Texture, dest, ClipRect, color, rotation, origin, effects, 0);
        }

        public void Draw(Rectangle dest, Rectangle clipRect, Vector2 origin, Color color, float rotation, SpriteEffects effects)
        {
#if DEBUG
            if (Texture.IsDisposed)
                throw new Exception("Texture2D Is Disposed");
#endif
            RenderManager.SpriteBatch.Draw(Texture, dest, clipRect, color, rotation, origin, effects, 0);
        }

        public void Draw(Vector2 position)
        {
#if DEBUG
            if (Texture.IsDisposed)
                throw new Exception("Texture2D Is Disposed");
#endif
            RenderManager.SpriteBatch.Draw(Texture, position, ClipRect, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public void Draw(Vector2 position, Color color)
        {
#if DEBUG
            if (Texture.IsDisposed)
                throw new Exception("Texture2D Is Disposed");
#endif
            RenderManager.SpriteBatch.Draw(Texture, position, ClipRect, color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public void Draw(Vector2 position, Vector2 origin, Color color)
        {
#if DEBUG
            if (Texture.IsDisposed)
                throw new Exception("Texture2D Is Disposed");
#endif
            RenderManager.SpriteBatch.Draw(Texture, position, ClipRect, color, 0, origin, 1f, SpriteEffects.None, 0);
        }

        public void Draw(Vector2 position, Vector2 origin, Color color, float scale, float rotation)
        {
#if DEBUG
            if (Texture.IsDisposed)
                throw new Exception("Texture2D Is Disposed");
#endif
            RenderManager.SpriteBatch.Draw(Texture, position, ClipRect, color, rotation, origin, scale, SpriteEffects.None, 0);
        }

        public void Draw(Vector2 position, Vector2 origin, Color color, Vector2 scale, float rotation, SpriteEffects flip)
        {
#if DEBUG
            if (Texture.IsDisposed)
                throw new Exception("Texture2D Is Disposed");
#endif
            RenderManager.SpriteBatch.Draw(Texture, position, ClipRect, color, rotation, origin, scale, flip, 0);
        }

        #endregion

        #region Draw Centered

        public void DrawCentered(Vector2 position, Color color)
        {
#if DEBUG
            if (Texture.IsDisposed)
                throw new Exception("Texture2D Is Disposed");
#endif
            RenderManager.SpriteBatch.Draw(Texture, position, ClipRect, color, 0, Center, 1f, SpriteEffects.None, 0);
        }

        #endregion

        public void Unload()
        {
            Texture.Dispose();
            Texture = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;


namespace Technolithic
{
    public class MMesh
    {

        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        private VertexPositionColorTexture[] vertices;
        public VertexPositionColorTexture[] Vertices
        {
            get { return vertices; }
            set
            {
                vertices = value;

                vertexBuffer = new VertexBuffer(Engine.Instance.GraphicsDevice,
                typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.WriteOnly);

                vertexBuffer.SetData(vertices);
            }
        }

        private short[] triangles;
        public short[] Triangles
        {
            get { return triangles; }
            set
            {
                triangles = value;

                indexBuffer = new IndexBuffer(
                Engine.Instance.GraphicsDevice,
                typeof(short),
                triangles.Length,
                BufferUsage.WriteOnly);

                indexBuffer.SetData(triangles);
            }
        }

        public void Render()
        {
            GraphicsDevice gd = Engine.Instance.GraphicsDevice;

            gd.Indices = indexBuffer;
            gd.SetVertexBuffer(vertexBuffer);

            gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, triangles.Length / 3);
        }

    }
}

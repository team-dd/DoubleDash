using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    class ShapeNPoints
    {
        ushort[] Indices = new ushort[200];
        VertexPositionColor[] vertices;
        //Vector2 location;
        GraphicsDeviceManager graphics;
        VertexBuffer vertexBuffer;
        DynamicIndexBuffer indexBuffer;
        BasicEffect basicEffect;
        Matrix world;
        Matrix view;
        static Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);
        Matrix rotation;
        int n;

        Vector3 pos;
        static Random rand = new Random();
        float zoomthing;

        public ShapeNPoints(GraphicsDeviceManager device, float zoomthing, Color color, int n)
        {
            graphics = device;
            vertices = new VertexPositionColor[n];
            this.n = n;

            for (int i = 0; i < n*2; i++)
            {
                if (i != (n*2) -1)
                {
                    Indices[i] = (ushort) ((i / 2) - ((i/2) % 1));
                }
                else
                {
                    Indices[i] = 0;
                }
            }

            world = Matrix.CreateTranslation(0, 0, 0);
            view = Matrix.CreateLookAt(new Vector3(0, 0, (zoomthing % 4f)), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            this.zoomthing = zoomthing;
            float size = 0.25f;
            double angle = MathHelper.TwoPi / n;

            for (int i = 0; i < n; i++)
            {
                Vector3 position = new Vector3(size * (float)Math.Round(Math.Sin(angle * i * 2), 4), size * (float)Math.Round(Math.Cos(angle * i * 2), 4), 0f);
                vertices[i] = new VertexPositionColor(position, Color.Red * .1f);
            }

            basicEffect = new BasicEffect(graphics.GraphicsDevice);

            vertexBuffer = new VertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColor), n, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            indexBuffer = new DynamicIndexBuffer(device.GraphicsDevice, typeof(ushort), 200, BufferUsage.WriteOnly);
            indexBuffer.SetData(0, Indices, 0, n*2);

            rotation = Matrix.CreateRotationZ(MathHelper.ToRadians(90 * zoomthing));

            basicEffect.Projection = projection;
            basicEffect.VertexColorEnabled = true;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;
        }

        public void Update()
        {
            zoomthing -= .02f;

            if (zoomthing <= 0f)
            {
                zoomthing = 4f;
            }
            //rotation = Matrix.CreateRotationZ(MathHelper.ToRadians(60 * zoomthing));
            view = Matrix.CreateLookAt(new Vector3(0, 0, (zoomthing % 4f)), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        }

        public void UpdateColor(Color newColor)
        {
            newColor *= 0.1f;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = newColor;
            }
            vertexBuffer.SetData<VertexPositionColor>(vertices);
        }

        public void Draw()
        {
            basicEffect.World = rotation * world;
            basicEffect.View = view;

            graphics.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            graphics.GraphicsDevice.Indices = indexBuffer;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, n, 0, n);
            }
        }
    }
}

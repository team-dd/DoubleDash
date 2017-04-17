using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    class ShapeSquare
    {
        ushort[] Indices = new ushort[20];
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

        Vector3 pos;
        static Random rand = new Random();
        float zoomthing;

        public ShapeSquare(GraphicsDeviceManager device, float zoomthing, Color color)
        {
            graphics = device;
            vertices = new VertexPositionColor[6];

            Indices[0] = (ushort)(0);
            Indices[1] = (ushort)(1);
            Indices[2] = (ushort)(1);
            Indices[3] = (ushort)(2);
            Indices[4] = (ushort)(2);
            Indices[5] = (ushort)(3);
            Indices[6] = (ushort)(3);
            Indices[7] = (ushort)(4);
            Indices[8] = (ushort)(4);
            Indices[9] = (ushort)(5);
            Indices[10] = (ushort)(5);
            Indices[11] = (ushort)(0);
            world = Matrix.CreateTranslation(0, 0, 0);
            view = Matrix.CreateLookAt(new Vector3(0, 0, (zoomthing % 4f)), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            this.zoomthing = zoomthing;
            float size = 0.25f;
            double angle = MathHelper.TwoPi / 6;

            for (int i = 0; i < 6; i++)
            {
                Vector3 position = new Vector3(size * (float)Math.Round(Math.Sin(angle * i), 4), size * (float)Math.Round(Math.Cos(angle * i), 4), 0f);
                vertices[i] = new VertexPositionColor(position, Color.Red * .25f);
            }

            basicEffect = new BasicEffect(graphics.GraphicsDevice);

            vertexBuffer = new VertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColor), 6, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            indexBuffer = new DynamicIndexBuffer(device.GraphicsDevice, typeof(ushort), 20, BufferUsage.WriteOnly);
            indexBuffer.SetData(0, Indices, 0, 12);

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
            newColor *= 0.2f;
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
                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 6, 0, 6);
            }
        }
    }
}

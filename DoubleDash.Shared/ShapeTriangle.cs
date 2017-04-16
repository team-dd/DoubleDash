using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    class ShapeTriangle
    {
        ushort[] Indices = new ushort[10];
        VertexPositionColor[] vertices;
        //Vector2 location;
        GraphicsDeviceManager graphics;
        VertexBuffer vertexBuffer;
        DynamicIndexBuffer indexBuffer;
        BasicEffect basicEffect;
        Matrix world;
        Matrix view;
        static Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);

        Vector3 pos;
        static Random rand = new Random();
        float zoomthing;

        public ShapeTriangle(GraphicsDeviceManager device, float zoomthing, Color color)
        {
            graphics = device;
            vertices = new VertexPositionColor[3];

            Indices[0] = (ushort)(0);
            Indices[1] = (ushort)(1);
            Indices[2] = (ushort)(1);
            Indices[3] = (ushort)(2);
            Indices[4] = (ushort)(2);
            Indices[5] = (ushort)(0);
            world = Matrix.CreateTranslation(0, 0, 0);
            view = Matrix.CreateLookAt(new Vector3(0, 0, zoomthing), new Vector3(0, 0, -2), new Vector3(0, 1, 0));
            this.zoomthing = zoomthing;

            float size = 0.1f;
            vertices[0] = new VertexPositionColor(new Vector3(-size, -size, 0f) + pos, Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(0, size, 0f) + pos, Color.Red);
            vertices[2] = new VertexPositionColor(new Vector3(size, -size, 0f) + pos, Color.Red);
            basicEffect = new BasicEffect(graphics.GraphicsDevice);

            vertexBuffer = new VertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            indexBuffer = new DynamicIndexBuffer(device.GraphicsDevice, typeof(ushort), 10, BufferUsage.WriteOnly);

            indexBuffer.SetData(0, Indices, 0, 6);
        }

        public void Update()
        {
            zoomthing -= .01f;

            if (zoomthing < 0.01)
            {
                zoomthing = 4f;
            }

            view = Matrix.CreateLookAt(new Vector3(0, 0, zoomthing), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        }

        public void UpdateColor(Color newColor)
        {
            vertices[0].Color = newColor;
            vertices[1].Color = newColor;
            vertices[2].Color = newColor;
            vertexBuffer.SetData<VertexPositionColor>(vertices);
        }

        public void Draw()
        {
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.VertexColorEnabled = true;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;
            graphics.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            graphics.GraphicsDevice.Indices = indexBuffer;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 3, 0, 3);
            }
        }
    }
}

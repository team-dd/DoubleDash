using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoubleDash
{
    class ShapeTwenty
    {
        ushort[] Indices = new ushort[50];
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

        public ShapeTwenty(GraphicsDeviceManager device, float zoomthing, Color color)
        {
            graphics = device;
            vertices = new VertexPositionColor[20];

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
            Indices[11] = (ushort)(6);
            Indices[12] = (ushort)(6);
            Indices[13] = (ushort)(7);
            Indices[14] = (ushort)(7);
            Indices[15] = (ushort)(8);
            Indices[16] = (ushort)(8);
            Indices[17] = (ushort)(9);
            Indices[18] = (ushort)(9);
            Indices[19] = (ushort)(10);
            Indices[20] = (ushort)(10);
            Indices[21] = (ushort)(11);
            Indices[22] = (ushort)(11);
            Indices[23] = (ushort)(12);
            Indices[24] = (ushort)(12);
            Indices[25] = (ushort)(13);
            Indices[26] = (ushort)(13);
            Indices[27] = (ushort)(14);
            Indices[28] = (ushort)(14);
            Indices[29] = (ushort)(15);
            Indices[30] = (ushort)(15);
            Indices[31] = (ushort)(16);
            Indices[32] = (ushort)(16);
            Indices[33] = (ushort)(17);
            Indices[34] = (ushort)(17);
            Indices[35] = (ushort)(18);
            Indices[36] = (ushort)(18);
            Indices[37] = (ushort)(19);
            Indices[38] = (ushort)(19);
            Indices[39] = (ushort)(0);
            world = Matrix.CreateTranslation(0, 0, 0);
            view = Matrix.CreateLookAt(new Vector3(0, 0, (zoomthing % 4f)), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            this.zoomthing = zoomthing;
            float size = 0.25f;
            double angle = MathHelper.TwoPi / 20;

            for (int i = 0; i < 20; i++)
            {
                Vector3 position = new Vector3(size * (float)Math.Round(Math.Sin(angle * i*2), 4), size * (float)Math.Round(Math.Cos(angle * i*2), 4), 0f);
                vertices[i] = new VertexPositionColor(position, Color.Red * .25f);
            }

            basicEffect = new BasicEffect(graphics.GraphicsDevice);

            vertexBuffer = new VertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColor), 20, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            indexBuffer = new DynamicIndexBuffer(device.GraphicsDevice, typeof(ushort), 50, BufferUsage.WriteOnly);
            indexBuffer.SetData(0, Indices, 0, 40);

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
                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 20, 0, 20);
            }
        }
    }
}

using System;
using OpenTK.Graphics.OpenGL4;

namespace Gonzo3d
{
    public class ShadowMap
    {
        public int ShadowMapFbo { get; private set; }
        public int DepthMap { get; private set; }

        public const int ShadowMapWidth = 4096;
        public const int ShadowMapHeight = 4096;

        public ShadowMap()
        {
            ShadowMapFbo = GL.GenFramebuffer();
            DepthMap = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, DepthMap);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent,
                ShadowMapWidth, ShadowMapHeight, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToBorder);
            float[] borderColor = { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ShadowMapFbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D, DepthMap, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
        }
    }
}
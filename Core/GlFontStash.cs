using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class GLFONScontext
    {
        public uint tex;
        public int width, height;
    }
    public class GlFontStash
    {
        public static FONScontext glfonsCreate(int width, int height, FONSflags flags)
        {
            FONSparams fparams;
            GLFONScontext gl = new GLFONScontext();

            fparams.width = width;
            fparams.height = height;
            fparams.flags = flags;
            fparams.renderCreate = glfons__renderCreate;
            fparams.renderResize = glfons__renderResize;
            fparams.renderUpdate = glfons__renderUpdate;
            fparams.renderDraw = glfons__renderDraw;
            fparams.renderDelete = glfons__renderDelete;
            fparams.userPtr = gl;

            return FontStash.fonsCreateInternal(ref fparams);
        }



        private static int _vao;
        private static int _vbo;

        public static int glfons__renderCreate(object userPtr, int width, int height)
        {
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);
            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

            GLFONScontext gl = userPtr as GLFONScontext;

            // Create may be called multiple times, delete existing texture.
            if (gl.tex != 0)
            {
                GL.DeleteTextures(1, ref gl.tex);
                gl.tex = 0;
            }
            GL.GenTextures(1, out gl.tex);
            if (!(gl.tex != 0))
                return 0;
            gl.width = width;
            gl.height = height;
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, gl.tex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                gl.width, gl.height, 0, PixelFormat.Red, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            return 1;
        }
        public static int glfons__renderResize(object uptr, int width, int height)
        {
            // Reuse create to resize too.
            return glfons__renderCreate(uptr, width, height);
        }
        public static void glfons__renderUpdate(object uptr, ref int[] rect, byte[] data)
        {
            GLFONScontext gl = (GLFONScontext)uptr;
            int w = rect[2] - rect[0];
            int h = rect[3] - rect[1];

            if (gl.tex == 0)
                return;
            //GL.PushClientAttrib(ClientAttribMask.ClientPixelStoreBit);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, gl.tex);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.PixelStore(PixelStoreParameter.UnpackRowLength, gl.width);
            GL.PixelStore(PixelStoreParameter.UnpackSkipPixels, rect[0]);
            GL.PixelStore(PixelStoreParameter.UnpackSkipRows, rect[1]);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, rect[0], rect[1], w, h,
                PixelFormat.Red, PixelType.UnsignedByte, data);

            //GL.PopClientAttrib();
        }
        public static void glfons__renderDraw(object userPtr, float[] verts, int nverts)
        {
            GLFONScontext gl = (GLFONScontext)userPtr;
            if (gl.tex == 0)
                return;

            GL.BindVertexArray(_vao);


            GL.BindTexture(TextureTarget.Texture2D, gl.tex);
            GL.Enable(EnableCap.Texture2D);


            GL.BufferData(BufferTarget.ArrayBuffer, nverts * 8 * sizeof(float), verts, BufferUsageHint.StaticDraw);
            var posLocation = 0;
            GL.EnableVertexAttribArray(posLocation);
            GL.VertexAttribPointer(posLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            var texCoordLocation = 1;
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 2 * sizeof(float));

            var colorLocation = 2;
            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 4, VertexAttribPointerType.Float, false, 8 * sizeof(float), 4 * sizeof(float));



            //GL.EnableClientState(ArrayCap.VertexArray);
            //GL.EnableClientState(ArrayCap.TextureCoordArray);
            //GL.EnableClientState(ArrayCap.ColorArray);

            ////GL.VertexPointer(2, VertexPointerType.Float, sizeof(float)*2, verts);
            ////GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeof(float)*2, tcoords);
            ////GL.ColorPointer(4, ColorPointerType.UnsignedByte, sizeof(uint), colors);

            //GL.VertexPointer(2, VertexPointerType.Float, 0, verts);
            //GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, tcoords);
            //GL.ColorPointer(4, ColorPointerType.UnsignedByte, 0, colors);

            GL.DrawArrays(PrimitiveType.Triangles, 0, nverts);

            //GL.Disable(EnableCap.Texture2D);
            //GL.DisableClientState(ArrayCap.VertexArray);
            //GL.DisableClientState(ArrayCap.TextureCoordArray);
            //GL.DisableClientState(ArrayCap.ColorArray);
        }
        public static void glfons__renderDelete(object userPtr)
        {
            GLFONScontext gl = (GLFONScontext)userPtr;
            if (gl.tex != 0)
                GL.DeleteTextures(1, ref gl.tex);
            gl.tex = 0;
            //free(gl);
        }
        public static uint glfonsRGBA(byte r, byte g, byte b, byte a)
        {
            return (uint)((r) | (g << 8) | (b << 16) | (a << 24));
        }
    }
}

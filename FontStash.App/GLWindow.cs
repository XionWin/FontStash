using Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Drawing;

namespace App
{

    class GLWindow : GameWindow
    {
        FONScontext fs;
        int fontNormal;
        int fontItalic;
        int fontBold;
        int fontJapanese;

        public Shader Shader { get; init; }
        private int _uniformViewPort;
        public GLWindow(string title, int width, int height) :
        base(
            new GameWindowSettings()
            {
                UpdateFrequency = 30,
                RenderFrequency = 120
            },
            new NativeWindowSettings()
            {
                Title = title,
                Size = new Vector2i(width, height),
                WindowBorder = WindowBorder.Fixed,
                API = ContextAPI.OpenGL,
                APIVersion = new Version(4, 5)
            }
        )
        {
            this.CenterWindow();
            this.Title = this.Title + $" | {this.API} {this.APIVersion.Major}.{this.APIVersion.Minor}";
            this.Shader = new Shader($"Shaders/FontStash.vert", $"Shaders/FontStash.frag");
        }

        #region Keyboard_KeyDown
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape)
                this.Close();

            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.F12)
                if (this.WindowState == WindowState.Fullscreen)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Fullscreen;
        }

        #endregion

        #region OnLoad

        /// <summary>
        /// Setup OpenGL and load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad()
        {
            GL.ClearColor(Color.MidnightBlue);

            fontNormal = FontStash.FONS_INVALID;

            fs = GlFontStash.glfonsCreate(512, 512, FONSflags.FONS_ZERO_TOPLEFT);

            fontNormal = FontStash.fonsAddFont(fs, "sans", "Resources/DroidSerif-Regular.ttf");
            if (fontNormal == FontStash.FONS_INVALID)
            {
                throw new Exception("Could not add font normal.\n");
            }

            this.Shader.Use();
            this._uniformViewPort = GL.GetUniformLocation(this.Shader.ProgramHandle, "aViewport");

        }

        #endregion

        #region OnResize

        /// <summary>
        /// Respond to resize events here.
        /// </summary>
        /// <param name="e">Contains information on the new GameWindow size.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        /// 
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            GL.Uniform3(this._uniformViewPort, this.Size.X, this.Size.Y, 1.0f);
        }

        #endregion

        #region OnUpdateFrame

        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Nothing to do!
        }

        #endregion

        #region OnRenderFrame

        /// <summary>
        /// Add your game rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            float sx, sy, dx, dy, lh = 0, pf1 = 0, pf2 = 0;
            //int width, height;
            uint white, black, brown, blue;
            //glfwGetFramebufferSize(window, &width, &height);
            // Update and render
            GL.Viewport(0, 0, this.Size.X, this.Size.Y);
            GL.ClearColor(0.3f, 0.3f, 0.32f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            var posLocation = GL.GetAttribLocation(Shader.ProgramHandle, "aPos");
            var texCoordLocation = GL.GetAttribLocation(Shader.ProgramHandle, "aTexCoord");

            GL.ActiveTexture(TextureUnit.Texture0);
            this.Shader.Uniform1("texture0", 0);
            //GL.EnableVertexAttribArray(posLocation);
            //GL.VertexAttribPointer(posLocation, attribLocation.Length, VertexAttribPointerType.Float, false, totalLen * sizeof(float), attribLocation.Start * sizeof(float));

            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.Disable(EnableCap.Texture2D);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(0, Width, Height, 0, -1, 1);

            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadIdentity();
            GL.Disable(EnableCap.DepthTest);
            //GL.Color4(255f, 255f, 255f, 255f);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.CullFace);

            white = GlFontStash.glfonsRGBA(255, 255, 255, 255);
            brown = GlFontStash.glfonsRGBA(192, 128, 0, 128);
            blue = GlFontStash.glfonsRGBA(0, 192, 255, 255);
            black = GlFontStash.glfonsRGBA(0, 0, 0, 255);

            sx = 50;
            sy = 50;

            dx = sx;
            dy = sy;


            FontStash.fonsClearState(ref fs);

            FontStash.fonsSetSize(ref fs, 124.0f);
            FontStash.fonsSetFont(ref fs, fontNormal);
            pf1 = 0;
            pf2 = 0;
            FontStash.fonsVertMetrics(ref fs, ref pf1, ref pf2, ref lh);
            dx = sx;
            dy += lh;

            FontStash.fonsSetSize(ref fs, 124.0f);
            FontStash.fonsSetFont(ref fs, fontNormal);
            FontStash.fonsSetColor(ref fs, white);
            dx = FontStash.fonsDrawText(ref fs, dx, dy, "AAA DEF");

            dx = sx;
            dy += lh;

            FontStash.fonsSetSize(ref fs, 124.0f);
            FontStash.fonsSetFont(ref fs, fontNormal);
            FontStash.fonsSetColor(ref fs, white);
            dx = FontStash.fonsDrawText(ref fs, dx, dy, "AAA DWS");

            //if (debug)
            FontStash.fonsDrawDebug(fs, 800.0f, 50.0f);


            GL.Enable(EnableCap.DepthTest);

            this.SwapBuffers();
        }

        #endregion


    }
}

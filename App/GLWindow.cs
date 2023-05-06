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

            fontItalic = FontStash.fonsAddFont(fs, "sans-italic", "Resources/DroidSerif-Italic.ttf");
            if (fontItalic == FontStash.FONS_INVALID)
            {
                throw new Exception("Could not add font italic.\n");
            }
            fontBold = FontStash.fonsAddFont(fs, "sans-bold", "Resources/DroidSerif-Bold.ttf");
            if (fontBold == FontStash.FONS_INVALID)
            {
                throw new Exception("Could not add font bold.\n");
            }
            fontJapanese = FontStash.fonsAddFont(fs, "sans-jp", "Resources/DroidSansJapanese.ttf");
            if (fontJapanese == FontStash.FONS_INVALID)
            {
                throw new Exception("Could not add font japanese.\n");
            }

            this.Shader.Use();
            this._uniformViewPort = GL.GetUniformLocation(this.Shader.ProgramHandle, "aViewport");

            GlFontStash.Locations.Add("aPos", GL.GetAttribLocation(Shader.ProgramHandle, "aPos"));
            GlFontStash.Locations.Add("aTexCoord", GL.GetAttribLocation(Shader.ProgramHandle, "aTexCoord"));
            GlFontStash.Locations.Add("aColor", GL.GetAttribLocation(Shader.ProgramHandle, "aColor"));

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
            brown = GlFontStash.glfonsRGBA(192, 128, 0, 255);
            blue = GlFontStash.glfonsRGBA(0, 192, 255, 255);
            black = GlFontStash.glfonsRGBA(0, 0, 0, 255);

            sx = 50;
            sy = 50;

            dx = sx;
            dy = sy;

            //dash(dx, dy);

            FontStash.fonsClearState(ref fs);

            FontStash.fonsSetSize(ref fs, 124.0f);
            FontStash.fonsSetFont(ref fs, fontNormal);
            pf1 = 0;
            pf2 = 0;
            FontStash.fonsVertMetrics(ref fs, ref pf1, ref pf2, ref lh);
            dx = sx;
            dy += lh;
            //dash(dx, dy);

            //FontStash.fonsSetSize(ref fs, 124.0f);
            //FontStash.fonsSetFont(ref fs, fontNormal);
            //FontStash.fonsSetColor(ref fs, white);
            //dx = FontStash.fonsDrawText(ref fs, dx, dy, "The quick ");

            //FontStash.fonsSetSize(ref fs, 48.0f);
            //FontStash.fonsSetFont(ref fs, fontItalic);
            //FontStash.fonsSetColor(ref fs, brown);
            //dx = FontStash.fonsDrawText(ref fs, dx, dy, "brown ");

            //FontStash.fonsSetSize(ref fs, 24.0f);
            //FontStash.fonsSetFont(ref fs, fontNormal);
            //FontStash.fonsSetColor(ref fs, white);
            //dx = FontStash.fonsDrawText(ref fs, dx, dy, "fox ");

            //FontStash.fonsVertMetrics(ref fs, ref pf1, ref pf2, ref lh);
            //dx = sx;
            //dy += lh * 1.2f;
            ////dash(dx, dy);
            //FontStash.fonsSetFont(ref fs, fontItalic);
            //dx = FontStash.fonsDrawText(ref fs, dx, dy, "jumps over ");
            //FontStash.fonsSetFont(ref fs, fontBold);
            //dx = FontStash.fonsDrawText(ref fs, dx, dy, "the lazy ");
            //FontStash.fonsSetFont(ref fs, fontNormal);
            //dx = FontStash.fonsDrawText(ref fs, dx, dy, "dog.");

            //dx = sx;
            //dy += lh * 1.2f;
            ////dash(dx, dy);
            //FontStash.fonsSetSize(ref fs, 12.0f);
            //FontStash.fonsSetFont(ref fs, fontNormal);
            //FontStash.fonsSetColor(ref fs, blue);
            //FontStash.fonsDrawText(ref fs, dx, dy, "Now is the time for all good men to come to the aid of the party.");

            //FontStash.fonsVertMetrics(ref fs, ref pf1, ref pf2, ref lh);
            //dx = sx;
            //dy += lh * 1.2f * 2;
            ////dash(dx, dy);
            //FontStash.fonsSetSize(ref fs, 18.0f);
            //FontStash.fonsSetFont(ref fs, fontItalic);
            //FontStash.fonsSetColor(ref fs, white);
            //FontStash.fonsDrawText(ref fs, dx, dy, "Ég get etið gler án þess að meiða mig.");

            //FontStash.fonsVertMetrics(ref fs, ref pf1, ref pf2, ref lh);
            //dx = sx;
            //dy += lh * 1.2f;
            ////dash(dx, dy);
            //FontStash.fonsSetFont(ref fs, fontJapanese);
            //FontStash.fonsDrawText(ref fs, dx, dy, "私はガラスを食べられます。それは私を傷つけません。");

            //// Font alignment
            //FontStash.fonsSetSize(ref fs, 18.0f);
            //FontStash.fonsSetFont(ref fs, fontNormal);
            //FontStash.fonsSetColor(ref fs, white);

            //dx = 50;
            //dy = 350;
            ////line(dx - 10, dy, dx + 250, dy);
            //FontStash.fonsSetAlign(fs, FONSalign.FONS_ALIGN_LEFT | FONSalign.FONS_ALIGN_TOP);
            //dx = FontStash.fonsDrawText(ref fs, dx, dy, "Top");
            //dx += 10;
            //FontStash.fonsSetAlign(fs, FONSalign.FONS_ALIGN_LEFT | FONSalign.FONS_ALIGN_MIDDLE);
            //dx = FontStash.fonsDrawText(ref fs, dx, dy, "Middle");
            //dx += 10;
            //FontStash.fonsSetAlign(fs, FONSalign.FONS_ALIGN_LEFT | FONSalign.FONS_ALIGN_BASELINE);
            //dx = FontStash.fonsDrawText(ref fs, dx, dy, "Baseline");
            //dx += 10;
            //FontStash.fonsSetAlign(fs, FONSalign.FONS_ALIGN_LEFT | FONSalign.FONS_ALIGN_BOTTOM);
            //FontStash.fonsDrawText(ref fs, dx, dy, "Bottom");

            //dx = 150;
            //dy = 400;
            ////line(dx, dy - 30, dx, dy + 80.0f);
            //FontStash.fonsSetAlign(fs, FONSalign.FONS_ALIGN_LEFT | FONSalign.FONS_ALIGN_BASELINE);
            //FontStash.fonsDrawText(ref fs, dx, dy, "Left");
            //dy += 30;
            //FontStash.fonsSetAlign(fs, FONSalign.FONS_ALIGN_CENTER | FONSalign.FONS_ALIGN_BASELINE);
            //FontStash.fonsDrawText(ref fs, dx, dy, "Center");
            //dy += 30;
            //FontStash.fonsSetAlign(fs, FONSalign.FONS_ALIGN_RIGHT | FONSalign.FONS_ALIGN_BASELINE);
            //FontStash.fonsDrawText(ref fs, dx, dy, "Right");

            //// Blur
            //dx = 500;
            //dy = 350;
            //FontStash.fonsSetAlign(fs, FONSalign.FONS_ALIGN_LEFT | FONSalign.FONS_ALIGN_BASELINE);

            //FontStash.fonsSetSize(ref fs, 60.0f);
            //FontStash.fonsSetFont(ref fs, fontItalic);
            //FontStash.fonsSetColor(ref fs, white);
            //FontStash.fonsSetSpacing(ref fs, 5.0f);
            //FontStash.fonsSetBlur(ref fs, 10.0f);
            //FontStash.fonsDrawText(ref fs, dx, dy, "Blurry...");

            //dy += 50.0f;

            //FontStash.fonsSetSize(ref fs, 18.0f);
            //FontStash.fonsSetFont(ref fs, fontBold);
            //FontStash.fonsSetColor(ref fs, black);
            //FontStash.fonsSetSpacing(ref fs, 0.0f);
            //FontStash.fonsSetBlur(ref fs, 3.0f);
            //FontStash.fonsDrawText(ref fs, dx, dy + 2, "DROP THAT SHADOW");

            //FontStash.fonsSetColor(ref fs, white);
            //FontStash.fonsSetBlur(ref fs, 0);
            //FontStash.fonsDrawText(ref fs, dx, dy, "DROP THAT SHADOW");



            //FontStash.fonsSetSize(ref fs, 124.0f);
            //FontStash.fonsSetFont(ref fs, fontNormal);
            //FontStash.fonsSetColor(ref fs, white);
            //dx = FontStash.fonsDrawText(ref fs, dx, dy, "ABCDEFGHIGKLMN");


            FontStash.fonsSetSize(ref fs, 22.0f);
            FontStash.fonsVertMetrics(ref fs, ref pf1, ref pf2, ref lh);
            dx = sx;
            dy += lh * 1.2f;
            //dash(dx, dy);
            FontStash.fonsSetFont(ref fs, fontItalic);
            FontStash.fonsSetColor(ref fs, brown);
            dx = FontStash.fonsDrawText(ref fs, dx, dy, "jumps");



            //if (debug)
            FontStash.fonsDrawDebug(fs, 720.0f - 512.0f, 50.0f);

            if (isInited is false)
            {
                FontStash.AddWhiteRect(ref fs, 512, 20);
                isInited = true;
            }


            GL.Enable(EnableCap.DepthTest);

            this.SwapBuffers();
        }
        public static bool isInited = false;
        #endregion


    }
}

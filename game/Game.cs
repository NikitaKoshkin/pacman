using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Input;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Pacman;
using Pacman.Graphics;
using Pacman.Objects;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Reflection;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using OpenTK.Compute.OpenCL;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace openTK_game
{

    internal class Game : GameWindow
    {


        // create a pacman

        Block blocks;
        List<Vector3> pos;

        public Ghost proj;

        List<List<Vector3>> eda_pos;
        List<List<bool>> was;

        Map gamemap;
        // coins
        Sphere eda;
      
        bool wb = true;
        // create a pacman
        public Sphere user;
        public Ghost enemy;

        // position light source
        //private readonly Vector3 _lightPos = new Vector3(0, -0.05f, -0.05f);
        private readonly Vector3 _lightPos = new Vector3(0, -0.05f, -2.0f);

        ShaderProgram light;
        ShaderProgram program;
        
        // camera
        Camera camera;

        float user_x, user_y;
        public Vector2 steps;

        // width and height of screen
        int width, height;

        // Constructor that sets the width, height, and calls the base constructor (GameWindow's Constructor) with default args
        public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.width = width;
            this.height = height;
            //center the window on monitor
            CenterWindow(new Vector2i(width, height));
        }

        // called whenever window is resized
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
        }

        // called once when game is started
        protected override void OnLoad()
        {
            base.OnLoad();
            blocks = new Block(new Vector3(0, 0, 0), "pomade.jpg");

            pos = new List<Vector3>();
            eda_pos = new List<List<Vector3>>();
            was = new List<List<bool>>();
            gamemap = new Map("../../../Textures/map.bmp");

            for (int i = 0; i < 28; ++i)
            {
                eda_pos.Add(new List<Vector3>());
                was.Add(new List<bool>());
                for (int j = 0; j < 31; ++j)
                {
                    was[i].Add(true);
                    if (gamemap.map[i, j] == 1)
                        pos.Add(new Vector3((28 - i) * 0.048f, (31 - j) * 0.048f, 0));

                    if (gamemap.map[i, j] == 2)
                    {
                        eda_pos[i].Add(new Vector3((28 - i) * 0.048f, (31 - j) * 0.048f, 0));
                    }
                    else
                    {
                        eda_pos[i].Add(new Vector3(-1, -1, -1));
                    }
                }
            }



            eda = new Sphere(new Vector3(0, 0, 0), "coin.jpg");
            user = new Sphere(new Vector3(13 * 0.048f, 14 * 0.048f, 0), "pac.jpg");
            enemy = new Ghost(new Vector3(12 * 0.048f, 16 * 0.048f, 0), "ghost.jpg");



            user_x = 0; user_y = 0;

            
            //program = new ShaderProgram("Default.vert", "Default.frag");
            program = new ShaderProgram("Default.vert", "d2.frag");

            light = new ShaderProgram("light.vert", "lighting.frag");
            proj = new Ghost(new Vector3(0, 6*0.048f, 4*0.048f), "ghost.jpg");
            GL.Enable(EnableCap.DepthTest);
            camera = new Camera(width, height, new Vector3(0.048f * 14, 0.048f * 15.5f, 2.5f));
            CursorState = CursorState.Grabbed;
        }

        // called once when game is closed
        protected override void OnUnload()
        {
            base.OnUnload();
            blocks.Delete();
            proj.Delete();
            enemy.Delete();
            user.Delete();
            eda.Delete();
            program.Delete();
            light.Delete();


            // Delete, VAO, VBO, Shader Program
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            // Set the color to fill the screen with
            GL.ClearColor(0.3f, 0.3f, 1f, 1f);
            // Fill the screen with the color
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            {
                //GL.Uniform3(GL.GetUniformLocation(program.ID, "viewPos"), camera.position);

                //GL.Uniform3(GL.GetUniformLocation(program.ID, "light.ambient"), new Vector3(0.2f, 0.2f, 0.2f));
                //GL.Uniform3(GL.GetUniformLocation(program.ID, "light.diffuse"), new Vector3(0.5f, 0.5f, 0.5f));
                //GL.Uniform3(GL.GetUniformLocation(program.ID, "light.specular"), new Vector3(0.5f, 0.5f, 0.5f));
                //GL.Uniform3(GL.GetUniformLocation(program.ID, "material.specular"), new Vector3(1.0f, 1.0f, 1.0f));
                //GL.Uniform1(GL.GetUniformLocation(program.ID, "material.shininess"), 32.0f);

                //GL.Uniform3(GL.GetUniformLocation(program.ID, "light.direction"), _lightPos);
                //GL.Uniform1(GL.GetUniformLocation(program.ID, "light.cutOff"), (float)Math.Cos(MathHelper.DegreesToRadians(12.5)));

                //GL.Uniform3(GL.GetUniformLocation(program.ID, "material.ambient"), new Vector3(0.2f));
                //GL.Uniform3(GL.GetUniformLocation(program.ID, "material.diffuse"), new Vector3(0.5f));
                //GL.Uniform3(GL.GetUniformLocation(program.ID, "material.specular"), new Vector3(1.0f));
            }
            {
                GL.Uniform3(GL.GetUniformLocation(program.ID, "viewPos"), camera.position);

                GL.Uniform3(GL.GetUniformLocation(program.ID, "light.ambient"), new Vector3(0.2f, 0.2f, 0.2f));
                GL.Uniform3(GL.GetUniformLocation(program.ID, "light.diffuse"), new Vector3(0.5f, 0.5f, 0.5f));
                GL.Uniform3(GL.GetUniformLocation(program.ID, "light.specular"), new Vector3(0.5f, 0.5f, 0.5f));
                GL.Uniform1(GL.GetUniformLocation(program.ID, "material.shininess"), 32.0f);

                GL.Uniform3(GL.GetUniformLocation(program.ID, "light.position"), _lightPos);

                GL.Uniform3(GL.GetUniformLocation(program.ID, "material.ambient"), new Vector3(0.2f));
                GL.Uniform3(GL.GetUniformLocation(program.ID, "material.diffuse"), new Vector3(0.5f));
                GL.Uniform3(GL.GetUniformLocation(program.ID, "material.specular"), new Vector3(1.0f));
            }


            if (gamemap.coins == 0)
            {
                Console.WriteLine("You Win!");
                this.Close();
            }

            // transformation matrices
            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();

            int modelLocation = GL.GetUniformLocation(program.ID, "model");
            int viewLocation = GL.GetUniformLocation(program.ID, "view");
            int projectionLocation = GL.GetUniformLocation(program.ID, "projection");

            //GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);
            for (int i = 0; i < 28; ++i)
            {
                for (int j = 0; j < 31; ++j)
                {
                    model = Matrix4.Identity;
                    model *= Matrix4.CreateTranslation((28 - i) * 0.048f, (31 - j) * 0.048f, 0);
                    GL.UniformMatrix4(modelLocation, true, ref model);

                    if (gamemap.map[i, j] == 1)
                        blocks.Render(program);
                    if (gamemap.map[i, j] == 2)
                    {
                        if (was[i][j] == true)
                            eda.Render(program);
                    }

                }
            }
            hit_eda();

            proj.Render(light);
            Matrix4 lampMatrix = Matrix4.CreateScale(1.0f);
            //lampMatrix = lampMatrix * Matrix4.CreateTranslation(_lightPos);
            Matrix4 view2 = camera.GetViewMatrix();
            Matrix4 projection2 = camera.GetProjectionMatrix();

            GL.UniformMatrix4(GL.GetUniformLocation(light.ID, "model"), true, ref lampMatrix);
            GL.UniformMatrix4(GL.GetUniformLocation(light.ID, "view"), true, ref view2);
            GL.UniformMatrix4(GL.GetUniformLocation(light.ID, "projection"), true, ref projection2);


            model = Matrix4.Identity;
            view = camera.GetViewMatrix();
            projection = camera.GetProjectionMatrix();

            enemy.Render(program);

            model *= Matrix4.CreateTranslation(user_x, user_y, 0);

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            user.Render(program);

            update();


            // swap the buffers
            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
        // called every frame. All updating happens here
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            MouseState mouse = MouseState;
            KeyboardState input = KeyboardState;


            base.OnUpdateFrame(args);
            camera.Update(input, mouse, args);
        }

        protected void hit_eda()
        {

            for (int i = 0; i < 28; ++i)
            {
                for (int j = 0; j < 31; ++j)
                {
                    if (gamemap.map[i, j] == 2 && was[i][j])
                    {
                        if (Math.Abs(eda_pos[i][j].X - user.position.X - user_x) < 0.047f &&
                           (Math.Abs(eda_pos[i][j].Y - user.position.Y - user_y) < 0.047f))
                        {
                            was[i][j] = false;
                            gamemap.coins--;
                        }
                    }
                }
            }
        }
        protected void update()
        {
            KeyboardState input = KeyboardState;

            // can play
            if (input.IsKeyDown(Keys.J))
            {
                wb = !wb;
            }
            if (!wb) return;


            float step = 0.0005f;
            if (input.IsKeyDown(Keys.Up))
            {
                user_y += step;
                if (is_wall())
                {
                    user_y -= step;
                }
            }
            if (input.IsKeyDown(Keys.Down))
            {
                user_y -= step;
                if (is_wall())
                {
                    user_y += step;
                }

            }
            if (input.IsKeyDown(Keys.Left))
            {
                user_x -= step;
                if (is_wall())
                {
                    user_x += step;
                }
            }
            if (input.IsKeyDown(Keys.Right))
            {
                user_x += step;
                if (is_wall())
                {
                    user_x -= step;
                }
            }


        }

        bool is_wall()
        {
            foreach (Vector3 cur_pos in pos)
            {
                //Console.WriteLine(cur_pos.Y);
                if (Math.Abs(cur_pos.X - user.position.X - user_x) < 0.047f &&
                (Math.Abs(cur_pos.Y - user.position.Y - user_y) < 0.047f))
                {
                    return true;
                }
            }
            return false;
        }


    }
}
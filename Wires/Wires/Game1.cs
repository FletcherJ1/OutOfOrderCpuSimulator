using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Wires.Placement;

namespace Wires
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D OnePx;

        private Grid PGrid;
        private PlaceAnnealer Annealer;

        private int CellWidth = 16;
        private int CellHeight = 16;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            OnePx = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            OnePx.SetData<Color>(new Color[1] { Color.White });

            PGrid = new Grid(40, 40);

            Gate CoutGate = null;

            int iPlace = 0;

            // Let's make an 8 bit adder.
            for (int i = 0;i < 32;i++)
            {
                // Graph of a basic adder
                ConstGate c0 = new ConstGate(false); // A
                ConstGate c1 = new ConstGate(false); // B
                //c0.Moveable = c1.Moveable = false;
                Gate cin;
                if (CoutGate == null)
                {
                    cin = new ConstGate(false); // Cin
                    //cin.Moveable = false;
                }
                else
                {
                    cin = CoutGate;
                }
                // Cout = G8 output
                Gate g0 = new NandGate();
                Gate g1 = new NandGate();
                Gate g2 = new NandGate();
                Gate g3 = new NandGate();
                Gate g4 = new NandGate();
                Gate g5 = new NandGate();
                Gate g6 = new NandGate();
                Gate g7 = new NandGate();
                Gate g8 = new NandGate();
                g0.AddInputs(c0, g1);
                g1.AddInputs(c0, c1);
                g2.AddInputs(g1, c1);
                g3.AddInputs(g2, g0);
                g4.AddInputs(g3, cin);
                g5.AddInputs(cin, g4);
                g6.AddInputs(g4, g3);
                g7.AddInputs(g5, g6);
                g8.AddInputs(g4, g1);

                if (CoutGate == null)
                {
                    //PGrid.PlaceGateAt(cin, iPlace, 0);
                    PGrid.PlaceGates(cin);
                    iPlace += 2;
                }
                //PGrid.PlaceGateAt(c0, iPlace, 0); iPlace += 2;
                //PGrid.PlaceGateAt(c1, iPlace, 0); iPlace += 2;
                PGrid.PlaceGates(c0, c1, g0, g1, g2, g3, g4, g5, g6, g7, g8);
                CoutGate = g8;
            }

            Annealer = new PlaceAnnealer(PGrid);

            //Debug.WriteLine("A={0}, B={1}, Cin={2}", c0.GetValue(), c1.GetValue(), c2.GetValue());
            //Debug.WriteLine("S: " + g7.Evaluate());
            //Debug.WriteLine("Cout: " + g8.Evaluate());

            _graphics.PreferredBackBufferWidth = PGrid.GetWidth() * CellWidth;
            _graphics.PreferredBackBufferHeight = PGrid.GetHeight() * CellHeight;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            for (int i = 0; i < 500; i++)
                this.Annealer.AnnealStep();

            _spriteBatch.Begin();
            for (int y = 0; y < PGrid.GetHeight(); y++)
            {
                for (int x = 0; x < PGrid.GetWidth(); x++)
                {
                    Gate v = PGrid.GetCell(x, y);
                    Color pc = v != null ? v.C : Color.Black;
                    int fx = x * CellWidth;
                    int fy = y * CellHeight;
                    _spriteBatch.Draw(OnePx, new Rectangle(fx, fy, CellWidth, CellHeight), pc);
                }
            }

            // Draw wires. Can be optimized through a list just containing gates.
            for (int y = 0; y < PGrid.GetHeight(); y++)
            {
                for (int x = 0; x < PGrid.GetWidth(); x++)
                {
                    Gate v = PGrid.GetCell(x, y);
                    int fx = x * CellWidth;
                    int fy = y * CellHeight;
                    
                    // Draw lines.
                    if (v != null)
                    {
                        foreach (Gate g in v.GetInputs())
                        {
                            int gPos = PGrid.FindGate(g);
                            int ex = gPos % PGrid.GetWidth();
                            int ey = gPos / PGrid.GetHeight();
                            DrawLine(_spriteBatch, new Vector2(fx + 0.5f * CellWidth, fy + 0.5f * CellHeight), new Vector2(ex * CellWidth + 0.5f * CellWidth, ey * CellHeight + 0.5f * CellHeight));
                        }
                    }
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);

            sb.Draw(OnePx,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                Color.White, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }
    }
}

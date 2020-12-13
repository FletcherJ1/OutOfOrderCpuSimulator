using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace SimulatedAnnealingExample
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D OnePx;

        private AtomGrid G;
        private SimulatedAnnealing SA;

        private int CellWidth = 16;
        private int CellHeight = 16;

        private Color[] ColorMap = { Color.Green, Color.Red, Color.Blue, Color.Black, Color.White};

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

            // NOTE: Too large a grid takes a long time per frame.
            G = new AtomGrid(14, 4);
            SA = new SimulatedAnnealing(G);
            //SA.Anneal();

            _graphics.PreferredBackBufferWidth = G.GetSize() * CellWidth;
            _graphics.PreferredBackBufferHeight = G.GetSize() * CellHeight;
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

            _spriteBatch.Begin();
            for (int y = 0; y < G.GetSize(); y++)
            {
                for (int x = 0; x < G.GetSize(); x++)
                {
                    int v = G.GetAtom(x, y);
                    Color pc = ColorMap[v];
                    _spriteBatch.Draw(OnePx, new Rectangle(x * CellWidth, y * CellHeight, CellWidth, CellHeight), pc);
                }
            }
            _spriteBatch.End();

            {
                // Do some simulated annealing.
                // Putting this in Update method ruins the animation due to large computation time.
                for (int i = 0; i < G.GetSize() * G.GetSize() * 10; i++)
                {
                    SA.AnnealStep();
                }

                // Slowly reduce temperature but don't allow it to go too low.
                if (SA.T > 0.1)
                    SA.T *= 0.99f;
            }
            

            base.Draw(gameTime);
        }
    }
}

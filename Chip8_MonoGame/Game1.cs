using Chip8_Library;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace Chip8_MonoGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Chip8 _chip8 = new();
        private const int ScreenHeight = 320;
        private const int ScreenWidth = 640;
        private bool[,] _screenFrame;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.GraphicsDevice.PresentationParameters.BackBufferHeight = ScreenHeight;
            _graphics.GraphicsDevice.PresentationParameters.BackBufferWidth = ScreenWidth;

            byte[] file = File.ReadAllBytes("C:/CODE/Test.ch8");
            _chip8.Load(file);

            _texture = new Texture2D(GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.DarkSlateGray });


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _chip8.Run();
            _screenFrame = _chip8.Screen;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            for(int y = 0; y < _screenFrame.GetLength(1); y++)
            {
                for(int x = 0; x < _screenFrame.GetLength(0); x++)
                {
                    if (_screenFrame[x, y] == true)
                    {
                        _spriteBatch.Draw(_texture, new Rectangle(x * 10, y * 10, 10, 10), Color.White);
                    }
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

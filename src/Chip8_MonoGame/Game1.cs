using Chip8_Library;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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
        private string _path;

        public Game1(string path)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _path = path;
        }

        protected override void Initialize()
        {
            _graphics.GraphicsDevice.PresentationParameters.BackBufferHeight = ScreenHeight;
            _graphics.GraphicsDevice.PresentationParameters.BackBufferWidth = ScreenWidth;

            byte[] file = File.ReadAllBytes(_path);
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
            
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.GetPressedKeyCount() > 0)
            {
                Keys[] keys = keyboardState.GetPressedKeys();
                int keyValue = Convert.ToInt32(keys[0]);
                if (keyValue >= 65 && keyValue < (65 + 16))
                {
                    byte keyPressed = (byte)(keyValue - 65);
                    _chip8.SetKey(keyPressed);
                }
            }
            _chip8.Run();
            _screenFrame = _chip8.Screen;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            for (int y = 0; y < _screenFrame.GetLength(1); y++)
            {
                for (int x = 0; x < _screenFrame.GetLength(0); x++)
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

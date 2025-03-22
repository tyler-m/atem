using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Atem.Views.MonoGame
{
    internal class FileSelector
    {
        private List<FileInfo> _files = new List<FileInfo>();
        private string _currentDirectory;
        private int _selected = 0;
        private int _start = 0;

        private Config _config;
        private SpriteFont _font;
        private Texture2D _highlightTexture;

        public bool Active { get; set; }

        public FileSelector(string directory, SpriteFont font, Texture2D highlightTexture, Config config)
        {
            _currentDirectory = directory;
            _font = font;
            _config = config;
            _highlightTexture = highlightTexture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 position = Vector2.Zero;

            int height = (int)(14 * _config.ScreenSizeFactor);
            for (int i = 0; i < 16 && _start + i < _files.Count; i++)
            {
                if (_start + i == _selected)
                {
                    spriteBatch.Draw(_highlightTexture, new Rectangle(0, i * height, (int)(_config.ScreenWidth * _config.ScreenSizeFactor), height), Color.DarkSlateBlue);
                }
                else
                {
                    spriteBatch.Draw(_highlightTexture, new Rectangle(0, i * height, (int)(_config.ScreenWidth * _config.ScreenSizeFactor), height), Color.Black);
                }
                spriteBatch.DrawString(_font, _files[_start + i].Name, position, Color.White, 0.0f, Vector2.Zero, 0.5f * _config.ScreenSizeFactor, SpriteEffects.None, 0);
                position.Y += height;
            }
        }

        public void Update()
        {
            _files.Clear();

            foreach(FileInfo item in new DirectoryInfo(_currentDirectory).GetFiles())
            {
                if (item.Extension.ToLower() == ".gbc")
                {
                    _files.Add(item);
                }
            }
        }

        internal void MoveCursor(int offset)
        {
            _selected = System.Math.Clamp(_selected + offset, 0, _files.Count - 1);

            if (_selected > _start + 15)
            {
                _start = _selected - 15;
            }
            else if (_selected < _start)
            {
                _start = _selected;
            }
        }

        public string SelectedFile
        {
            get
            {
                if (_selected < 0 || _selected >= _files.Count)
                {
                    return string.Empty;
                }

                return _files[_selected].FullName;
            }
        }
    }
}

// This file contains modified sample code from the ImGui.NET project,
// please see THIRD-PARTY-LICENSES for the relevant copyright notice.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ImGuiNET;

namespace Atem.Views.MonoGame
{
    public class ImGuiRenderer
    {
        private Game _game;

        private GraphicsDevice _graphicsDevice;
        private BasicEffect _effect;
        private RasterizerState _rasterizerState;

        private byte[] _vertexData;
        private VertexBuffer _vertexBuffer;
        private int _vertexBufferSize;
        private int _vertexDeclarationSize;
        private byte[] _indexData;
        private IndexBuffer _indexBuffer;
        private int _indexBufferSize;
        private Dictionary<IntPtr, Texture2D> _loadedTextures;
        private int _textureId;

        private int _verticalScrollWheelValue;
        private int _horizontalScrollWheelValue;
        private readonly float SCROLL_WHEEL_FACTOR = 0.01f;
        private Keys[] _allKeys = Enum.GetValues<Keys>();

        public ImGuiRenderer(Game game)
        {
            _game = game;

            unsafe
            {
                _vertexDeclarationSize = sizeof(ImDrawVert);
            }

            ImGui.SetCurrentContext(ImGui.CreateContext());

            game.Window.TextInput += (s, a) =>
            {
                if (a.Character == '\t') return;
                ImGui.GetIO().AddInputCharacter(a.Character);
            };

            _graphicsDevice = game.GraphicsDevice;
            _effect = new BasicEffect(_graphicsDevice);

            _loadedTextures = new Dictionary<IntPtr, Texture2D>();

            _rasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                DepthBias = 0,
                FillMode = FillMode.Solid,
                MultiSampleAntiAlias = false,
                ScissorTestEnable = true,
                SlopeScaleDepthBias = 0
            };

            PrepareFont();
        }

        private void PrepareFont()
        {
            ImGuiIOPtr io = ImGui.GetIO();

            unsafe
            {
                io.Fonts.GetTexDataAsRGBA32(out byte* textureData, out int width, out int height, out int bytesPerPixel);

                // move texture data to managed array
                byte[] managedTextureData = new byte[width * height * bytesPerPixel];
                Marshal.Copy(new IntPtr(textureData), managedTextureData, 0, managedTextureData.Length);

                Texture2D fontTexture = new Texture2D(_graphicsDevice, width, height, false, SurfaceFormat.Color);
                fontTexture.SetData(managedTextureData);

                io.Fonts.SetTexID(BindTexture(fontTexture));
            }

            io.Fonts.ClearTexData();
        }

        public IntPtr BindTexture(Texture2D texture)
        {
            nint id = new IntPtr(_textureId++);
            _loadedTextures.Add(id, texture);
            return id;
        }

        public bool UnbindTexture(IntPtr textureId)
        {
            return _loadedTextures.Remove(textureId);
        }

        public void BeginDraw(GameTime gameTime)
        {
            ImGui.GetIO().DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_game.IsActive)
            {
                UpdateInput();
            }

            ImGui.NewFrame();
        }

        public void EndDraw()
        {
            ImGui.Render();

            unsafe
            {
                RenderDrawData(ImGui.GetDrawData());
            }
        }

        private void UpdateInput()
        {
            ImGuiIOPtr io = ImGui.GetIO();

            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            io.AddMousePosEvent(mouse.X, mouse.Y);
            io.AddMouseButtonEvent(0, mouse.LeftButton == ButtonState.Pressed);
            io.AddMouseButtonEvent(1, mouse.RightButton == ButtonState.Pressed);
            io.AddMouseButtonEvent(2, mouse.MiddleButton == ButtonState.Pressed);
            io.AddMouseButtonEvent(3, mouse.XButton1 == ButtonState.Pressed);
            io.AddMouseButtonEvent(4, mouse.XButton2 == ButtonState.Pressed);
            io.AddMouseWheelEvent((mouse.HorizontalScrollWheelValue - _horizontalScrollWheelValue) * SCROLL_WHEEL_FACTOR, (mouse.ScrollWheelValue - _verticalScrollWheelValue) * SCROLL_WHEEL_FACTOR);

            _verticalScrollWheelValue = mouse.ScrollWheelValue;
            _horizontalScrollWheelValue = mouse.HorizontalScrollWheelValue;

            foreach (Keys key in _allKeys)
            {
                if (TryMapKey(key, out ImGuiKey imGuiKey))
                {
                    io.AddKeyEvent(imGuiKey, keyboard.IsKeyDown(key));
                }
            }

            io.DisplaySize = new System.Numerics.Vector2(_graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(1f, 1f);
        }

        private bool TryMapKey(Keys key, out ImGuiKey imGuiKey)
        {
            if (key == Keys.None)
            {
                imGuiKey = ImGuiKey.None;
                return true;
            }

            imGuiKey = key switch
            {
                Keys.Back => ImGuiKey.Backspace,
                Keys.Tab => ImGuiKey.Tab,
                Keys.Enter => ImGuiKey.Enter,
                Keys.CapsLock => ImGuiKey.CapsLock,
                Keys.Escape => ImGuiKey.Escape,
                Keys.Space => ImGuiKey.Space,
                Keys.PageUp => ImGuiKey.PageUp,
                Keys.PageDown => ImGuiKey.PageDown,
                Keys.End => ImGuiKey.End,
                Keys.Home => ImGuiKey.Home,
                Keys.Left => ImGuiKey.LeftArrow,
                Keys.Right => ImGuiKey.RightArrow,
                Keys.Up => ImGuiKey.UpArrow,
                Keys.Down => ImGuiKey.DownArrow,
                Keys.PrintScreen => ImGuiKey.PrintScreen,
                Keys.Insert => ImGuiKey.Insert,
                Keys.Delete => ImGuiKey.Delete,
                >= Keys.D0 and <= Keys.D9 => ImGuiKey._0 + (key - Keys.D0),
                >= Keys.A and <= Keys.Z => ImGuiKey.A + (key - Keys.A),
                >= Keys.NumPad0 and <= Keys.NumPad9 => ImGuiKey.Keypad0 + (key - Keys.NumPad0),
                Keys.Multiply => ImGuiKey.KeypadMultiply,
                Keys.Add => ImGuiKey.KeypadAdd,
                Keys.Subtract => ImGuiKey.KeypadSubtract,
                Keys.Decimal => ImGuiKey.KeypadDecimal,
                Keys.Divide => ImGuiKey.KeypadDivide,
                >= Keys.F1 and <= Keys.F24 => ImGuiKey.F1 + (key - Keys.F1),
                Keys.NumLock => ImGuiKey.NumLock,
                Keys.Scroll => ImGuiKey.ScrollLock,
                Keys.LeftShift => ImGuiKey.ModShift,
                Keys.LeftControl => ImGuiKey.ModCtrl,
                Keys.LeftAlt => ImGuiKey.ModAlt,
                Keys.OemSemicolon => ImGuiKey.Semicolon,
                Keys.OemPlus => ImGuiKey.Equal,
                Keys.OemComma => ImGuiKey.Comma,
                Keys.OemMinus => ImGuiKey.Minus,
                Keys.OemPeriod => ImGuiKey.Period,
                Keys.OemQuestion => ImGuiKey.Slash,
                Keys.OemTilde => ImGuiKey.GraveAccent,
                Keys.OemOpenBrackets => ImGuiKey.LeftBracket,
                Keys.OemCloseBrackets => ImGuiKey.RightBracket,
                Keys.OemPipe => ImGuiKey.Backslash,
                Keys.OemQuotes => ImGuiKey.Apostrophe,
                Keys.BrowserBack => ImGuiKey.AppBack,
                Keys.BrowserForward => ImGuiKey.AppForward,
                _ => ImGuiKey.None,
            };

            return imGuiKey != ImGuiKey.None;
        }

        private void RenderDrawData(ImDrawDataPtr drawData)
        {
            Viewport lastViewport = _graphicsDevice.Viewport;
            Rectangle lastScissorRectangle = _graphicsDevice.ScissorRectangle;
            RasterizerState lastRasterizerState = _graphicsDevice.RasterizerState;
            DepthStencilState lastDepthStencilState = _graphicsDevice.DepthStencilState;
            Color lastBlendFactor = _graphicsDevice.BlendFactor;
            BlendState lastBlendState = _graphicsDevice.BlendState;

            _graphicsDevice.BlendFactor = Color.White;
            _graphicsDevice.BlendState = BlendState.NonPremultiplied;
            _graphicsDevice.RasterizerState = _rasterizerState;
            _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            // handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);

            _graphicsDevice.Viewport = new Viewport(0, 0, _graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight);

            UpdateBuffers(drawData);
            RenderCommandLists(drawData);

            _graphicsDevice.Viewport = lastViewport;
            _graphicsDevice.ScissorRectangle = lastScissorRectangle;
            _graphicsDevice.RasterizerState = lastRasterizerState;
            _graphicsDevice.DepthStencilState = lastDepthStencilState;
            _graphicsDevice.BlendState = lastBlendState;
            _graphicsDevice.BlendFactor = lastBlendFactor;
        }

        private void UpdateBuffers(ImDrawDataPtr drawData)
        {
            if (drawData.TotalVtxCount == 0)
            {
                return;
            }

            // expand buffers if we need more room
            if (drawData.TotalVtxCount > _vertexBufferSize)
            {
                _vertexBuffer?.Dispose();

                _vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5f);
                var vertexDeclaration = new VertexDeclaration(_vertexDeclarationSize,
                    new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                    new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                    new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0));

                _vertexBuffer = new VertexBuffer(_graphicsDevice, vertexDeclaration, _vertexBufferSize, BufferUsage.None);
                _vertexData = new byte[_vertexBufferSize * _vertexDeclarationSize];
            }

            if (drawData.TotalIdxCount > _indexBufferSize)
            {
                _indexBuffer?.Dispose();

                _indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
                _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, _indexBufferSize, BufferUsage.None);
                _indexData = new byte[_indexBufferSize * sizeof(ushort)];
            }

            // copy ImGui's vertices and indices to managed byte arrays
            int vertexOffset = 0;
            int indexOffset = 0;
            for (int i = 0; i < drawData.CmdListsCount; i++)
            {
                ImDrawListPtr cmdList = drawData.CmdLists[i];

                unsafe
                {
                    fixed (void* vertexDestinationPtr = &_vertexData[vertexOffset * _vertexDeclarationSize])
                    fixed (void* indexDestinationPtr = &_indexData[indexOffset * sizeof(ushort)])
                    {
                        Buffer.MemoryCopy((void*)cmdList.VtxBuffer.Data, vertexDestinationPtr, _vertexData.Length, cmdList.VtxBuffer.Size * _vertexDeclarationSize);
                        Buffer.MemoryCopy((void*)cmdList.IdxBuffer.Data, indexDestinationPtr, _indexData.Length, cmdList.IdxBuffer.Size * sizeof(ushort));
                    }
                }

                vertexOffset += cmdList.VtxBuffer.Size;
                indexOffset += cmdList.IdxBuffer.Size;
            }

            // copy managed byte arrays to GPU buffers
            _vertexBuffer.SetData(_vertexData, 0, drawData.TotalVtxCount * _vertexDeclarationSize);
            _indexBuffer.SetData(_indexData, 0, drawData.TotalIdxCount * sizeof(ushort));
        }

        private void RenderCommandLists(ImDrawDataPtr drawData)
        {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            int vertexOffset = 0;
            int indexOffset = 0;

            for (int commandListIndex = 0; commandListIndex < drawData.CmdListsCount; commandListIndex++)
            {
                ImDrawListPtr commandList = drawData.CmdLists[commandListIndex];

                for (int commandIndex = 0; commandIndex < commandList.CmdBuffer.Size; commandIndex++)
                {
                    ImDrawCmdPtr drawCommand = commandList.CmdBuffer[commandIndex];

                    if (drawCommand.ElemCount == 0)
                    {
                        continue;
                    }

                    if (!_loadedTextures.ContainsKey(drawCommand.TextureId))
                    {
                        throw new InvalidOperationException($"Could not find a texture with id '{drawCommand.TextureId}'.");
                    }

                    _graphicsDevice.ScissorRectangle = new Rectangle(
                        (int)drawCommand.ClipRect.X, (int)drawCommand.ClipRect.Y,
                        (int)(drawCommand.ClipRect.Z - drawCommand.ClipRect.X),
                        (int)(drawCommand.ClipRect.W - drawCommand.ClipRect.Y));

                    ImGuiIOPtr io = ImGui.GetIO();
                    _effect.World = Matrix.Identity;
                    _effect.View = Matrix.Identity;
                    _effect.Projection = Matrix.CreateOrthographicOffCenter(0f, io.DisplaySize.X, io.DisplaySize.Y, 0f, -1f, 1f);
                    _effect.TextureEnabled = true;
                    _effect.Texture = _loadedTextures[drawCommand.TextureId];
                    _effect.VertexColorEnabled = true;

                    foreach (EffectPass effectPass in _effect.CurrentTechnique.Passes)
                    {
                        effectPass.Apply();

                        _graphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            (int)drawCommand.VtxOffset + vertexOffset,
                            (int)drawCommand.IdxOffset + indexOffset,
                            (int)drawCommand.ElemCount / 3
                        );
                    }
                }

                vertexOffset += commandList.VtxBuffer.Size;
                indexOffset += commandList.IdxBuffer.Size;
            }
        }
    }
}
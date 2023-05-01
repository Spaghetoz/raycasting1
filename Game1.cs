using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace Raycasting1;

public class Game1 : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    private Texture2D whiteTexture;
    private Texture2D blackTexture;
    private Texture2D redTexture;
    private Texture2D limeTexture;
    private Texture2D greenTexture;
    private Texture2D shadedGreenTexture;
    private Texture2D chocolateTexture;

    private SpriteFont font;

    private int minimapScale = 25;
    private Map map;
    private Player player;

    MouseState mouseState;
    KeyboardState keyboardState;

    private int windowWidth = 960; 
    private int windowHeight = 680;
 
    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = windowWidth;
        graphics.PreferredBackBufferHeight = windowHeight;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        map = new Map(minimapScale);
        player = new Player(new Vector2(1.2f, 1.2f), minimapScale);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
        whiteTexture.SetData(new Color[] { Color.White });

        blackTexture = new Texture2D(GraphicsDevice, 1, 1);
        blackTexture.SetData(new Color[] { Color.Black });
        
        redTexture = new Texture2D(GraphicsDevice, 1, 1);
        redTexture.SetData(new Color[] { Color.Red });

        limeTexture = new Texture2D(GraphicsDevice, 1, 1);
        limeTexture.SetData(new Color[] { Color.Lime});

        greenTexture = new Texture2D(GraphicsDevice, 1, 1);
        greenTexture.SetData(new Color[] { new Color(45, 196, 71)});

        shadedGreenTexture = new Texture2D(GraphicsDevice, 1, 1);
        shadedGreenTexture.SetData(new Color[] { new Color(40, 156, 60)});

        chocolateTexture = new Texture2D(GraphicsDevice, 1, 1);
        chocolateTexture.SetData(new Color[] { Color.Chocolate});

        font = Content.Load<SpriteFont>("font");

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        keyboardState = Keyboard.GetState();
        mouseState = Mouse.GetState();
        
        player.HandleInput(keyboardState, mouseState);

        player.MoveHorizontally(map.Grid);
        player.MoveVertically(map.Grid);
        player.Update();

        player.CastRays(map.Grid);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Aquamarine);

        spriteBatch.Begin();

        player.DrawFloor(spriteBatch, chocolateTexture, windowWidth, windowHeight);
        player.DrawWalls(spriteBatch, shadedGreenTexture, greenTexture, windowWidth, windowHeight);

        map.DrawMiniMap(spriteBatch, whiteTexture, blackTexture);
        player.DrawOnMinimap(spriteBatch, redTexture);
        player.DrawRaysOnMinimap(spriteBatch, redTexture);
        
        // hud rendering
        
        spriteBatch.DrawString(font, $"q:{windowWidth/player.CastedRays.Length}", new Vector2(400, 600), Color.White);
        spriteBatch.DrawString(font, $"Player position x:{player.Position.Y} y:{player.Position.X}", new Vector2(400, 0), Color.White);
        //DrawLine(spriteBatch, limeTexture, player.Position*minimapScale, new Vector2(mouseState.X, mouseState.Y));

        spriteBatch.End();

        base.Draw(gameTime);
    }



}

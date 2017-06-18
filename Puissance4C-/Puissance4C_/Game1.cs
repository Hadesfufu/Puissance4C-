using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Puissance4C_
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ObjetPuissance4 cadre;
        ObjetPuissance4 pionjaune;
        ObjetPuissance4 pionrouge;

        int VX, VY;
        int currentSelectedColumn;
        int player1, player2, currentPlayer;


        int[,] map;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            currentSelectedColumn = 0;
            this.IsMouseVisible = true;
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 700;
            graphics.ApplyChanges();

            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 10.0f);

            VX = 7;
            VY = 6;

            player1 = 1;
            player2 = 2;
            currentPlayer = 1;

            map = new int[6, 7]{
            {0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0}
            };

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            cadre = new ObjetPuissance4(Content.Load<Texture2D>("cadre"), new Vector2(0f, 0f), new Vector2(100f, 100f));
            pionjaune = new ObjetPuissance4(Content.Load<Texture2D>("jaune"), new Vector2(0f, 0f), new Vector2(100f, 100f));
            pionrouge = new ObjetPuissance4(Content.Load<Texture2D>("rouge"), new Vector2(0f, 0f), new Vector2(100f, 100f));



            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Right))
            {
                if (isDirectionOk(1))
                {
                    currentSelectedColumn++;
                }
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                if (isDirectionOk(-1))
                {
                    currentSelectedColumn--;
                }
            }
            else if (keyboard.IsKeyDown(Keys.Down))
            {
                bool done = false;
                for (int x = 1; x < map.GetUpperBound(0) + 2 && !done; x++)
                {
                    if (x == map.GetUpperBound(0) + 1 || map[x, currentSelectedColumn] != 0)
                    {
                        map[x-1, currentSelectedColumn] = currentPlayer;
                        done = true;
                        Console.WriteLine(currentSelectedColumn);
                        Console.WriteLine(x);
                        Console.WriteLine(map.GetUpperBound(0));
                    }
                }

                currentPlayer = 3 - currentPlayer;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }


        private bool isDirectionOk(int i)
        {
            return (currentSelectedColumn + i >= 0 && currentSelectedColumn + i < VX);
        }

        public Boolean isGameWon()
        {
            for (int ligne = 1; ligne < VX; ligne++)
            {
                // Vérifie les horizontales ( - )
                if (cherche4alignes(0, ligne, 1, 0))
                {
                    return true;
                }
                // Première diagonale ( \ ) inférieur
                if (cherche4alignes(0, ligne, 1, 1))
                {
                    return true;
                }
                // Deuxième diagonale ( / ) inférieur
                if (cherche4alignes(VY - 1, ligne, -1, 1))
                {
                    return true;
                }
            }

            for (int col = 0; col < VY; col++)
            {
                // Vérifie les verticales ( ¦ )
                if (cherche4alignes(col, 1, 0, 1))
                {
                    return true;
                }
                // Première diagonale ( / ) supérieur
                if (cherche4alignes(col, 1, -1, 1))
                {
                    return true;
                }
                // Deuxième diagonale ( \ ) supérieur
                if (cherche4alignes(col, 1, 1, 1))
                {
                    return true;
                }
            }
            // On n'a rien trouvé
            return false;
        }

        private Boolean cherche4alignes(int oCol, int oLigne, int dCol, int dLigne)
        {
            int couleur = 0;
            int compteur = 0;

            int curCol = oCol;
            int curRow = oLigne;

            while ((curCol >= 0) && (curCol < VY) && (curRow >= 1) && (curRow < VX))
            {
                if (map[curRow, curCol] != couleur)
                {
                    // Si la couleur change, on réinitialise le compteur
                    couleur = map[curRow, curCol];
                    compteur = 1;
                }
                else
                {
                    // Sinon on l'incrémente
                    compteur++;
                }

                // On sort lorsque le compteur atteint 4
                if ((couleur != 0) && (compteur == 4))
                {
                    return true;
                }

                // On passe à l'itération suivante
                curCol += dCol;
                curRow += dLigne;
            }

            // Aucun alignement n'a été trouvé
            return false;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            // TODO: Add your drawing code here
            int offsetX = 0;
            int offsetY = 100;

            Vector2 pos = new Vector2(0, 0);
            for (int x = 0; x < map.GetUpperBound(0)+1; x++)
            {
                for (int y = 0; y < map.GetUpperBound(1)+1; y++)
                {
                    pos.X = offsetX + y * 100;
                    pos.Y = offsetY + x * 100;
                    if (map[x, y] == 0)
                    {
                        spriteBatch.Draw(cadre.Texture, pos, Color.White);
                    }
                    else if(map[x, y] == 1){
                        spriteBatch.Draw(pionjaune.Texture, pos, Color.White);
                    }
                    else{
                        spriteBatch.Draw(pionrouge.Texture, pos, Color.White);
                    }
                    
                }
            }
            pos.X = currentSelectedColumn * 100;
            pos.Y = 0;
            if(currentPlayer == 1)
                spriteBatch.Draw(pionjaune.Texture , pos, Color.White);
            else
                spriteBatch.Draw(pionrouge.Texture , pos, Color.White);

            spriteBatch.End();
            base.Draw(gameTime);

        }
    }
}

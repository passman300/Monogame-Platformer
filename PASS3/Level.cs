using Animation2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PASS3
{
    public class Level
    {
        // store the content manger class
        ContentManager content;

        // used to view hitboxes
        GraphicsDevice graphicsDevice;

        // store the tiles of an level in 2D array
        private Tile[,] tiles;

        // store the enemies of an level in a 2D array
        private Enemy[,] enemies;

        // store the current level
        public int levelNum;

        // store the player start location
        private Vector2 spawnPoint;

        // store the player
        private Player player;

        // property that gets level width and height in terms of tiles
        public int GetTileWidth
        {
            get { return tiles.GetLength(0); }
        }
        public int GetTileHeight
        {
            get { return tiles.GetLength(1); }
        }

        public int GetEnemyWidth
        {
            get { return enemies.GetLength(0); }
        }
        public int GetEnemyHeight
        {
            get { return enemies.GetLength(1); }
        }


        // create a new level
        public Level(ContentManager content, GraphicsDevice graphics, int levelNum, Vector2 spawnPoint, Player player)
        {
            // store the passed contentManger and graphicsDevice
            this.content = content;
            this.graphicsDevice = graphics;

            // store the passed through level number
            this.levelNum = levelNum;

            // store the level's spawnPoint
            this.spawnPoint = spawnPoint;

            // store the gloval player variable
            this.player = player;

            // change players spawn point to level's
            player.SetSpawnPoint = spawnPoint;

        }

        // load all tiles in a level
        public void LoadTiles(Stream filePath)
        {
            // create new file reader
            StreamReader reader = new StreamReader(filePath);

            // temporarily store the width of the level in tiles
            int width;

            // store the input files lines
            List<string> lines = new List<string>();

            string line = reader.ReadLine();

            // add all lines of the text file into the line list
            width = line.Length;

            // add the line, until there is no lines remaining (null)
            while (line != null)
            {
                lines.Add(line);
                line = reader.ReadLine();
            }

            // Allocate the tile grid.
            tiles = new Tile[lines.Count, width];

            // Loop over every tile position,
            for (int y = 0; y < tiles.GetLength(0);  y++)
            {
                for (int x = 0; x < tiles.GetLength(1); x++)
                { 
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[y, x] = LoadTileManger(tileType, x, y);
                }
            }
            reader.Close();
        }

        // load a tile appearance and behavior given its type and cell value
        private Tile LoadTileManger(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // floor/ground tile
                case '#':
                    return LoadTile("Floor", TileCollision.Impassable, new Vector2(x, y));

                case '.':
                    return LoadTile("Blank", TileCollision.Passable, new Vector2(x, y));

                case 'A':
                    return LoadTile("Floor", TileCollision.Impassable, new Vector2(x, y));

                // default is tileType (' ')
                default:
                    return null;
            }
        }

        // building block to load a tile
        // other load tile methods will call this after computing other logic
        // tileCell is the row, and column the tile is located on the level txt
        private Tile LoadTile(string tileName, TileCollision collision, Vector2 tileCell)
        {
            // temporarily create tile arrays (stores location, images)
            Vector2 tilePos = Vector2.Zero;
            tilePos.X = tileCell.X * Tile.Size.X;
            tilePos.Y = tileCell.Y * Tile.Size.Y;

            // create each tile
            return new Tile(content.Load<Texture2D>("Images/Tiles/" + tileName), collision, tilePos);
        }


        // building block to load 
        private void LoadEnemies(Stream filePath)
        {
            // create a file reader
            StreamReader reader = new StreamReader(filePath);

            // temp width variable
            int width;

            // store the input files lines
            List<string> lines = new List<string>();

            string line = reader.ReadLine();

            // add all lines of the text file into the line list
            width = line.Length;

            // add the line, until there is no lines remaining (null)
            while (line != null)
            {
                lines.Add(line);
                line = reader.ReadLine();
            }

            // Allocate the tile grid.
            enemies = new Enemy[lines.Count, width];

            // Loop over every tile position,
            for (int y = 0; y < enemies.GetLength(0); y++)
            {
                for (int x = 0; x < enemies.GetLength(1); x++)
                {
                    // to load each tile.
                    char enemyType = lines[y][x];
                    enemies[y, x] = LoadEnemyManger(enemyType, x, y);
                    
                    if (enemies[y, x] != null)
                    {
                        ((Runner)(enemies[y, x])).LoadRunner();
                    }
                }
            }

            // close reader
            reader.Close();
        }

        // enemy manger
        private Enemy LoadEnemyManger(char enemyType, int x, int y)
        {
            // load diffrent types of enemies
            switch (enemyType)
            {
                case 'R':
                    return LoadRunner(new Vector2(x * Tile.Size.X, y * Tile.Size.Y), Enemy.FaceDirection.Right);
                case 'r':
                    return LoadRunner(new Vector2(x * Tile.Size.Length(), y * Tile.Size.Length()), Enemy.FaceDirection.Left);

                default:
                    return null;
            }
        }

        // building block of runner enemy
        private Runner LoadRunner(Vector2 loc, Enemy.FaceDirection dir)
        {
            float maxHealth = 100;
            Vector2 spd = new Vector2(2, 0);

            // create each enemy
            return new Runner(content, graphicsDevice, Enemy.EnemeyState.Walk, loc, dir, maxHealth, spd);
        }

        public void LoadContent()
        {
            LoadTiles(TitleContainer.OpenStream("Content/Levels/" + levelNum + "/Tiles" + ".txt"));

            LoadEnemies(TitleContainer.OpenStream("Content/Levels/" + levelNum + "/Enemies" + ".txt"));
        }

        // updates the level and player
        public void UpdateLevel(GameTime gameTime)
        {
            player.Update(gameTime, tiles);

            UpdateEnemies(gameTime);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            for (int y = 0; y < GetEnemyHeight; y++)
            {
                for (int x = 0; x < GetEnemyWidth; x++)
                {
                    if (enemies[x, y] != null)
                    {
                        ((Runner)(enemies[x, y])).Update(gameTime, tiles);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            DrawTiles(spriteBatch);

            DrawEnemies(spriteBatch);

            spriteBatch.End();
        }

        // draws the tiles of the level
        public void DrawTiles(SpriteBatch spriteBatch)
        {
            // Loop over every tile position,
            for (int y = 0; y < GetTileHeight; y++)
            {
                for (int x = 0; x < GetTileWidth; x++)
                {
                    tiles[x, y].DrawTile(spriteBatch);
                }
            }
        }

        // draw the enemies of the level
        public void DrawEnemies(SpriteBatch spriteBatch)
        {
            // Loop over every tile position,
            for (int y = 0; y < GetEnemyHeight; y++)
            {
                for (int x = 0; x < GetEnemyWidth; x++)
                {
                    if (enemies[x, y] != null)
                    {
                        enemies[x, y].Draw(spriteBatch);
                    }
                }
            }
        }

    }
}

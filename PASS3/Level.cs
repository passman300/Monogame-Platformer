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

        // store all active fireball in the level
        private Dictionary<(int x, int y), List<FireBall>> lvlFireBalls = new Dictionary<(int x, int y), List<FireBall>>();

        // store the current level
        public int levelNum;

        // store the player start location
        private Vector2 spawnPoint;

        // store the player
        private Player player;

        // flag if player reaches next level;
        private bool isNextLvl = false;

        // flag if the player is dead
        private bool isPlayerDead = false;


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
        public Level(ContentManager content, GraphicsDevice graphics, int levelNum, Player player)
        {
            // store the passed contentManger and graphicsDevice
            this.content = content;
            this.graphicsDevice = graphics;

            // store the passed through level number
            this.levelNum = levelNum;

            // store the gloval player variable
            this.player = player;

        }

        // load all tiles in a level
        private void LoadTiles(Stream filePath)
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
            tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1);  y++)
                { 
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTileManger(tileType, x, y);
                }
            }
            reader.Close();
        }

        // load a tile appearance and behavior given its type and cell value
        private Tile LoadTileManger(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // player spawn point
                case '!':
                    Vector2 spawn = new Vector2(x * Tile.Size.X, y * Tile.Size.Y);

                    player.SetSpawnPoint = spawn;
                    return LoadTile("Blank", TileCollision.Passable, new Vector2(x, y));

                // floor/ground tile
                case '#':
                    return LoadTile("Floor", TileCollision.Impassable, new Vector2(x, y));

                case '.':
                    return LoadTile("Blank", TileCollision.Passable, new Vector2(x, y));

                case '1':
                    return LoadTile("Plat1", TileCollision.Platform, new Vector2(x, y));
                case '2':
                    return LoadTile("Plat2", TileCollision.Platform, new Vector2(x, y));
                case '3':
                    return LoadTile("Plat3", TileCollision.Platform, new Vector2(x, y));
                case '4':
                    return LoadTile("Plat4", TileCollision.Impassable, new Vector2(x, y));
                case '5':
                    return LoadTile("Plat5", TileCollision.Impassable, new Vector2(x, y));
                case '6':
                    return LoadTile("Plat6", TileCollision.Impassable, new Vector2(x, y));
                case 'E':
                    return LoadTile("ExitArrow", TileCollision.Exit, new Vector2(x, y));

                case '^':
                    return LoadTile("Spike", TileCollision.Spike, new Vector2(x, y));
                // default is tileType (' ')
                default:
                    return LoadTile("Blank", TileCollision.Passable, new Vector2(x, y));
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
            enemies = new Enemy[width, lines.Count];

            // Loop over every tile position,
            for (int x = 0; x < enemies.GetLength(0); x++)
            {
                for (int y = 0; y < enemies.GetLength(1); y++)
                {
                    // to load each tile.
                    char enemyType = lines[y][x];

                    enemies[x, y] = LoadEnemyManger(enemyType, x, y);

                    if (enemies[x, y] != null)
                    {
                        if ((enemies[x, y]) is Runner)
                        {
                            ((Runner)(enemies[x, y])).LoadRunner();
                        }
                        else if ((enemies[x, y]) is Goblin)
                        {
                            ((Goblin)enemies[x, y]).LoadGoblin();
                        }

                        // also load an blank tile where the enemy was spawned at
                        tiles[x, y] = LoadTileManger('.', x, y);
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
                    return CreateRunner(new Vector2(x * Tile.Size.X, y * Tile.Size.Y), Enemy.FaceDirection.Right);
                case 'r':
                    return CreateRunner(new Vector2(x * Tile.Size.X, y * Tile.Size.Y), Enemy.FaceDirection.Left);
                case 'D':
                    return CreateGoblin(new Vector2(x * Tile.Size.X, y * Tile.Size.Y), Enemy.FaceDirection.Right);
                case 'd':
                    return CreateGoblin(new Vector2(x * Tile.Size.X, y * Tile.Size.Y), Enemy.FaceDirection.Left);
                default:
                    return null;
            }
        }

        // building block of runner enemy
        private Runner CreateRunner(Vector2 loc, Enemy.FaceDirection dir)
        {
            float maxHealth = 100;
            Vector2 spd = new Vector2(2, 0);

            // create each enemy
            return new Runner(content, graphicsDevice, Enemy.EnemyState.Active, loc, dir, maxHealth, spd);
        }

        // building block for goblin enemy
        private Goblin CreateGoblin(Vector2 loc, Enemy.FaceDirection dir)
        {
            float maxHealth = 60;
            Vector2 spd = new Vector2(0, 0); // they don't move

            lvlFireBalls[((int)(loc.X / Tile.Size.Y), (int)(loc.Y / Tile.Size.Y))] = new List<FireBall>();

            // create each enemy
            return new Goblin(content, graphicsDevice, Enemy.EnemyState.Active, loc, dir, maxHealth, spd);
        }

        // load all content in the level
        public void LoadContent()
        {
            LoadTiles(TitleContainer.OpenStream("Content/Levels/" + levelNum + "/Tiles" + ".txt"));

            LoadEnemies(TitleContainer.OpenStream("Content/Levels/" + levelNum + "/Enemies" + ".txt"));
        }

        // updates the level and player
        public void UpdateLevel(GameTime gameTime)
        {
            UpdateEnemies(gameTime);

            player.Update(gameTime, tiles, enemies, lvlFireBalls);

            // check if player reach next level
            if (player.PlayerNextLvl())
            {
                isNextLvl = true;
            }
            if (player.IsDead())
            {
                isPlayerDead = true;
            }
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            for (int y = 0; y < GetEnemyHeight; y++)
            {
                for (int x = 0; x < GetEnemyWidth; x++)
                {
                    if (enemies[x, y] != null)
                    {
                        // tempuary damage variable
                        int enemyReward = 0;

                        if ((enemies[x, y]) is Runner)
                        {
                            ((Runner)(enemies[x, y])).Update(gameTime, tiles);

                            enemyReward = ((Runner)(enemies[x, y])).GetReward();
                        }
                        else if ((enemies[x, y]) is Goblin)
                        {
                            ((Goblin)(enemies[x, y])).Update(gameTime, tiles);

                            // hash the goblin ID by cords, and save their lvlFireBalls
                            lvlFireBalls[(x, y)] = ((Goblin)enemies[x, y]).GetFireBalls;

                            enemyReward = ((Goblin)(enemies[x, y])).GetReward();
                        }


                        if (enemies[x, y].IsDead && enemies[x, y].IsReward)
                        {
                            player.Score = player.Score + enemyReward;

                            enemies[x, y].IsReward = true;

                            //Console.WriteLine(player.Score);
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawTiles(spriteBatch);

            DrawEnemies(spriteBatch);
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
                        if (enemies[x, y] is Runner)
                        {
                            ((Runner)enemies[x, y]).Draw(spriteBatch);
                        }
                        else if (enemies[x, y] is Goblin)
                        {
                            ((Goblin)enemies[x, y]).Draw(spriteBatch);
                        }
                    }
                }
            }
        }

        // tells the game1 to move onto next lvl
        public bool NextLvl()
        {
            return isNextLvl;
        }

        // tells gameone to return to game over screen
        public bool IsPlayerDead()
        {
            return isPlayerDead;
        }

    }
}

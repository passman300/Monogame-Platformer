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

        // store the tiles of an level in 2D array
        private Tile[,] tiles;

        // store the current level
        public int levelNum;

        // store the player start location
        private Vector2 spawnPoint;

        // store the player
        private Player player;

        // property that gets level width in terms of tiles
        public int GetTileWidth
        {
            get { return tiles.GetLength(0); }
        }

        // property that gets level height in terms of tiles
        public int GetTileHeight
        {
            get { return tiles.GetLength(1); }
        }


        // create a new level
        public Level(ContentManager content, int levelNum, Vector2 spawnPoint, Player player)
        {
            // store the passed contentManger
            this.content = content;

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

        // updates the level and player
        public void UpdateLevel(GameTime gameTime)
        {
            player.Update(gameTime, tiles);
        }

        // draws the tiles of the level
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            // Loop over every tile position,
            for (int y = 0; y < GetTileHeight; y++)
            {
                for (int x = 0; x < GetTileWidth; x++)
                {
                    tiles[x, y].DrawTile(spriteBatch);
                }
            }
            spriteBatch.End();
        }
    }
}

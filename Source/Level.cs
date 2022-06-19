using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace computing_project
{
    class Level
    {

        private int _firstGid; // Each tile is given an id, this stores the first id in a tile map.
        private string _fileName; // The file name of the image.
        private Texture2D _tileset;  // The image containing the tiles.
        private List<Tile> _tileList; // The list of tiles for the background (the player walks in front of).
        private List<Tile> _tileListForeground; // The list of tiles in the foreground (the player walks behind of).
        private List<Collider> _tileColliders; // The list of colliders for the level
        private List<Warp> _warps; // The list of warps
        private List<Entity> _enemies; // The list of enemies
        private List<Key> _keys;
        private List<Door> _doors;
        private List<int> _gids;

        private Player _player;
        private PersistenceHandler _persistance;

        public Level()
        {
            _player = new Player(120, 150);
            _persistance = new PersistenceHandler();
        }

        private void LoadEntities(Game game)
        {

            // This function is called by LoadLevel, it does not matter that the player is loaded twice between rooms as the object itself is not destroyed therefore the texture stays loaded.
            _player.Load(game);

            // Load all enemies. This has to be called again when the player leaves a room as the textures are unloaded.
            for (int i = 0; i < _enemies.Count(); i++)
            {
                _enemies.ElementAt(i).Load(game);
            }

        }

        public void LoadLevel(Game game, string mapFileName)
        {

            // Initialise the tileset.
            _tileList = new List<Tile>();
            _tileListForeground = new List<Tile>();

            // Initialise colliders, warps and enemies.
            _tileColliders = new List<Collider>();
            _warps = new List<Warp>();
            _enemies = new List<Entity>();
            _keys = new List<Key>();
            _doors = new List<Door>();

            // Parse the .tmx file.
            XmlDocument map = new XmlDocument();
            map.Load("Maps/" + mapFileName + ".tmx");

            // Get the map node.
            XmlNode mapNode = map.DocumentElement.SelectSingleNode("/map");

            // Get the width and height of the whole map.
            int mapWidth, mapHeight;
            mapWidth = Int32.Parse(mapNode.Attributes["width"]?.InnerText);
            mapHeight = Int32.Parse(mapNode.Attributes["height"]?.InnerText);

            // Get the width and height of the tiles.
            int tileWidth, tileHeight;
            tileWidth = Int32.Parse(mapNode.Attributes["tilewidth"]?.InnerText);
            tileHeight = Int32.Parse(mapNode.Attributes["tileheight"]?.InnerText);

            // Loading the tileset.
            XmlNode tilesetNode = map.DocumentElement.SelectSingleNode("/map/tileset");

            // Get the first gid.
            _firstGid = Int32.Parse(tilesetNode.Attributes["firstgid"]?.InnerText);

            // Get the file name (takes the filename without path or extension, simply the name).
            XmlNode imageNode = map.DocumentElement.SelectSingleNode("/map/tileset/image");
            _fileName = Path.GetFileNameWithoutExtension(imageNode.Attributes["source"]?.InnerText);

            // Load the image file.
            _tileset = game.Content.Load<Texture2D>(_fileName);

            // Loading the layer data.
            XmlNodeList layerNodes = map.DocumentElement.SelectNodes("/map/layer");
            foreach (XmlNode layerNode in layerNodes)
            {
                // If the name of the layer is 'foreground' then foreground is true.
                bool foreground = false || layerNode.Attributes["name"]?.InnerText == "foreground";

                // Get the CSV data and store it as a string.
                XmlNode dataNode = layerNode.SelectSingleNode("data");
                string csvData = dataNode.InnerText;

                // Load the layer data into an array (CSV is comma delimited).
                _gids = csvData.Split(',').Select(Int32.Parse).ToList();

                // Loop through the array.
                for (int j = 0; j < _gids.Count; j++)
                {
                    // Skip blank tiles (they will have a gid of 0).
                    if (_gids.ElementAt(j) != 0)
                    {

                        // Get the position of the current tile in the level.
                        int xx = 0;
                        int yy = 0;
                        xx = j % mapWidth; // For example j = 5 (sixth tile), 5 % 16 = 5
                        xx *= tileWidth; // Then, 5 * 16 = 80
                        yy += tileHeight * (j / mapWidth); // So j = 32 (third tile down). (32 / 16) * 16 = 32
                        Vector2 finalTilePosition = new Vector2(xx, yy);

                        // Get the position of the tile in the tileset.
                        Vector2 finalTilesetPosition = GetTilesetPosition(_gids, j, tileWidth, tileHeight);

                        // Make the tile object.
                        Tile tile = new Tile(_fileName, tileWidth, tileHeight, (int)finalTilesetPosition.X, (int)finalTilesetPosition.Y, (int)finalTilePosition.X, (int)finalTilePosition.Y);

                        // Load the tile and add it to the tile list.
                        tile.Load(game);
                        if (foreground)
                        {
                            _tileListForeground.Add(tile);
                        }
                        else
                        {
                            _tileList.Add(tile);
                        }

                    }
                }
            }

            // Loading the object group data.
            XmlNodeList objectGroupNodes = map.DocumentElement.SelectNodes("/map/objectgroup");
            foreach (XmlNode objectGroupNode in objectGroupNodes)
            {
                XmlNodeList objectNodes = objectGroupNode.SelectNodes("object");

                // Parsing the collision shapes.
                if (objectGroupNode.Attributes["name"]?.InnerText == "collisions")
                {

                    // Load each collision shape into the _tileColliders list.
                    foreach (XmlNode objectNode in objectNodes)
                    {
                        int x, y, width, height;
                        x = Int32.Parse(objectNode.Attributes["x"]?.InnerText);
                        y = Int32.Parse(objectNode.Attributes["y"]?.InnerText);
                        width = Int32.Parse(objectNode.Attributes["width"]?.InnerText);
                        height = Int32.Parse(objectNode.Attributes["height"]?.InnerText);
                        _tileColliders.Add(new Collider(new Rectangle(x, y, width, height)));
                    }
                }

                // Parsing the warps.
                if (objectGroupNode.Attributes["name"]?.InnerText == "warps")
                {
                    foreach (XmlNode objectNode in objectNodes)
                    {
                        int x, y, width, height;
                        string name, type;
                        x = Int32.Parse(objectNode.Attributes["x"]?.InnerText);
                        y = Int32.Parse(objectNode.Attributes["y"]?.InnerText);
                        width = Int32.Parse(objectNode.Attributes["width"]?.InnerText);
                        height = Int32.Parse(objectNode.Attributes["height"]?.InnerText);
                        name = objectNode.Attributes["name"]?.InnerText;
                        type = objectNode.Attributes["type"]?.InnerText;
                        _warps.Add(new Warp(name, type, new Collider(new Rectangle(x, y, width, height)), new Vector2(x, y)));
                    }

                }

                // Parsing the enemy spawn locations
                if (objectGroupNode.Attributes["name"]?.InnerText == "enemies")
                {
                    foreach (XmlNode objectNode in objectNodes)
                    {
                        string enemy = objectNode.Attributes["name"]?.InnerText;
                        int x = Int32.Parse(objectNode.Attributes["x"]?.InnerText);
                        int y = Int32.Parse(objectNode.Attributes["y"]?.InnerText);
                        // Determine the type of enemy to add
                        if (enemy == "BasicEnemy")
                        {
                            _enemies.Add(new BasicEnemy(x, y));
                        }
                        if (enemy == "Boss")
                        {
                            _enemies.Add(new BossEnemy(x, y));
                        }

                    }
                }

                // Parsing keys
                if (objectGroupNode.Attributes["name"]?.InnerText == "keys")
                {
                    bool blacklisted = false;
                    foreach (XmlNode objectNode in objectNodes)
                    {
                        int x = Int32.Parse(objectNode.Attributes["x"]?.InnerText);
                        int y = Int32.Parse(objectNode.Attributes["y"]?.InnerText);
                        string name = objectNode.Attributes["name"]?.InnerText;
                        if (!_persistance.CheckKeyBlacklist(name))
                        {
                            _keys.Add(new Key(x, y, name));
                        }
                        else
                        {
                            blacklisted = true;
                        }
                    }
                    if (!blacklisted)
                    {
                        for (int i = 0; i < _keys.Count(); i++)
                        {
                            _keys.ElementAt(i).Load(game);
                        }
                    }
                }

                // Parsing doors
                if (objectGroupNode.Attributes["name"]?.InnerText == "doors")
                {
                    bool blacklisted = false;
                    foreach (XmlNode objectNode in objectNodes)
                    {
                        int x = Int32.Parse(objectNode.Attributes["x"]?.InnerText);
                        int y = Int32.Parse(objectNode.Attributes["y"]?.InnerText);
                        int tile = Int32.Parse(objectNode.Attributes["type"]?.InnerText);
                        string name = objectNode.Attributes["name"]?.InnerText;
                        if (!_persistance.CheckDoorBlacklist(name))
                        {
                            Vector2 tilesetPosition = GetTilesetPosition(new List<int>() { tile }, 0, tileWidth, tileHeight);
                            _doors.Add(new Door(x, y, _fileName, (int)tilesetPosition.X, (int)tilesetPosition.Y, name));
                        }
                        else
                        {
                            blacklisted = true;
                        }
                    }
                    if (!blacklisted)
                    {
                        for (int i = 0; i < _doors.Count(); i++)
                        {
                            _doors.ElementAt(i).Load(game);
                        }
                    }
                }

            }

            LoadEntities(game);

        }



        // Return any colliders that 'other' is colliding with.
        public List<Collider> CheckTileCollisions(Collider other)
        {
            List<Collider> others = new List<Collider>();
            for (int i = 0; i < _tileColliders.Count(); i++)
            {

                if (_tileColliders.ElementAt(i).CheckCollision(other))
                {
                    others.Add(_tileColliders.ElementAt(i));
                }

            }

            return others;
        }

        // Checks for a collision, if there is a collision then the next map is loaded
        public void CheckWarp(Player player, Game game)
        {
            // Loop through each of the warps to check for a collision with the player (parsed in as other).
            Warp warp = new Warp();
            for (int i = 0; i < _warps.Count(); i++)
            {

                if (_warps.ElementAt(i).Collision.CheckCollision(player.GetCollider()))
                {
                    warp = _warps.ElementAt(i);
                    warp.Direction = _warps.ElementAt(i).Collision.GetCollisionSide(player.GetCollider());
                }
            }

            // If there is a warp collision.
            if (!warp.Equals(default(Warp)))
            {
                // Save the warp 'type' this is used to find the corrosponding warp in the newly loaded level.
                string type = warp.Type;
                // Save the player displacement in relation to the origin point of the warp.
                Vector2 displacement = new Vector2(_player.X - (int)warp.Position.X, _player.Y - (int)warp.Position.Y);
                // Load the new level, the name of the warp corrosponds to the level to load.
                LoadLevel(game, warp.Name);

                // Loop through the warps until the one with the corrosponding type is found.
                for (int i = 0; i < _warps.Count; i++)
                {
                    if (_warps.ElementAt(i).Type == type)
                    {

                        // Work out the new position for the player based on the original displacement from the warp.
                        Vector2 newPos = new Vector2();
                        switch (warp.Direction)
                        {
                            case Side.LEFT:
                                newPos = new Vector2(_warps.ElementAt(i).Position.X, _warps.ElementAt(i).Position.Y + displacement.Y);
                                break;
                            case Side.RIGHT:
                                newPos = new Vector2(_warps.ElementAt(i).Position.X - 24, _warps.ElementAt(i).Position.Y + displacement.Y);
                                break;
                            case Side.TOP:
                                newPos = new Vector2(_warps.ElementAt(i).Position.X + displacement.X, _warps.ElementAt(i).Position.Y);
                                break;
                            case Side.BOTTOM:
                                newPos = new Vector2(_warps.ElementAt(i).Position.X + displacement.X, _warps.ElementAt(i).Position.Y - 24);
                                break;
                            case Side.NONE:
                                break;

                        }
                        // Spawn the player in the new position.
                        _player.Spawn(newPos);
                    }
                }

            }

        }

        // Update
        public void Update(GameTime gameTime, Game game)
        {

            _player.Update(gameTime);
            _player.HandleTileCollisions(CheckTileCollisions(_player.GetCollider()));

            // For each enemy...
            for (int i = 0; i < _enemies.Count(); i++)
            {
                _enemies.ElementAt(i).Update(gameTime, new Vector2(_player.X, _player.Y));
                _enemies.ElementAt(i).HandleTileCollisions(CheckTileCollisions(_enemies.ElementAt(i).GetCollider()));

                // Ensures the attack collider is not null, if it is not then it will check to see if it intercepts with the enemy.
                if (_player.GetAttackCollider() != null && _player.GetAttackCollider().CheckCollision(_enemies.ElementAt(i).GetCollider()))
                {
                    _enemies.ElementAt(i).DealDamage(PlayerConstants.ATTACK_DAMAGE, PlayerConstants.ATTACK_KNOCKBACK, _player.GetAttackSide());
                }

                // Deal knockback to the player upon a collision with an enemy.
                if (_player.GetCollider().CheckCollision(_enemies.ElementAt(i).GetCollider())){
                    _player.DealDamage(1, 15, _enemies.ElementAt(i).GetCollider().GetCollisionSide(_player.GetCollider()));
                }

                if (_enemies.ElementAt(i).Dead)
                {
                    _enemies.Remove(_enemies.ElementAt(i));
                }

            }

            // For each key..
            for (int i = 0; i < _keys.Count(); i++)
            {

                // Get the key
                if (_player.GetCollider().CheckCollision(_keys.ElementAt(i).GetCollider()))
                {
                    _player.GetKey();
                    _persistance.BlacklistKey(_keys.ElementAt(i).GetName());
                    _keys.Remove(_keys.ElementAt(i));
                }

            }

            // For each door...
            for (int i = 0; i < _doors.Count(); i++)
            {

                // Upon the collision between the door and the player.
                if (_player.GetCollider().CheckCollision(_doors.ElementAt(i).GetCollider()))
                {
                    // Handle the collision, if the player has a key then remove the door and the key.
                    _player.HandleTileCollisions(new List<Collider>() { _doors.ElementAt(i).GetCollider() });
                    if (UI.Key)
                    {
                        _persistance.BlacklistDoor(_doors.ElementAt(i).GetName());
                        _doors.Remove(_doors.ElementAt(i));
                        _player.UseKey();
                    }
                }

            }

            CheckWarp(_player, game);
        }

        // Draw each tile to make up the level.
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, bool debug)
        {

            for (int i = 0; i < _tileList.Count; i++)
            {
                _tileList.ElementAt(i).Draw(spriteBatch);
            }

            // If debug is on then draw the colliders for the level.
            if (debug)
            {
                for (int i = 0; i < _tileColliders.Count; i++)
                {
                    _tileColliders.ElementAt(i).DrawDebug(spriteBatch, graphicsDevice, Color.Yellow);
                }
            }

            // Draw entities between the background and foreground so foreground is draw in front.
            for (int i = 0; i < _enemies.Count(); i++)
            {
                _enemies.ElementAt(i).Draw(spriteBatch, graphicsDevice, debug);
            }
            for (int i = 0; i < _keys.Count(); i++)
            {
                _keys.ElementAt(i).Draw(spriteBatch);
            }
            for (int i = 0; i < _doors.Count(); i++)
            {
                _doors.ElementAt(i).Draw(spriteBatch);
            }
            _player.Draw(spriteBatch, graphicsDevice, debug);

            for (int i = 0; i < _tileListForeground.Count; i++)
            {
                _tileListForeground.ElementAt(i).Draw(spriteBatch);
            }

        }

        // Calculate the position of the tile within the tileset image.
        private Vector2 GetTilesetPosition(List<int> gids, int tile, int tileWidth, int tileHeight)
        {
            int tilesetWidth = _tileset.Width; // Get the width of the texture (in pixels).
            int tsxx = gids.ElementAt(tile) % (tilesetWidth / tileWidth) - 1; // For example, gid=7(3, 2 in tileset), 7 % ((96/16)-1) = 2
            tsxx *= tileWidth; // 2 * 16 = 32 so x = 32
            tsxx = (tsxx < 0) ? ((tilesetWidth / tileWidth) - 1) * tileWidth : tsxx; // Solves issue with tiles at end of row.
            int tsyy = ((gids.ElementAt(tile) - _firstGid) / (tilesetWidth / tileWidth)) * tileHeight; // For example, gid=7, (7 - 1) / (96/16) * 16 = 16
            return new Vector2(tsxx, tsyy);
        }

    }
}

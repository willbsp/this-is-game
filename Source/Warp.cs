using System;
using Microsoft.Xna.Framework;

namespace computing_project
{
    public struct Warp
    {

        // Name of the warp corrosponds to the name of the next map to load.
        public string Name { get; }
        // Type matches the warp with the sister warp in the next map.
        public string Type { get; }
        // Collider the player collides with.
        public Collider Collision { get; }
        // Position within the map, used to find the players resultant position.
        public Vector2 Position { get; }
        // The direction at which the player entered the warp.
        public Side Direction { get; set; }

        public Warp(string name, string type, Collider collision, Vector2 position, Side direction = Side.NONE)
        {
            Name = name;
            Type = type;
            Collision = collision;
            Position = position;
            Direction = direction;
        }

    }
}

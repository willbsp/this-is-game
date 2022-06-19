using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace computing_project
{

    // Used to denote the side of a collision.
    public enum Side
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
        NONE
    }

    public class Collider
    {

        // BoundingBox - the rectangle acting as the collision shape.
        public Rectangle BoundingBox { get; set; }
        // Active - functions in the class will only work if the collider is currently 'active', ready to accept collisions.
        public bool Active { get; set; } = true;

        public Collider(Rectangle boundingBox)
        {
            BoundingBox = boundingBox;
        }

        // Gets the side on which a collision with 'other' occurs.
        public Side GetCollisionSide(Collider other)
        {
            if (Active)
            {
                int amtRight, amtLeft, amtTop, amtBottom;
                amtRight = Math.Abs(BoundingBox.Right - other.BoundingBox.Left);
                amtLeft = Math.Abs(other.BoundingBox.Right - BoundingBox.Left);
                amtTop = Math.Abs(other.BoundingBox.Bottom - BoundingBox.Top);
                amtBottom = Math.Abs(BoundingBox.Bottom - other.BoundingBox.Top);

                int[] vals = { amtRight, amtLeft, amtTop, amtBottom };
                int lowest = vals[0];

                // Find the lowest value
                for (int i = 0; i < 4; i++)
                {
                    if (vals[i] < lowest)
                    {
                        lowest = vals[i];
                    }
                }

                return // the side of intersection
                    lowest == amtRight ? Side.RIGHT :
                    lowest == amtLeft ? Side.LEFT :
                    lowest == amtTop ? Side.TOP :
                    lowest == amtBottom ? Side.BOTTOM :
                    Side.NONE;
            }
            return Side.NONE;
        }

        // Finds if there is a collision with 'other'.
        public bool CheckCollision(Collider other)
        {
            if (Active)
            {
                return
                    BoundingBox.Right >= other.BoundingBox.Left &&
                    BoundingBox.Left <= other.BoundingBox.Right &&
                    BoundingBox.Top <= other.BoundingBox.Bottom &&
                    BoundingBox.Bottom >= other.BoundingBox.Top;
            }
            return false;
        }

        // When called, it will draw the collider in the color specified.
        public void DrawDebug(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Color color)
        {
            if (Active)
            {
                Color[] debugData = new Color[BoundingBox.Width * BoundingBox.Height];
                Texture2D debugTexture = new Texture2D(graphicsDevice, BoundingBox.Width, BoundingBox.Height);
                for (int i = 0; i < debugData.Length; ++i)
                    debugData[i] = color;
                debugTexture.SetData(debugData);
                spriteBatch.Draw(debugTexture, new Vector2(BoundingBox.X, BoundingBox.Y), Color.White);
            }
        }

    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Raycasting1 {

    class Entity {

        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Direction;

        public Entity(Vector2 position) {
            this.Position = position;
            Velocity = new Vector2(0.0f, 0.0f);
            Direction = new Vector2(1.0f, 0.0f); // need to be an unit vector
        }

        public virtual void Update() {}

        // check if the entity is colliding a wall
        public bool isCollidingWalls(int[,] grid) {
            if((int)Position.Y < grid.GetLength(0) && (int)Position.X < grid.GetLength(1)) {
                if(grid[(int)Position.Y, (int)Position.X] == 1) {
                    return true;
                } 
            }
            return false;
        }

        public void ResetVelocity() {
            Velocity.X = 0.0f;
            Velocity.Y = 0.0f;
        }
    }
}
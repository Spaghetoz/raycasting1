
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace Raycasting1
{
    static class Functions {
        public static void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 pointA, Vector2 pointB) {

            Rectangle lineRect = new Rectangle((int)pointA.X, (int)pointA.Y, (int)(pointB - pointA).Length() + 1, 1);
            float angle = (float)Math.Atan2(pointB.Y - pointA.Y, pointB.X - pointA.X);
            spriteBatch.Draw(texture, lineRect, null, Color.White, angle, Vector2.Zero, SpriteEffects.None, 0);
        } 
        
        public static double VectorLength(Vector2 vector) {
            return Math.Sqrt((vector.X*vector.X)+(vector.Y*vector.Y));
        }

        /*// vector must be a unit vector
        public static Vector2 RotateVector(Vector2 vector, float angle) {

            vector.X = (float)Math.Cos(angle);
            vector.Y = (float)Math.Sin(angle);

            return vector;
        }   */

        public static bool IndexExists(int index, int arrayLength) {
            if(index < arrayLength) {
                return true;
            }
            return false;
        }

        public static float DegToRad(float deg) {
            return (float)(deg*Math.PI)/180;
        }


    }

}


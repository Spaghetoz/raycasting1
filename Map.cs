using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Raycasting1 {

    class Map {

        private int minimapScale;

        public int[,] Grid = {
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
                {1,0,0,0,0,1,0,0,0,1,0,0,0,1},
                {1,0,1,0,0,0,1,0,1,0,0,0,0,1},
                {1,0,1,0,0,0,0,1,0,0,0,0,0,1},
                {1,0,1,0,0,1,0,0,0,1,0,0,0,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1}
            };


        public Map(int minimapScale) {
            this.minimapScale = minimapScale;
        }

        public void DrawMiniMap(SpriteBatch spriteBatch,Texture2D whiteTexture, Texture2D blackTexture) {
            
            for( int y = 0; y < Grid.GetLength(0); y++ ) {
                
                for(int x = 0; x < Grid.GetLength(1); x++) {    

                    if(Grid[y, x] == 0) {
                        spriteBatch.Draw(whiteTexture, new Rectangle(x*minimapScale, y*minimapScale, minimapScale, minimapScale), Color.White);
                    }
                    else if(Grid[y, x] == 1) {
                        spriteBatch.Draw(blackTexture, new Rectangle(x*minimapScale, y*minimapScale, minimapScale, minimapScale), Color.White);
                    }

                }
            }
            
        }
    }
}
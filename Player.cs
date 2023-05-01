using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Raycasting1{

    class Player : Entity {

        private int minimapScale;

        private float movingSpeed = 0.05f;
        private float rotationAngle = (float)(Math.PI/40.0f);  // value applied when rotating with keys left and right

        public float angle = 0.0f;
        private float renderDistance = 5.0f;

        // stores the number of rays that will be casted
        static int raysAmount = 120;
        // Array that will store every rays vector
        public Vector2[] CastedRays = new Vector2[raysAmount];

        // Array that will store if the hitted wall is a vertical line axis or an horizontal line axis 
        // 0 corresponds to vertical axis (x) and 1 corresponds to horizontal line axis (y)
        // the indices of this array will correspond to the CastedRays array indices    
        public short[] CastedRaysHittedWall = new short[raysAmount];

        // Array that will store the length of the casted rays
        // it will be useful for the render distance
        // the indices of this array will also correspond to the CastedRays array indices    
        public float[] CastedRaysLength = new float[raysAmount];

        // Array that will store the angle of casted rays
        // also needed for render distance
        public float[] CastedRaysAngle = new float[raysAmount];

        public Player(Vector2 position, int minimapScale) : base(position) {
            
            this.minimapScale = minimapScale;
        }

        public void HandleInput(KeyboardState keyboardState, MouseState mouseState) {

            if(keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up)) Velocity += Direction*movingSpeed;
            if(keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down)) Velocity -= Direction*movingSpeed;

            if(keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left)) {
                angle -= rotationAngle;
                Direction.X = (float)Math.Cos(angle);
                Direction.Y = (float)Math.Sin(angle);
            } 
            if(keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right)) {
                angle += rotationAngle;
                Direction.X = (float)Math.Cos(angle);
                Direction.Y = (float)Math.Sin(angle);
            } 
        }

        public void MoveHorizontally(int[,] grid) {
            Position.X += Velocity.X;
            
            // we first check if the index where the player is exists
            if((int)Position.Y < grid.GetLength(0) && (int)Position.X < grid.GetLength(1)) {
                // then we check if the tile at position of the player is the wall
                if(grid[(int)Position.Y, (int)Position.X] == 1) {

                    // if the player is colliding the wall and was moving right 
                    if(Velocity.X > 0) {
                        // we place him to the left of the wall
                        // -0.01f to don't make the player stuck in the wall
                        Position.X = (int)Position.X-0.01f;
                    } 
                    // we do the same for if the player is moving left
                    else if(Velocity.X < 0) {
                        Position.X = (int)Position.X+1.01f;
                    }
                } 
            }

        }

        public void MoveVertically(int[,] grid) {
            Position.Y += Velocity.Y;

            if((int)Position.Y < grid.GetLength(0) && (int)Position.X < grid.GetLength(1)) {
                if(grid[(int)Position.Y, (int)Position.X] == 1) {
                    if(Velocity.Y > 0) {
                        Position.Y = (int)Position.Y-0.01f;
                    } 
                    else if(Velocity.Y < 0) {
                        Position.Y = (int)Position.Y+1.01f;
                    }
                } 
            }
        }

        public override void Update() {
            ResetVelocity();
        }

        /////////// method that will cast a single ray starting from the player and finishing to the first encountered wall
        // parameters are map grid, angle of projection and index in the CastedRays arrays
        public void CastRay(int[,] grid, float angle, int indexInArrays) {
            ////// Thanks to WeirdDevers for the explanations : https://youtu.be/g8p7nAbDz6Y  (but there is some mistakes) /////

            //----- checking vertical lines intersection
            Vector2 deltaDistX = new Vector2();
            deltaDistX.X = 1.0f;
            deltaDistX.Y = (float)(Math.Tan(angle));

            Vector2 sideDistX = new Vector2();

            //----- checking horizontal lines intersection
            Vector2 deltaDistY = new Vector2();
            deltaDistY.X = (float)(1/Math.Tan(angle)); // tan(a)=deltadistYy/deltadistYx <=> deltadistYx=deltadistYy/tan(a) <=> deltadistYx=1/tan(a) 
            deltaDistY.Y = 1.0f;    

            Vector2 sideDistY = new Vector2();

            if(Math.Cos(angle) < 0) {
                 
                // if the angle is pointing left we invert the direction of deltaDistX            
                deltaDistX = -deltaDistX;

                //////////////we find the left nearest tile
                sideDistX.X = -(Position.X-(int)Position.X);
                sideDistX.Y = (float)(sideDistX.X*Math.Tan(angle));
            } else {
                sideDistX.X = 1-(Position.X-(int)Position.X);
                sideDistX.Y = (float)(sideDistX.X*Math.Tan(angle));
            }                  

            if(Math.Sin(angle) < 0) {
                deltaDistY = -deltaDistY;

                //////////////we find the upper nearest tile
                sideDistY.Y = -(Position.Y-(int)Position.Y);   
                sideDistY.X = (float)(sideDistY.Y/Math.Tan(angle));
                
            } else {
                sideDistY.Y = 1-(Position.Y-(int)Position.Y);   // cellsize-nearestcellfromplayer
                sideDistY.X = (float)(sideDistY.Y/Math.Tan(angle));
            }
    
            ///// After the sideDist and deltaDist calculations, we can cast the ray until it hits a wall
            ////In this part we will use sidedist as a ray that will check every tile by adding it deltaDist

            // vector with real map position that will check the X lines axis
            Vector2 rayXPosition = Position + sideDistX;
            Vector2 rayYPosition = Position + sideDistY;

            // the final ray that will have a length from the player position to the hitted wall
            Vector2 ray = new Vector2(0.0f, 0.0f);

            //Variable that will store if the last hitted wall was on x axis or y axis
            short hittedWall = 0;

            bool hit = false;
            while(hit == false) {

                if(Functions.VectorLength(sideDistX) < Functions.VectorLength(sideDistY)) {
                    // we replace the ray by the furthest casted ray, so the sideDistX in this case
                    ray = sideDistX;
                    
                    // then we refresh the position on the map of the sideDistX  
                    rayXPosition = Position + sideDistX;


                    hittedWall = 0;

                    // if the angle is pointing left, we will check left tile so it won't pass through it
                    if(Math.Cos(angle) < 0) {
                        
                        // we first check if the index exists in the grid array
                        if((int)rayXPosition.Y < grid.GetLength(0) && (int)rayXPosition.X-1 < grid.GetLength(1)) {

                            // then we check if the tile is a wall
                            if(grid[(int)rayXPosition.Y, (int)rayXPosition.X-1] == 1) {
                                // if it is the case we stop the casting of the ray
                                hit = true;
                            }
                        // if the index doesnt exists we stop the casting of the ray
                        } else {
                            break;
                        }
                    // else we simply check the tile at the coordinates of the ray
                    } else {
                        
                        // we check if the index exists
                        if((int)rayXPosition.Y < grid.GetLength(0) && (int)rayXPosition.X < grid.GetLength(1)) {

                            if(grid[(int)rayXPosition.Y, (int)rayXPosition.X] == 1) {
                                hit = true;
                            }   
                        } else {
                            break;
                        }
                    }
                    sideDistX += deltaDistX;
                    
                } else {
                    ray=sideDistY;
                    rayYPosition = Position + sideDistY;

                    hittedWall = 1;

                    // if the angle is pointing left, we will check upper tile
                    if(Math.Sin(angle) < 0) {

                        if((int)rayYPosition.Y-1 < grid.GetLength(0) && (int)rayYPosition.X < grid.GetLength(1)) {
                            if(grid[(int)rayYPosition.Y-1, (int)rayYPosition.X] == 1) {
                                hit = true;
                            }
                        } else break;
                    } else {

                        if((int)rayYPosition.Y < grid.GetLength(0) && (int)rayYPosition.X < grid.GetLength(1)) {
                            if(grid[(int)rayYPosition.Y, (int)rayYPosition.X] == 1) {
                                hit = true;
                            }              
                        } else break;
                    }
                    sideDistY += deltaDistY;
                    
                }

            }

            if(hittedWall == 0) CastedRaysHittedWall[ indexInArrays] = 0;
            else CastedRaysHittedWall[ indexInArrays] = 1;

            // add the casted ray to the castedrays array
            CastedRays[indexInArrays] = ray;    



            // -------------Debug  

            
            //public Vector2 CastRay(SpriteBatch spriteBatch, Texture2D colorTexture, int[,] grid, SpriteFont font, Texture2D greenTexture) {      

            //Functions.DrawLine(spriteBatch, colorTexture, Position*minimapScale, (Position+deltaDistX)*minimapScale );
            //Functions.DrawLine(spriteBatch, colorTexture, Position*minimapScale, (Position+sideDistX)*minimapScale ); 

            //Functions.DrawLine(spriteBatch, colorTexture, Position*minimapScale, (Position+deltaDistY)*minimapScale );
            //Functions.DrawLine(spriteBatch, colorTexture, Position*minimapScale, (Position+sideDistY)*minimapScale );  
            
            //Functions.DrawLine(spriteBatch, colorTexture, Position*minimapScale, (Position+Direction)*minimapScale );

            //Functions.DrawLine(spriteBatch, colorTexture, Position*minimapScale, rayXPosition*minimapScale );  
            //Functions.DrawLine(spriteBatch, greenTexture, Position*minimapScale, rayYPosition*minimapScale );  

            //Vector2 rayPosition = Position+ray;
            //Functions.DrawLine(spriteBatch, colorTexture, Position*minimapScale, rayPosition*minimapScale);

        }

        // method that will cast all rays to make the player's field of view and store it in a list
        public void CastRays(int[,] grid) {
            // the first ray will be casted 30deg left to player
            float angle = this.angle-Functions.DegToRad(30);
            for(int i = 0; i < 120; i++) {
                CastRay(grid, angle, i);

                // then we cast the next ray 0.5degrees right
                angle+=Functions.DegToRad(0.5f);
            }
        }

        public void DrawRaysOnMinimap(SpriteBatch spriteBatch, Texture2D colorTexture) {
            for(int i = 0; i < CastedRays.Length; i++) {
                Functions.DrawLine(spriteBatch, colorTexture, Position*minimapScale, (Position+CastedRays[i])*minimapScale);
            }   
        }

        public void DrawOnMinimap(SpriteBatch spriteBatch, Texture2D redTexture) {
            spriteBatch.Draw(redTexture, new Rectangle((int)(Position.X*minimapScale), (int)(Position.Y*minimapScale), 1,1), Color.White);
            

        }
        public void DrawDirectionRay(SpriteBatch spriteBatch, Texture2D colorTexture, int[,] grid) {
                                                          // start pos of line        // end pos of line
            Functions.DrawLine(spriteBatch, colorTexture, Position*minimapScale, (Position+Direction)*minimapScale );
        }
        
        // Method for drawing walls, walls are drawed vertical line by vertical line, a vertical line being a single ray 
        // wallColorX corresponds to the color for the rays that hitted the right or left side of wall
        // wallColorY corresponds to the color for the rays that hitted the up or down side of wall
        public void DrawWalls(SpriteBatch spriteBatch, Texture2D wallColorX, Texture2D wallColorY, int windowWidth, int windowHeight) {

            int lineWidth = windowWidth/CastedRays.Length;
            Texture2D wallColor;

            for(int i = 0; i < CastedRays.Length; i++) {
                // the line height will depends on the ray length
                int lineHeight = (int)(windowHeight/Functions.VectorLength(CastedRays[i]));
                // reduce the line height if it exceeds the window height
                if(lineHeight > windowHeight) lineHeight = windowHeight;  

                // draw with different color according to the hitted side of wall
                if(CastedRaysHittedWall[i] == 0) wallColor = wallColorX;
                else wallColor = wallColorY;

                spriteBatch.Draw(wallColor, new Rectangle( i*lineWidth, windowHeight/2-lineHeight/2 ,lineWidth ,lineHeight), Color.White);
            }
        }

        public void DrawFloor(SpriteBatch spriteBatch, Texture2D floorColor, int windowWidth, int windowHeight) {
            int floorPosY = windowHeight/2;
            spriteBatch.Draw(floorColor, new Rectangle(0,floorPosY, windowWidth, floorPosY), Color.White);
        }

    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceRanger
{
    internal class Barriers
    {
        //-----------------------------------------------------------------------------------------------------------------------------------------
        //THIS CLASS IS NOT IMPLEMENTED IN THE FINAL BUILD FOR THE TURN IN. WE PLANNED TO IMPLEMENT THIS FEATURE BUT DID NOT HAVE THE TIME TO DO SO
        //-----------------------------------------------------------------------------------------------------------------------------------------

        //FIELDS
        //screen barrier
        private GraphicsDeviceManager _graphics;


        //creation
        protected Texture2D barrierTexture;
        protected Vector2 barrierPos;
        protected Vector2 barrierOrigin;
        protected Rectangle? barrierRect;
        protected int barrierWidth;
        protected int barrierHeight;

        //Specific barrier dimensions based on types

        //small rock 1
        protected int barrier1Width = 19 * 6;
        protected int barrier1Height = 14 * 6;
        protected Vector2 barrier1Vect = new Vector2(0, 0);

        //small rock 2
        protected int barrier2Width = 16 * 6;
        protected int barrier2Height = 14 * 6;
        protected Vector2 barrier2Vect = new Vector2(0, 15 * 6);

        //small rock 3
        protected int barrier3Width = 19 * 6;
        protected int barrier3Height = 14 * 6;
        protected Vector2 barrier3Vect = new Vector2(0, 2*(15 * 6));

        //huge rock
        protected int barrier4Width = 43 * 6;
        protected int barrier4Height = 55 * 6;
        protected Vector2 barrier4Vect = new Vector2(20 * 6, 0);

        //long rock
        protected int barrier5Width = 22 * 6;
        protected int barrier5Height = 49 * 6;
        protected Vector2 barrier5Vect = new Vector2(64 * 6, 0);

        //satellite
        protected int barrier6Width = 60 * 6;
        protected int barrier6Height = 33 * 6;
        protected Vector2 barrier6Vect = new Vector2(0, 0);         
        //Movement
        protected float barrierSpeedX;
        protected float barrierSpeedY;
        protected float rotateSpeed;


        //PROPERTIES
        public Texture2D BarrierTexture
        {
            get { return barrierTexture; }
        }

        //CONSTRUCTOR
        public Barriers(Texture2D texture, string barriertype, int x, int y, int width, int height, float rotate)
        {
            //texture for the barrier
            this.barrierTexture = texture;

            //vector for the barrier
            this.barrierPos = new Vector2(x, y);

            //width and height
            this.barrierWidth = width;
            this.barrierHeight = height;

            //rectangle
            barrierRect = new Rectangle(x, y, width, height);

            //origin
            this.barrierOrigin = new Vector2(width / 2, height / 2);

            rotateSpeed = rotate;
            barrierSpeedX = 1.0f;
            barrierSpeedY = 1.0f;

        }

        //METHODS

        /// <summary>
        /// logic for what happens when a rock runs into something. should force the entities to bounch off the barrier and the bullets to count as hitting a target
        /// </summary>
        /// <param name="p"></param>
        /// <param name="e"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool BarrierCollide(Rectangle r)
        {
            //collide check logic that checks if the given rectangle intersects with the barriers rectangle 
            return (r.Intersects(new Rectangle((int)this.barrierPos.X, (int)this.barrierPos.Y, this.barrierWidth, this.barrierHeight)));
        }

        public List<Rectangle> GetBarrierList(List<string> types, List<Rectangle> rects)
        {
            //initialize the list of rectangles as a new list
            rects = new List<Rectangle>();

            //check each string inside the list of the types of barriers
            foreach(string type in types)
            {
                //determine which kind of barrier will be created from the list of types given
                switch (type)
                { 
                    //small rock 1
                    case "1":
                        rects.Add(new Rectangle(0, 0, barrier1Width, barrier1Height));
                        break;
                    //small rock 2
                    case "2":
                        rects.Add(new Rectangle());
                        break;
                    //small rock 3
                    case "3":

                        break;
                    //huge rock
                    case "4":

                        break;
                    //long rock
                    case "5":

                        break;
                    //satellite
                    case "6":

                        break;
                }
            }

            return rects;
        }

        /// <summary>
        /// default movement of the barriers
        /// </summary>
        public void Update()
        {
            //ambiently move barrier across screen
            barrierPos.X += barrierSpeedX;
            barrierPos.Y += barrierSpeedY;

            //logic for barrier movement
            //if the barrier goes over the right wall
            if (barrierPos.X > _graphics.PreferredBackBufferWidth)
            {
                //sets the position on the left wall
                barrierPos.X = 1;
            }
            //if the barrier goes over the left wall
            if (barrierPos.X < 0)
            {
                //sets position on the right wall
                barrierPos.X = _graphics.PreferredBackBufferWidth - barrierWidth;
            }
            //if the barrier goes over bottom wall
            if (barrierPos.Y > _graphics.PreferredBackBufferHeight)
            {
                //sets position on top wall
                barrierPos.Y = 1;
            }
            //if the barrier for over top wall
            if (barrierPos.Y < 0)
            {
                //sets position to bottom wall
                barrierPos.Y = _graphics.PreferredBackBufferHeight - barrierHeight;
            }

        }
        /// <summary>
        /// default draw for the barriers
        /// </summary>
        public void Draw(Barriers b, SpriteBatch sb)
        {
            //drawws barrier texture vector position  rectangle    color        rotate speed    rotate origin    scalar vector      no sprite effect   layer depth
            sb.Draw(b.barrierTexture, b.barrierPos, b.barrierRect, Color.White, b.rotateSpeed, b.barrierOrigin, new Vector2 (1,1), SpriteEffects.None, 0f);
        }
    }
}

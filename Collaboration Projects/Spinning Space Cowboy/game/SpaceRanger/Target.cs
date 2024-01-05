using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
//using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceRanger
{
    internal class Target 
    {
        //fields
        //the target rectangle 
        private Rectangle entityRect;
        //the sprite 
        private Texture2D entitySprite;
        //spawn in timer
        private double targetTimer = 10;
        //the list of target rectangles
        private List<Rectangle> targetsRects;

        //the board
        private List<List<string>> board;
        //the board spawn locations 
        private List<List<Vector2>> bL;
        //how many rows and columns are in the board
        private int rows;
        private int columns;


        //amount of target there are 
        private int targetcount;

        // for if alive only, one HP
        private bool isAlive;

        //counter for targets hit
        private static int counter;

        /// <summary>
        /// Returns if the target is alive
        /// </summary>
        public bool IsAlive
        {
            get { return isAlive; }
        }

        /// <summary>
        /// retrun an int of how many targets there are
        /// </summary>
        public int Targetcount
        {
            get { return targetcount; }
            set { targetcount = value; }
        }

        /// <summary>
        /// retrung int to how many time targets have been hit 
        /// </summary>
        public static int Counter
        {
            get { return counter; }
            set { counter = value; }
        }

        /// <summary>
        /// retrungs the list of tragets rectangle
        /// </summary>
        public List<Microsoft.Xna.Framework.Rectangle> TargetsRects
        {
            get { return targetsRects; }
        }

        /// <summary>
        /// readys the targets
        /// </summary>
        /// <param name="entitySprite"></param>
        /// <param name="sWidth"></param>
        /// <param name="sHeight"></param>
        public Target( Texture2D entitySprite, int sWidth, int sHeight)
        {
            //makes the list of rectangles
            targetsRects = new List<Microsoft.Xna.Framework.Rectangle>();

            // makes target alive
            isAlive = true;
            //sets the sprite
            this.entitySprite = entitySprite;
            //readys the Io for the targets
            IoMannager Io = new IoMannager(sWidth,sHeight);

            //gets all of the board info 
            Io.reader(0);
            board = Io.Board;
            rows = Io.Rows;
            columns = Io.Columns;
            bL = Io.BoardLocations;
        }

        /// <summary>
        /// makes the target rectangles 
        /// </summary>
        public void targetMaker()
        {
            //for each row on the board
            for(int row=0;row<rows;row++)
            {
                //for each colume on the board 
                for(int col=0;col<columns;col++)
                {
                    //if there is a target marker on that location on the board
                    if(board[row][col]=="t")
                    {
                        //makes the rectangle for this target 
                        entityRect = new Microsoft.Xna.Framework.Rectangle((int)bL[row][col].X, (int)bL[row][col].Y, entitySprite.Width, entitySprite.Height);
                        //adds it to the list of rectangles
                        targetsRects.Add(entityRect);
                        //adds to the count for the new target
                        targetcount++;
                    }
                }
            }




        }


        /// <summary>
        /// Need bullet rectangle for the entityRect.Intersects(rec)
        /// </summary>
        /// <param name="rec"></param>
        public void Update(GameTime timer, Microsoft.Xna.Framework.Rectangle targetrec, Microsoft.Xna.Framework.Rectangle bulletrec)
        {
            //checks if bullet and target are intersecting and only when alive
            if(targetrec.Intersects(bulletrec)&&isAlive)
            {
                isAlive = false;
                counter++;
            }
            //if not alive 
            if (!isAlive)
            {
                //timer for respawning 
                targetTimer -= timer.ElapsedGameTime.TotalSeconds;
                //onece the timer hits 0 then target becomes alive again.
                if(targetTimer <= 0.0)
                {
                    isAlive = true;
                    targetTimer = 10;
                }   
            }
     
        }

        /// <summary>
        /// Simple draw method Draw(entitySprite, entityRect, Color.White);
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void draw(SpriteBatch _spriteBatch, Microsoft.Xna.Framework.Rectangle rec)
        {
            if(isAlive)
                _spriteBatch.Draw(entitySprite, rec, Microsoft.Xna.Framework.Color.White);
        }


    }
}

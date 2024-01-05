using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SpaceRanger
{
    //states the enemy can be in
    public enum EnemyState
    {
        Idle,
        Shoot,
    }
    internal class Enemies
    {
        //enemy state field
        public EnemyState enemyState;

        //enemy vectors and rotations
        private Vector2 enemyVect;
        public Vector2 enemyArmVect;
        public Vector2 fenemyArmVect;

        private Vector2 direction;
        private float movementAngle;


        //enemy speed
        private const float enemySpeed = 5;


        //Sprites
        private Texture2D enemySprite;


        //Body Rectangle on sprite sheet
        private int enemyWidth = 31 * 5;
        private int enemyHeight = 47 * 5;
        private int enemyOffsetY = 52 * 5;
        private Rectangle enemyRect;


        //Idle arm Rectangle on sprite sheet
        private int idleArmWidth = 35 * 5;
        private int idleArmHeight = 10 * 5;
        private int idleArmOffsetY = 0 * 5;
        private Vector2 idleShoulder = new Vector2(2 * 5, 4 * 5);
        private Vector2 FidleShoulder = new Vector2(120, 0);


        //Shoot arm Rectangle on sprite sheet
        private int shootArmWidth = 47 * 5;
        private int shootArmHeight = 13 * 5;
        private int shootArmOffsetY = 13 * 5;
        private Vector2 shootShoulder = new Vector2(2 * 5, 7 * 5);
        private Vector2 fshootShoulder = new Vector2(185, 15);


        //checking for gamestate in the enemy class
        public enum GameMode
        {
            Menu, Tutorial, Game, GameOver
        }
        public GameMode gameState;

        //Gametime variables
        private double gametimecurrent;
        private double gametimechange;

        //health
        private int health = 1;
        private SpriteEffects flipSprite = SpriteEffects.None;
        private bool isAlive;
        
        //list of all the enemy's rectangles
        private List<Rectangle> enemyRects = new List<Rectangle>();


        //file IO
        private List<List<string>> board;
        private List<List<Vector2>> bL;
        private int rows;
        private int columns;



        //properties
        public bool IsAlive
        {
            get { return isAlive; }
        }

        public int Health
        {
            get { return health; }
        }

        public Rectangle EnemyRect
        {
            get { return enemyRect; }
        }

        public Vector2 EnemyVect
        {
            get { return enemyVect; }
            set { enemyVect = value; }
        }


        public Enemies (Texture2D body, Vector2 bodyVect, Texture2D enemyArm,int lvl, Rectangle rect)
        {
            health = 1;
            isAlive = true;

            //setting up file IO
            IoMannager Io = new IoMannager(enemyWidth, enemyHeight);

            Io.reader(lvl);
            board = Io.Board;
            rows = Io.Rows;
            columns = Io.Columns;
            bL = Io.BoardLocations;

            //enemy body
            this.enemySprite = body;
            this.enemyVect = bodyVect;

            //enemy arm
            this.enemyArmVect = new Vector2(bodyVect.X + 65, bodyVect.Y + 95);
            this.fenemyArmVect = new Vector2(bodyVect.X -255 , bodyVect.Y + 120);

            this.enemyRect = new Rectangle((int)enemyVect.X, (int)enemyVect.Y, enemyWidth,enemyHeight);

            direction = new Vector2();
        }

        /// <summary>
        /// chcecks for collision with player's bullets
        /// </summary>
        /// <param name="rec"></param>
        public bool Update(Rectangle rec, GameTime gameTime,Rectangle enemieRec)
        {
            if (enemieRec.Intersects(rec))
            {
                if(health ==1)
                {
                    health -= 1;
                    isAlive = false;
                }

            }

            //set game state to game 
            gameState = GameMode.Game;

            return isAlive;
        }


        /// <summary>
        /// draws either the idle or the shoot sprites based on the enemy state
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(Enemies enemy, SpriteBatch sb, Vector2 playerVect)
        {
            //Draw enemy
            enemy.DrawBody(enemy, sb, playerVect);

            switch (enemyState)
            {
                case EnemyState.Idle:
                    enemy.DrawIdle(sb, playerVect);

                    break;

                case EnemyState.Shoot:
                    enemy.DrawShoot(sb, playerVect);

                    break;
            }
        }

        /// <summary>
        /// checks if the ai is in range of the player, if so it returns true, if not then it moves towards the player
        /// </summary>
        /// <returns></returns>
        public bool Movement(Vector2 playerLoc)
        {
            if (Vector2.Distance(enemyVect, playerLoc) < 600)
            {
                return true;
            }
            else
            {
                //Math to figure out rotation. Finding the angle from player to mouse is done by finding the hypotenuse,
                //(which is distance from player vector to mouse vector), and the opposite. Divide the opp by hyp, and use arcsine to find the angle from this triangle.
                Vector2 origin = enemyVect;

                //Create the vector of the opposite so we can calculate the angle
                Vector2 opposite = new Vector2(playerLoc.X, origin.Y);


                //This if else chain is to determine the correct calculation to use for each of the
                //4 quadrants of where the mouse can be in relation to the player: bottom right, bottom left, top right, and top left in that order.
                //this determines the angle the enemy should be going towards the player
                if (playerLoc.X > origin.X && playerLoc.Y > origin.Y)
                {
                    movementAngle = (float)Math.Asin(Vector2.Distance(playerLoc, opposite) / Vector2.Distance(playerLoc, enemyVect));
                }
                else if (playerLoc.X < origin.X && playerLoc.Y > origin.Y)
                {
                    movementAngle = (float)((Math.PI / 2.0) - Math.Asin(Vector2.Distance(playerLoc, opposite) / Vector2.Distance(playerLoc, enemyVect)) + (Math.PI / 2.0));
                }
                else if (playerLoc.X > origin.X && playerLoc.Y < origin.Y)
                {
                    movementAngle = (float)-Math.Asin(Vector2.Distance(playerLoc, opposite) / Vector2.Distance(playerLoc, enemyVect));
                }
                else if (playerLoc.X < origin.X && playerLoc.Y < origin.Y)
                {
                    movementAngle = (float)-((Math.PI / 2.0) - Math.Asin(Vector2.Distance(playerLoc, opposite) / Vector2.Distance(playerLoc, enemyVect)) + (Math.PI / 2.0));
                }

                //moving the enemy towards the player
                direction.X = (float)(Math.Cos(movementAngle) * enemySpeed);
                direction.Y = (float)(Math.Sin(movementAngle) * enemySpeed);
                enemyArmVect.X += direction.X;
                enemyArmVect.Y += direction.Y;
                fenemyArmVect.X += direction.X;
                fenemyArmVect.Y += direction.Y;

                enemyVect += direction;
                enemyRect = new Rectangle((int)enemyVect.X, (int)enemyVect.Y, EnemyRect.Width, EnemyRect.Height);
                return false;
            }
        }



        /// <summary>
        /// draws the body of the enemy
        /// </summary>
        public void DrawBody(Enemies enemy, SpriteBatch sb, Vector2 playerVect)
        {
            if (enemyArmVect.X > playerVect.X)
            {
                 flipSprite = SpriteEffects.FlipHorizontally;
            }
            else
            {
                flipSprite = SpriteEffects.None;
            }

            sb.Draw(
            enemySprite,                                                  // - The texture to draw
            enemyVect,                                                   // - The location to draw on the screen
            new Rectangle(0, enemyOffsetY, enemyWidth, enemyHeight),   // - The "source" rectangle
            Color.White,                                                  // - The color
            0,                                                            // - Rotation (none currently)
            new Vector2(0, 0),                                             // - Origin inside the image (top left)
            1.0f,                                                         // - Scale (100% - no change)
            flipSprite,                                                   // - Can be used to flip the image
            1);                                                           // - Layer depth
        }

        /// <summary>
        /// draws arm as idle
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="sb"></param>
        public void DrawIdle(SpriteBatch sb, Vector2 playerVect)
        {
            if (enemyArmVect.X > playerVect.X)
            {
                 flipSprite = SpriteEffects.FlipHorizontally;
                 sb.Draw(
                 enemySprite,
                 enemyArmVect,
                 new Rectangle(0, 0 , idleArmWidth, idleArmHeight),
                 Color.White,
                  0,
                  FidleShoulder,
                  1.0f,
                  flipSprite,
                  1);
            }
            else
            {
                flipSprite = SpriteEffects.None;
                sb.Draw(
                 enemySprite,
                  enemyArmVect,
                  new Rectangle(0, idleArmOffsetY, idleArmWidth, idleArmHeight),
                   Color.White,
                   0,
                  idleShoulder,
                   1.0f,
                   flipSprite,
    1);
            }
        }

        /// <summary>
        /// draws arm with muzzle flash
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="sb"></param>
        public void DrawShoot(SpriteBatch sb, Vector2 playerVect)
        {
            if (enemyArmVect.X > playerVect.X)
            {
                flipSprite = SpriteEffects.FlipHorizontally;
                sb.Draw(
                 enemySprite,
                 enemyArmVect,
                 new Rectangle(0, shootArmOffsetY, shootArmWidth, shootArmHeight),
                 Color.White,
                  0,
                  fshootShoulder,
                   1.0f,
                   flipSprite,
                   1);
            }
            else
            {
                flipSprite = SpriteEffects.None;
                sb.Draw(
                  enemySprite,
                   enemyArmVect,
                  new Rectangle(0, shootArmOffsetY, shootArmWidth, shootArmHeight),
                  Color.White,
                  0,
                   shootShoulder,
                   1.0f,
                   flipSprite,
                   1);
            }

        }
    }
}

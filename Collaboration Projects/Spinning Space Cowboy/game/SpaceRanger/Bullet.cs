using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;

namespace SpaceRanger
{
    internal class Bullet
    {
        //bullet position, rotation, and movement fields
        private Vector2 bulletVect;
        private float bulletRotation;
        private Vector2 direction;
        private Vector2 newBulletVect;
        private Rectangle bulletRect;

        private int positionCheck = 0;
        private int rotationCheck = 0;
        private static int reloadCheck = 0;

        private int aiPositionCheck = 0;
        private int aiRotationCheck = 0;

        //player speed
        private const float playerSpeed = 10;

        //arm's vector used for determining where bullets spawn
        private Vector2 armVect;

        //sprite things
        private SpriteBatch sb;
        private Texture2D spriteSheet;

        //game state
        public enum GameMode
        {
            Menu, Tutorial, Game, GameOver
        }
        public GameMode gameState;

        //bullet's speed
        private const float bulletspeed = 25;

        //bullet's state
        private bool bulletAlive;

        //size of bullet
        private int bulletWidth = 13 * 5;
        private int bulletHeight = 3 * 5;

        //mouse state to check if player shoots
        private MouseState mouseState;

        //Bullet properties
        public Rectangle BulletRect
        {
            get { return bulletRect; }
        }

        //bool used to determine if bullet should be spawned
        public bool BulletAlive
        {
            get { return BulletAlive; }
            set { BulletAlive = value; }
        }

        //bool for checking if player needs to reload
        public static int ReloadCheck
        {
            get { return reloadCheck; }
            set { reloadCheck = value; }
        }

        //bullet constructor
        public Bullet(Texture2D bulletSprite, Vector2 armVect)
        {
            this.spriteSheet = bulletSprite;
            this.armVect = armVect;
            this.bulletVect = new Vector2(armVect.X + 180, armVect.Y - 15);
            this.bulletRect = new Rectangle((int)bulletVect.X, (int)bulletVect.Y, bulletWidth, bulletHeight);
            bulletAlive = true;
            direction = new Vector2();
        }

        /// <summary>
        /// general shoot method for both player and AI, taken in who shot the bullet and where the player and enemy's vectors are 
        /// </summary>
        /// <param name="whoShoot"></param>
        /// <param name="playerLoc"></param>
        /// <param name="enemyVect"></param>
        /// <param name="enemyArmVect"></param>
        public void Shoot(bool whoShoot, Vector2 playerLoc, Vector2 enemyVect, Vector2 enemyArmVect)
        {
            //player code
            if (whoShoot == true)
            {
                mouseState = Mouse.GetState();

                //Math to figure out rotation. Finding the angle from player to mouse is done by finding the hypotenuse, (which is distance from player vector to mouse vector), and the opposite. Divide the opp by hyp, and use arcsine to find the angle from this triangle.
                Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
                Vector2 origin = new Vector2(armVect.X + 44 / 2f, armVect.Y + 6 / 2f);

                //Create the vector of the opposite so we can calculate the angle
                Vector2 opposite = new Vector2(mousePos.X, origin.Y);

                if (rotationCheck == 0)
                {
                    //This if else chain is to determine the correct calculation to use for each of the 4 quadrants of where the mouse can be in relation to the player: bottom right, bottom left, top right, and top left in that order.
                    //determines what angle a bullet should be shot at
                    if (mousePos.X > origin.X && mousePos.Y > origin.Y)
                    {
                        bulletRotation = (float)Math.Asin(Vector2.Distance(mousePos, opposite) / Vector2.Distance(mousePos, newBulletVect));
                    }
                    else if (mousePos.X > origin.X && mousePos.Y < origin.Y)
                    {
                        bulletRotation = (float)(-Math.Asin(Vector2.Distance(mousePos, opposite) / Vector2.Distance(mousePos, newBulletVect)));
                            
                        if (float.IsNaN(bulletRotation))
                        {
                            bulletRotation = -1.4f;
                        }

                    }

                    
                    if (mousePos.X < origin.X && mousePos.Y > origin.Y)
                    {
                        bulletRotation = -((float)-((Math.PI / 2.0) - Math.Asin(Vector2.Distance(mousePos, opposite) / Vector2.Distance(mousePos, newBulletVect)) + (Math.PI / 2.0)));

                    }
                    else if (mousePos.X < origin.X && mousePos.Y < origin.Y)
                    {
                        bulletRotation = -((float)((Math.PI / 2.0) - Math.Asin(Vector2.Distance(mousePos, opposite) / Vector2.Distance(mousePos, newBulletVect)) + (Math.PI / 2.0)));
                    }
                }
            }
            //enemy code
            else
            {
                Vector2 origin = enemyArmVect;
                bulletVect = new Vector2(enemyArmVect.X + 180, enemyArmVect.Y - 15);
                
                //Create the vector of the opposite so we can calculate the angle
                Vector2 opposite = new Vector2(playerLoc.X, origin.Y);

                if (aiRotationCheck == 0)
                {
                    //This if else chain is to determine the correct calculation to use for each of the 4 quadrants of where the mouse can be in relation to the player: bottom right, bottom left, top right, and top left in that order.
                    //determines what angle the bullet should be shot from
                    if (playerLoc.X > origin.X && playerLoc.Y > origin.Y)
                    {
                        bulletRotation = (float)Math.Asin(Vector2.Distance(playerLoc, opposite) / Vector2.Distance(playerLoc, enemyVect));
                    }
                    else if (playerLoc.X < origin.X && playerLoc.Y > origin.Y)
                    {
                        bulletRotation = (float)((Math.PI / 2.0) - Math.Asin(Vector2.Distance(playerLoc, opposite) / Vector2.Distance(playerLoc, enemyVect)) + (Math.PI / 2.0));
                    }
                    else if (playerLoc.X > origin.X && playerLoc.Y < origin.Y)
                    {
                        bulletRotation = (float)-Math.Asin(Vector2.Distance(playerLoc, opposite) / Vector2.Distance(playerLoc, enemyVect));
                    }
                    else if (playerLoc.X < origin.X && playerLoc.Y < origin.Y)
                    {
                        bulletRotation = (float)-((Math.PI / 2.0) - Math.Asin(Vector2.Distance(playerLoc, opposite) / Vector2.Distance(playerLoc, enemyVect)) + (Math.PI / 2.0));
                    }
                }
            }

        }


        /// <summary>
        /// move the bullets on screen using the bullets speed and who shot the bullet
        /// </summary>
        /// <param name="whatBullet"></param>
        /// <returns></returns>
        public float Move(bool whatBullet)
        {
            //player code
            if (whatBullet == true)
            {
                direction.X = (float)-(Math.Cos(bulletRotation) * playerSpeed);
                direction.Y = (float)-(Math.Sin(bulletRotation) * playerSpeed);
                
                newBulletVect.X += direction.X;
                newBulletVect.Y += direction.Y;

                return bulletRotation;

            }
            //enemy code
            else
            {
                direction.X = (float)(Math.Cos(bulletRotation));
                direction.Y = (float)(Math.Sin(bulletRotation));
                
                newBulletVect.X += direction.X;
                newBulletVect.Y += direction.Y;

                return bulletRotation;
            }
        }

        /// <summary>
        /// checks for collision and calls the shoot method based on who is shooting
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="current"></param>
        /// <param name="last"></param>
        /// <param name="targets"></param>
        /// <param name="enemies"></param>
        /// <param name="aiShoot"></param>
        /// <param name="playerLoc"></param>
        /// <param name="enemyVect"></param>
        /// <param name="enemyArmVect"></param>
        public void Update(GameTime gameTime, MouseState current, MouseState last, List<Rectangle> targets, List<Rectangle> enemies, bool aiShoot, Vector2 playerLoc, Vector2 enemyVect, Vector2 enemyArmVect)
        {
            //collision code
            if (targets != null)
            {
                foreach (Rectangle t in targets)
                {
                    if (bulletRect.Intersects(t))
                    {
                        bulletAlive = false;
                    }
                }
            }
            if (enemies != null)
            {
                foreach (Rectangle e in enemies)
                {
                    if (bulletRect.Intersects(e))
                    {
                        bulletAlive = false;
                    }
                }
            }


            //player shoot
            if (aiShoot == false)
            {
                if (current.LeftButton == ButtonState.Pressed && last.LeftButton == ButtonState.Released)
                {
                    if (positionCheck == 0)
                    {
                        newBulletVect = bulletVect;
                    }

                    Shoot(true, playerLoc, new Vector2(10000, 10000), new Vector2(10000, 10000));
                    
                    positionCheck++;
                    rotationCheck++;
                }
            }
            //ai shoot
            else if (aiShoot == true)
            {
                if (aiPositionCheck == 0)
                {
                    newBulletVect = bulletVect;
                }
               
                Shoot(false, playerLoc, enemyVect, enemyArmVect);
                
                aiPositionCheck++;
                aiRotationCheck++;
            }
        }

        /// <summary>
        /// draws the bullet based on the rotation 
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb,Color color)
        {
            if (bulletAlive == true)
            {
                direction.X = (float)(Math.Cos(bulletRotation) * bulletspeed);
                direction.Y = (float)(Math.Sin(bulletRotation) * bulletspeed);

                newBulletVect += direction;
                bulletRect = new Rectangle((int)newBulletVect.X, (int)newBulletVect.Y, bulletWidth, bulletHeight);

                sb.Draw(spriteSheet,
                    bulletRect,
                                    new Rectangle(51 * 5, 16 * 5, bulletWidth, bulletHeight),
                                    color,
                                    bulletRotation,
                                    new Vector2(bulletWidth / 2f, bulletHeight / 2f),
                                    SpriteEffects.None,
                                    0f);
            }
            else
            {
                bulletRect = new Rectangle(10000, 10000, bulletWidth, bulletHeight);
            }
        }
    }
}

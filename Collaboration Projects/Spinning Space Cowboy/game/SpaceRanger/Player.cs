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

namespace SpaceRanger
{ //player state enum
    public enum PlayerState
    {
        Idle,
        Aim,
        Shoot,
        Reload,
    }

    internal class Player
    {
        //timers
        private double reloadTimer = 0.0;

        //bullet counter
        private int bulletsCounter;

        //player state
        private PlayerState playerState;

        //Body Rectangle on sprite sheet
        private int playerWidth = 31 *5;
        private int playerHeight = 47 *5;
        private int playerOffsetY = 52 *5;
        private Vector2 bodyShoulder = new Vector2(10 *5, 19 *5);

        //Idle arm Rectangle on sprite sheet
        private int idleArmWidth = 35 * 5;
        private int idleArmHeight = 10 * 5; 
        private int idleArmOffsetY = 0 * 5;
        private Vector2 idleShoulder = new Vector2(2 * 5, 4 * 5);
        private Vector2 fidleShoulder = new Vector2(140, 20);

        //Shoot arm Rectangle on sprite sheet
        private int shootArmWidth = 47 * 5;
        private int shootArmHeight = 13 * 5;
        private int shootArmOffsetY = 13 * 5;
        private Vector2 shootShoulder = new Vector2(2 * 5, 7 * 5);
        private Vector2 fshootShoulder = new Vector2(200, 35);

        //Reload arm Rectangle on sprite sheet
        private int reloadArmWidth = 27 * 5;
        private int reloadArmHeight = 25 * 5;
        private int reloadArmOffsetY = 27 * 5;
        private Vector2 reloadShoulder = new Vector2(2 * 5, 19 * 5);
        private Vector2 freloadShoulder = new Vector2(100, 19 * 5);


        //gamestate
        public enum GameMode
        {
            Menu, Tutorial, Game, GameOver
        }
        public GameMode gameState;

        //Gametime variables
        private double gametimecurrent;
        private double gametimechange;
        private double gamet;
        private double gametc;

        //player health
        private int health = 3;

        //player's vectors
        private Vector2 playerVect;
        public Vector2 armVect;
        public Vector2 farmVect;

        //movement speed
        private const float playerSpeed = 10;
        private SpriteEffects flipSprite = SpriteEffects.None;
        private bool shot = false;
        private Vector2 resetv = new Vector2();

        //direction for arm movement
        private Vector2 direction;

        //mouse state for changing player states
        private MouseState mouseState;

        //Player Textures
        private Texture2D spriteSheet;

        //get for the health
        public int Health
        {
            get { return health; }
        }

        //player's position property
        public Vector2 PlayerVect
        {
            get { return playerVect; }
        }

        public int BulletsCounter
        {
            get { return bulletsCounter; }
        }

        //constuctor takes spritesheet and sets the player rectangle
        public Player(Texture2D body, Vector2 bodyVect, Texture2D arm)
        {
            //player body
            this.spriteSheet = body;
            this.playerVect = bodyVect;

            //player arm
            this.spriteSheet = arm;
            this.armVect = new Vector2(bodyVect.X + 13 * 5, bodyVect.Y + 19 * 5);
            this.farmVect = new Vector2(playerVect.X - 255, playerVect.Y + 120); ;

            direction = new Vector2();
        }

        /// <summary>
        /// resets all of the player values for every time a new wave happens
        /// </summary>
        /// <param name="body"></param>
        /// <param name="bodyVect"></param>
        /// <param name="arm"></param>
        public void Reset(Texture2D body, Vector2 bodyVect, Texture2D arm)
        {
            //player body
            this.spriteSheet = body;
            this.playerVect = bodyVect;

            //player arm
            this.spriteSheet = arm;
            this.armVect = new Vector2(bodyVect.X + 13 * 5, bodyVect.Y + 19 * 5);
            this.farmVect = new Vector2(bodyVect.X - 255, bodyVect.Y + 120);

            //this.bulletVect = new Vector2(armVect.X + 180, armVect.Y - 15);
            this.bulletsCounter = 0;
            playerState = PlayerState.Idle;

            this.health = 3;

            gamet = 0;
            gametc = 0;
            direction = resetv;
        }


        /// <summary>
        /// checks if the player has been hit and returns the bullet that hit the player
        /// </summary>
        /// <param name="playerVect"></param>
        /// <param name="bulletRects"></param>
        /// <returns></returns>
        public Bullet Hit(Vector2 playerVect, List<Bullet> bulletRects,bool god)
        {
            //checks for god mode
            if (god)
            { }
            else
            {
                Rectangle playerRect = new Rectangle((int)playerVect.X, (int)playerVect.Y, playerWidth, playerHeight);
                foreach (Bullet b in bulletRects)
                {
                    if (playerRect.Intersects(b.BulletRect))
                    {
                        health--;
                        return b;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// moves the player based on where the gun was shot and then slows the player down 
        /// </summary>
        /// <param name="mState"></param>
        /// <param name="pmState"></param>
        /// <param name="bulletRotation"></param>
        public void Move(MouseState mState, MouseState pmState, float bulletRotation)
        {
            if (mState.LeftButton == ButtonState.Pressed && pmState.LeftButton == ButtonState.Released&&bulletsCounter<6)
            {
                shot = true;

                direction.X = (float)-(Math.Cos(bulletRotation) * playerSpeed);
                direction.Y = (float)-(Math.Sin(bulletRotation) * playerSpeed);
            }

            if ((gametc + .8) <= gamet)
            {
                direction.X = direction.X * .6f;
                direction.Y = direction.Y * .6f;
            }

            if (shot)
            {
                //moves player directly
                playerVect.X += direction.X;
                playerVect.Y += direction.Y;

                farmVect.X += direction.X;
                farmVect.Y += direction.Y;
                armVect.X += direction.X;
                armVect.Y += direction.Y;


                //if it hits side walls 
                if (playerVect.X < 0 || playerVect.X >= Game1.sWidth - playerWidth )
                {
                    //flip the x movement direction
                    direction.X = -direction.X;

                    //if it goes past 0 again (
                    if (playerVect.X < 0)
                    {
                        playerVect.X += 1;
                        farmVect.X += 1;
                        armVect.X += 1;
                    }
                    
                    if(playerVect.X >= Game1.sWidth - playerWidth)
                    {
                        playerVect.X -= 1;
                        armVect.X -= 1;
                        farmVect.X -= 1;

                    }
                }

                //if hits the top walls
                if (playerVect.Y < -40 || playerVect.Y >= Game1.sHeight - playerHeight )
                {
                    //flip the y movement direction
                    direction.Y = -direction.Y;

                    if (playerVect.Y < 0)
                    {
                        playerVect.Y += 1;
                        farmVect.Y += 1;
                        armVect.Y += 1;
                    }

                    if (playerVect.Y >= Game1.sHeight - playerHeight)
                    {
                        playerVect.Y -= 1;
                        armVect.Y -= 1;
                        farmVect.Y -= 1;
                    }

                }
            }
            if ((gametc + .8) <= gamet)
            {
                gametc = gamet;
            }
        }

        /// <summary>
        /// changes player state based on player's actions and checks for reload when needed to stop the player from shooting
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="current"></param>
        /// <param name="last"></param>
        public void Update(GameTime gameTime, MouseState current, MouseState last)
        {
            mouseState = current;
            gamet = gameTime.TotalGameTime.Seconds;
            gameState = GameMode.Game;

            switch (playerState)
            {
                case PlayerState.Idle:
                    reloadTimer = 2.0;
                    //if press right mouse button 
                    if (current.RightButton == ButtonState.Pressed)
                    {
                        playerState = PlayerState.Aim;
                    }
                    if (current.LeftButton == ButtonState.Pressed && last.LeftButton == ButtonState.Released)
                    {
                        playerState = PlayerState.Shoot;
                    }

                    break;

                case PlayerState.Aim:
                    reloadTimer = 2.0;
                    if (current.RightButton == ButtonState.Released)
                    {
                        playerState = PlayerState.Idle;
                    }
                    else if(current.LeftButton == ButtonState.Pressed && last.LeftButton == ButtonState.Released)
                    {
                        playerState = PlayerState.Shoot;
                    }
                    break;

                case PlayerState.Shoot:

                    //shoots for .5 seconds and then determines where to switch the case to
                    gametimecurrent = gameTime.ElapsedGameTime.TotalSeconds;
                    gametimechange = gametimecurrent + 0.5;

                    bulletsCounter++;
                    if (last.LeftButton == ButtonState.Pressed && current.RightButton == ButtonState.Pressed)
                    {
                        playerState = PlayerState.Aim;
                    }
                    else if(last.LeftButton == ButtonState.Pressed && current.RightButton == ButtonState.Released)
                    {
                        playerState = PlayerState.Idle;
                    }

                    //if you have shot 6 times/ forced reload
                    if(bulletsCounter >= 6)
                    {
                        playerState = PlayerState.Reload;
                    }
                    break;

                case PlayerState.Reload:
                    //force a reload for however long timer goes
                    reloadTimer = reloadTimer - gameTime.ElapsedGameTime.TotalSeconds;

                    if (bulletsCounter >= 0)
                    {
                        if (reloadTimer <= 0)
                        {
                            bulletsCounter = 0;
                            playerState = PlayerState.Idle;
                        }
                    }

                    break;
            }
            last = current;
        }


        /// <summary>
        /// Draw method for the player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="flipSprite"></param>
        /// <param name="sb"></param>
        public void Draw(Player player,SpriteBatch sb,Texture2D crosshair,MouseState mState)
        {
            //Draw Astronaut
            player.DrawBody(player, sb);

            switch (playerState)
            {
                case PlayerState.Idle:
                    player.DrawIdle(sb);

                    break;

                case PlayerState.Aim:
                    player.DrawAim(sb,crosshair,mState);

                    break;

                case PlayerState.Shoot:
                    player.DrawShoot(sb);

                    break;

                case PlayerState.Reload:
                    player.DrawReload(sb);

                    break;
            }
        }

        /// <summary>
        /// draws the body of the player
        /// </summary>
        public void DrawBody(Player player, SpriteBatch sb)
        {
            if(mouseState.X < PlayerVect.X)
            {
                 flipSprite = SpriteEffects.FlipHorizontally;
            }
            else
            {
                flipSprite = SpriteEffects.None;
            }

            sb.Draw(
            spriteSheet,                                                  // - The texture to draw
            playerVect,                                                   // - The location to draw on the screen
            new Rectangle(0, playerOffsetY, playerWidth, playerHeight),   // - The "source" rectangle
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
        public void DrawIdle(SpriteBatch sb)
        {
            if (mouseState.X < PlayerVect.X)
            {
                 flipSprite = SpriteEffects.FlipHorizontally;
                
                sb.Draw(
                spriteSheet,
                armVect,
                new Rectangle(0, idleArmOffsetY, idleArmWidth, idleArmHeight),
                Color.White,
                0,
                fidleShoulder,
                1.0f,
                flipSprite,
                1);
                

            }
            else
            {
                flipSprite = SpriteEffects.None;
                sb.Draw(
                spriteSheet,
                armVect,
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
        /// draws arm with the aim dots
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="sb"></param>
        public void DrawAim(SpriteBatch sb,Texture2D crosshair,MouseState mState)
        {
            if (mouseState.X < PlayerVect.X)
            {
                flipSprite = SpriteEffects.FlipHorizontally;

                sb.Draw(
                spriteSheet,
                armVect,
                new Rectangle(0, idleArmOffsetY, idleArmWidth, idleArmHeight),
                Color.White,
                0,
                fidleShoulder,
                1.0f,
                flipSprite,
                1);


            }
            else
            {
                flipSprite = SpriteEffects.None;
                sb.Draw(
                spriteSheet,
                armVect,
                new Rectangle(0, idleArmOffsetY, idleArmWidth, idleArmHeight),
                Color.White,
                0,
                idleShoulder,
                1.0f,
                flipSprite,
                1);
            }
            sb.Draw(crosshair, new Rectangle((mState.X) - (crosshair.Width / 2), mState.Y - (crosshair.Height / 2), 40, 40), Color.White);
        }

        /// <summary>
        /// draws arm with muzzle flash
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="sb"></param>
        public void DrawShoot(SpriteBatch sb)
        {
            if (mouseState.X < PlayerVect.X)
            {
                 flipSprite = SpriteEffects.FlipHorizontally;
                sb.Draw(
                spriteSheet,
                armVect,
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
                spriteSheet,
                armVect,
                new Rectangle(0, shootArmOffsetY, shootArmWidth, shootArmHeight),
                Color.White,
                0,
                shootShoulder,
                1.0f,
                flipSprite,
                1);
            }
        }

        /// <summary>
        /// draws arm reload
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="sb"></param>
        public void DrawReload(SpriteBatch sb)
        {
            if (mouseState.X < PlayerVect.X)
            {
                sb.Draw(
                spriteSheet,
                armVect,
                new Rectangle(0, reloadArmOffsetY, reloadArmWidth, reloadArmHeight),
                Color.White,
                0,
                freloadShoulder,
                1.0f,
                flipSprite,
                1);
            }
            else
            {
                sb.Draw(
                spriteSheet,
                armVect,
                new Rectangle(0, reloadArmOffsetY, reloadArmWidth, reloadArmHeight),
                Color.White,
                0,
                reloadShoulder,
                1.0f,
                flipSprite,
                1);
            }
        }
    }
}

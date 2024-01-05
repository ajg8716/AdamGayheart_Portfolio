using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
//using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
//using System.Numerics;
using System.Reflection.Metadata;

namespace SpaceRanger
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //textures for ui
        Texture2D titleText;
        Texture2D gameText;
        Texture2D exitText;
        Texture2D bg;

        Texture2D ResumeButton;
        Texture2D playAgain;
        Texture2D tutorial;

        Texture2D heartSprite;
        Texture2D bulletCounterSprite;
        Texture2D crosshair;

        Texture2D winText;
        Texture2D loseText;
        

        //player sheet
        Texture2D playerSheet;


        //fonts for menu
        public static SpriteFont font;
        public static SpriteFont font2;
        public static SpriteFont wavetext;

        //menu rectangles for art and buttons
        Rectangle titleRect;
        Rectangle tutorialButton;
        Rectangle gameButton;
        Rectangle exitButton;
        Rectangle bgRect;
        Rectangle playerRect;


        //UI sprite rects
        List<Rectangle> heartRects;
        List<Rectangle> bulletCounterRects;

       
        //Pause and gameover rectangles for art and buttons
        Rectangle resumeRec;
        Rectangle playAgainRec;


        //screen width and height
        public static int sWidth;
        public static int sHeight;


        //keyboard and mouse states
        KeyboardState kState;
        KeyboardState pkState;

        MouseState mState;
        MouseState pmState;


        //player things
        Player player;
        //are used
        Vector2 playerPos = new Vector2(960,540);
        //check for godmode
        bool god;


        //checking if player has won or lost
        bool isWin;

        //bullet lists and rotations
        List<Bullet> playerBullets = new List<Bullet>();
        List<Bullet> enemyBullets = new List<Bullet>();
        float bulletRotation;

        //File Io
        IoMannager Io;
        int lvl;

        //target things
        Texture2D targetsprite;
        List<Rectangle> targetsRecs;
        List<Target> targets;

        //enemy things
        List<Enemies> enemies;
        Texture2D enemySprite;
        List<Rectangle> enemyRecs;
        private Texture2D enemyArmSprite;
        double cooldowntime;



        //enum for each of the menu states(gameplay, menu, credits, tutorial, game over)
        public enum GameMode 
        { 
            Menu, Tutorial, Game, GameOver , Pause
        }
        public GameMode gameState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// sets the size of the screen along with the starting state and sets up the file io as well as some rectangles for the ui during gameplay
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //changeing windows size 
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();
            sWidth = _graphics.GraphicsDevice.Viewport.Width;
            sHeight = _graphics.GraphicsDevice.Viewport.Height;

            gameState = GameMode.Menu;

            god = false;
            lvl = 0;
            cooldowntime = 0;

            //makes a new Iomannager
            Io = new IoMannager(sWidth,sHeight);
            //reads the level 0
            Io.reader(lvl);

            //UI heart rect
            heartRects = new List<Rectangle>() { new Rectangle(20, 20, 100, 100), new Rectangle(120 + 10, 20, 100, 100), new Rectangle(220 + 20, 20, 100, 100) };

            //UI ammo rect
            bulletCounterRects = new List<Rectangle>();

            base.Initialize();
        }

        /// <summary>
        /// loads all of the textures needed as well as creating rectangles for them along with the creation of the player, targets, and enemies
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //loading in all of the textures 
            titleText = Content.Load<Texture2D>("space_title");
            gameText = Content.Load<Texture2D>("space_ranger_button");
            exitText = Content.Load<Texture2D>("exit_button");
            
            bg = Content.Load<Texture2D>("Stars2");
            
            font = Content.Load<SpriteFont>("File");
            font2 = Content.Load<SpriteFont>("font2");

            ResumeButton = Content.Load<Texture2D>("Resume");
            playAgain = Content.Load<Texture2D>("playAgain");
            tutorial = Content.Load<Texture2D>("tutorial_button");

            winText = Content.Load<Texture2D>("You_Win_Sheet");
            loseText = Content.Load<Texture2D>("game_over");

            targetsprite = Content.Load<Texture2D>("target");

            crosshair = Content.Load<Texture2D>("crosshair");

            heartSprite = Content.Load<Texture2D>("heart");

            //player sprite takes info from the astronaut sheet
            playerSheet = Content.Load<Texture2D>("Astronaut_Sheet");

            //enemy sprite take info from the bandit sheet
            enemySprite = Content.Load<Texture2D>("Bandit_Sheet");

            //bullet counter sprite
            bulletCounterSprite = Content.Load<Texture2D>("Round_Sheet");

            wavetext= Content.Load<SpriteFont>("wavefont");

            //Locations of the main menu buttons and background
            titleRect = new Rectangle(sWidth / 2 - titleText.Width / 2, 50, titleText.Width, titleText.Height);
            gameButton = new Rectangle(sWidth / 2 - gameText.Width / 2, 320, gameText.Width, gameText.Height);
            tutorialButton = new Rectangle(sWidth / 2 - gameText.Width / 2, 550, gameText.Width, gameText.Height);
            exitButton = new Rectangle(sWidth / 2 - gameText.Width / 2, 780, gameText.Width, gameText.Height);
            
            bgRect = new Rectangle(0, 0, sWidth, sHeight);

            //Locations of the Pause and gameover buttons
            resumeRec = new Rectangle(sWidth / 2 - ResumeButton.Width / 2, 220, ResumeButton.Width, ResumeButton.Height);
            playAgainRec = new Rectangle(sWidth / 2 - ResumeButton.Width / 2, 220, ResumeButton.Width, ResumeButton.Height);


            //Player creation
            player = new Player(playerSheet, playerPos, playerSheet);


            //creating all of the targets for tutorial 
            targets = new List<Target>();
            Target target = new Target(targetsprite,sWidth,sHeight);
            target.targetMaker();
            targetsRecs = target.TargetsRects;
            // loop to add targets to targets lists
            for(int count =0;count< target.Targetcount;count++)
            {
                targets.Add(new Target(targetsprite, sWidth, sHeight));
            }


            //creating enemies
            enemies = new List<Enemies>();
            enemyRecs=Io.EnemyMaker(enemySprite);
            //test enemy
            int enemyWidth = 31 * 5;
            int enemyHeight = 47 * 5;
            int enemyOffsetY = 52 * 5;
            Vector2 enemyPos = new Vector2(500, 500);
        }

        /// <summary>
        /// using a switch for the fsm we have conditionals for checking button presses and conditionals that check for win/lose conditions and switch the state accordingly
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //assigns keyboard and mouse variables
            kState = Keyboard.GetState();
            mState = Mouse.GetState();


            // FSM- uses gameState(Menu,Tutorial,Pause,Game,GameOver)
            switch (gameState)
            {
                //Menu: Has conditionals that act when the player presses a button as well as resetting everythign back to its default state
                case GameMode.Menu:
                    IsMouseVisible = true;

                    //checking if a button was pushed by checking for position and if the player has clicked the mouse
                    if ((mState.X >= gameButton.X && mState.X <= gameButton.Width + gameButton.X 
                        && mState.Y >= gameButton.Y && mState.Y <= gameButton.Height + gameButton.Y)
                        && mState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameMode.Game;
                    }
                    if ((mState.X >= exitButton.X && mState.X <= exitButton.Width + exitButton.X
                        && mState.Y >= exitButton.Y && mState.Y <= exitButton.Height + exitButton.Y)
                        && mState.LeftButton == ButtonState.Pressed && pmState.LeftButton == ButtonState.Released)
                    {
                        System.Environment.Exit(0);           
                    }
                    if ((mState.X >= tutorialButton.X && mState.X <= tutorialButton.Width + tutorialButton.X
                        && mState.Y >= tutorialButton.Y && mState.Y <= tutorialButton.Height + tutorialButton.Y)
                         && mState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameMode.Tutorial;
                    }

                    //resetting everything in case the player goes back to the main menu
                    player.Reset(playerSheet, playerPos, playerSheet);
                    enemies.Clear();
                    enemyRecs.Clear();
                    playerBullets.Clear();
                    enemyBullets.Clear();
                    lvl = 0;
                    Io.reader(lvl);
                    break;

                //Tutorial: turns off mouse cursor you the player can aim with the in-game cursor, resets and makes the bullet UI,
                //using conditionals and loops checks for bullets that are on screen and targets that have been hit
                case GameMode.Tutorial:
                    IsMouseVisible = false;

                    //clear the ammo list so it can be updated;
                    bulletCounterRects.Clear();

                    //update the ammo 
                    CreateAmmoRects();

                    //adds bullet to list when shooting gun
                    if (mState.LeftButton == ButtonState.Pressed && pmState.LeftButton == ButtonState.Released && player.BulletsCounter < 6)
                    {
                        if (mState.X < player.PlayerVect.X)
                        {
                           playerBullets.Add(new Bullet(playerSheet, player.farmVect));
                        }
                        else
                        {
                            playerBullets.Add(new Bullet(playerSheet, player.armVect));
                        }
                    }


                    //updates each of the targets
                    for (int count = 0; count < targets.Count; count++)
                    {
                        foreach (Bullet b in playerBullets)
                        {
                            targets[count].Update(gameTime, targetsRecs[count], b.BulletRect);
                        }
                    }

                    //for returning to the main menu
                    if (kState.IsKeyDown(Keys.Enter))
                    {
                        gameState = GameMode.Menu;
                    }


                    //updates each bullets position and gets the rotation
                    foreach (Bullet b in playerBullets)
                    {
                        b.Update(gameTime, mState, pmState, targetsRecs, null, false, player.PlayerVect, new Vector2(10000, 10000), new Vector2(10000, 10000));
                        bulletRotation = b.Move(true);
                        foreach(Enemies e in enemies)
                        {
                            if (b.BulletRect.Intersects(e.EnemyRect))
                            {
                                b.BulletAlive = false;
                            }
                        }
                    }

                    //updating the player's fields
                    player.Update(gameTime, mState, pmState);

                    //moves the players location 
                    player.Move(mState, pmState, bulletRotation);
                    break;

                //Pause: acts as a pause screen with some buttons to return to the menu or to unpause
                case GameMode.Pause:
                    IsMouseVisible = true;

                    //checking mouse location and if pressed for resume 
                    if ((mState.X >= resumeRec.X && mState.X <= resumeRec.Width + resumeRec.X
                         && mState.Y >= resumeRec.Y && mState.Y <= resumeRec.Height + resumeRec.Y)
                         && mState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameMode.Game;
                    }

                    //checking mouse location and if pressed for quit
                    if ((mState.X >= exitButton.X && mState.X <= exitButton.Width + exitButton.X
                         && mState.Y >= exitButton.Y && mState.Y <= exitButton.Height + exitButton.Y)
                         && mState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameMode.Menu;
                    }
                    break;

                //Game: 
                case GameMode.Game:

                    IsMouseVisible = false;

                    //clear the ammo list so it can be updated;
                    bulletCounterRects.Clear();

                    //update the ammo tracker
                    CreateAmmoRects();
                    
                    //checking for moving to the next wave 
                    if (enemies.Count == 0)
                    {
                        foreach (Rectangle rec in enemyRecs)
                        {
                            enemies.Add(new Enemies(enemySprite,new Vector2(rec.X,rec.Y), enemyArmSprite, 1,rec));
                        }
                        lvl += 1;
                    }


                    //taking the enemy's rectangles and adding them to their own list
                    for(int ecount=0;ecount<enemies.Count;ecount++)
                    {
                        enemyRecs[ecount] = enemies[ecount].EnemyRect;
                    }


                    //checking if player is dead
                    if (player.Health == 0)
                    {
                        isWin = false;
                        gameState = GameMode.GameOver;
                    }


                    //adding bullets to screen
                    if (mState.LeftButton == ButtonState.Pressed && pmState.LeftButton == ButtonState.Released&&player.BulletsCounter<6)
                    {
                        if (mState.X < player.PlayerVect.X)
                        {
                            playerBullets.Add(new Bullet(playerSheet, player.farmVect));
                        }
                        else
                        {
                            playerBullets.Add(new Bullet(playerSheet, player.armVect));
                        }
                    }

                    player.Update(gameTime, mState, pmState);
                    


                    //check if the player has activated god mode
                    if (kState.IsKeyDown(Keys.G)&&pkState.IsKeyUp(Keys.G))
                    {
                        god = !god;
                    }


                    //checks if the player has been hit then removes that bullet from the list so it despawns
                    Bullet remove = player.Hit(player.PlayerVect, enemyBullets, god);
                    if (remove != null)
                    {
                        enemyBullets.Remove(remove);
                    }


                    //updating enemies
                    foreach (Enemies e in enemies)
                    {
                        //checks if an enemy can shoot or if it should keep moving towards the player
                        bool shootCheck = e.Movement(player.PlayerVect);
                        cooldowntime += gameTime.ElapsedGameTime.TotalMilliseconds;
                        //check if enemy can shoot yet based on cooldown
                        if (shootCheck == true && cooldowntime >= 6000 && e.EnemyRect.X<sHeight&& e.EnemyRect.X > 0 && e.EnemyRect.Y < sWidth && e.EnemyRect.Y>0) 
                        {
                            //flipping sprite and changing states
                            if (e.enemyArmVect.X > player.armVect.X)
                            {
                                enemyBullets.Add(new Bullet(enemySprite, e.fenemyArmVect));
                            }
                            else 
                            {
                                enemyBullets.Add(new Bullet(enemySprite, e.enemyArmVect));
                            }
                            e.enemyState = EnemyState.Shoot;
                            //reset cooldown
                            cooldowntime = 0;
                        }
                        else
                        {
                            e.enemyState = EnemyState.Idle;
                        }
                        //updates each enemy bullet
                        foreach (Bullet b in enemyBullets)
                        {
                            b.Update(gameTime, mState, pmState, null, enemyRecs, true, player.PlayerVect, e.EnemyVect, e.enemyArmVect);
                            bulletRotation = b.Move(false);
  
                        }
                    }


                    //checking if an enemy has been hit by a player bullet, iif so then that enemy is removed from the list, despawning it
                    for(int ene=0; ene< enemies.Count ; ene++)
                    {
                        foreach (Bullet b in playerBullets)
                        {
                            if (ene < enemies.Count)
                            {
                                bool eneAlive = enemies[ene].Update(enemies[ene].EnemyRect, gameTime, b.BulletRect);
                                if (!eneAlive)
                                {
                                    enemies.Remove(enemies[ene]);
                                }
                            }
                        }
                    }


                    //updates each bullet's position and gets the rotation
                    foreach (Bullet b in playerBullets)
                    {
                        b.Update(gameTime, mState, pmState, null, enemyRecs, false, player.PlayerVect, new Vector2(10000, 10000), new Vector2(10000, 10000));
                        bulletRotation = b.Move( true);
                    }


                    //moves the players location 
                    player.Move(mState, pmState, bulletRotation);


                    // if p is pressed then will go to the pause menu
                    if (kState.IsKeyDown(Keys.P))
                    {
                        gameState = GameMode.Pause;
                    }


                    //checking if player has won based on enemies left and waves left
                    if (enemies.Count == 0&& lvl!=0)
                    {
                        Io.reader(lvl);
                        if (Io.Endgame)
                        {
                            isWin = true;
                            gameState = GameMode.GameOver;
                        }
                        else
                        {
                            enemies.Clear();
                            enemyRecs.Clear();
                            enemyRecs = Io.EnemyMaker(enemySprite);
                            playerBullets.Clear();
                            enemyBullets.Clear();
                        }

                    }
                    break;

                //Game Over: checks if the player has won or lost the game
                case GameMode.GameOver:
                    IsMouseVisible = true;

                    //if playagain button is pressed then resets game 
                    if ((mState.X >= playAgainRec.X && mState.X <= playAgainRec.Width + playAgainRec.X
                        && mState.Y >= playAgainRec.Y && mState.Y <= playAgainRec.Height + playAgainRec.Y)
                        && mState.LeftButton == ButtonState.Pressed)
                    {
                        lvl = 0;
                        Target.Counter = 0;
                        player.Reset(playerSheet, playerPos, playerSheet);
                        gameState = GameMode.Game;
                        enemies.Clear();
                        enemyRecs.Clear();
                        playerBullets.Clear();
                        enemyBullets.Clear();

                    }

                    //checking mouse location and if pressed for quit
                    if ((mState.X >= exitButton.X && mState.X <= exitButton.Width + exitButton.X
                         && mState.Y >= exitButton.Y && mState.Y <= exitButton.Height + exitButton.Y)
                         && mState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameMode.Menu;
                    }
                    break;

            }
            pkState = kState;
            pmState = mState;
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws each bullet, enemy, target, and the player depending on the gamestate and the properties of each list
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            switch (gameState)
            {
                //Menu: draws the menu buttons
                case GameMode.Menu:

                    //background
                    _spriteBatch.Draw(bg, bgRect, Color.White);
                    //title
                    _spriteBatch.Draw(titleText, titleRect, Color.White);
                    //game/play
                    _spriteBatch.Draw(gameText, gameButton, Color.White);
                    //exit
                    _spriteBatch.Draw(exitText, exitButton, Color.White);

                    _spriteBatch.Draw(tutorial, tutorialButton, Color.White);
                   
                    break;

                //Tutorial: draws the tutorial text, targets, bullets, and ui and of couse the player
                case GameMode.Tutorial:
                    _spriteBatch.Draw(bg, bgRect, Color.White);
                    _spriteBatch.DrawString(Game1.font, "Target Hits: " + Target.Counter, new Vector2(20, 20), Color.White);
                    _spriteBatch.DrawString(Game1.font2, "As a space ranger, your only source of movement is the recoil from your gun!\nClick on the screen to shoot in any direction! Press Right click for aim ", new Vector2(50, 50), Color.White);
                    _spriteBatch.DrawString(Game1.font2, "\nThere are only targets here now, but you'll need to be able to defeat space bandits in the game!", new Vector2(50, 80), Color.White);
                    _spriteBatch.DrawString(Game1.font2, "\nPress enter to return to the main menu.", new Vector2(50, 110), Color.White);
                    
                    
                    //draws the targets 
                    for (int count = 0; count < targets.Count; count++)
                    {
                        targets[count].draw(_spriteBatch, targetsRecs[count]);
                    }
                    

                    //draws bullets
                    foreach (Bullet b in playerBullets)
                    {
                        b.Draw(_spriteBatch,Color.White);
                    }


                    //draws player
                    player.Draw(player, _spriteBatch, crosshair, mState);


                    //draws the ammo count
                    foreach (Rectangle h in bulletCounterRects)
                    {
                        _spriteBatch.Draw(bulletCounterSprite, h, Color.White);
                    }
                    break;

                //Pause: draws the buttons
                case GameMode.Pause:

                    //the resumeButton
                    _spriteBatch.Draw(ResumeButton, resumeRec, Color.White);
                    
                    //exit button
                    _spriteBatch.Draw(exitText, exitButton, Color.White);
                    break;

                //Game: draws the player, targets, bullets, and ui
                case GameMode.Game:
                    //background 
                    _spriteBatch.Draw(bg, bgRect, Color.White);


                    //draw enemies
                    foreach (Enemies e in enemies)
                    {
                        e.Draw(e, _spriteBatch, player.PlayerVect);
                    }


                    //draws bullets
                    foreach(Bullet b in playerBullets)
                    {
                        b.Draw(_spriteBatch,Color.White);
                    }
                    foreach (Bullet b in enemyBullets)
                    {
                        b.Draw(_spriteBatch, Color.White);
                    }


                    //draws player 
                    player.Draw(player, _spriteBatch, crosshair, mState);

                    
                    //draws player's hearts based on player health
                    if (player.Health == 3)
                    {
                        foreach (Rectangle h in heartRects)
                        {
                            _spriteBatch.Draw(heartSprite, h, Color.White);
                        }
                    }
                    else if (player.Health == 2)
                    {
                        _spriteBatch.Draw(heartSprite, heartRects[0], Color.White);
                        _spriteBatch.Draw(heartSprite, heartRects[1], Color.White);
                        _spriteBatch.Draw(heartSprite, heartRects[2], Color.Gray);
                    }
                    else if (player.Health == 1)
                    {
                        _spriteBatch.Draw(heartSprite, heartRects[0], Color.White);
                        _spriteBatch.Draw(heartSprite, heartRects[1], Color.Gray);
                        _spriteBatch.Draw(heartSprite, heartRects[2], Color.Gray);
                    }

                    //draws players ammo count based on how many bullets left before reload
                    foreach (Rectangle h in bulletCounterRects)
                    {
                        _spriteBatch.Draw(bulletCounterSprite, h, Color.White);
                    }

                    _spriteBatch.DrawString(wavetext, string.Format("" + (lvl-1)) , new Vector2(sWidth/2, 20), Color.Orange);
                    _spriteBatch.DrawString(Game1.font2, " Press G for god mode,\n P to pause ", new Vector2(50, 150), Color.White);

                    break;

                //Game Over: drawws buttons and depending on if the player ahs won or lost draws the appropriate sprite
                case GameMode.GameOver:
                    //draws win or lose text
                    if (isWin == true)
                    {
                        _spriteBatch.Draw(bg, bgRect, Color.White);
                        
                        //draws playagain button
                        _spriteBatch.Draw(playAgain, playAgainRec, Color.White);
                        
                        //draws exit button
                        _spriteBatch.Draw(exitText, exitButton, Color.White);
                        
                        //draws win
                        _spriteBatch.Draw(winText, new Rectangle(sWidth / 2 - (winText.Width/8) + 40, 400, 200, 200), Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(bg, bgRect, Color.White);
                        
                        //draws playagain button
                        _spriteBatch.Draw(playAgain, playAgainRec, Color.White);
                        
                        //draws exit button
                        _spriteBatch.Draw(exitText, exitButton, Color.White);
                        
                        //draws lose
                        _spriteBatch.Draw(loseText, new Rectangle(sWidth/2 - (loseText.Width/4), 400, 200, 200), Color.Red);
                        _spriteBatch.DrawString(wavetext, string.Format("you reached wave :" + (lvl - 1)), new Vector2((sWidth / 2)-200, 20), Color.Orange);
                    }
                    break;
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// This is a helper method to find out the ammmo count of the player in game and tutorial state
        /// </summary>
        public void CreateAmmoRects()
        {
            //initial position
            int pos = 100;

            for (int i = 0; i < 6 - player.BulletsCounter; i++)
            {
                //add to the ammo counter
                bulletCounterRects.Add(new Rectangle((_graphics.PreferredBackBufferWidth - pos), 20, 100, 100 ));

                //update for a new position
                pos += 100;
            }
        }
    }
}
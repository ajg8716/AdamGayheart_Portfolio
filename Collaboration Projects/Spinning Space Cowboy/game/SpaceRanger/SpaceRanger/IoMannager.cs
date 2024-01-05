using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceRanger
{
    internal class IoMannager
    {


        //board
        private List<List<string>> board;
        private List<string> boardline;

        //board spawning location
        private List<List<Vector2>> boardLocations;
        private List<Vector2> boardLocationsrows;

        //the row and columns each level board has 
        private int rows;
        private int columns;

        //list of enemy rectangles
        private List<Rectangle> enemyRects = new List<Rectangle>();
        //holds enemy rectangle before getting added to list 
        Rectangle enemyRect;

        //list of barrier rectangles
        private List<string> barrierTypes = new List<string>();
        

        //for breaking down incoming line from text file
        private string line;
        private string[] lineinfo;

        //stream reader
        private StreamReader sr;

        //check for next level 
        private bool nextLevel;
        private bool endgame;
        //the string of the level 
        private string currentlevelChk;

        //the amount of line are being
        private int linecount;

        string srcheck;

        /// <summary>
        /// gives what is on certain spots on the board 
        /// </summary>
        public List<List<string>> Board
        {
            get { return board; }
        }

        /// <summary>
        /// gives the locations of each of the spots on the board
        /// </summary>
        public List<List<Vector2>> BoardLocations
        {
            get { return boardLocations; }
        }

        //the amount of rows of the board
        public int Rows
        {
            get { return rows; }
        }

        //the amount of columns of the board
        public int Columns
        {
            get { return columns; }
        }

        /// <summary>
        /// if there is no more levels to read
        /// </summary>
        public bool Endgame
        {
            get { return endgame; }
        }

        /// <summary>
        /// temporary list that reads out all the barriers for the level to then be created later
        /// </summary>
        public List<string> BarrierTypes
        {
            get { return barrierTypes; }
        }
        

        /// <summary>
        /// need the screen width and height 
        /// makes all the spots vectors on the board
        /// </summary>
        /// <param name="sWidth"></param>
        /// <param name="sHeight"></param>
        public IoMannager(int sWidth,int sHeight)
        {
            //its what is one the board in what location
            board = new List<List<string>>();
            //its for one line of the board 
            boardline = new List<string>();
            //its for all the vector location on the board
            boardLocations = new List<List<Vector2>>();
            //its for one line of the boards locations 
            boardLocationsrows = new List<Vector2>();
            //how many rows on the board there are 
            rows = 5;
            //how many columns there are on the board 
            columns = 5;

            endgame = false;

            //the witdh broken into fiths 
            float switdthchunk = ((sWidth )/5f);
            // the height is broken into 1.75
            float sheightchunk = ((sHeight)/1.75f);
            //starts at -1 so that we can have off screen rows is adjusted for starting at -1
            for (int row = -1; row < rows-1; row++)
            {
                //makes a new row
                boardLocationsrows = new List<Vector2>();
                //start at -1 so that we can have off screen columns is adjusted for starting at -1
                for (int col = -1; col < columns-1; col++)
                {
                    //makes the vectors of each spot on the board 
                    Vector2 vec = new Vector2((sheightchunk / 3) + (sheightchunk * col), (switdthchunk / 3) + (switdthchunk * row));
                    
                    //addeds it to the row
                    boardLocationsrows.Add(vec);
                    
                }

                boardLocations.Add(boardLocationsrows);
            }

        }

        public void reader(int level)
        {
            //used for looping 
            linecount = 0;
            //used to loop to the right level 
            nextLevel = false;
            //clears board to ready for new one 
            board.Clear();
            try
            {
                //sets up the reader for the board
                sr = new StreamReader("..\\..\\..\\Spawns.txt");
                //makes level string to for the later line setter
                currentlevelChk = string.Format("level {0}", level);
                //the text is read when there is no more level to be read
                string endgametxt = "end";

                //if level 0/ Tutorial
                if (currentlevelChk == sr.ReadLine() )
                {



                    //fills list with enemies 
                    while (linecount<columns )
                    {
                        //reads line 
                        line = sr.ReadLine();
                        //splites up the entitys 
                        lineinfo = line.Split(",");
                        //makes a new boardline so the boardlines stay correct in board
                        boardline = new List<string>();
                        //for length of the lineinfo it loops 
                        for (int count = 0; count < lineinfo.Length; count++)
                        {
                            //adds each info to the board line 
                            boardline.Add(lineinfo[count]);
                        }
                        //adds new board line to the board
                        board.Add(boardline);
                        //increase the board line count
                        linecount++;
                    }


                }
                else
                {

                    //will got through the txt file till reching desired level
                    do
                    {
                        //get line 
                        srcheck = sr.ReadLine();
                        //check if at the next level if so make nextlevel true ending loop 
                        if (currentlevelChk == srcheck)
                        {
                            nextLevel = true;
                            endgame = false;
                        }
                        //check if reader has gotten to the end if so make nextlevel and endgame true and ends loop 
                        if (endgametxt== srcheck)
                        {
                            nextLevel = true;
                            endgame = true;
                        }

                    }
                    while (!nextLevel);

                    //if endgame is true skips over it 
                    if (!endgame)
                    {
                        //loops through all of the columes
                        while (linecount < columns)
                        {
                            //reads line 
                            line = sr.ReadLine();
                            //splites up the entitys 
                            lineinfo = line.Split(",");
                            //makes a new boardline so the boardlines stay correct in board
                            boardline = new List<string>();
                            //for length of the lineinfo it loops 
                            for (int count = 0; count < lineinfo.Length; count++)
                            {
                                //adds each info to the board line 
                                boardline.Add(lineinfo[count]);
                            }
                            //adds new board line to the board
                            board.Add(boardline);
                            //increase the board line count
                            linecount++;
                        }




                    }




                }
                //cloes stream reader
                sr.Close();
            }
            catch
            {
                //cloes stream reader
                sr.Close();
            }
        }


        /// <summary>
        /// makes the enemy rectangles, need the Texture2D enemy sprite
        /// </summary>
        /// <param name="enemySprite"></param>
        /// <returns></returns>
        public List<Rectangle> EnemyMaker(Texture2D enemySprite)
        {
            //for each row on the board
            for (int row = 0; row < rows; row++)
            {
                //for each colume on the board 
                for (int col = 0; col < columns; col++)
                {
                    if (board[row][col] == "e")
                    {
                        //makes that new enemy rectangle/ entity rectanlge  
                        enemyRect = new Rectangle((int)boardLocations[row][col].X, (int)boardLocations[row][col].Y, enemySprite.Width, enemySprite.Height);
                        //adds it to the list of enemies rectangles/ entity rectangles
                        enemyRects.Add(enemyRect);
                    }
                }
            }
            //returns the list
            return enemyRects;
        }

        public List<string> BarrierMaker()
        {
            //for each row on the board
            for (int row = 0; row < rows; row++)
            {
                //for each colume on the board 
                for (int col = 0; col < columns; col++)
                {
                    //switch the type of barrier to add based off what is read
                    switch (board[row][col])
                    {
                        //first small rock
                        case "1":
                            barrierTypes.Add("1");
                            break;
                        //second small rock
                        case "2":
                            barrierTypes.Add("2");
                            break;
                        //third small rock
                        case "3":
                            barrierTypes.Add("3");
                            break;
                        //huge rock
                        case "4":
                            barrierTypes.Add("4");
                            break;
                        //long rock
                        case "5":
                            barrierTypes.Add("5");
                            break;
                        //satelite
                        case "6":
                            barrierTypes.Add("6");
                            break;
                    }
                }
            }
            //return the list of the barrier types
            return barrierTypes;
        }





    }
}


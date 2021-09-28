using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ChessEngine001
{
    class Board
    {
        private Piece[,] board;
        private bool whiteToPlay;
        public Color ColorToPlay;

        public bool CanCastleQueensideWhite { get; set; }
        public bool CanCastleQueensideBlack { get; set; }
        public bool CanCastleKingsideWhite { get; set; }
        public bool CanCastleKingsideBlack { get; set; }
        public Coord EnPassantTarget { get; set; }
        public Move MostRecentMove { get; set; }



        public Piece this[int row,int column]
        {
            get => board[row,column];
            set => board[row, column] = value;

        }

        public Piece this[Coord coord]
        {
            get => this[coord.Row, coord.Col];
            set => this[coord.Row, coord.Col] = value;
        }

        public Piece this[int index]
        {
            get => this[index / 8, index % 8];
            set => this[index / 8, index % 8] = value;

        }
        public Piece this[string coord]
        {
            get => this[new Coord(coord)];
            set => this[new Coord(coord)] = value;
        }


        public Board() : this("rnbqkbnr/pppppppp/--------/--------/--------/--------/PPPPPPPP/RNBQKBNR")
        {

        }

        public Board(string simpleString)
        {
            board = new Piece[8, 8];
            whiteToPlay = true;
            ColorToPlay = Color.White;

            CanCastleQueensideBlack = true;
            CanCastleKingsideBlack = true;
            CanCastleQueensideWhite = true;
            CanCastleKingsideWhite = true;


            //Console.WriteLine("simpleString\n" + simpleString);

            simpleString = simpleString.Replace("/", "");
            //Console.WriteLine( "simpleString\n" + simpleString);

            var charArray = simpleString.ToCharArray();

            for( int i = 0; i < 64; i++)
            {
                if( i%8 == 0)
                {
                    //Console.WriteLine();
                }
                //Console.Write(" {0} ", charArray[i]);

                int row = 7 - i / 8;
                int col = i % 8;

                board[row, col] = new Piece(charArray[i]);
            }
        }

        public void MakeMove(Move move)
        {

            // If the king moves, set the castling rights to false
            if( this[move.StartSquare].Type == Type.King && ColorToPlay == Color.White )
            {
                CanCastleKingsideWhite = false;
                CanCastleQueensideWhite = false;
            }
            else if (this[move.StartSquare].Type == Type.King && ColorToPlay == Color.Black)
            {
                CanCastleKingsideBlack = false;
                CanCastleQueensideBlack = false;
            }

            // If A8, H8, A1, or A8 are start position for this move, then the rook has moved
            // and that castle is now forever illegal
            if( move.StartSquare == "a1")
            {
                CanCastleQueensideWhite = false;
            }
            else if (move.StartSquare == "a8")
            {
                CanCastleKingsideWhite = false;
            }
            else if (move.StartSquare == "h1")
            {
                CanCastleQueensideBlack = false;
            }
            else if (move.StartSquare == "h8")
            {
                CanCastleKingsideBlack = false;
            }

            // Move the piece
            this[move.EndSquare] = this[move.StartSquare];
            this[move.StartSquare] = new Piece('-');            

            // Swap color to play
            ColorToPlay = 1 - ColorToPlay;

            // Update most recent move
            MostRecentMove = move;

            // Set en passant square
            EnPassantTarget = move.EnPassantTarget;
            
        }

        public string ToFenBoard()
        {
            string rv = "";

            rv += "     A   B   C   D   E   F   G   H";
            for ( int row = 7; row >= 0; row--)
            {
                rv += "\n   +-------------------------------+";

                rv += string.Format( "\n {0} |",row+1);

                for ( int col = 0; col < 8; col++)
                {
                    string pieceString = board[row, col].ToFenString();
                    if( pieceString == "-")
                    {
                        pieceString = " ";
                    }
                    rv += string.Format( " {0} |", pieceString );
                }
                rv += string.Format(" {0}", row + 1);
                if( row == 7)
                {
                    rv += "    " + ColorToPlay + " to play";
                }
                else if (row == 6)
                {
                    rv += "    Most recent move: " + MostRecentMove;
                }
                else if (row == 5)
                {
                    rv += "    Castling: " + GetCastlingString();
                }
                else if (row == 4)
                {
                    rv += "    En passant target: " + EnPassantTarget;
                }
                else if (row == 3)
                {
                    rv += "    Halfmove clock: " + "NOT IMPLEMENTED";
                }
                else if (row == 2)
                {
                    rv += "    Position count: " + "NOT IMPLEMENTED";
                }

            }
            rv += "\n   +-------------------------------+";

            rv += "\n     A   B   C   D   E   F   G   H";
            return rv;
        }

        public override string ToString()
        {
            return ToFenBoard();
        }
        private string GetCastlingString()
        {
            string rv = "";
            if( CanCastleQueensideBlack && CanCastleKingsideBlack)
            {
                rv += "Black: Q and K";
            }
            else if( CanCastleQueensideBlack )
            {
                rv += "Black: Q";
            }
            else if (CanCastleKingsideBlack)
            {
                rv += "Black: K";
            }

            if (CanCastleQueensideWhite && CanCastleKingsideWhite)
            {
                rv += "   White: Q and K";
            }
            else if (CanCastleQueensideWhite)
            {
                rv += "   White: Q";
            }
            else if (CanCastleKingsideWhite)
            {
                rv += "   Black: Q and K";
            }

            if (rv.Trim() == "")
            {
                return "None";
            }
            else
            {
                return rv.Trim();
            }
        }

        public void PrintBoard()
        {
            string boardString = ToFenBoard();
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }

            string[] whitePieces = { "P", "N", "B", "R", "Q", "K" };
            string[] blackPieces = { "p", "n", "b", "r", "q", "k" };

            int column = 0;
            int row = 0;

            foreach( char ch in boardString.ToCharArray())
            {
                if( ch == '\n')
                {
                    column = 0;
                    row++;
                }
                else
                {
                    column++;
                }
                string chString = string.Format("{0}", ch);

                if (column < 36 && (row > 1 && row <= 16))
                {
                    if (whitePieces.Contains(chString))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (blackPieces.Contains(chString))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                Console.Write(ch);
            }
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}

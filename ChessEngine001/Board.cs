using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ChessEngine001
{
    class Board
    {
        public static readonly string StartPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        
        private Piece[,] board;
        //private bool whiteToPlay;
        public Color ColorToPlay;
        private int halfmoveClock;
        private int fullmoveCount;


        public bool CanCastleQueensideWhite { get; set; }
        public bool CanCastleQueensideBlack { get; set; }
        public bool CanCastleKingsideWhite { get; set; }
        public bool CanCastleKingsideBlack { get; set; }
        public Coord EnPassantTarget { get; set; }
        public Move MostRecentMove { get; set; }
        public int HalfmoveClock
        { 
            get
            {
                return halfmoveClock;
            }
        }

        public int Fullmove
        {
            get
            {
                return fullmoveCount;
            }
        }

        public Piece this[int row, int column]
        {
            get => board[row, column];
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


        //public Board() : this("rnbqkbnr/pppppppp/--------/--------/--------/--------/PPPPPPPP/RNBQKBNR")
        public Board() : this("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")

        {

        }

        //public Board(string simpleString)
        public Board(string fenString)
        {
            board = new Piece[8, 8];

            UpdateBoardFromFenString(fenString);
/*
            //whiteToPlay = true;
            ColorToPlay = Color.White;

            CanCastleQueensideBlack = true;
            CanCastleKingsideBlack = true;
            CanCastleQueensideWhite = true;
            CanCastleKingsideWhite = true;

            halfmoveClock = 5;
            fullmoveCount = 40;
            //Console.WriteLine("simpleString\n" + simpleString);

            simpleString = simpleString.Replace("/", "");
            //Console.WriteLine( "simpleString\n" + simpleString);

            var charArray = simpleString.ToCharArray();

            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0)
                {
                    //Console.WriteLine();
                }
                //Console.Write(" {0} ", charArray[i]);

                int row = 7 - i / 8;
                int col = i % 8;

                board[row, col] = new Piece(charArray[i]);
            }*/
        }

        public void UpdateBoardFromFenString( string fenString )
        {
            string[] stringFields = fenString.Trim().Split(' ');
            if( stringFields.Length != 6 )
            {
                throw new ArgumentException("FEN string did not have six fields");
            }

            string piecePositionString   = stringFields[0].Trim();
            string sideToMoveString      = stringFields[1].Trim();
            string castlingString        = stringFields[2].Trim();
            string enPassantTargetString = stringFields[3].Trim();
            string halfmoveString        = stringFields[4].Trim();
            string fullmoveString        = stringFields[5].Trim();

            // Figure out the board
            string[] rowStrings = piecePositionString.Trim().Split('/');
            if (rowStrings.Length != 8)
            {
                throw new ArgumentException("FEN string did contain eight rows");
            }
            for( int row = 0; row < 8; row++)
            {
                string rowString = rowStrings[row];
                // rowString will be something like "2Q3p1"

                // Expand the numbers into dashes
                rowString = rowString.Replace("1", "-");
                rowString = rowString.Replace("2", "--");
                rowString = rowString.Replace("3", "---");
                rowString = rowString.Replace("4", "----");
                rowString = rowString.Replace("5", "-----");
                rowString = rowString.Replace("6", "------");
                rowString = rowString.Replace("7", "-------");
                rowString = rowString.Replace("8", "--------");

                // rowString should now be something like "--Q---p-"
                if( rowString.Length != 8)
                {
                    throw new ArgumentException("FEN string included a row that didn't have 8 squares [" + rowString + "]");
                }

                // TODO: Update all the pieces

                char[] charArray = rowString.ToCharArray();
                for( int col = 0; col < 8; col++)
                {
                    board[7-row, col] = new Piece(charArray[col]);
                }

            } // for loop

            // Update side to move
            if (sideToMoveString.Trim().ToLower() == "w")
            {
                ColorToPlay = Color.White;
            }
            else if (sideToMoveString.Trim().ToLower() == "b")
            {
                ColorToPlay = Color.Black;
            }
            else
            {
                throw new ArgumentException(
                    "FEN string included an invalid side to play [" + sideToMoveString + "]");
            }

            // Update castling
            CanCastleKingsideWhite  = castlingString.Contains('K');
            CanCastleQueensideWhite = castlingString.Contains('Q');
            CanCastleKingsideBlack  = castlingString.Contains('k');
            CanCastleQueensideBlack = castlingString.Contains('q');


            // Update en passant target
            if( enPassantTargetString == "-")
            {
                EnPassantTarget = null;
            }
            else
            {
                EnPassantTarget = new Coord(enPassantTargetString.Trim());
            }

            // Update halfmove count
            try
            {
                halfmoveClock = int.Parse(halfmoveString.Trim());
            }
            catch (FormatException)
            {
                throw new ArgumentException(
                    "FEN string included an invalid halfmove string [" + halfmoveString + "]");
            }

            // TODO: Update move count
            try
            {
                fullmoveCount = int.Parse(fullmoveString.Trim());
            }
            catch (FormatException)
            {
                throw new ArgumentException(
                    "FEN string included an invalid fullmove string [" + fullmoveString + "]");
            }
        }

        public List<Coord> GetAllSquares()
        {
            List<Coord> allSquares = new List<Coord>(64);
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    allSquares.Add(new Coord(row, col));
                }
            }
            return allSquares;
        }

        public void MakeMove(Move move)
        {
            if (!move.IsLegal)
            {
                return;
            }

            Color sideMoving = move.SideMoving;

            Coord fromCoord = move.FromSquare;
            Coord toCoord = move.ToSquare;

            Piece piece = move.PieceToMove;

            // Castling is special:
            if ( move.IsCastle)
            {
                // Move the king
                this[toCoord]   = piece;
                this[fromCoord] = new Piece(Type.Empty, sideMoving);

                // Move the rook
                this[move.RookCastlingTo] = this[move.RookCastlingFrom];
                this[move.RookCastlingFrom] = new Piece(Type.Empty, sideMoving);

                // Revoke castling rights.
                if (sideMoving == Color.White)
                {
                    CanCastleKingsideWhite = false;
                    CanCastleQueensideWhite = false;
                }
                else
                {
                    CanCastleKingsideBlack = false;
                    CanCastleQueensideBlack = false;
                }
            } // castling

            else if( move.IsEnPassantCapture)
            {
                // Move FROM to TO
                this[toCoord] = piece;

                // Remove FROM from board
                this[fromCoord] = new Piece(Type.Empty,sideMoving);

                // Remove en passant from board
                Coord capturedCoord = new Coord(fromCoord.Row, toCoord.Col);
                this[capturedCoord] = new Piece(Type.Empty, sideMoving); 
            } // en passant

            else if( move.IsPawnPromotion )
            {
                // Set moving piece to be the promotion piece
                // Then it will all get figured out later.
                piece = move.PawnPromotionType;
            } // Pawn promotion

            // Normal move
            else
            {
                ;
            }

            // If the king is moving, castling is forever illegal.
            if( move.PieceToMove.Type == Type.King )
            {
                if( sideMoving == Color.White )
                {
                    CanCastleKingsideWhite = false;
                    CanCastleQueensideWhite = false;
                }
                else
                {
                    CanCastleKingsideBlack = false;
                    CanCastleQueensideBlack = false;
                }
            }

            // If the From or To are in the corners, castling that way is forever illegal
            if (move.FromSquare == "a1" || move.ToSquare == "a1")
                CanCastleQueensideWhite = false;
            if (move.FromSquare == "h1" || move.ToSquare == "h1")
                CanCastleKingsideWhite = false;
            if (move.FromSquare == "a8" || move.ToSquare == "a8")
                CanCastleQueensideBlack = false;
            if (move.FromSquare == "h8" || move.ToSquare == "h8")
                CanCastleKingsideBlack = false;

            // Move the piece
            this[toCoord] = piece;
            this[fromCoord] = new Piece(Type.Empty,sideMoving);

            // Swap color to play
            ColorToPlay = 1 - ColorToPlay;

            // Update most recent move
            MostRecentMove = move;

            // Set en passant square
            EnPassantTarget = move.ResultingEnPassantTarget;
        }

        public string ToFenString()
        {
            string fenString = "";

            // Iterate over the rows
            for (int row = 7; row >= 0; row--)
            {
                // Build a string for this row
                // It will be of the form "-P--Q---"
                string rowString = "";

                for (int col = 0; col < 8; col++)
                {
                    rowString += board[row, col].ToFenString();
                }

                // Convert from "-P--Q---" to "1P2Q3"
                rowString = ConvertRowStringToFen(rowString);

                // Append this rowString to the overall string
                fenString += rowString;

                // If this isn't the bottom row, add a /
                if (row != 0)
                {
                    fenString += "/";
                }
            }

            // Encode the side to play
            fenString += (ColorToPlay == Color.White ? " w " : " b ");


            // Encode castling rights
            string castleString = "";
            if (CanCastleKingsideWhite)
                castleString += "K";
            if (CanCastleQueensideWhite)
                castleString += "Q";
            if (CanCastleKingsideBlack)
                castleString += "k";
            if (CanCastleQueensideBlack)
                castleString += "q";

            // If no castling is possible, replace with "-"
            if (castleString == "")
                castleString = "-";

            fenString += castleString;


            // Encode the en passant target
            if (EnPassantTarget is null)
            {
                fenString += " -";
            }
            else
            {
                fenString += " " + EnPassantTarget;
            }

            // Encode the halfmove clock
            fenString += string.Format(" {0}",HalfmoveClock);

            // Encode the fullmove number
            fenString += string.Format(" {0}",Fullmove);

            return fenString;
        }

        // Convert a string like "-P--Q---" to a FEN-approved string like "1P2Q3"
        private static string ConvertRowStringToFen(string rowString)
        {
            string rv = "";
            for (int index = 0; index < rowString.Length; index++)
            {
                char current = rowString[index];

                // If it's not '-', then it's an actual piece and can be added to the FEN string
                if (current != '-')
                {
                    rv += current;
                }


                else
                {
                    // Otherwise, search until we find a non '-' char
                    // And count the '-' we find
                    int count = 1;
                    for (int peekIndex = index + 1; peekIndex < rowString.Length; peekIndex++)
                    {
                        if (rowString[peekIndex] == '-')
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    index = index + count - 1;
                    rv += string.Format("{0}", count);
                }
            }

            return rv;
        }

        public string ToFancyString()
        {
            string rv = "";

            rv += "     A   B   C   D   E   F   G   H";
            for (int row = 7; row >= 0; row--)
            {
                rv += "\n   +-------------------------------+";

                rv += string.Format("\n {0} |", row + 1);

                for (int col = 0; col < 8; col++)
                {
                    string pieceString = board[row, col].ToFenString();
                    if (pieceString == "-")
                    {
                        pieceString = " ";
                    }
                    rv += string.Format(" {0} |", pieceString);
                }
                rv += string.Format(" {0}", row + 1);
                if (row == 7)
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
                else if (row == 1)
                {
                    rv += "    FEN: " + ToFenString();
                }

            }
            rv += "\n   +-------------------------------+";

            rv += "\n     A   B   C   D   E   F   G   H";
            return rv;
        }

        public override string ToString()
        {
            return ToFancyString();
        }
        private string GetCastlingString()
        {
            string rv = "";
            if (CanCastleQueensideBlack && CanCastleKingsideBlack)
            {
                rv += "Black: Q and K";
            }
            else if (CanCastleQueensideBlack)
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
            string boardString = ToFancyString();
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }

            string[] whitePieces = { "P", "N", "B", "R", "Q", "K" };
            string[] blackPieces = { "p", "n", "b", "r", "q", "k" };

            int column = 0;
            int row = 0;

            foreach (char ch in boardString.ToCharArray())
            {
                if (ch == '\n')
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

using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine001
{
    class Move
    {
        private Board board;
        public Coord StartSquare { get; }
        public Coord EndSquare { get; }
        public Piece Piece { get; }
        public Coord EnPassantTarget { get => enPassantTarget; }
        public string MoveString { get => moveString; set => moveString = value; }

        
        private Coord enPassantTarget;
        private string moveString;

        public bool IsCapture()
        {
            return !(board[EndSquare] is null) && !(board[EndSquare].Type == Type.Empty);
        }

        public bool IsCheck()
        {
            // TODO
            return false;
        }
        public override string ToString()
        {
            return this.MoveString;
            //return string.Format("{0}-{1} [{2}]", StartSquare, EndSquare);
        }

        public Move( Coord start, Coord end, Board board)
        {
            enPassantTarget = null;
            StartSquare = start;
            EndSquare = end;
            this.board = board;

            this.Piece = this.board[StartSquare];
        }

        public bool IsPseudoLegalMove()
        {
            if( Piece.Type == Type.Empty)
            {
                moveString = string.Format("[{0}-{1}] Illegal move: {0} unoccupied",StartSquare,EndSquare);
                return false;
            }
            if( Piece.Color != board.ColorToPlay )
            {
                moveString = string.Format("[{0}-{1}] Illegal move: {0} wrong color", StartSquare, EndSquare);
                return false;
            }

            if (Piece.Type == Type.King)
            {
                if (IsPseudoLegalCastleMove())
                {
                    moveString = string.Format("[{0}-{1}] Castle", StartSquare, EndSquare);
                    if( EndSquare.Col == 2)
                    {
                        moveString = string.Format("[{0}-{1}] O-O-O", StartSquare, EndSquare);
                    }
                    else
                    {
                        moveString = string.Format("[{0}-{1}] O-O", StartSquare, EndSquare);
                    }
                    return true;
                }
                else if (IsPseudoLegalKingMove())
                {
                    //moveString = string.Format("[{0}-{1}] King move", StartSquare, EndSquare);
                    if( IsCapture())
                        moveString = string.Format("[{0}-{1}] Kx{1}", StartSquare, EndSquare);
                    else
                        moveString = string.Format("[{0}-{1}] K{1}", StartSquare, EndSquare);
                    return true;
                }
                else
                {
                    moveString = string.Format("[{0}-{1}] Illegal king move", StartSquare, EndSquare);
                    return false;
                }
            }

            else if (Piece.Type == Type.Queen)
            {
                if (IsPseudoLegalQueenMove())
                {
                    //moveString = string.Format("[{0}-{1}] Queen move", StartSquare, EndSquare);
                    if (IsCapture())
                        moveString = string.Format("[{0}-{1}] Qx{1}", StartSquare, EndSquare);
                    else
                        moveString = string.Format("[{0}-{1}] Q{1}", StartSquare, EndSquare);
                    return true;
                }
                else
                {
                    moveString = string.Format("[{0}-{1}] Illegal queen move", StartSquare, EndSquare);
                    return false;
                }
            }
            else if (Piece.Type == Type.Rook)
            {
                if (IsPseudoLegalRookMove())
                {
                    //moveString = string.Format("[{0}-{1}] Rook move", StartSquare, EndSquare);
                    if (IsCapture())
                        moveString = string.Format("[{0}-{1}] Rx{1}", StartSquare, EndSquare);
                    else
                        moveString = string.Format("[{0}-{1}] R{1}", StartSquare, EndSquare);
                    return true;
                    return true;
                }
                else
                {
                    moveString = string.Format("[{0}-{1}] Illegal rook move", StartSquare, EndSquare);
                    return false;
                }
            }
            else if (Piece.Type == Type.Bishop)
            {
                if (IsPseudoLegalBishopMove())
                {
                    //moveString = string.Format("[{0}-{1}] Bishop move", StartSquare, EndSquare);
                    if (IsCapture())
                        moveString = string.Format("[{0}-{1}] Bx{1}", StartSquare, EndSquare);
                    else
                        moveString = string.Format("[{0}-{1}] B{1}", StartSquare, EndSquare);
                    return true;
                    return true;
                }
                else
                {
                    moveString = string.Format("[{0}-{1}] Illegal bishop move", StartSquare, EndSquare);
                    return false;
                }
            }
            else if (Piece.Type == Type.Knight)
            {
                if (IsPseudoLegalKnightMove())
                {
                    //moveString = string.Format("[{0}-{1}] Knight move", StartSquare, EndSquare);
                    if (IsCapture())
                        moveString = string.Format("[{0}-{1}] Nx{1}", StartSquare, EndSquare);
                    else
                        moveString = string.Format("[{0}-{1}] N{1}", StartSquare, EndSquare);
                    return true;
                }
                else
                {
                    moveString = string.Format("[{0}-{1}] Illegal Knight move", StartSquare, EndSquare);
                    return false;
                }
            }
            else if (Piece.Type == Type.Pawn)
            {
                if (IsPseudoLegalPawnEnPassantCapture())
                {
                    moveString = string.Format("[{0}-{1}] Pawn capture (en passant)", StartSquare, EndSquare);
                    return true;
                }
                else if (IsPseudoLegalPawnCapture())
                {
                    moveString = string.Format("[{0}-{1}] {2}x{1}", StartSquare, EndSquare, StartSquare.ToString()[0]);
                    return true;
                }
                else if (IsPseudoLegalPawnPush())
                {
                    //moveString = string.Format("[{0}-{1}] Pawn push", StartSquare, EndSquare);
                    moveString = string.Format("[{0}-{1}] {1}", StartSquare, EndSquare);
                    return true;
                }
                else
                {
                    moveString = string.Format("[{0}-{1}] Illegal pawn move", StartSquare, EndSquare);
                    return false;
                }
            }
            else
            {
                // This should be unreachable code
                moveString = string.Format("[{0}-{1}] Illegal move", StartSquare, EndSquare);
                return false;
            }
        }

        private bool IsPseudoLegalPawnMove()
        {
            return IsPseudoLegalPawnCapture() || IsPseudoLegalPawnPush();
        }

        private bool IsPseudoLegalPawnCapture()
        {
            // En passant
            if( IsPseudoLegalPawnEnPassantCapture() )
            {
                return true;
            }

            // Piece is a pawn
            if (board[StartSquare].Type != Type.Pawn)
            {
                //Console.WriteLine("BAD: {0} is not a pawn", StartSquare);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} is a pawn", StartSquare);
            }

            // Piece is correct color
            if (board[StartSquare].Color != board.ColorToPlay)
            {
                //Console.WriteLine("BAD: {0} is {1}, but it is {2} to play", StartSquare, board[StartSquare].Color, board.ColorToPlay);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} is {1}, and it is {2} to play", StartSquare, board[StartSquare].Color, board.ColorToPlay);
            }

            // Destination is +1 column or -1 columns
            if (StartSquare.Col != EndSquare.Col + 1 && StartSquare.Col != EndSquare.Col - 1)
            {
                //Console.WriteLine("BAD: {0} and {1} not in neighboring column", StartSquare, EndSquare);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} and {1} not in neighboring column", StartSquare, EndSquare);
            }

            // Destination is 1 rank further
            // -1 if black to play
            // +1 if white to play
            int direction = 1 - 2 * (int)board.ColorToPlay;
            if( StartSquare.Row + direction != EndSquare.Row)
            {
                //Console.WriteLine("BAD: {1} is not one rank further than {0}", StartSquare, EndSquare);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {1} is one rank further than {0}", StartSquare, EndSquare);
            }

            // Destination is occupied
            if ( board[EndSquare].Type == Type.Empty)
            {
                //Console.WriteLine("BAD: {0} is empty", EndSquare);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} is occupied", EndSquare);
            }

            // Destination square is of opposite color
            if ( board[EndSquare].Color == board.ColorToPlay)
            {
                //Console.WriteLine("BAD: {0} is the same color as {1}", StartSquare, EndSquare);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} is opposite color as {1}", StartSquare, EndSquare);
                //Console.WriteLine("GOOD: MOVE IS PSEDUOLEGAL", StartSquare, EndSquare);
                return true;
            }


            //return false;
            //throw new NotImplementedException();
        }

        private bool IsPseudoLegalPawnEnPassantCapture()
        {
            // Make sure Start is a pawn of the right color
            if( Piece.Type != Type.Pawn || Piece.Color != board.ColorToPlay)
            {
                //Console.WriteLine("DEBUG: bad type or color");
                return false;
            }
            else
            {
                //Console.WriteLine("DEBUG: good type and color");
            }

            // Make sure EnPassantTarget is not null
            if ( board.EnPassantTarget is null )
            {
                //Console.WriteLine("DEBUG: EnPassantTarget is null");
                return false;
            }
            else
            {
                //Console.WriteLine("DEBUG: EnPassantTarget exists");
            }


            // Make sure that End == EnPassantTarget
            if ( EndSquare != board.EnPassantTarget)
            {
                //Console.WriteLine("DEBUG: BAD: EndSquare not equal EnPassantTarget");
                //Console.WriteLine("DEBUG: EndSquare={0}   EnPassantTarget={1}",EndSquare,board.EnPassantTarget);

                return false;
            }
            else
            {
                //Console.WriteLine("DEBUG: GOOD: EndSquare equal EnPassantTarget");
                //Console.WriteLine("DEBUG: EndSquare={0}   EnPassantTarget={1}", EndSquare, board.EnPassantTarget);

            }


            // Make sure that Start is the valid row
            int validStartRow = Piece.Color == Color.White ? 4 : 3;
            //Console.WriteLine("DEBUG: validStartRow=" + validStartRow);
            //Console.WriteLine("DEBUG: StartSquare.Row=" + StartSquare.Row);

            if ( StartSquare.Row != validStartRow )
            {
                return false;
            }

            // Make sure that EnPassantTarget is exactly one column offset from Start
            if( (board.EnPassantTarget.Col - StartSquare.Col != 1) &&
                (board.EnPassantTarget.Col - StartSquare.Col != -1))
            {
                return false;
            }

            // If we got here, it's valid
            return true;
            throw new NotImplementedException();
        }

        private bool IsPseudoLegalKnightMove()
        {
            // Piece is a pawn
            if (board[StartSquare].Type != Type.Knight)
            {
                return false;
            }
            else
            {
            }

            // Piece is correct color
            if (board[StartSquare].Color != board.ColorToPlay)
            {
                return false;
            }
            else
            {
            }

            // End is (+/-2,+/-1) or (+/-1,+/-2) away
            int rowDistance = Math.Abs(StartSquare.Row - EndSquare.Row);
            int colDistance = Math.Abs(StartSquare.Col - EndSquare.Col);
            if( ( rowDistance == 1 && colDistance == 2 ) ||
                ( rowDistance == 2 && colDistance ==1 ))
            {
                ; // Do nothing
            }
            else
            {
                return false;
            }

            // Check end square

            // If end square is empty, move is good.
            if (board[EndSquare].Type == Type.Empty)
                return true;

            // If end square is occupied and opposite color, it's good (a capture)
            else if (board[EndSquare].Color != board.ColorToPlay)
            {
                return true;
            }

            // If end square is occupied and not opposite color, it's not legal
            // (Can't capture own piece)
            else
            {
                return false;
            }
        }

        private bool IsPseudoLegalCastleMove()
        {
            // Start square must be a king and must be correct color
            if (board[StartSquare].Type != Type.King || board[StartSquare].Color != board.ColorToPlay)
            {
                return false;
            }

            // Make sure START and END are on same row
            if (StartSquare.Row != EndSquare.Row)
                return false;

            // Make sure START is rank 1 or rank 8
            if (StartSquare.Row != 0 && StartSquare.Row != 7)
            {
                return false;
            }

            // Make sure START is on E file
            if (StartSquare.Col != 4)
                return false;

            // Make sure END is on C or G file
            if (EndSquare.Col != 2 && EndSquare.Col != 6)
                return false;

            // Determine if it's queenside
            bool isQueenside;
            if (EndSquare.Col == 2)
                isQueenside = true;
            else
                isQueenside = false;

            // Determine color
            bool isWhite;
            int row;
            if (EndSquare.Row == 0)
            {
                row = 0;
                isWhite = true;
            }
            else
            {
                row = 7;
                isWhite = false;
            }

            // Make sure castling in the given direction is possible
            if (isQueenside && isWhite && !board.CanCastleQueensideWhite)
                return false;
            if (isQueenside && !isWhite && !board.CanCastleQueensideBlack)
                return false;
            if (!isQueenside && isWhite && !board.CanCastleKingsideWhite)
                return false;
            if (!isQueenside && !isWhite && !board.CanCastleKingsideBlack)
                return false;

            // Make arrays of columns to check for being unoccupied and not in check
            int[] columnsNotOccupied;
            int[] columnsNotInCheck;
            if( isQueenside )
            {
                columnsNotOccupied = new int[] { 1,2,3 };
                columnsNotInCheck = new int[] { 2,3,4 };
            }
            else
            {
                columnsNotOccupied = new int[] { 5, 6 };
                columnsNotInCheck = new int[] { 4,5,6 };
            }

            // Make sure all the squares in columnsNotInCheck are not in check
            foreach( int column in columnsNotInCheck)
            {
                Coord coord = new Coord(row, column);
                if( IsAttacked(coord) )
                {
                    return false;
                }
            }

            // Make sure all the squares in columnsNotOccupied are Empty
            foreach (int column in columnsNotOccupied)
            {
                Coord coord = new Coord(row, column);
                if (board[coord].Type != Type.Empty)
                {
                    return false;
                }
            }

            //TODO
            return true;
            throw new NotImplementedException();
        }

        private bool IsAttacked(Coord coord)
        {
            // TODO
            return false;
            throw new NotImplementedException();
        }

        private bool IsPseudoLegalPawnPush()
        {
            if (board[StartSquare].Type != Type.Pawn)
            {
                //Console.WriteLine("BAD: {0} is not a pawn", StartSquare);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} is a pawn", StartSquare);
            }

            if ( board[StartSquare].Color != board.ColorToPlay)
            {
                //Console.WriteLine("BAD: {0} is {1}, but it is {2} to play", StartSquare, board[StartSquare].Color, board.ColorToPlay);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} is {1}, and it is {2} to play", StartSquare, board[StartSquare].Color, board.ColorToPlay);
            }

            if ( StartSquare.Col != EndSquare.Col)
            {
                //Console.WriteLine("BAD: {0} and {1} not in same column", StartSquare, EndSquare);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} and {1} in same column", StartSquare, EndSquare);

            }
            // -1 if black to play
            // +1 if white to play
            int direction = 1 - 2 * (int)board.ColorToPlay;

            int distance = (EndSquare.Row - StartSquare.Row)*direction;
            //Console.WriteLine("DEBUG: ColorToPlay={0}  direction={1}", board.ColorToPlay, direction);
            //Console.WriteLine("DEBUG: end={0}  start={1}  distance={2}", EndSquare.Row, StartSquare.Row, distance);

            if( distance < 0)
            {
                //Console.WriteLine("BAD: {0} --> {1} is going the wrong way", StartSquare, EndSquare);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} --> {1} is going the right way", StartSquare, EndSquare);
            }


            if ( distance == 1 )
            {
                //Console.WriteLine("DEBUG: Moving one square", board.ColorToPlay, direction);
                if (board[EndSquare].Type == Type.Empty)
                {
                    //Console.WriteLine("GOOD: Destination square is empty");
                    //Console.WriteLine("GOOD: MOVE IS PSEUDOLEGAL");

                    return true;
                }
                else
                {
                    //Console.WriteLine("BAD: Destination square is occupied");
                    return false;
                }
            }

            if (distance == 2)
            {
                //Console.WriteLine("DEBUG: Moving two squares", board.ColorToPlay, direction);

                // 1 or 6
                int validStartRow;

                if( board.ColorToPlay == Color.White)
                {
                    validStartRow = 1;
                }
                else
                {
                    validStartRow = 6;
                }

                if( StartSquare.Row != validStartRow)
                {
                    //Console.WriteLine("BAD: {0} is not on the correct row. Its row is {1}. The valid row with {2} to play is {3}",StartSquare, StartSquare.Row, board.ColorToPlay, validStartRow);
                    return false;
                }
                else
                {
                    //Console.WriteLine("GOOD: {0} is on the correct row. Its row is {1}. The valid row with {2} to play is {3}", StartSquare, StartSquare.Row, board.ColorToPlay, validStartRow);
                }

                Coord midPoint = new Coord( StartSquare.Row + direction, StartSquare.Col );
                //Console.WriteLine("DEBUG: Midpoint is {0}", midPoint);

                if (board[EndSquare].Type == Type.Empty && board[midPoint].Type == Type.Empty)
                {
                    //Console.WriteLine("GOOD: {0} and {1} are both empty.", midPoint, EndSquare);
                    //Console.WriteLine("GOOD: MOVE IS PSEUDOLEGAL.");

                    // Set en passant target
                    enPassantTarget = midPoint;
                    return true;
                }
                else
                {
                    //Console.WriteLine("BAD: {0} and/or {1} are non-empty.", midPoint, EndSquare);

                    return false;
                }
            }

            // distance > 2
            else
            {
                //Console.WriteLine("BAD: Moving more than two squares", board.ColorToPlay, direction);
                return false;
            }

            //if( distance*direction )
            //return true;

        }

        private bool IsPseudoLegalBishopMove()
        {
            return IsPseduoLegalLongDistanceMove(Type.Bishop);
        }

        private bool IsPseudoLegalRookMove()
        {
            return IsPseduoLegalLongDistanceMove(Type.Rook);
        }

        private bool IsPseudoLegalQueenMove()
        {
            return IsPseduoLegalLongDistanceMove(Type.Queen);
        }

        private bool IsPseudoLegalKingMove()
        {
            return IsPseudoLegalCastleMove() || IsPseduoLegalLongDistanceMove(Type.King);
        }

        private bool IsPseduoLegalLongDistanceMove(Type type)
        {
            bool limitedDistance = false;
            bool movesDiagonal = false;
            bool movesStraight = false;

            if( type == Type.Bishop || type == Type.Queen || type == Type.King)
            {
                movesDiagonal = true;
            }
            if (type == Type.Rook || type == Type.Queen || type == Type.King)
            {
                movesStraight = true;
            }
            if( type == Type.King)
            {
                limitedDistance = true;
            }

            // Start is of Type type
            if( board[StartSquare].Type != type)
            {
                return false;
            }

            // Can't start and end on same square
            if( StartSquare == EndSquare )
            {
                return false;
            }

            // Start is of correct color
            if (board[StartSquare].Color != board.ColorToPlay)
            {
                return false;
            }

            // Figure out if axis exists and if it's valid
            int rowsAhead = (EndSquare.Row - StartSquare.Row);
            int colsAhead = (EndSquare.Col - StartSquare.Col);

            // Will be either 0 or 1
            int rowDirection;
            int colDirection;
            string axis;

            // Vertical
            if( colsAhead == 0)
            {
                if( !movesStraight )
                {
                    return false;
                }
                axis = "vertical";
                colDirection = 0;
                if (rowsAhead > 0)
                    rowDirection = 1;
                else
                    rowDirection = -1;
            }

            // Horizontal
            else if( rowsAhead == 0)
            {
                if (!movesStraight)
                {
                    return false;
                }
                axis = "horizontal";
                rowDirection = 0;

                if (colsAhead > 0)
                    colDirection = 1;
                else
                    colDirection = -1;
                
            }

            // Slope = +1 ==> northeast
            else if( rowsAhead == colsAhead )
            {
                if (!movesDiagonal)
                {
                    return false;
                }
                axis = "northeast";
                if (colsAhead > 0)
                {
                    colDirection = 1;
                    rowDirection = 1;
                }
                else
                {
                    colDirection = -1;
                    rowDirection = -1;
                }
            }

            // Slope = -1 ==> northeast
            else if ( rowsAhead == - colsAhead)
            {
                if (!movesDiagonal)
                {
                    return false;
                }
                axis = "southeast";
                if (colsAhead > 0)
                {
                    colDirection = 1;
                    rowDirection = -1;
                }
                else
                {
                    colDirection = -1;
                    rowDirection = 1;
                }
            }

            // Any other slope is invalid for any long range piece
            else
            {
                axis = "invalid";
                return false;
            }

            // Check king distance
            if( limitedDistance && ( colsAhead > 1 || colsAhead < -1 || rowsAhead >1 || rowsAhead < -1 ))
            {
                return false;
            }


            // Iterate along direction until the end square
            // All intermediate squares should be empty
            Coord currentCoord = new Coord(StartSquare.Row + rowDirection, StartSquare.Col + colDirection);
            while( currentCoord != EndSquare )
            {
                if( board[currentCoord].Type != Type.Empty)
                {
                    return false;
                }
                currentCoord = new Coord(currentCoord.Row + rowDirection, currentCoord.Col + colDirection);

            }

            // Check end square

            // If end square is empty, move is good.
            if (board[EndSquare].Type == Type.Empty)
                return true;

            // If end square is occupied and opposite color, it's good (a capture)
            else if(board[EndSquare].Color != board.ColorToPlay)
            {
                return true;
            }

            // If end square is occupied and not opposite color, it's not legal
            // (Can't capture own piece)
            else
            {
                return false;
            }
        }
    }
}

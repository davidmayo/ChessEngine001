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

        private Coord enPassantTarget;

        public bool IsCapture()
        {
            // TODO
            return false;
        }

        public bool IsCheck()
        {
            // TODO
            return false;
        }
        public override string ToString()
        {
            return string.Format("{0}-{1}", StartSquare, EndSquare);
        }

        public Move( Coord start, Coord end, Board board)
        {
            enPassantTarget = null;
            StartSquare = start;
            EndSquare = end;
            this.board = board;

            this.Piece = this.board[StartSquare];
        }

        private bool IsPseudoLegalPawnMove()
        {
            return IsPseudoLegalPawnCapture() || IsPseudoLegalPawnPush();

        }

        public bool IsPseudoLegalPawnCapture()
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
            //TODO
            return false;
            throw new NotImplementedException();
        }

        public bool IsPseudoLegalKnightMove()
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

            // EndSquare should be empty or opposite color
            if (board[EndSquare].Type != Type.Empty || board[EndSquare].Color == board.ColorToPlay)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsPseudoLegalCastleMove()
        {
            //TODO
            return false;
            throw new NotImplementedException();
        }

        public bool IsPseudoLegalPawnPush()
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
                    enPassantTarget = EndSquare;
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

        public bool IsPseudoLegalBishopMove()
        {
            return IsPseduoLegalLongDistanceMove(Type.Bishop);
        }

        public bool IsPseudoLegalRookMove()
        {
            return IsPseduoLegalLongDistanceMove(Type.Rook);
        }

        public bool IsPseudoLegalQueenMove()
        {
            return IsPseduoLegalLongDistanceMove(Type.Queen);
        }

        public bool IsPseudoLegalKingMove()
        {
            return IsPseduoLegalLongDistanceMove(Type.King);
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
                //Console.WriteLine("   {0} {1}", currentCoord, EndSquare);
                if( board[currentCoord].Type != Type.Empty)
                {
                    return false;
                }
                currentCoord = new Coord(currentCoord.Row + rowDirection, currentCoord.Col + colDirection);

            }
            // EndSquare should be empty or opposite color
            if (board[EndSquare].Type != Type.Empty || board[EndSquare].Color == board.ColorToPlay)
            {
                return false;
            }
            else
            {
                return true;
            }
            //Console.WriteLine("SHOULDN'T EVER GET HERE");
            //return false;


            //throw new NotImplementedException();
        }
    }
}

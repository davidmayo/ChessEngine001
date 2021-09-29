using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine001
{
    class Move
    {
        // The board on which the move will take place
        private Board board;

        // To coordinate that the piece is moving from
        public Coord FromSquare { get; }

        // The coordinate that the piece is moving to
        public Coord ToSquare { get; }

        // The specific Piece being moved
        public Piece PieceToMove { get; }

        // The piece that a pawn will promote to
        // Will either be null or a Piece, if provided
        public Piece PawnPromotionType { get => pawnPromotionType; }

        // The en passant target square that will result if this move is carried out
        // Will be null UNLESS this is a 2 square pawn push, in which case
        // It will be the coordinate BEHIND the pawn (ie, the square between
        // the To and From squares)
        public Coord ResultingEnPassantTarget { get => resultingEnPassantTarget; }

        // Field for EnPassantTarget property
        private Coord resultingEnPassantTarget;

        // A string representing the move, form like "[a2-a4] Ra4"
        // This is a placeholder that needs to be removed eventually
        private string moveString;

        // field for PawnPromotionType property
        private Piece pawnPromotionType;

        public bool IsCapture()
        {
            // TODO: Handle en passant
            //
            // For non-en passant moves, it's a capture if and only if ToSquare is occupied
            //
            // This will return true for cases of capturing own pieces, but that's illegal,
            // so we don't waste time checking for that.
            //
            // Castling can never be a capture, and castling never has an occupied ToSquare,
            // so no problem there.
            return !(board[ToSquare].Type == Type.Empty);
        }

        public bool IsCheck()
        {
            // TODO
            return false;
        }
        public override string ToString()
        {
            return this.moveString;
        }

        public Move(Coord start, Coord end, Board board, Piece pawnPromotionTarget = null)
        {
            resultingEnPassantTarget = null;
            FromSquare = start;
            ToSquare = end;
            this.board = board;

            this.PieceToMove = this.board[FromSquare];
            pawnPromotionType = pawnPromotionTarget;

            // TODO: Refactor this to be something like EvaluateMove();
            _ = IsPseudoLegalMove();
        }

        public Move(string uciString, Board board)
        {
            string startString;
            string endString;
            string promotionString;
            if (uciString.Length == 4)
            {
                startString = uciString.Substring(0, 2);
                endString = uciString.Substring(2, 2);
                promotionString = "";

            }
            else if (uciString.Length == 5)
            {
                startString = uciString.Substring(0, 2);
                endString = uciString.Substring(2, 2);
                promotionString = uciString.Substring(4, 1);

            }
            else
            {
                throw new ArgumentException("UCI string incorrect length");
            }

            FromSquare = new Coord(startString);
            ToSquare = new Coord(endString);
            this.board = board;
            this.PieceToMove = this.board[FromSquare];

            resultingEnPassantTarget = null;

            Type pieceType;

            switch (promotionString.ToLower())
            {
                case "":
                    pieceType = Type.Unknown;
                    break;
                case "q":
                    pieceType = Type.Queen;
                    break;
                case "n":
                    pieceType = Type.Knight;
                    break;
                case "r":
                    pieceType = Type.Rook;
                    break;
                case "b":
                    pieceType = Type.Bishop;
                    break;
                default:
                    pieceType = Type.Unknown;
                    break;
            }

            pawnPromotionType = new Piece(pieceType, board.ColorToPlay);

            // TODO: Refactor this to be something like EvaluateMove();
            _ = IsPseudoLegalMove();
        }

        public string ToUciMoveString()
        {
            string pawnSuffix;

            if (PawnPromotionType is null)
            {
                pawnSuffix = "";
            }
            else
            {
                pawnSuffix = PawnPromotionType.ToFenString().ToLower();
            }

            return string.Format("{0}{1}{2}", FromSquare, ToSquare, pawnSuffix);
        }

        public bool IsPseudoLegalMove()
        {
            if (PieceToMove.Type == Type.Empty)
            {
                moveString = string.Format("[{0}-{1}] Illegal move: {0} unoccupied", FromSquare, ToSquare);
                return false;
            }
            if (PieceToMove.Color != board.ColorToPlay)
            {
                moveString = string.Format("[{0}-{1}] Illegal move: {0} wrong color", FromSquare, ToSquare);
                return false;
            }

            if (PieceToMove.Type == Type.King)
            {
                if (IsPseudoLegalCastleMove())
                {
                    moveString = string.Format("[{0}-{1}] Castle", FromSquare, ToSquare);
                    if (ToSquare.Col == 2)
                    {
                        moveString = string.Format("[{0}-{1}] O-O-O", FromSquare, ToSquare);
                    }
                    else
                    {
                        moveString = string.Format("[{0}-{1}] O-O", FromSquare, ToSquare);
                    }
                    return true;
                }
                else if (IsPseudoLegalKingMove())
                {
                    //moveString = string.Format("[{0}-{1}] King move", StartSquare, EndSquare);
                    if (IsCapture())
                        moveString = string.Format("[{0}-{1}] Kx{1}", FromSquare, ToSquare);
                    else
                        moveString = string.Format("[{0}-{1}] K{1}", FromSquare, ToSquare);
                    return true;
                }
                else
                {
                    moveString = string.Format("[{0}-{1}] Illegal king move", FromSquare, ToSquare);
                    return false;
                }
            }

            else if (PieceToMove.Type == Type.Queen)
            {
                if (IsPseudoLegalQueenMove())
                {
                    //moveString = string.Format("[{0}-{1}] Queen move", StartSquare, EndSquare);
                    if (IsCapture())
                        moveString = string.Format("[{0}-{1}] Qx{1}", FromSquare, ToSquare);
                    else
                        moveString = string.Format("[{0}-{1}] Q{1}", FromSquare, ToSquare);
                    return true;
                }
                else
                {
                    moveString = string.Format("[{0}-{1}] Illegal queen move", FromSquare, ToSquare);
                    return false;
                }
            }
            else if (PieceToMove.Type == Type.Rook)
            {
                if (IsPseudoLegalRookMove())
                {
                    //moveString = string.Format("[{0}-{1}] Rook move", StartSquare, EndSquare);
                    if (IsCapture())
                        moveString = string.Format("[{0}-{1}] Rx{1}", FromSquare, ToSquare);
                    else
                        moveString = string.Format("[{0}-{1}] R{1}", FromSquare, ToSquare);
                    return true;
                    return true;
                }
                else
                {
                    moveString = string.Format("[{0}-{1}] Illegal rook move", FromSquare, ToSquare);
                    return false;
                }
            }
            else if (PieceToMove.Type == Type.Bishop)
            {
                if (IsPseudoLegalBishopMove())
                {
                    //moveString = string.Format("[{0}-{1}] Bishop move", StartSquare, EndSquare);
                    if (IsCapture())
                        moveString = string.Format("[{0}-{1}] Bx{1}", FromSquare, ToSquare);
                    else
                        moveString = string.Format("[{0}-{1}] B{1}", FromSquare, ToSquare);
                    return true;
                    return true;
                }
                else
                {
                    moveString = string.Format("[{0}-{1}] Illegal bishop move", FromSquare, ToSquare);
                    return false;
                }
            }
            else if (PieceToMove.Type == Type.Knight)
            {
                if (IsPseudoLegalKnightMove())
                {
                    //moveString = string.Format("[{0}-{1}] Knight move", StartSquare, EndSquare);
                    if (IsCapture())
                        moveString = string.Format("[{0}-{1}] Nx{1}", FromSquare, ToSquare);
                    else
                        moveString = string.Format("[{0}-{1}] N{1}", FromSquare, ToSquare);
                    return true;
                }
                else
                {
                    moveString = string.Format("[{0}-{1}] Illegal Knight move", FromSquare, ToSquare);
                    return false;
                }
            }
            else if (PieceToMove.Type == Type.Pawn)
            {
                if (IsPseudoLegalPawnEnPassantCapture())
                {
                    moveString = string.Format("[{0}-{1}] {2}x{1} e.p.", FromSquare, ToSquare, FromSquare.ToString()[0]);
                    //moveString = string.Format("[{0}-{1}] Pawn capture (en passant)", StartSquare, EndSquare);
                    return true;
                }
                else if (IsPseudoLegalPawnCapture())
                {
                    moveString = string.Format("[{0}-{1}] {2}x{1}", FromSquare, ToSquare, FromSquare.ToString()[0]);
                    return true;
                }
                else if (IsPseudoLegalPawnPush())
                {
                    //moveString = string.Format("[{0}-{1}] Pawn push", StartSquare, EndSquare);
                    moveString = string.Format("[{0}-{1}] {1}", FromSquare, ToSquare);
                    return true;
                }
                else
                {
                    moveString = string.Format("[{0}-{1}] Illegal pawn move", FromSquare, ToSquare);
                    return false;
                }
            }
            else
            {
                // This should be unreachable code
                moveString = string.Format("[{0}-{1}] Illegal move", FromSquare, ToSquare);
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
            if (IsPseudoLegalPawnEnPassantCapture())
            {
                return true;
            }

            // Piece is a pawn
            if (board[FromSquare].Type != Type.Pawn)
            {
                //Console.WriteLine("BAD: {0} is not a pawn", StartSquare);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} is a pawn", StartSquare);
            }

            // Piece is correct color
            if (board[FromSquare].Color != board.ColorToPlay)
            {
                //Console.WriteLine("BAD: {0} is {1}, but it is {2} to play", StartSquare, board[StartSquare].Color, board.ColorToPlay);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} is {1}, and it is {2} to play", StartSquare, board[StartSquare].Color, board.ColorToPlay);
            }

            // Destination is +1 column or -1 columns
            if (FromSquare.Col != ToSquare.Col + 1 && FromSquare.Col != ToSquare.Col - 1)
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
            if (FromSquare.Row + direction != ToSquare.Row)
            {
                //Console.WriteLine("BAD: {1} is not one rank further than {0}", StartSquare, EndSquare);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {1} is one rank further than {0}", StartSquare, EndSquare);
            }

            // Destination is occupied
            if (board[ToSquare].Type == Type.Empty)
            {
                //Console.WriteLine("BAD: {0} is empty", EndSquare);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} is occupied", EndSquare);
            }

            // Destination square is of opposite color
            if (board[ToSquare].Color == board.ColorToPlay)
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
            if (PieceToMove.Type != Type.Pawn || PieceToMove.Color != board.ColorToPlay)
            {
                //Console.WriteLine("DEBUG: bad type or color");
                return false;
            }
            else
            {
                //Console.WriteLine("DEBUG: good type and color");
            }

            // Make sure EnPassantTarget is not null
            if (board.EnPassantTarget is null)
            {
                //Console.WriteLine("DEBUG: EnPassantTarget is null");
                return false;
            }
            else
            {
                //Console.WriteLine("DEBUG: EnPassantTarget exists");
            }


            // Make sure that End == EnPassantTarget
            if (ToSquare != board.EnPassantTarget)
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
            int validStartRow = PieceToMove.Color == Color.White ? 4 : 3;
            //Console.WriteLine("DEBUG: validStartRow=" + validStartRow);
            //Console.WriteLine("DEBUG: StartSquare.Row=" + StartSquare.Row);

            if (FromSquare.Row != validStartRow)
            {
                return false;
            }

            // Make sure that EnPassantTarget is exactly one column offset from Start
            if ((board.EnPassantTarget.Col - FromSquare.Col != 1) &&
                (board.EnPassantTarget.Col - FromSquare.Col != -1))
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
            if (board[FromSquare].Type != Type.Knight)
            {
                return false;
            }
            else
            {
            }

            // Piece is correct color
            if (board[FromSquare].Color != board.ColorToPlay)
            {
                return false;
            }
            else
            {
            }

            // End is (+/-2,+/-1) or (+/-1,+/-2) away
            int rowDistance = Math.Abs(FromSquare.Row - ToSquare.Row);
            int colDistance = Math.Abs(FromSquare.Col - ToSquare.Col);
            if ((rowDistance == 1 && colDistance == 2) ||
                (rowDistance == 2 && colDistance == 1))
            {
                ; // Do nothing
            }
            else
            {
                return false;
            }

            // Check end square

            // If end square is empty, move is good.
            if (board[ToSquare].Type == Type.Empty)
                return true;

            // If end square is occupied and opposite color, it's good (a capture)
            else if (board[ToSquare].Color != board.ColorToPlay)
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
            if (board[FromSquare].Type != Type.King || board[FromSquare].Color != board.ColorToPlay)
            {
                return false;
            }

            // Make sure START and END are on same row
            if (FromSquare.Row != ToSquare.Row)
                return false;

            // Make sure START is rank 1 or rank 8
            if (FromSquare.Row != 0 && FromSquare.Row != 7)
            {
                return false;
            }

            // Make sure START is on E file
            if (FromSquare.Col != 4)
                return false;

            // Make sure END is on C or G file
            if (ToSquare.Col != 2 && ToSquare.Col != 6)
                return false;

            // Determine if it's queenside
            bool isQueenside;
            if (ToSquare.Col == 2)
                isQueenside = true;
            else
                isQueenside = false;

            // Determine color
            bool isWhite;
            int row;
            if (ToSquare.Row == 0)
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
            if (isQueenside)
            {
                columnsNotOccupied = new int[] { 1, 2, 3 };
                columnsNotInCheck = new int[] { 2, 3, 4 };
            }
            else
            {
                columnsNotOccupied = new int[] { 5, 6 };
                columnsNotInCheck = new int[] { 4, 5, 6 };
            }

            // Make sure all the squares in columnsNotInCheck are not in check
            foreach (int column in columnsNotInCheck)
            {
                Coord coord = new Coord(row, column);
                if (IsAttacked(coord))
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
            if (board[FromSquare].Type != Type.Pawn)
            {
                //Console.WriteLine("BAD: {0} is not a pawn", StartSquare);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} is a pawn", StartSquare);
            }

            if (board[FromSquare].Color != board.ColorToPlay)
            {
                //Console.WriteLine("BAD: {0} is {1}, but it is {2} to play", StartSquare, board[StartSquare].Color, board.ColorToPlay);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} is {1}, and it is {2} to play", StartSquare, board[StartSquare].Color, board.ColorToPlay);
            }

            if (FromSquare.Col != ToSquare.Col)
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

            int distance = (ToSquare.Row - FromSquare.Row) * direction;
            //Console.WriteLine("DEBUG: ColorToPlay={0}  direction={1}", board.ColorToPlay, direction);
            //Console.WriteLine("DEBUG: end={0}  start={1}  distance={2}", EndSquare.Row, StartSquare.Row, distance);

            if (distance < 0)
            {
                //Console.WriteLine("BAD: {0} --> {1} is going the wrong way", StartSquare, EndSquare);
                return false;
            }
            else
            {
                //Console.WriteLine("GOOD: {0} --> {1} is going the right way", StartSquare, EndSquare);
            }


            if (distance == 1)
            {
                //Console.WriteLine("DEBUG: Moving one square", board.ColorToPlay, direction);
                if (board[ToSquare].Type == Type.Empty)
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

                if (board.ColorToPlay == Color.White)
                {
                    validStartRow = 1;
                }
                else
                {
                    validStartRow = 6;
                }

                if (FromSquare.Row != validStartRow)
                {
                    //Console.WriteLine("BAD: {0} is not on the correct row. Its row is {1}. The valid row with {2} to play is {3}",StartSquare, StartSquare.Row, board.ColorToPlay, validStartRow);
                    return false;
                }
                else
                {
                    //Console.WriteLine("GOOD: {0} is on the correct row. Its row is {1}. The valid row with {2} to play is {3}", StartSquare, StartSquare.Row, board.ColorToPlay, validStartRow);
                }

                Coord midPoint = new Coord(FromSquare.Row + direction, FromSquare.Col);
                //Console.WriteLine("DEBUG: Midpoint is {0}", midPoint);

                if (board[ToSquare].Type == Type.Empty && board[midPoint].Type == Type.Empty)
                {
                    //Console.WriteLine("GOOD: {0} and {1} are both empty.", midPoint, EndSquare);
                    //Console.WriteLine("GOOD: MOVE IS PSEUDOLEGAL.");

                    // Set en passant target
                    resultingEnPassantTarget = midPoint;
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

            if (type == Type.Bishop || type == Type.Queen || type == Type.King)
            {
                movesDiagonal = true;
            }
            if (type == Type.Rook || type == Type.Queen || type == Type.King)
            {
                movesStraight = true;
            }
            if (type == Type.King)
            {
                limitedDistance = true;
            }

            // Start is of Type type
            if (board[FromSquare].Type != type)
            {
                return false;
            }

            // Can't start and end on same square
            if (FromSquare == ToSquare)
            {
                return false;
            }

            // Start is of correct color
            if (board[FromSquare].Color != board.ColorToPlay)
            {
                return false;
            }

            // Figure out if axis exists and if it's valid
            int rowsAhead = (ToSquare.Row - FromSquare.Row);
            int colsAhead = (ToSquare.Col - FromSquare.Col);

            // Will be either 0 or 1
            int rowDirection;
            int colDirection;
            string axis;

            // Vertical
            if (colsAhead == 0)
            {
                if (!movesStraight)
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
            else if (rowsAhead == 0)
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
            else if (rowsAhead == colsAhead)
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
            else if (rowsAhead == -colsAhead)
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
            if (limitedDistance && (colsAhead > 1 || colsAhead < -1 || rowsAhead > 1 || rowsAhead < -1))
            {
                return false;
            }


            // Iterate along direction until the end square
            // All intermediate squares should be empty
            Coord currentCoord = new Coord(FromSquare.Row + rowDirection, FromSquare.Col + colDirection);
            while (currentCoord != ToSquare)
            {
                if (board[currentCoord].Type != Type.Empty)
                {
                    return false;
                }
                currentCoord = new Coord(currentCoord.Row + rowDirection, currentCoord.Col + colDirection);

            }

            // Check end square

            // If end square is empty, move is good.
            if (board[ToSquare].Type == Type.Empty)
                return true;

            // If end square is occupied and opposite color, it's good (a capture)
            else if (board[ToSquare].Color != board.ColorToPlay)
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

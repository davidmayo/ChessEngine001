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
        public Piece PawnPromotionType { get => new Piece(pawnPromotionType, PieceToMove.Color); }

        // The en passant target square that will result if this move is carried out
        // Will be null UNLESS this is a 2 square pawn push, in which case
        // It will be the coordinate BEHIND the pawn (ie, the square between
        // the To and From squares)
        public Coord ResultingEnPassantTarget { get => resultingEnPassantTarget; }
        public bool IsPseudoLegal { get => isPseudoLegal; }
        public bool IsLegal { get => isLegal; }
        public bool IsCapture { get => isCapture; }
        public bool IsCastle { get => isCastle; }
        public bool IsPawnPromotion { get => isPawnPromotion; }
        public bool IsEnPassantCapture { get => isEnPassantCapture; }


        private Coord resultingEnPassantTarget;

        // A string representing the move, form like "[a2-a4] Ra4"
        // This is a placeholder that needs to be removed eventually
        private string moveString;

        // field for PawnPromotionType property
        private Type pawnPromotionType;

        private bool isPseudoLegal;
        private bool isLegal;
        private bool isCapture;
        private bool isPawnPromotion;
        private bool isEnPassantCapture;
        private bool isCastle;

        private void EvaluateMove()
        {
            // If no piece on From Square, move is not legal
            if (PieceToMove.Type == Type.Empty)
            {
                moveString = string.Format("[{0}-{1}] Illegal move: {0} unoccupied", FromSquare, ToSquare);
                isPseudoLegal = false;
                isLegal = false;
                return;
            }

            // If piece on From Square is opposite color, move is not legal
            if (PieceToMove.Color != board.ColorToPlay)
            {
                moveString = string.Format("[{0}-{1}] Illegal move: {0} wrong color", FromSquare, ToSquare);
                isPseudoLegal = false;
                isLegal = false;
                return;
            }

            // Otherwise, check for the individual piece moves
            if (PieceToMove.Type == Type.Pawn)
                EvaluatePawnMove();
            else if (PieceToMove.Type == Type.Knight)
                EvaluateKnightMove();
            else if (PieceToMove.Type == Type.Bishop)
                EvaluateBishopMove();
            else if (PieceToMove.Type == Type.Rook)
                EvaluateRookMove();
            else if (PieceToMove.Type == Type.Queen)
                EvaluateQueenMove();
            else if (PieceToMove.Type == Type.King)
                EvaluateKingMove();
            else
                throw new Exception("What the hell kind of chess piece is this?");
        }
        private void EvaluateSlidingPieceMove(Type type)
        {
            bool limitedDistance = false;
            bool movesDiagonal = false;
            bool movesStraight = false;

            if (type == Type.Bishop || type == Type.Queen || type == Type.King)
                movesDiagonal = true;

            if (type == Type.Rook || type == Type.Queen || type == Type.King)
                movesStraight = true;

            if (type == Type.King)
                limitedDistance = true;

            // Can't start and end on same square
            if (FromSquare == ToSquare)
            {
                moveString = string.Format("[{0}-{1}] Illegal {2} move: Start and end square can't be same", FromSquare, ToSquare, type);
                isPseudoLegal = false;
                isLegal = false;
                return;
            }

            // Figure out if axis exists and if it's valid
            int rowsAhead = (ToSquare.Row - FromSquare.Row);
            int colsAhead = (ToSquare.Col - FromSquare.Col);

            // Will be either 0 or 1
            int rowDirection;
            int colDirection;

            // Vertical
            if (colsAhead == 0)
            {
                if (!movesStraight)
                {
                    moveString = string.Format("[{0}-{1}] Illegal {2} move: {2} can't move vertically", FromSquare, ToSquare, type);
                    isPseudoLegal = false;
                    isLegal = false;
                    return;
                }
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
                    moveString = string.Format("[{0}-{1}] Illegal {2} move: {2} can't move horizontally", FromSquare, ToSquare, type);
                    isPseudoLegal = false;
                    isLegal = false;
                    return;
                }
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
                    moveString = string.Format("[{0}-{1}] Illegal {2} move: {2} can't move diagonally", FromSquare, ToSquare, type);
                    isPseudoLegal = false;
                    isLegal = false;
                    return;
                }
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
                    moveString = string.Format("[{0}-{1}] Illegal {2} move: {2} can't move diagonally", FromSquare, ToSquare, type);
                    isPseudoLegal = false;
                    isLegal = false;
                    return;
                }
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
                moveString = string.Format("[{0}-{1}] Illegal {2} move: not a valid direction for any sliding piece.", FromSquare, ToSquare, type);
                isPseudoLegal = false;
                isLegal = false;
                return;
            }

            // Check king distance
            if (limitedDistance && (colsAhead > 1 || colsAhead < -1 || rowsAhead > 1 || rowsAhead < -1))
            {
                moveString = string.Format("[{0}-{1}] Illegal {2} move: {2} can't move more than 1 square.", FromSquare, ToSquare, type);
                isPseudoLegal = false;
                isLegal = false;
                return;
            }


            // Iterate along direction until the end square
            // All intermediate squares should be empty
            Coord currentCoord = new Coord(FromSquare.Row + rowDirection, FromSquare.Col + colDirection);
            while (currentCoord != ToSquare)
            {
                if (board[currentCoord].Type != Type.Empty)
                {
                    moveString = string.Format("[{0}-{1}] Illegal {2} move: Intermediate square {3} occupied", FromSquare, ToSquare, type, currentCoord);
                    isPseudoLegal = false;
                    isLegal = false;
                    return;
                }
                //Console.WriteLine("  DEBUG: Evaluating {0}-{1} [{2}]",FromSquare,ToSquare,type);
                //Console.WriteLine("  DEBUG: Creating new Coord({0},{1})", currentCoord.Row + rowDirection, currentCoord.Col + colDirection);

                currentCoord = new Coord(currentCoord.Row + rowDirection, currentCoord.Col + colDirection);
            }

            // Check end square

            // If end square is empty, move is good.
            if (board[ToSquare].Type == Type.Empty)
            {
                moveString = string.Format("[{0}-{1}] {2}{1}", FromSquare, ToSquare, PieceToMove.ToFenString().ToUpper());
                isPseudoLegal = true;
                isLegal = true;
                return;
            }

            // If end square is occupied and opposite color, it's good (a capture)
            else if (board[ToSquare].Color != board.ColorToPlay)
            {
                isCapture = true;

                moveString = string.Format("[{0}-{1}] {2}x{1}", FromSquare, ToSquare, PieceToMove.ToFenString().ToUpper());
                isPseudoLegal = true;
                isLegal = true;
                return;
            }

            // If end square is occupied and not opposite color, it's not legal
            // (Can't capture own piece)
            else
            {
                moveString = string.Format("[{0}-{1}] Illegal {2} move: Can't capture own piece.", FromSquare, ToSquare, type);
                isPseudoLegal = false;
                isLegal = false;
            }
        }

        private void EvaluateKingMove()
        {
            EvaluateCastleMove();
            if (isCastle)
                return;

            EvaluateSlidingPieceMove(Type.King);
        }

        private void EvaluateCastleMove()
        {
            //// Start square must be a king and must be correct color
            //if (board[FromSquare].Type != Type.King || board[FromSquare].Color != board.ColorToPlay)
            //{
            //    return false;
            //}

            // Make sure START and END are on same row
            if (FromSquare.Row != ToSquare.Row)
                return;

            // Make sure START is rank 1 or rank 8
            if (FromSquare.Row != 0 && FromSquare.Row != 7)
                return;

            // Make sure START is on E file
            if (FromSquare.Col != 4)
                return;

            // Make sure END is on C or G file
            if (ToSquare.Col != 2 && ToSquare.Col != 6)
                return;

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
                return;
            if (isQueenside && !isWhite && !board.CanCastleQueensideBlack)
                return;
            if (!isQueenside && isWhite && !board.CanCastleKingsideWhite)
                return;
            if (!isQueenside && !isWhite && !board.CanCastleKingsideBlack)
                return;

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
                    return;
                }
            }

            // Make sure all the squares in columnsNotOccupied are Empty
            foreach (int column in columnsNotOccupied)
            {
                Coord coord = new Coord(row, column);
                if (board[coord].Type != Type.Empty)
                {
                    return;
                }
            }

            //TODO

            isCastle = true;
            isPseudoLegal = true;
            isLegal = true;
            if( isQueenside)
                moveString = string.Format("[{0}-{1}] O-O-O", FromSquare, ToSquare);
            else
                moveString = string.Format("[{0}-{1}] O-O", FromSquare, ToSquare);

            //return true;
            //throw new NotImplementedException();
        }

        private void EvaluateQueenMove()
        {
            EvaluateSlidingPieceMove(Type.Queen);
        }

        private void EvaluateRookMove()
        {
            EvaluateSlidingPieceMove(Type.Rook);
        }

        private void EvaluateBishopMove()
        {
            EvaluateSlidingPieceMove(Type.Bishop);
        }

        private void EvaluateKnightMove()
        {
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
                isPseudoLegal = false;
                isLegal = false;
                this.moveString = string.Format("[{0}-{1}] Illegal knight move: not a <2,1> or <1,2> move.", FromSquare, ToSquare);
                return;
            }

            // If end square is empty, move is good.
            if (board[ToSquare].Type == Type.Empty)
            {
                isPseudoLegal = true;
                isLegal = true;
                this.moveString = string.Format("[{0}-{1}] N{1}", FromSquare, ToSquare);
            }

            // If end square is occupied and opposite color, it's good (a capture)
            else if (board[ToSquare].Color != board.ColorToPlay)
            {
                isPseudoLegal = true;
                isLegal = true;
                isCapture = true;
                this.moveString = string.Format("[{0}-{1}] Nx{1}", FromSquare, ToSquare);
            }

            // If end square is occupied and not opposite color, it's not legal
            // (Can't capture own piece)
            else
            {
                isPseudoLegal = false;
                isLegal = false;
                this.moveString = string.Format("[{0}-{1}] Illegal knight move: {1} occupied by own color.", FromSquare, ToSquare);
            }
        }

        private void EvaluatePawnMove()
        {
            int queeningRow = PieceToMove.Color == Color.White ? 7 : 0;

            int columnOffset = FromSquare.Col - ToSquare.Col;
            if (columnOffset == 0)
                EvaluatePawnPush();
            else if (columnOffset == 1 || columnOffset == -1 )
                EvaluatePawnCapture();
            else
            {
                isPseudoLegal = false;
                isLegal = false;
                this.moveString = string.Format("[{0}-{1}] Illegal pawn move: {0} and {1} more than 1 rank apart.", FromSquare, ToSquare);
            }

            if( ToSquare.Row == queeningRow )
            {
                isPawnPromotion = true;
                moveString += "=" + PawnPromotionType.ToFenString().ToUpper();
            }
        }

        private void EvaluatePawnCapture()
        {
            if (ToSquare == board.EnPassantTarget)
                EvaluateEnPassant();

            if( isEnPassantCapture )
            {
                return;
            }

            // -1 if black to play
            // +1 if white to play
            int direction = 1 - 2 * (int)board.ColorToPlay;
            if (FromSquare.Row + direction != ToSquare.Row)
            {
                isPseudoLegal = false;
                isLegal = false;
                this.moveString = string.Format("[{0}-{1}] Illegal pawn capture: {0} and {1} not on correct ranks.", FromSquare, ToSquare);

                return;
            }

            // Destination is occupied
            if (board[ToSquare].Type == Type.Empty || board[ToSquare].Color == board.ColorToPlay)
            {
                isPseudoLegal = false;
                isLegal = false;
                this.moveString = string.Format("[{0}-{1}] Illegal pawn capture: {1} empty or wrong color", FromSquare, ToSquare);
                return;
            }
            else
            {
                isCapture = true;
                isPseudoLegal = true;
                isLegal = true;
                this.moveString = string.Format("[{0}-{1}] {2}x{1}", FromSquare, ToSquare, FromSquare.ToString()[0]);
                return;
            }
        }

        private void EvaluateEnPassant()
        {
            // Make sure EnPassantTarget is not null
            if (board.EnPassantTarget is null)
            {
                return;
            }

            // Make sure that End == EnPassantTarget
            if (ToSquare != board.EnPassantTarget)
            {
                return;
            }

            // Make sure that Start is the valid row
            int validStartRow = PieceToMove.Color == Color.White ? 4 : 3;
            
            if (FromSquare.Row != validStartRow)
            {
                return;
            }

            //// Make sure that EnPassantTarget is exactly one column offset from Start
            //if ((board.EnPassantTarget.Col - FromSquare.Col != 1) &&
            //    (board.EnPassantTarget.Col - FromSquare.Col != -1))
            //{
            //    return false;
            //}

            // If we got here, it's valid


            isCapture = true;
            isEnPassantCapture = true;
            
            isLegal = true;
            isPseudoLegal = true;
            this.moveString = string.Format("[{0}-{1}] {2}x{1} (e.p.)", FromSquare, ToSquare, FromSquare.ToString()[0]);

        }

        private void EvaluatePawnPush()
        {
            // EvaluatePawnMove() already determined that the Col's are right, so don't
            // wast time here
            //if (FromSquare.Col != ToSquare.Col)
            //{
            //}
            //else
            //{
            //    //Console.WriteLine("GOOD: {0} and {1} in same column", StartSquare, EndSquare);
            //
            //}

            // -1 if black to play
            // +1 if white to play
            int direction = 1 - 2 * (int)board.ColorToPlay;

            int rowDistance = (ToSquare.Row - FromSquare.Row) * direction;
            
            if (rowDistance <= 0 || rowDistance > 2)
            {
                isPseudoLegal = false;
                isLegal = false;
                this.moveString = string.Format("[{0}-{1}] Illegal pawn push: Wrong direction and/or too far", FromSquare, ToSquare);
                return;
            }

            else if (rowDistance == 1)
            {
                if (board[ToSquare].Type == Type.Empty)
                {
                    //Console.WriteLine("GOOD: Destination square is empty");
                    //Console.WriteLine("GOOD: MOVE IS PSEUDOLEGAL");

                    isPseudoLegal = true;
                    isLegal = true;
                    this.moveString = string.Format("[{0}-{1}] {1}", FromSquare, ToSquare);

                    return;
                }
                else
                {
                    isPseudoLegal = false;
                    isLegal = false;
                    this.moveString = string.Format("[{0}-{1}] Illegal pawn push: {1} already occupied", FromSquare, ToSquare);
                }
            } // else if (rowDistance == 1)

            // Validate two square pawn push
            if (rowDistance == 2)
            {
                // 1 or 6
                int validStartRow = board.ColorToPlay == Color.White ? 1 : 6;

                if (FromSquare.Row != validStartRow)
                {
                    isPseudoLegal = false;
                    isLegal = false;
                    this.moveString = string.Format("[{0}-{1}] Illegal pawn push: 2 square push from {0} not allowed.", FromSquare, ToSquare);
                    return;
                }

                // The midPoint of From and To must be empty
                // and will also be the En Passant target square
                Coord midPointSquare = new Coord(FromSquare.Row + direction, FromSquare.Col);

                // If ToSquare and midPointSquare are both empty, move is valid.
                if (board[ToSquare].Type == Type.Empty && board[midPointSquare].Type == Type.Empty)
                {
                    // Set en passant info
                    resultingEnPassantTarget = midPointSquare;
                    isEnPassantCapture = true;

                    isPseudoLegal = true;
                    isLegal = true;
                    this.moveString = string.Format("[{0}-{1}] {1}", FromSquare, ToSquare);
                    return;
                }
                else
                {
                    isPseudoLegal = false;
                    isLegal = false;
                    this.moveString = string.Format("[{0}-{1}] Illegal pawn push: {2} and/or {1} already occupied",
                        FromSquare, ToSquare, midPointSquare);

                    return;
                }
            } // if (rowDistance == 2)
        } // EvaluatePawnPush()

        public bool IsCheck()
        {
            // TODO
            return false;
        }
        public override string ToString()
        {
            return this.moveString;
        }

        public Move(Coord start, Coord end, Board board, Type pawnPromotionTarget = Type.Queen)
        {
            resultingEnPassantTarget = null;
            FromSquare = start;
            ToSquare = end;
            this.board = board;

            this.PieceToMove = this.board[FromSquare];
            pawnPromotionType = pawnPromotionTarget;

            // TODO: Refactor this to be something like EvaluateMove();
            //_ = IsPseudoLegalMove();

            EvaluateMove();
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
            var pieceType = (promotionString.ToLower()) switch
            {
                "" => Type.Unknown,
                "q" => Type.Queen,
                "n" => Type.Knight,
                "r" => Type.Rook,
                "b" => Type.Bishop,
                _ => Type.Unknown,
            };
            pawnPromotionType = pieceType;

            // TODO: Refactor this to be something like EvaluateMove();
            //_ = IsPseudoLegalMove();

            EvaluateMove();

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

        private bool IsAttacked(Coord coord)
        {
            // TODO
            return false;
            throw new NotImplementedException();
        }
    }
}

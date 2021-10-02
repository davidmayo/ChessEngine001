using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine001
{
    class MoveGenerator
    {
        private Coord heroKingCoord;
        private Coord villainKingCoord;
        private Color heroColor;
        private Color villainColor;
        private Board board;
        private List<Coord> heroOccupiedSquares;
        private List<Coord> villainOccupiedSquares;
        private HashSet<Coord> attackedSquares;
        private List<Move> legalMoves;
        private List<Coord> allSquares;

        public List<Move> LegalMoves { get => legalMoves; }

        public MoveGenerator(Board board)
        {
            this.board = board;
            this.heroColor    = this.board.ColorToPlay;
            this.villainColor = 1 - heroColor;
            this.attackedSquares = new HashSet<Coord>();

            // Generate all Coords
            allSquares = GetSquares();

            // Locate hero and villain pieces
            heroOccupiedSquares = GetSquares(heroColor);
            villainOccupiedSquares = GetSquares(villainColor);

            // Locate the kings
            FindKings();

            // Kind legal moves
            FindLegalMoves();
        }

        private void FindLegalMoves()
        {
            int queeningRow = heroColor == Color.White ? 7 : 0;
            int castlingRow = heroColor == Color.White ? 0 : 0;

            legalMoves = new List<Move>();

            // Add all the normal moves
            FindNormalLegalMoves();

            // Add pawn pushes
            FindPawnPushMoves();

            // Add en passant
            FindEnPassantMoves();

            // Add castling
            FindCastlingMoves();
        }

        private void FindPawnPushMoves()
        {
            int queeningRow = heroColor == Color.White ?  7 :  0;
            int startRow    = heroColor == Color.White ?  1 :  6;
            int direction   = heroColor == Color.White ?  1 : -1;

            CoordOffset attackOffsetQueenside = new CoordOffset(direction,-1);
            CoordOffset attackOffsetKingside  = new CoordOffset(direction,1);
            CoordOffset pushOffset = new CoordOffset(direction, 0);
            
            Coord pushSquare;
            Coord attackSquareQueenside;
            Coord attackSquareKingside;


            var startSquares = GetSquares(Type.Pawn, heroColor);
            foreach ( var startSquare in startSquares)
            {
                pushSquare = startSquare + pushOffset;
                attackSquareQueenside = startSquare + attackOffsetQueenside;
                attackSquareKingside  = startSquare + attackOffsetKingside;

                // Check queenside attack square
                foreach (var attackSquare in new Coord[]{ attackSquareQueenside, attackSquareKingside })
                {
                    if (
                        !(attackSquare is null) &&
                        board[attackSquare].Type != Type.Empty &&
                        board[attackSquare].Color == villainColor
                    )
                    {
                        // TODO: See if hero is in check as a result

                        // If pushing to the queening square, create a move for each promotion
                        if (attackSquare.Row == queeningRow)
                        {
                            legalMoves.Add(new Move(startSquare, attackSquare, board, Type.Queen));
                            legalMoves.Add(new Move(startSquare, attackSquare, board, Type.Rook));
                            legalMoves.Add(new Move(startSquare, attackSquare, board, Type.Bishop));
                            legalMoves.Add(new Move(startSquare, attackSquare, board, Type.Knight));
                        }
                        // Otherwise, don't specify promotion type
                        else
                        {
                            legalMoves.Add(new Move(startSquare, attackSquare, board));
                        }
                    }
                }

                

                // if the push square is empty, we can push
                if ( board[pushSquare].Type == Type.Empty)
                {
                    // If pushing to the queening square, create a move for each promotion
                    if( pushSquare.Row == queeningRow )
                    {
                        legalMoves.Add(new Move(startSquare, pushSquare, board, Type.Queen));
                        legalMoves.Add(new Move(startSquare, pushSquare, board, Type.Rook));
                        legalMoves.Add(new Move(startSquare, pushSquare, board, Type.Bishop));
                        legalMoves.Add(new Move(startSquare, pushSquare, board, Type.Knight));
                    }

                    // Otherwise, don't specify promotion type
                    else
                    {
                        legalMoves.Add(new Move(startSquare, pushSquare, board));
                    }

                    // If on the start square, look one more square
                    if (startSquare.Row == startRow)
                    {
                        pushSquare += pushOffset;
                        if( board[pushSquare].Type == Type.Empty)
                        {
                            // Can never be pawn promotion, so don't bother checking.
                            legalMoves.Add(new Move(startSquare, pushSquare, board));
                        }
                    }
                } // if
            } // foreach loop
        } // FindPawnPushes()

        private void FindEnPassantMoves()
        {
        }

        private void FindCastlingMoves()
        {
            int castlingRow = heroColor == Color.White ? 0 : 7;
            Coord startSquare = new Coord(castlingRow, 4);

            Coord kingsideEndSquare  = new Coord(castlingRow, 6);

            // The squares the king will pass through that need to be non-attacked
            List<Coord> kingsideKingSquares = new List<Coord>() {
                new Coord( castlingRow, 4),
                new Coord( castlingRow, 5),
                new Coord( castlingRow, 6)
            };

            // The squares that must be empty
            List<Coord> kingsideEmptySquares = new List<Coord>() {
                new Coord( castlingRow, 5),
                new Coord( castlingRow, 6)
            };


            Coord queensideEndSquare = new Coord(castlingRow, 2);
            
            // The squares the king will pass through that need to be non-attacked
            List<Coord> queensideKingSquares = new List<Coord>() {
                new Coord( castlingRow, 4),
                new Coord( castlingRow, 3),
                new Coord( castlingRow, 2)
            };

            // The squares that must be empty
            List<Coord> queensideEmptySquares = new List<Coord>() {
                new Coord( castlingRow, 3),
                new Coord( castlingRow, 2),
                new Coord( castlingRow, 1)
            };

            // Kingside
            bool isKingsidePossible = (heroColor == Color.White ? board.CanCastleKingsideWhite : board.CanCastleKingsideBlack); ;
            foreach( var square in kingsideEmptySquares )
            {
                if( board[square].Type != Type.Empty)
                {
                    isKingsidePossible = false;
                    break;
                }
            }
            foreach (var square in kingsideKingSquares)
            {
                if (attackedSquares.Contains(square))
                {
                    isKingsidePossible = false;
                    break;
                }
            }
            if( isKingsidePossible )
            {
                legalMoves.Add(new Move(startSquare, kingsideEndSquare, board));
            }

            // Kingside
            bool isQueensidePossible = ( heroColor == Color.White ? board.CanCastleQueensideWhite : board.CanCastleQueensideBlack );
            foreach (var square in queensideEmptySquares)
            {
                if (board[square].Type != Type.Empty)
                {
                    isQueensidePossible = false;
                    break;
                }
            }
            foreach (var square in queensideKingSquares)
            {
                if (attackedSquares.Contains(square))
                {
                    isQueensidePossible = false;
                    break;
                }
            }
            if (isQueensidePossible)
            {
                legalMoves.Add(new Move(startSquare, queensideEndSquare,board));
            }
        }

        // All moves except pawn pushes, en passant, and castling
        private void FindNormalLegalMoves()
        {
            int queeningRow = heroColor == Color.White ? 7 : 0;
            Move move;

            foreach (var startSquare in heroOccupiedSquares)
            {
                // If it's a pawn, ignore it.
                if( board[startSquare].Type == Type.Pawn)
                {
                    continue;
                }

                List<Coord> attackSquares = GetSquaresAttackedBy(startSquare);

                foreach (Coord destSquare in attackSquares)
                {
                    // Create a different move for each pawn promotion
                    if (destSquare.Row == queeningRow && board[startSquare].Type == Type.Pawn)
                    {
                        move = new Move(startSquare, destSquare, board, Type.Queen);
                        move = new Move(startSquare, destSquare, board, Type.Rook);
                        move = new Move(startSquare, destSquare, board, Type.Bishop);
                        move = new Move(startSquare, destSquare, board, Type.Knight);
                        continue;
                    }
                    move = new Move(startSquare, destSquare, board);

                    // TODO: Verify doesn't move into check

                    legalMoves.Add(move);
                }
            }
        }

        private void FindKings()
        {
            heroKingCoord = null;
            villainKingCoord = null;

            foreach (Coord square in heroOccupiedSquares)
            {
                if (board[square].Type == Type.King)
                {
                    heroKingCoord = square;
                }
            }
            if (heroKingCoord is null)
                throw new Exception(string.Format("Board does not have a {0} king.", heroColor));


            foreach (Coord square in villainOccupiedSquares)
            {
                if (board[square].Type == Type.King)
                {
                    villainKingCoord = square;
                }
            }
            if (villainKingCoord is null)
                throw new Exception(string.Format("Board does not have a {0} king.", villainColor));
        }

        public List<Coord> GetSquares()
        {
            List<Coord> allSquares = new List<Coord>(64);

            for( int row = 0; row < 8; row++)
            {
                for( int col = 0; col < 8; col++)
                {
                    allSquares.Add(new Coord(row,col));
                }
            }
            return allSquares;
        }

        public List<Coord> GetSquares(Color color)
        {
            List<Coord> allSquares = new List<Coord>();
            foreach (var square in GetSquares())
            {
                if (board[square].Type != Type.Empty && board[square].Color == color)
                {
                    allSquares.Add(square);
                }
            }
            return allSquares;
        }

        public List<Coord> GetSquares(Type type)
        {
            List<Coord> allSquares = new List<Coord>();
            foreach (var square in GetSquares())
            {
                if (board[square].Type == Type.Empty)
                {
                    allSquares.Add(square);
                }
            }
            return allSquares;
        }

        public List<Coord> GetSquares(Type type, Color color)
        {
            List<Coord> allSquares = new List<Coord>();
            foreach (var square in GetSquares())
            {
                if (board[square].Type == type && board[square].Color == color)
                {
                    allSquares.Add(square);
                }
            }
            return allSquares;
        }

        private void FindAttackedSquares()
        {
            attackedSquares = new HashSet<Coord>();

            foreach( Coord startSquare in GetSquares())
            {
                List<Coord> destSquares = GetSquaresAttackedBy(startSquare);

                foreach (var destSquare in destSquares)
                {
                    attackedSquares.Add(destSquare);
                }
            }
        }


        private List<Coord> GetSquaresAttackedBy(Coord coord)
        {
            Piece piece = board[coord];
            Type type = piece.Type;
            Color color = piece.Color;

            if (type == Type.Empty || type == Type.Unknown)
                return new List<Coord>();
            else if (type == Type.Knight)
                return GetSquaresAttackedByKnight(coord);
            else if (type == Type.Pawn)
                return GetSquaresAttackedByPawn(coord);
            else
                return GetSquaresAttackedBySlidingPiece(coord);
        }

        private List<Coord> GetSquaresAttackedBySlidingPiece(Coord startCoord)
        {
            Type type = board[startCoord].Type;
            bool shortDistancePiece = (type == Type.King);
            List<Coord> pieceAttackSquares = new List<Coord>();
            List<CoordOffset> directions = new List<CoordOffset>();

            // Horizontal/vertical
            if (type == Type.Queen || type == Type.Rook || type == Type.King)
            {
                directions.Add(new CoordOffset(0, -1));
                directions.Add(new CoordOffset(0, 1));
                directions.Add(new CoordOffset(1, 0));
                directions.Add(new CoordOffset(-1, 0));
            }

            // Diagonal
            if (type == Type.Queen || type == Type.Bishop || type == Type.King)
            {
                directions.Add(new CoordOffset(1, 1));
                directions.Add(new CoordOffset(1, -1));
                directions.Add(new CoordOffset(-1, 1));
                directions.Add(new CoordOffset(-1, -1));
            }

            foreach (var direction in directions)
            {
                Coord targetCoord = startCoord;

                while (!(targetCoord is null))
                {
                    targetCoord += direction;

                    // If it's not null, it's a square to investigate
                    if (!(targetCoord is null))
                    {
                        // If target is empty, add it to the attacked squares and continue
                        if (board[targetCoord].Type == Type.Empty)
                        {
                            pieceAttackSquares.Add(targetCoord);
                            ;
                        }

                        // If target is same color, do not add it to attacked squares
                        // and end loop searching in this direction
                        else if (board[targetCoord].Color == heroColor)
                        {
                            break;
                        }

                        // Otherwise, add to attacked squares
                        // and end loop searching in this direction
                        else
                        {
                            pieceAttackSquares.Add(targetCoord);
                            break;
                        }
                    } // End of if targetCoord is not null

                    // If it's the king, only go one square any any direction
                    if (shortDistancePiece)
                        break;
                } // for loop for a given direction
            } // outer for loop
            return pieceAttackSquares;
        }


        private List<Coord> GetSquaresAttackedByKnight(Coord coord)
        {
            
            List<Coord> knightAttackSquares = new List<Coord>();
            List<CoordOffset> offsets = new List<CoordOffset>();

            offsets.Add(new CoordOffset(2, 1));
            offsets.Add(new CoordOffset(2, -1));
            offsets.Add(new CoordOffset(-2, 1));
            offsets.Add(new CoordOffset(-2, -1));
            offsets.Add(new CoordOffset(1, 2));
            offsets.Add(new CoordOffset(1, -2));
            offsets.Add(new CoordOffset(-1, 2));
            offsets.Add(new CoordOffset(-1, -2));

            foreach (var offset in offsets)
            {
                Coord attackedSquare = coord + offset;

                // If it's null, that means it's off the board. Take no action.
                if (attackedSquare is null)
                {
                    continue;
                }

                // If it's empty, add to targetedCoords
                if (board[attackedSquare].Type == Type.Empty)
                {
                    knightAttackSquares.Add(attackedSquare);
                }

                // If the square is same color, it's not attacking
                else if (board[attackedSquare].Color == heroColor)
                {
                    continue;
                }

                else
                {
                    knightAttackSquares.Add(attackedSquare);
                }
            }
            return knightAttackSquares;
        }


        private List<Coord> GetSquaresAttackedByPawn(Coord coord)
        {
            List<Coord> pawnAttackSquares = new List<Coord>();
            List<CoordOffset> offsets = new List<CoordOffset>();

            if (heroColor == Color.White)
            {
                offsets.Add(new CoordOffset(1, 1));
                offsets.Add(new CoordOffset(1, -1));
            }
            else
            {
                offsets.Add(new CoordOffset(-1, 1));
                offsets.Add(new CoordOffset(-1, -1));
            }
            foreach (var offset in offsets)
            {
                Coord attackedSquare = coord + offset;

                // If it's null, that means it's off the board. Take no action.
                if (attackedSquare is null)
                {
                    continue;
                }

                // If it's empty, add to targetedCoords
                if (board[attackedSquare].Type == Type.Empty)
                {
                    pawnAttackSquares.Add(attackedSquare);
                }

                // If the square is same color, it's not attacking
                else if (board[attackedSquare].Color == heroColor)
                {
                    continue;
                }

                else
                {
                    pawnAttackSquares.Add(attackedSquare);
                }
            }
            return pawnAttackSquares;
        }

    }
}

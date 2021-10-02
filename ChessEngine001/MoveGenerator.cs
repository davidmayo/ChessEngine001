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
        private List<Coord> attackedSquares;
        private List<Move> legalMoves;
        private List<Coord> allSquares;


        MoveGenerator(Board board)
        {
            this.board = board;
            this.heroColor    = this.board.ColorToPlay;
            this.villainColor = 1 - heroColor;

            // Generate all Coords
            allSquares = board.GetAllSquares();

            // Locate the kings
            FindKings();

            // Locate hero and villain pieces
            heroOccupiedSquares    = FindPieces( heroColor );
            villainOccupiedSquares = FindPieces( villainColor );

        }


        private List<Coord> FindPieces(Color villainColor)
        {
            throw new NotImplementedException();
        }

        private void FindKings()
        {
            heroKingCoord = null;
            villainKingCoord = null;



            throw new NotImplementedException();
        }

        public static bool[,] GetAttackedSquares(Board board)
        {
            Color colorToPlay = board.ColorToPlay;
            bool[,] result = new bool[8, 8];

            for( int row = 0; row <8; row++)
            {
                for( int col = 0; col < 8; col++)
                {
                    Coord coord = new Coord(row, col);
                    if (board[coord].Type != Type.Empty && board[coord].Color == colorToPlay)
                    {

                        var attackedSquares = SquaresAttackedBy(coord, board);

                        foreach (var square in attackedSquares)
                        {
                            result[square.Row, square.Col] = true;
                        }
                    }
                }
            }

            return result;
        }

        public static List<Coord> SquaresAttackedBy(Coord coord, Board board)
        {
            Piece piece = board[coord];
            Type type = piece.Type;
            Color color = piece.Color;

            if (type == Type.Empty || type == Type.Unknown)
                return new List<Coord>();
            else if (type == Type.Knight)
                return SquaresAttackedByKnight(coord, type, color, board);
            else if (type == Type.Pawn)
                return SquaresAttackedByPawn(coord, type, color, board);
            else
                return SquaresAttackedBySlidingPiece(coord, type, color, board);
        }

        private static List<Coord> SquaresAttackedBySlidingPiece(Coord startCoord, Type type, Color color, Board board)
        {
            bool shortDistancePiece = (type == Type.King);
            List<Coord> attackedCoords = new List<Coord>();
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
                        if( board[targetCoord].Type == Type.Empty)
                        {
                            attackedCoords.Add(targetCoord);
                            ;
                        }

                        // If target is same color, do not add it to attacked squares
                        // and end loop searching in this direction
                        else if(board[targetCoord].Color == color)
                        {
                            break;
                        }

                        // Otherwise, add to attacked squares
                        // and end loop searching in this direction
                        else
                        {
                            attackedCoords.Add(targetCoord);
                            break;
                        }
                    } // End of if targetCoord is not null

                    // If it's the king, only go one square any any direction
                    if (shortDistancePiece)
                        break;
                } // for loop for a given direction
            } // outer for loop
            return attackedCoords;
            throw new NotImplementedException();
        }

        private static List<Coord> SquaresAttackedByKnight(Coord coord, Type type, Color color, Board board)
        {
            List<Coord> attackedSquares = new List<Coord>();
            List<CoordOffset> offsets = new List<CoordOffset>();

            offsets.Add(new CoordOffset(2, 1));
            offsets.Add(new CoordOffset(2, -1));
            offsets.Add(new CoordOffset(-2, 1));
            offsets.Add(new CoordOffset(-2, -1));
            offsets.Add(new CoordOffset(1, 2));
            offsets.Add(new CoordOffset(1, -2));
            offsets.Add(new CoordOffset(-1, 2));
            offsets.Add(new CoordOffset(-1, -2));

            foreach( var offset in offsets )
            {
                Coord attackedSquare = coord + offset;

                // If it's null, that means it's off the board. Take no action.
                if(attackedSquare is null)
                {
                    continue;
                }

                // If it's empty, add to targetedCoords
                if( board[attackedSquare].Type == Type.Empty)
                {
                    attackedSquares.Add(attackedSquare);
                }

                // If the square is same color, it's not attacking
                else if( board[attackedSquare].Color == color )
                {
                    continue;
                }

                else
                {
                    attackedSquares.Add(attackedSquare);
                }
            }
            return attackedSquares;
        }

        private static List<Coord> SquaresAttackedByPawn(Coord coord, Type type, Color color, Board board)
        {
            List<Coord> attackedSquares = new List<Coord>();
            List<CoordOffset> offsets = new List<CoordOffset>();

            if( color == Color.White )
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
                    attackedSquares.Add(attackedSquare);
                }

                // If the square is same color, it's not attacking
                else if (board[attackedSquare].Color == color)
                {
                    continue;
                }

                else
                {
                    attackedSquares.Add(attackedSquare);
                }
            }
            return attackedSquares;
        }
    }
}

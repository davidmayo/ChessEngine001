using System;

namespace ChessEngine001
{
    class Program
    {
        static void Main(string[] args)
        {
            // Make a change again
            Board board;
            board = new Board("--------/--------/----k---/--------/--------/--------/--------/--------");

            board = new Board("rnb-k-nr/-pppp---/--------/--------/pbq-----/-P--Rppp/P-PPPPPP/RNBQKBN-");
            //board = new Board("n-------/--------/--------/--------/--------/--------/--------/--------");
            //board = new Board();
            //board = new Board("rnbqkbnr/pppppppp/--------/--------/-p-p----/--------/PPPPPPPP/RNBQKBNR");
            //board = new Board("r---k--r/pppppppp/--------/--------/--------/--------/PPPPPPPP/R---K--R");

            //board = new Board();
            //board.ColorToPlay = Color.Black;
            //Console.WriteLine("Hello World!");
            //Console.WriteLine();
            //Console.WriteLine(board);


            /*Move move;
            //move = new Move(new Coord("E2"),new Coord("E4"), board);
            board.ColorToPlay = Color.Black;
            board.ColorToPlay = Color.White;

            move = new Move("A2", "A4", board);
            //Console.WriteLine(move);
            //Console.WriteLine(move.Piece);
            //Console.WriteLine(move.Piece.Color);
            Console.WriteLine(move.IsPseudoLegalPawnPush());

            board.PrintBoard();
            board.ColorToPlay = Color.Black;*/


            //board.ColorToPlay = Color.Black;
            System.Collections.Generic.List<Move> moves;
            board.PrintBoard();
            Console.WriteLine();
            Console.WriteLine();
            Move move;


            //board.ColorToPlay = Color.Black;
            /*move = new Move("E8", "G8", board);
            if( move.IsPseudoLegalCastleMove())
            {
                Console.WriteLine("Legal!");
            }
            else
            {
                Console.WriteLine("ILLEGAL!");

            }*/


            moves = new System.Collections.Generic.List<Move>();


            for (int startIndex = 0; startIndex < 64; startIndex++)
            {
                for (int endIndex = 0; endIndex < 64; endIndex++)
                {
                    Coord start = new Coord(startIndex);
                    Coord end = new Coord(endIndex);

                    Move mv = new Move(start, end, board);

                    if (mv.IsPseudoLegalMove())
                    {
                        //moves.Add(mv);
                        Console.WriteLine("Pseudo-legal move: {0}", mv.MoveString);
                    }
                }
            }
        }
    }
}

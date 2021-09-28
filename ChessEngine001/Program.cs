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

            //board = new Board("rnb-k-nr/-pppp---/--------/--------/pbq-----/-P--Rppp/P-PPPPPP/RNBQKBN-");
            //board = new Board("n-------/--------/--------/--------/--------/--------/--------/--------");
            //board = new Board();
            //board = new Board("rnbqkbnr/pppppppp/--------/--------/-p-p----/--------/PPPPPPPP/RNBQKBNR");
            //board = new Board("r---k--r/pppppppp/--------/--------/--------/--------/PPPPPPPP/R---K--R");
            
            board.PrintBoard();


            Move move = new Move("e2", "e4", board);
            if( move.IsPseudoLegalMove())
            {
                board.MakeMove(move);
            }
            board.PrintBoard();

        }
    }
}

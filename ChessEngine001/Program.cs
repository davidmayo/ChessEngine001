﻿using System;

namespace ChessEngine001
{
    class Program
    {
        static void Main(string[] args)
        {
            // Make a change again
            Board board;
            board = new Board();
            //board = new Board("--------/--------/----k---/--------/--------/--------/--------/--------");

            //board = new Board("rnb-k-nr/-pppp---/--------/--------/pbq-----/-P--Rppp/P-PPPPPP/RNBQKBN-");
            //board = new Board("n-------/--------/--------/--------/--------/--------/--------/--------");
            //board = new Board();
            //board = new Board("rnbqkbnr/pppppppp/--------/--------/-p-p----/--------/PPPPPPPP/RNBQKBNR");
            //board = new Board("r---k--r/pppppppp/--------/--------/--------/--------/PPPPPPPP/R---K--R");
            

            string testPositionFen  = "rnbNk1nr/1pppp3/8/8/pbq5/1P2Rppp/P1PPPPPP/RNBQKBN1 w KQkq - 5 40";
            string startPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            string castlePositionFen = "r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1";
            //board.UpdateBoardFromFenString(testPositionFen);
            //board.UpdateBoardFromFenString(startPositionFen);
            //board.UpdateBoardFromFenString(testPositionFen);

            board.PrintBoard();
            board = new Board(testPositionFen);
            board.ColorToPlay = Color.Black;
            board.PrintBoard();

            Move move;
            int count = 0;
            for( int start = 0; start < 64; start++)
            {
                for( int end = 0; end < 64; end++ )
                {
                    move = new Move(new Coord(start), new Coord(end), board);
                    if (move.IsPseudoLegalMove())
                    {
                        count++;
                        Console.WriteLine("[pseudo-legal] {0,5}: {1}", count, move);
                    }
                }
            }
        } // Main()
    }
}

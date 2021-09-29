using System;

namespace ChessEngine001
{
    class Program
    {
        static void Main(string[] args)
        {
            // Make a change again
            Board board;
            string testPositionFen  = "rnbNk1nr/1pppp3/8/8/pbq5/1P2Rppp/P1PPPPPP/RNBQKBN1 w KQkq - 5 40";
            string startPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            string castlePositionFen = "r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1";
            string capturePositionFen = "k7/4B3/2Kb4/2P5/2NR1Q2/8/8/8 w - - 0 1";
            
            board = new Board(capturePositionFen);
            //board.ColorToPlay = Color.Black;
            board.PrintBoard();

            Move move;

            move = new Move("e2","e4",board);
            move = new Move("a2a4r", board);

            Console.WriteLine("MOVE DEFAULT: {0}", move);
            Console.WriteLine("MOVE UCI    : {0}", move.ToUciMoveString());

            if ( move.IsPseudoLegalMove())
            {
                board.MakeMove(move);
            }
            board.PrintBoard();

            //return;
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

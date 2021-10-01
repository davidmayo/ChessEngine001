using System;

namespace ChessEngine001
{
    class Program
    {
        static void Main()
        {
            // Make a change again
            Board board;
            string testPositionFen  = "rnbNk1nr/1pppp3/8/8/pbq5/1P2Rppp/P1PPPPPP/RNBQKBN1 w KQkq - 5 40";
            string startPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            string castlePositionFen = "r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1";
            string blackEnPassant = "rnbqkbnr/ppp1pppp/8/8/3pP3/NP6/P1PP1PPP/R1BQKBNR b KQkq e3 0 3";
            string whiteEnPassant = "rnbqkbnr/1pp1pppp/p7/3pP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3";
            string pawnPromotion = "3n4/k3P3/8/8/8/8/K7/8 w - - 0 1";

            string capturePositionFen = "k7/4B3/2Kb4/2P5/2NR1Q2/8/8/8 w - - 0 1";

            string specialMoves = "r3k2r/6P1/8/Pp6/8/8/8/R3K2R w KQkq b6 0 3";
            
            board = new Board(specialMoves);
            //board.ColorToPlay = Color.Black;


            board.PrintBoard();

            Move move;


            //return;
            int count = 0;
            for( int start = 0; start < 64; start++)
            {
                for( int end = 0; end < 64; end++ )
                {
                    move = new Move(new Coord(start), new Coord(end), board);
                    if (move.IsLegal)
                    {
                        if (move.IsPawnPromotion)
                        {
                            foreach (var type in new Type[] { Type.Queen, Type.Rook, Type.Bishop, Type.Knight})
                            {
                                move= new Move(new Coord(start), new Coord(end), board, type);
                                Console.Write("[pseudo-legal] {0,5}: {1}", count, move);

                                if (move.IsCapture)
                                    Console.Write("   capture");
                                if (move.IsCastle)
                                    Console.Write("   castle");
                                if (move.IsCheck())
                                    Console.Write("   check");
                                if (move.IsPawnPromotion)
                                    Console.Write("   pawn promotion");
                                if (move.IsEnPassantCapture)
                                    Console.Write("   en passant");
                                Console.WriteLine();
                                count++;
                            }
                            continue;
                        }
                        count++;
                        Console.Write("[pseudo-legal] {0,5}: {1}", count, move);
                        if (move.IsCapture)
                            Console.Write("   capture");
                        if (move.IsCastle)
                            Console.Write("   castle");
                        if (move.IsCheck())
                            Console.Write("   check");
                        if (move.IsPawnPromotion)
                            Console.Write("   pawn promotion");
                        if (move.IsEnPassantCapture)
                            Console.Write("   en passant");
                        Console.WriteLine();
                    }
                }
            }

            Move newMove = new Move("c5d6", board);
            ;
            
        } // Main()
    }
}

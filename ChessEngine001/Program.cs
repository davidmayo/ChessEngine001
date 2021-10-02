using System;

namespace ChessEngine001
{
    class Program
    {
        static void Main()
        {
            Board board = new Board("k7/pp6/3n4/8/3rQ3/4KPq1/5B2/7N w - - 0 1");
            board = new Board("k7/8/8/8/8/1K4B1/8/8 w - - 0 1");
            board = new Board();
            board.MakeMove(new Move("a2a4",board));
            board.MakeMove(new Move("b7b5",board));
            board = new Board("3n1n2/k3P3/8/8/8/8/K7/8 w - - 0 1");
            //board.ColorToPlay = Color.Black;
            board.PrintBoard();

            MoveGenerator mg = new MoveGenerator(board);

            var moves = mg.LegalMoves;

            foreach( var move in moves )
            {
                Console.WriteLine(move);
            }

            //var attacked = MoveValidator.SquaresAttackedBy("b7",board);

            int count = 0;
            


            string testPositionFen  = "rnbNk1nr/1pppp3/8/8/pbq5/1P2Rppp/P1PPPPPP/RNBQKBN1 w KQkq - 5 40";
            string startPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            string castlePositionFen = "r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1";
            string blackEnPassant = "rnbqkbnr/ppp1pppp/8/8/3pP3/NP6/P1PP1PPP/R1BQKBNR b KQkq e3 0 3";
            string whiteEnPassant = "rnbqkbnr/1pp1pppp/p7/3pP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3";
            string pawnPromotion = "3n4/k3P3/8/8/8/8/K7/8 w - - 0 1";

            string capturePositionFen = "k7/4B3/2Kb4/2P5/2NR1Q2/8/8/8 w - - 0 1";

            string specialMoves = "r3k2r/6P1/8/Pp6/8/8/8/R3K2R w KQkq b6 0 3";
            
        } // Main()
    }
}

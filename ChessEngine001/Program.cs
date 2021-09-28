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
            board = new Board("n-------/--------/--------/--------/--------/--------/--------/--------");
            board = new Board();
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

            Move move = new Move("e2","e4", board);

            if( move.IsPseudoLegalPawnPush() )
                board.MakeMove(move);

            move = new Move("e7", "e5", board);
            if(move.IsPseudoLegalPawnPush())
                board.MakeMove(move);


            //board.MakeMove(new Move("e1", "e2", board));
            //board.MakeMove(new Move("e8", "e7", board));


            board.PrintBoard();


            //return;
            Console.WriteLine(move.IsPseudoLegalRookMove());

            moves = new System.Collections.Generic.List<Move>();

            return;

            for (int startIndex = 0; startIndex < 64; startIndex++)
            {
                for (int endIndex = 0; endIndex < 64; endIndex++)
                {
                    Coord start = new Coord(startIndex);
                    Coord end = new Coord(endIndex);

                    Move mv = new Move(start, end, board);

                    if (mv.IsPseudoLegalPawnCapture())
                    {
                        //moves.Add(mv);
                        Console.WriteLine("Pseudo-legal pawn capture: {0}", mv);
                    }
                    if (mv.IsPseudoLegalPawnPush())
                    {
                        //moves.Add(mv);
                        Console.WriteLine("Pseudo-legal pawn push: {0}", mv);
                    }
                    if (mv.IsPseudoLegalBishopMove())
                    {
                        //moves.Add(mv);
                        Console.WriteLine("Pseudo-legal bishop move: {0}", mv);
                    }
                    if (mv.IsPseudoLegalRookMove())
                    {
                        //moves.Add(mv);
                        Console.WriteLine("Pseudo-legal rook move: {0}", mv);
                    }
                    if (mv.IsPseudoLegalQueenMove())
                    {
                        //moves.Add(mv);
                        Console.WriteLine("Pseudo-legal queen move: {0}", mv);
                    }
                    if (mv.IsPseudoLegalKingMove())
                    {
                        //moves.Add(mv);
                        Console.WriteLine("Pseudo-legal king move: {0}", mv);
                    }
                    if (mv.IsPseudoLegalKnightMove())
                    {
                        //moves.Add(mv);
                        Console.WriteLine("Pseudo-legal knight move: {0}", mv);
                    }
                }
            }
        }
    }
}

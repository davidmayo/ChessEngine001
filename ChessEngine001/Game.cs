using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine001
{
    class Game
    {

        public Board Board { get => board; }

        private Board board;

        public Game() : this("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {}

        public Game(string fen)
        {
            board = new Board(fen);

            PlayGame();
        }

        private void PlayGame()
        {
            Console.Clear();

            Board.PrintBoard();

            // Generate legal moves
            List<Move> moves = new List<Move>();
            Move move;
            for (int start = 0; start < 64; start++)
            {
                for (int end = 0; end < 64; end++)
                {
                    move = new Move(new Coord(start), new Coord(end), this.Board);
                    if (move.IsLegal)
                    {
                        if (move.IsPawnPromotion)
                        {
                            foreach (var type in new Type[] { Type.Queen, Type.Rook, Type.Bishop, Type.Knight })
                            {
                                move = new Move(new Coord(start), new Coord(end), board, type);
                                moves.Add(move);
                            }
                            continue;
                        }
                        moves.Add(move);
                    }
                }
            } // outer for loop

            // Display legal moves
            for( int i = 0; i < moves.Count; i++)
            {
                if( i % 5 == 0)
                {
                    Console.WriteLine();
                }
                if (moves[i].IsCapture)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                else if (moves[i].IsCheck())
                    Console.ForegroundColor = ConsoleColor.Cyan;
                else
                    Console.ForegroundColor = ConsoleColor.White;
                Console.Write("{0,3}. {1,-15}", i + 1, moves[i]);
            }
            Console.WriteLine();
            Console.ResetColor();

            string input;
            int moveChoice;
            do
            {
                Console.Write("Enter a move from [1 to {0}]: ", moves.Count);
                input = Console.ReadLine();

            } while (!int.TryParse(input, out moveChoice) || moveChoice < 1 || moveChoice > moves.Count);
            board.MakeMove(moves[moveChoice - 1]);

            // Recursive call
            PlayGame();
        }
    }
}

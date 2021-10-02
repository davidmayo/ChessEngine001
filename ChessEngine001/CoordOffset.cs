namespace ChessEngine001
{
    public class CoordOffset
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public CoordOffset(int row, int col)
        {
            this.Row = row;
            this.Col = col;
        }

        public CoordOffset() : this(0, 0)
        {

        }

        public static CoordOffset operator +(CoordOffset left, CoordOffset right)
        {
            return new CoordOffset(left.Row + right.Row, left.Col + right.Col);
        }


        public static Coord operator +(Coord left, CoordOffset right)
        {
            if (left.Row + right.Row < 0 || left.Row + right.Row > 7 ||
                left.Col + right.Col < 0 || left.Col + right.Col > 7)
            {
                return null;
            }
            else
            {
                return new Coord(left.Row + right.Row, left.Col + right.Col);
            }
        }
        public static Coord operator +(CoordOffset left, Coord right)
        {
            if (left.Row + right.Row < 0 || left.Row + right.Row > 7 ||
                left.Col + right.Col < 0 || left.Col + right.Col > 7)
            {
                return null;
            }
            else
            {
                return new Coord(left.Row + right.Row, left.Col + right.Col);
            }
        }

        public static CoordOffset operator -(CoordOffset left, CoordOffset right)
        {
            return new CoordOffset(left.Row - right.Row, left.Col - right.Col);
        }

        public static CoordOffset operator *(int left, CoordOffset right)
        {
            return new CoordOffset(left * right.Row, left * right.Col);
        }

        public static CoordOffset operator *(CoordOffset left, int right)
        {
            return new CoordOffset(left.Row * right, left.Col * right);
        }
    }
}

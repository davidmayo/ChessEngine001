using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ChessEngine001
{
    class Coord : IEquatable<Coord>
    {
        public int Row{get;}
        public int Col{ get; }
        public int Index { get; }

        public static readonly Coord Unknown;

        //public Coord Unknown = new Coord(-1);
        public Coord()
        {
            //_ = string.Empty;
        }
        public Coord(int index)
        {
            if( index < 0 || index >= 64 )
            {
                throw new ArgumentOutOfRangeException("Index out of bounds.");
            }
            Index = index;
            Row = Index / 8;
            Col = Index % 8;
        }
        public Coord( int row, int col)
        {
            if( row < 0 || row >= 8 || col < 0 || col >= 8)
            {
                throw new ArgumentOutOfRangeException("Row or column out of bounds.");
            }
            Row = row;
            Col = col;
            Index = 8 * row + col;
        }

        public Coord(string coord)
        {
            string coordString = coord.ToLower();
            char fileChar = coordString[0];
            char rankChar = coordString[1];

            int rank = rankChar - '1';
            if( rank < 0 || rank >= 8)
            {
                throw new ArgumentException("Unable to parse " + coord);
            }

            int file = fileChar - 'a';
            if (file < 0 || file >= 8)
            {
                throw new ArgumentException("Unable to parse " + coord);
            }

            Row = rank;
            Col = file;
            Index = 8 * Row + Col;
        }

        public override string ToString()
        {
            return ToCoordinateString();
        }

        public string ToCoordinateString()
        {
            char file = (char)('a' + ((char)Col));
            string rv = string.Format("{0}{1}",file,Row+1);
            return rv;
        }

        public bool Equals([AllowNull] Coord other)
        {
            if( other is null )
            {
                return false;
            }
            return this.Index == other.Index;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Coord coord = (Coord)obj;

            return this.Index == coord.Index;
        }

        public override int GetHashCode()
        {
            return this.Index;
        }

        public static bool operator ==(Coord left, Coord right)
        {
            if ((object)left == null)
                return (object)right == null;

            return left.Equals(right);
        }

        public static bool operator !=(Coord left, Coord right)
        {
            return !(left == right);
        }

        public static implicit operator Coord(string c)
        {
            return new Coord(c);
        }

        /*public static explicit operator Coord(string c)
        {
            return new Coord(c);
        }*/
    }
}

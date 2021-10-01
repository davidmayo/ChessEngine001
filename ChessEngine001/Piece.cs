using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine001
{
    enum Type : int
    {
        Pawn = 1,
        Bishop = 2,
        Knight = 3,
        Rook = 4,
        Queen = 5,
        King = 6,
        Empty = 0,
        Unknown = -1
    }

    enum Color : int
    {
        White = 0,
        Black = 1,
        Unknown = -1
    }
    class Piece
    {
        public Type Type {get;}
        public Color Color { get; }

        /*public Piece( string type , string color = "white")
        {
            type = type.Trim().ToLower();

            if( type == )

            color = color.Trim().ToLower();
        }*/

        public Piece(Type type, Color color = Color.White)
        {
            this.Type = type;
            this.Color = color;
        }

        public Piece( char fenChar)
        {
            Type = (fenChar.ToString().ToLower()) switch
            {
                "p" => Type.Pawn,
                "n" => Type.Knight,
                "b" => Type.Bishop,
                "r" => Type.Rook,
                "q" => Type.Queen,
                "k" => Type.King,
                "-" => Type.Empty,
                _   => Type.Unknown,
            };
            if ( char.IsLetter(fenChar) && char.IsUpper(fenChar))
            {
                Color = Color.White;
            }
            else if(char.IsLetter(fenChar) && !char.IsUpper(fenChar))
            {
                Color = Color.Black;
            }
            else
            {
                Color = Color.Unknown;
            }
        }

        //public string Color { get; set; }
        //public string Type { get; }

        public string ToFenString()
        {
            string rv = Type switch
            {
                Type.Pawn => "p",
                Type.Knight => "n",
                Type.Bishop => "b",
                Type.Rook => "r",
                Type.Queen => "q",
                Type.King => "k",
                Type.Empty => "-",
                Type.Unknown => "?",
                _ => "-",
            };
            if ( Color == Color.White)
            {
                return rv.ToUpper();
            }
            else
            {
                return rv.ToLower();
            }
            
        }
        public override string ToString()
        {
            return ToFenString();
        }
    }
}

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

        public Piece( char fenChar)
        {
            switch( fenChar.ToString().ToLower() )
            {
                case "p":
                    Type = Type.Pawn;
                    break;
                case "n":
                    Type = Type.Knight;
                    break;
                case "b":
                    Type = Type.Bishop;
                    break;
                case "r":
                    Type = Type.Rook;
                    break;
                case "q":
                    Type = Type.Queen;
                    break;
                case "k":
                    Type = Type.King;
                    break;
                case "-":
                    Type = Type.Empty;
                    break;
                default:
                    Type = Type.Unknown;
                    break;
            }

            if( char.IsLetter(fenChar) && char.IsUpper(fenChar))
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
            string rv;
            
            switch( Type )
            {
                case Type.Pawn:
                    rv = "p";
                    break;
                case Type.Knight:
                    rv = "n";
                    break;
                case Type.Bishop:
                    rv = "b";
                    break;
                case Type.Rook:
                    rv = "r";
                    break;
                case Type.Queen:
                    rv = "q";
                    break;
                case Type.King:
                    rv = "k";
                    break;
                case Type.Empty:
                    rv = " ";
                    break;
                case Type.Unknown:
                    rv = "?";
                    break;
                default:
                    rv = "-";
                    break;
            }

            if( Color == Color.White)
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

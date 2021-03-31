using System.Collections.Generic;

namespace SudokuKata
{
    public class Applesauce1
    {
        public int Discriminator { get; set; }
        public string Description { get; set; }
        public int Index { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public override string ToString()
        {
            return
                $"{{ Discriminator = {Discriminator}, Description = {Description}, Index = {Index}, Row = {Row}, Column = {Column} }}";
        }

        public override bool Equals(object value)
        {
            var type = value as Applesauce1;
            return type != null && EqualityComparer<int>.Default.Equals(type.Discriminator, Discriminator) &&
                   EqualityComparer<string>.Default.Equals(type.Description, Description) &&
                   EqualityComparer<int>.Default.Equals(type.Index, Index) &&
                   EqualityComparer<int>.Default.Equals(type.Row, Row) &&
                   EqualityComparer<int>.Default.Equals(type.Column, Column);
        }

        public override int GetHashCode()
        {
            var num = 0x7a2f0b42;
            num = -1521134295 * num + EqualityComparer<int>.Default.GetHashCode(Discriminator);
            num = -1521134295 * num + EqualityComparer<string>.Default.GetHashCode(Description);
            num = -1521134295 * num + EqualityComparer<int>.Default.GetHashCode(Index);
            num = -1521134295 * num + EqualityComparer<int>.Default.GetHashCode(Row);
            return -1521134295 * num + EqualityComparer<int>.Default.GetHashCode(Column);
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace SudokuKata
{
    public class Applesauce2
    {
        public int Mask { get; set; }
        public int Discriminator { get; set; }
        public string Description { get; set; }
        public IGrouping<int, SudokuConstraints_OrSomething> Cells { get; set; }

        public override string ToString()
        {
            return
                $"{{ Mask = {Mask}, Discriminator = {Discriminator}, Description = {Description}, Cells = {Cells} }}";
        }

        public override bool Equals(object value)
        {
            var type = value as Applesauce2;
            return type != null && EqualityComparer<int>.Default.Equals(type.Mask, Mask) &&
                   EqualityComparer<int>.Default.Equals(type.Discriminator, Discriminator) &&
                   EqualityComparer<string>.Default.Equals(type.Description, Description) &&
                   EqualityComparer<IGrouping<int, SudokuConstraints_OrSomething>>.Default.Equals(type.Cells, Cells);
        }

        public override int GetHashCode()
        {
            var num = 0x7a2f0b42;
            num = -1521134295 * num + EqualityComparer<int>.Default.GetHashCode(Mask);
            num = -1521134295 * num + EqualityComparer<int>.Default.GetHashCode(Discriminator);
            num = -1521134295 * num + EqualityComparer<string>.Default.GetHashCode(Description);
            return -1521134295 * num +
                   EqualityComparer<IGrouping<int, SudokuConstraints_OrSomething>>.Default.GetHashCode(Cells);
        }
    }
}
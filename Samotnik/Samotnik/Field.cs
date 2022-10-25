using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotnik
{
    internal class Field
    {
        public enum FieldState
        {
            outside = -1,
            empty = 0,
            full = 1,
            highlighted  =2
        }

        private int row;
        private int column;
        private FieldState state;
        public Field()
        {
            this.row = 0;
            this.column = 0;
            this.state = FieldState.outside;
        }
        public Field(int row, int column, FieldState state)
        {
            this.row = row;
            this.column = column;
            this.state = state;
        }
        public int Row { get { return this.row; } }
        public int Column { get { return this.column; } }
        public FieldState State { get { return this.state; } set { this.state = value; } }

    }
}

using System.Collections.Generic;
using System.ComponentModel;

namespace Młynek.Model
{
    class Field : INotifyPropertyChanged
    {
        private FieldState _state;
        private Field[] _neighbours;
        private Field[] _row;
        private Field[] _column;


        public string ID { get; }
        public int Position { get; }
        public FieldState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                NotifyPropertyChanged("State");
            }
        }
        public Field[] Neighbours
        {
            get
            {
                if (_neighbours == null)
                {
                    int count = 0;
                    _neighbours = new Field[HorizontalNeighbours.Length + VerticalNeighbours.Length];

                    for(int i = 0; i < HorizontalNeighbours.Length; i++)
                    {
                        _neighbours[count] = HorizontalNeighbours[i];
                        count++;
                    }

                    for (int i = 0; i < VerticalNeighbours.Length; i++)
                    {
                        _neighbours[count] = VerticalNeighbours[i];
                        count++;
                    }

                    return _neighbours;
                }
                else return _neighbours;
            }
        }
        public Field[] Row
        {
            get
            {
                if (_row == null)
                {
                    _row = new Field[3];

                    HashSet<Field> row = new HashSet<Field>();
                    row.UnionWith(HorizontalNeighbours);
                    foreach (Field field in HorizontalNeighbours) row.UnionWith(field.HorizontalNeighbours);

                    int count = 0;
                    foreach(Field field in row)
                    {
                        _row[count] = field;
                        count++;
                    }

                    return _row;
                }
                else return _row;
            }
        }
        public Field[] Column
        {
            get
            {
                if (_column == null)
                {
                    _column = new Field[3];

                    HashSet<Field> column = new HashSet<Field>();
                    column.UnionWith(VerticalNeighbours);
                    foreach (Field field in VerticalNeighbours) column.UnionWith(field.VerticalNeighbours);

                    int count = 0;
                    foreach (Field field in column)
                    {
                        _column[count] = field;
                        count++;
                    }

                    return _column;
                }
                else return _column;
            }
        }
        public Field[] HorizontalNeighbours { get; set; }
        public Field[] VerticalNeighbours { get; set; }

        public Field(string id, int position)
        {
            ID = id;
            Position = position;
            State = 0;
        }

        public bool IsNeighbour(Field field)
        {
            bool isNeighbour = false;
            for (int i = 0; i < Neighbours.Length; i++)
            {
                if (Neighbours[i] == field)
                {
                    isNeighbour = true;
                    break;
                }
            }
            return isNeighbour;
        }

        public bool IsInMill()
        {
            bool millInRow = true;
            for(int i = 0; i < 3; i++)
            {
                if (Row[i].State != State)
                {
                    millInRow = false;
                    break;
                }
            }

            if (millInRow) return true;

            bool millInColumn = true;
            for (int i = 0; i < 3; i++)
            {
                if (Column[i].State != State)
                {
                    millInColumn = false;
                    break;
                }
            }

            return millInColumn;
        }

        public bool WillBeInMill(Field movedFrom, FieldState newState)
        {
            bool millInRow = true;
            for (int i = 0; i < 3; i++)
            {
                Field current = Row[i];
                if (current == movedFrom || (current.Position != Position && current.State != newState))
                {
                    millInRow = false;
                    break;
                }
            }

            if (millInRow) return true;

            bool millInColumn = true;
            for (int i = 0; i < 3; i++)
            {
                Field current = Column[i];
                if (current == movedFrom || (current.Position != Position && current.State != newState))
                {
                    millInColumn = false;
                    break;
                }
            }

            return millInColumn;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    enum FieldState { Empty, White, Black = -1 }
}

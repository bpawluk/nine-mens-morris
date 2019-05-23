using System;
using System.Collections.Generic;
using System.Text;

namespace Młynek.Model
{
    class GameBoard
    {
        private int _whitesOnBoard;
        private int _blacksOnBoard;

        public Field[] Fields { get; set; }


        public GameBoard()
        {
            InstantiateFields();
            _whitesOnBoard = 0;
            _blacksOnBoard = 0;
        }

        public Field GetField(int position) => Fields[position];

        public int GetFieldsCount(FieldState player)
        {
            int count = 0;
            switch (player)
            {
                case FieldState.White:
                    count = _whitesOnBoard;
                    break;
                case FieldState.Black:
                    count = _blacksOnBoard;
                    break;
                case FieldState.Empty:
                    count = Fields.Length - _whitesOnBoard - _blacksOnBoard;
                    break;
                default:
                    break;
            }
            return count;
        }

        public Field[] GetFields(FieldState player)
        {
            Field[] result = new Field[GetFieldsCount(player)];

            int count = 0;
            for (int i = 0; i < Fields.Length; i++)
            {
                if (Fields[i].State == player)
                {
                    result[count] = Fields[i];
                    count++;
                }
            }

            return result;
        }

        public Move[] GetAvaiablePlacements(FieldState player)
        {
            List<Move> avaiablePlacements = new List<Move>();
            for (int i = 0; i < Fields.Length; i++)
            {
                Field currentField = Fields[i];
                if (currentField.State == FieldState.Empty)
                {
                    Move possiblePlacement = new Move(player, from: -1, to: currentField.Position);
                    if (currentField.WillBeInMill(null, player)) possiblePlacement.CreatesMill = true;
                    avaiablePlacements.Add(possiblePlacement);
                }
            }
            return avaiablePlacements.ToArray();
        }

        public Move[] GetAvaiableMoves(FieldState player, Move lastMove, bool freeMovementEnabled)
        {
            List<Move> avaiableMoves = new List<Move>();
            for (int i = 0; i < Fields.Length; i++)
            {
                Field currentField = Fields[i];
                if (currentField.State == player)
                {
                    if (freeMovementEnabled)
                    {
                        for (int j = 0; j < Fields.Length; j++)
                        {
                            Field nextField = Fields[j];
                            if (nextField.State == FieldState.Empty)
                            {
                                Move newMove = new Move(player, from: currentField.Position, to: nextField.Position);
                                if (nextField.WillBeInMill(currentField, player)) newMove.CreatesMill = true;
                                if (!newMove.Equals(lastMove.Inverse())) avaiableMoves.Add(newMove);
                            }
                        }
                    }
                    else
                    {
                        Field[] neighbours = currentField.Neighbours;
                        for (int j = 0; j < neighbours.Length; j++)
                        {
                            Field neighbour = neighbours[j];
                            if (neighbour.State == FieldState.Empty)
                            {
                                Move newMove = new Move(player, from: currentField.Position, to: neighbour.Position);
                                if (neighbour.WillBeInMill(currentField, player)) newMove.CreatesMill = true;
                                if (!newMove.Equals(lastMove.Inverse())) avaiableMoves.Add(newMove);
                            }
                        }
                    }
                }
            }
            return avaiableMoves.ToArray();
        }

        public Field[] GetAvaiableCaptures(FieldState opponent)
        {
            Field[] avaiableCaptures = GetFieldsNotInMill(opponent);
            if (avaiableCaptures.Length == 0) avaiableCaptures = GetFields(opponent);
            return avaiableCaptures;
        }

        public Field[] GetFieldsInMill(FieldState player)
        {
            List<Field> fieldsInMill = new List<Field>();
            for (int i = 0; i < Fields.Length; i++)
            {
                Field currentField = Fields[i];
                if (currentField.State == player && currentField.IsInMill()) fieldsInMill.Add(currentField);
            }
            return fieldsInMill.ToArray();
        }

        public Field[] GetFieldsNotInMill(FieldState player)
        {
            List<Field> fieldsNotInMill = new List<Field>();
            for (int i = 0; i < Fields.Length; i++)
            {
                Field currentField = Fields[i];
                if (currentField.State == player && !currentField.IsInMill()) fieldsNotInMill.Add(currentField);
            }
            return fieldsNotInMill.ToArray();
        }

        public void RemoveFrom(int position)
        {
            Field field = Fields[position];
            if (field.State == FieldState.Empty) throw new InvalidOperationException();
            else
            {
                if (field.State == FieldState.White) _whitesOnBoard--;
                else if (field.State == FieldState.Black) _blacksOnBoard--;
                field.State = FieldState.Empty;
            }
        }

        public void PlaceAt(int position, FieldState player)
        {
            Field field = Fields[position];
            if (field.State != FieldState.Empty) throw new InvalidOperationException();
            else
            {
                if (player == FieldState.White)
                {
                    field.State = FieldState.White;
                    _whitesOnBoard++;
                }
                else if (player == FieldState.Black)
                {
                    field.State = FieldState.Black;
                    _blacksOnBoard++;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Field field in Fields) sb.Append($"{field.ID}:{(int)field.State}|");
            return sb.ToString();
        }

        public GameBoard Duplicate()
        {
            GameBoard duplicate = new GameBoard();
            duplicate._whitesOnBoard = _whitesOnBoard;
            duplicate._blacksOnBoard = _blacksOnBoard;
            for (int i = 0; i < Fields.Length; i++)
            {
                duplicate.Fields[i].State = Fields[i].State;
            }
            return duplicate;
        }

        public void Clear()
        {
            _whitesOnBoard = 0;
            _blacksOnBoard = 0;
            foreach (Field field in Fields) field.State = FieldState.Empty;
        }

        private void InstantiateFields()
        {
            Fields = new Field[24];

            Fields[0] = new Field("A7", 0);
            Fields[1] = new Field("D7", 1);
            Fields[2] = new Field("G7", 2);
            Fields[3] = new Field("B6", 3);
            Fields[4] = new Field("D6", 4);
            Fields[5] = new Field("F6", 5);
            Fields[6] = new Field("C5", 6);
            Fields[7] = new Field("D5", 7);
            Fields[8] = new Field("E5", 8);
            Fields[9] = new Field("A4", 9);
            Fields[10] = new Field("B4", 10);
            Fields[11] = new Field("C4", 11);
            Fields[12] = new Field("E4", 12);
            Fields[13] = new Field("F4", 13);
            Fields[14] = new Field("G4", 14);
            Fields[15] = new Field("C3", 15);
            Fields[16] = new Field("D3", 16);
            Fields[17] = new Field("E3", 17);
            Fields[18] = new Field("B2", 18);
            Fields[19] = new Field("D2", 19);
            Fields[20] = new Field("F2", 20);
            Fields[21] = new Field("A1", 21);
            Fields[22] = new Field("D1", 22);
            Fields[23] = new Field("G1", 23);

            Fields[21].HorizontalNeighbours = new Field[] { Fields[22] };
            Fields[21].VerticalNeighbours = new Field[] { Fields[9] };
            Fields[9].HorizontalNeighbours = new Field[] { Fields[10] };
            Fields[9].VerticalNeighbours = new Field[] { Fields[21], Fields[0] };
            Fields[0].HorizontalNeighbours = new Field[] { Fields[1] };
            Fields[0].VerticalNeighbours = new Field[] { Fields[9] };

            Fields[18].HorizontalNeighbours = new Field[] { Fields[19] };
            Fields[18].VerticalNeighbours = new Field[] { Fields[10] };
            Fields[10].HorizontalNeighbours = new Field[] { Fields[9], Fields[11] };
            Fields[10].VerticalNeighbours = new Field[] { Fields[18], Fields[3] };
            Fields[3].HorizontalNeighbours = new Field[] { Fields[4] };
            Fields[3].VerticalNeighbours = new Field[] { Fields[10] };

            Fields[15].HorizontalNeighbours = new Field[] { Fields[16] };
            Fields[15].VerticalNeighbours = new Field[] { Fields[11] };
            Fields[11].HorizontalNeighbours = new Field[] { Fields[10] };
            Fields[11].VerticalNeighbours = new Field[] { Fields[15], Fields[6] };
            Fields[6].HorizontalNeighbours = new Field[] { Fields[7] };
            Fields[6].VerticalNeighbours = new Field[] { Fields[11] };

            Fields[22].HorizontalNeighbours = new Field[] { Fields[21], Fields[23] };
            Fields[22].VerticalNeighbours = new Field[] { Fields[19] };
            Fields[19].HorizontalNeighbours = new Field[] { Fields[18], Fields[20] };
            Fields[19].VerticalNeighbours = new Field[] { Fields[22], Fields[16] };
            Fields[16].HorizontalNeighbours = new Field[] { Fields[15], Fields[17] };
            Fields[16].VerticalNeighbours = new Field[] { Fields[19] };
            Fields[7].HorizontalNeighbours = new Field[] { Fields[6], Fields[8] };
            Fields[7].VerticalNeighbours = new Field[] { Fields[4] };
            Fields[4].HorizontalNeighbours = new Field[] { Fields[3], Fields[5] };
            Fields[4].VerticalNeighbours = new Field[] { Fields[7], Fields[1] };
            Fields[1].HorizontalNeighbours = new Field[] { Fields[0], Fields[2] };
            Fields[1].VerticalNeighbours = new Field[] { Fields[4] };

            Fields[17].HorizontalNeighbours = new Field[] { Fields[16] };
            Fields[17].VerticalNeighbours = new Field[] { Fields[12] };
            Fields[12].HorizontalNeighbours = new Field[] { Fields[13] };
            Fields[12].VerticalNeighbours = new Field[] { Fields[17], Fields[8] };
            Fields[8].HorizontalNeighbours = new Field[] { Fields[7] };
            Fields[8].VerticalNeighbours = new Field[] { Fields[12] };

            Fields[20].HorizontalNeighbours = new Field[] { Fields[19] };
            Fields[20].VerticalNeighbours = new Field[] { Fields[13] };
            Fields[13].HorizontalNeighbours = new Field[] { Fields[12], Fields[14] };
            Fields[13].VerticalNeighbours = new Field[] { Fields[20], Fields[5] };
            Fields[5].HorizontalNeighbours = new Field[] { Fields[4] };
            Fields[5].VerticalNeighbours = new Field[] { Fields[13] };

            Fields[23].HorizontalNeighbours = new Field[] { Fields[22] };
            Fields[23].VerticalNeighbours = new Field[] { Fields[14] };
            Fields[14].HorizontalNeighbours = new Field[] { Fields[13] };
            Fields[14].VerticalNeighbours = new Field[] { Fields[23], Fields[2] };
            Fields[2].HorizontalNeighbours = new Field[] { Fields[1] };
            Fields[2].VerticalNeighbours = new Field[] { Fields[14] };
        }
    }
}

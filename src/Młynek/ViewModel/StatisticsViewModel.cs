using Młynek.Model;
using System.ComponentModel;

namespace Młynek.ViewModel
{
    class StatisticsViewModel
    {
        private long _time;
        private long _nodes;
        private int _moves;

        public event PropertyChangedEventHandler PropertyChanged;

        public long Time
        {
            get => _time;
            set
            {
                _time = value;
                NotifyPropertyChanged("Time");
            }
        }

        public long Nodes
        {
            get => _nodes;
            set
            {
                _nodes = value;
                NotifyPropertyChanged("Nodes");
            }
        }

        public int Moves
        {
            get => _moves;
            set
            {
                _moves = value;
                NotifyPropertyChanged("Moves");
            }
        }

        public void AddMove(Move move)
        {
            Time += move.Time;
            Nodes += move.NodesVisited;
            Moves++;
        }

        public void AddCapture(Move move)
        {
            Time += move.Time;
            Nodes += move.NodesVisited;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

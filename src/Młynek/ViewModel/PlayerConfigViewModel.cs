using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Młynek.ViewModel
{
    class PlayerConfigViewModel
    {
        private bool _isHuman = true;
        private bool _isAI = false;
        private bool _alfaBetaEnabled = true;
        private int _treeDepth = 5;
        private float _positionsWeight = 0.1f;
        private float _rowsWeight = 0.8f;
        private float _columnsWeight = 0.8f;
        private float _movesWeight = 0.2f;
        private float _pawnsWeight = 0.9f;

        public event PropertyChangedEventHandler PropertyChanged;

        public int TreeDepth
        {
            get => _treeDepth;
            set
            {
                _treeDepth = value;
                NotifyPropertyChanged("TreeDepth");
            }
        }

        public float PositionsWeight
        {
            get => _positionsWeight;
            set
            {
                _positionsWeight = value;
                NotifyPropertyChanged("PositionsWeight");
            }
        }

        public float RowsWeight
        {
            get => _rowsWeight;
            set
            {
                _rowsWeight = value;
                NotifyPropertyChanged("RowsWeight");
            }
        }

        public float ColumnsWeight
        {
            get => _columnsWeight;
            set
            {
                _columnsWeight = value;
                NotifyPropertyChanged("ColumnsWeight");
            }
        }

        public float MovesWeight
        {
            get => _movesWeight;
            set
            {
                _movesWeight = value;
                NotifyPropertyChanged("MovesWeight");
            }
        }

        public float PawnsWeight
        {
            get => _pawnsWeight;
            set
            {
                _pawnsWeight = value;
                NotifyPropertyChanged("PawnsWeight");
            }
        }

        public bool IsHuman
        {
            get => _isHuman;
            set
            {
                _isHuman = value;
                NotifyPropertyChanged("IsHuman");
            }
        }

        public bool IsAI
        {
            get => _isAI;
            set
            {
                _isAI = value;
                NotifyPropertyChanged("IsAI");
            }
        }

        public bool AlfaBetaEnabled
        {
            get => _alfaBetaEnabled;
            set
            {
                _alfaBetaEnabled = value;
                NotifyPropertyChanged("AlfaBetaEnabled");
            }
        }


        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

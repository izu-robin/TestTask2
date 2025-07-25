﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestTask2.Models.Core
{
    public class Question : INotifyPropertyChanged
    {
        public int id { get; set; }

        private string _questionText;
        public string questionText 
        {
            get => _questionText;
            set
            {
                _questionText = value;
                OnPropertyChanged();
            }
        }
        public string answer1 { get; set; }
        public string answer2 { get; set; }
        public string answer3 { get; set; }
        public string answer4 { get; set; }

        private bool[] _chBStatus = new bool[5];

        public bool[] ChBStatus
        {
            get
            {
                if (_chBStatus==null)
                {
                    _chBStatus = new bool[] { false, false, false, false, false };
                }
                return _chBStatus;
            }
            set
            {
                _chBStatus = value;
                OnPropertyChanged();
            }
        }

        public string correctAnswer { get; set; }

        public string givenAnswer { get; set; }

        public Question() {}

        //------------- INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //----------------------------------------

    }
}

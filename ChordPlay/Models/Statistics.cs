using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ChordPlay
{
    [XmlRoot()]
    public class Statistics
    {
        public Statistics()
        {
            StatisticsObservableList = new ObservableCollection<Statistic>();
        }
        public ObservableCollection<Statistic> StatisticsObservableList { get; set; }

        public void AddGoodTryObservable(string chord)
        {
            var stat = StatisticsObservableList.Where(x => x.Chord == chord).FirstOrDefault();
            if (stat == null)
            {
                StatisticsObservableList.Add(new Statistic() { Chord = chord });
                stat = StatisticsObservableList.Where(x => x.Chord == chord).FirstOrDefault();
            }

            stat.Tries++;
            stat.CorrectTries++;
        }

        public void AddWrongTryObservable(string chord)
        {
            var stat = StatisticsObservableList.Where(x => x.Chord == chord).FirstOrDefault();
            if (stat == null)
            {
                StatisticsObservableList.Add(new Statistic() { Chord = chord });
                stat = StatisticsObservableList.Where(x => x.Chord == chord).FirstOrDefault();
            }

            stat.Tries++;
        }

        public class Statistic : INotifyPropertyChanged
        {
            private string chord;

            public string Chord
            {
                get { return chord; }
                set { chord = value; }
            }

            private int tries;

            public int Tries
            {
                get { return tries; }
                set { tries = value;
                    RaisePropertyChanged("Tries");
                    RaisePropertyChanged("Ratio");
                }
            }

            private int correctTries;
            public int CorrectTries
            {
                get { return correctTries; }
                set { correctTries = value;
                    RaisePropertyChanged("CorrectTries");
                    RaisePropertyChanged("Ratio");
                }
            }

            public float Ratio
            {
                get
                {
                    float result = ((float)CorrectTries/Tries);
                    return result;
                }
            }

            public void RaisePropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}

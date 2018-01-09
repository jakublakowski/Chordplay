using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordPlay.Helpers
{
    public static class GuessingHelper
    {
        static Random ran = new Random();
        static Record currentRecordBeingGuessed;

        private static Record GetRandomRecord()
        {
            return MainPage.instance.RecordingsList[ran.Next(0, MainPage.instance.RecordingsList.Count)];
        }

        public static void PickNewRecord()
        {
            currentRecordBeingGuessed = GetRandomRecord();
        }

        public static Record GetRecord()
        {
            return currentRecordBeingGuessed;
        }
    }
}

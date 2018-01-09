using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;

namespace ChordPlay.Helpers
{
    public static class StatisticsHelper
    {
        public static Statistics LoadStatistics()
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();
            using (var fileStream = isoStorage.OpenFile("stats.xml", FileMode.OpenOrCreate))
            {
                try
                {
                    XmlSerializer SerializerObj = new XmlSerializer(typeof(Statistics));
                    return (Statistics)SerializerObj.Deserialize(fileStream);
                }
                catch (Exception)
                {
                    return new Statistics();
                }       
            }
        }

        public static void SaveStatistics(Statistics statistics)
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();
            using (var fileStream = isoStorage.OpenFile("stats.xml", FileMode.Create))
            {
                XmlSerializer SerializerObj = new XmlSerializer(typeof(Statistics));
                using (TextWriter WriteFileStream = new StreamWriter(fileStream))
                {
                    SerializerObj.Serialize(WriteFileStream, statistics);
                }
            }
        }
    }
}

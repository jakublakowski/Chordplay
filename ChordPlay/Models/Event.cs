using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordPlay
{
    public class PlaceOfEvent
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Urls
    {
        public string www { get; set; }
        public string tickets { get; set; }
    }

    public class Attachment
    {
        public string fileName { get; set; }
    }

    public class Organizer
    {
        public int id { get; set; }
        public string designation { get; set; }
    }

    public class Tickets
    {
        public string type { get; set; }
        public string startTicket { get; set; }
        public string endTicket { get; set; }
    }

    public class Event
    {
        public int id { get; set; }
        public PlaceOfEvent place { get; set; }

        private string _endDate;
        public string endDate
        {
            get
            {
                return DateTime.Parse(_endDate).ToString("yyyy-MM-dd HH:mm");
            }
            set { _endDate = value; }
        }
        public string name { get; set; }
        public Urls urls { get; set; }
        public List<Attachment> attachments { get; set; }
        public string descLong { get; set; }
        public int categoryId { get; set; }

        private string _startDate;
        public string startDate
        {
            get
            {
                return DateTime.Parse(_startDate).ToString("yyyy-MM-dd HH:mm");
            }
            set { _startDate = value; }
        }
        public Organizer organizer { get; set; }
        public int active { get; set; }
        public string descShort { get; set; }
        public Tickets tickets { get; set; }
    }

}

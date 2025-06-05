using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace RallyCalendar
{
    public class Day
    {
        public Event[] events;

        public Day()
        {
            events = new Event[0];
        }
    }

    public class Event
    {
        public string Name { get; set; }
        public string Description { get; set; }
        private int hour, minute;
        public int Hour { get { return hour; } set { if (value >= 0 && value < 60) { hour = value; } } }
        public int Minute { get { return minute; } set { if (value >= 0 && value < 60) { minute = value; } } }

        public Event()
        {
            Name = string.Empty;
            Description = string.Empty;
            Minute = 0;
            Hour = 0;
        }

        public Event(string name, string desctiption, int hour, int minute)
        {
            this.Name = name;
            this.Description = desctiption;
            this.Hour = hour;
            this.Minute = minute;
        }

        public override string ToString()
        {
            return $"{Name} {Hour}:{Minute}";
        }
    }
}

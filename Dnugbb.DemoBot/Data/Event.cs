﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dnugbb.DemoBot.Data
{
    public class Event
    {
        public DateTime Date { get; set; }

        public string Topic { get; set; }

        public string Address { get; set; }

        public string Speaker { get; set; }

        public string Abstract { get; set; }
        public string SpeakerImage { get; internal set; }
    }
}
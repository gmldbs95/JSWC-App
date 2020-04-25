using System;
using System.Collections.Generic;

namespace massage.Models {
    public class JsonFilterObject {
        public string PractitionerId { get; set; }
        public string ServiceId { get; set; }
        public string CustomerId { get; set; }
        // public List<int> OldList { get; set; }
    }
    // public class OldListObject {
    //     public int success { get; set; }
    //     public List<EventObject> result { get; set; }
    // }
    // public class EventObject {
    //     public int id { get; set; }
    //     public string title { get; set; }
    //     public long start { get; set; }
    //     public long end { get; set; }
    // }
}
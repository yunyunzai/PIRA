using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.ViewModels
{
    public class LoggingModel
    {
        public Int64 UserId { get; set; }
        public Int64 PatientId { get; set; }
        public Int64 RequestId { get; set; }
        public DateTime? time { get; set; }
        public string Action { get; set; }



    }
}
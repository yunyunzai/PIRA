using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.ViewModels
{
    public class UserGroup
    {
        
        public string Abbreviate { get; set; }
        public string Name { get; set; }
        public Boolean IsAssigned { get; set; }
    }
}
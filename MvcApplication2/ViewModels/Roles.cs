using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.ViewModels
{
    public class Roles
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public Boolean IsSuperUser { get; set; }
        public Boolean IsAssigned { get; set; }
          }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MvcApplication2.Models
{
    public class AdminContext : DbContext
    {
        public AdminContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Request> Request { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<QuestionKeyword> QuestionKeyword { get; set; }
        
    }

    public class FieldModel
    {
        public string TypeAbbreviate { get; set; }
        public string Name { get; set; }
    }

    public class AdminRequestModel
    {
        public Int64 RequestId { get; set; }
        public bool IsActive { get; set; }
        public string Keyword { get; set; }
    }


}

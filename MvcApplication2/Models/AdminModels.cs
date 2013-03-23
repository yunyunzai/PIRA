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

        public DbSet<TumorGroup> TumorGroup { get; set; }
        public DbSet<QuestionType> QuestionType { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
    }

    public class FieldModel
    {
        public string TypeAbbreviate { get; set; }
        public string Name { get; set; }
    }


}

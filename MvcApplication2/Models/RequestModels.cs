using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace MvcApplication2.Models
{
    public class RequestContext : DbContext
    {
        public RequestContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Caller> Callers { get; set; }
        public DbSet<Request> Requests { get; set; }

        //from tutorial 
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<Request>()
            //.HasRequired(p => p.Patient)
            //.WithMany(r => r.Requests)
            //.HasForeignKey(p => p.PatientID)            
            //.WillCascadeOnDelete(false);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    //[Table("Request")]    
    //public class Request
    //{
    //    [Key]
    //    [Required(ErrorMessage="RequestID is required")]
    //    public int RequestID { get; set; }
    //    [System.ComponentModel.DisplayName("Activation")]
    //    public bool IsActive { get; set; }
    //    [System.ComponentModel.DisplayName("Time Spent")]
    //    public DateTime TotalTimeSpent { get; set; }

    //    [ForeignKey("Caller"), Column(Order = 0)]
    //    [System.ComponentModel.DisplayName("Caller Name")]
    //    public string Name { get; set; }

    //    [ForeignKey("Caller"), Column(Order = 1)]
    //    [System.ComponentModel.DisplayName("Caller Phone")]
    //    public string Phone { get; set; }
        
    //    public int PatientID { get; set; }
    //    public virtual Patient Patient { get; set; }

    //    public virtual Caller Caller { get; set; }

    //}

    //[Table("Patient")]
    //public class Patient 
    //{
    //    [Key, Column(Order=1)]
    //    [Required(ErrorMessage="PaientID is required")]
    //    [Editable(true)]
    //    public int PatientID { get; set; }
    //    public string Name { get; set; }
    //    public int AgencyID { get; set; }
    //    public int Age { get; set; }
    //    public string Gender { get; set; }
    //    public virtual ICollection<Request> Requests { get; set; }
    //}

    //[Table("Caller")]
    //public class Caller 
    //{   
    //    [Key, Column(Order=1)]
    //    [Required(ErrorMessage="Name of the caller is required")]
    //    public string Name { get; set; }
    //    [Key, Column(Order=2)]
    //    [Required(ErrorMessage="Phone number of the caller is required")]
    //    public string Phone { get; set; }
    //    public string Email { get; set; }
    //    public string Region { get; set; }
    //    public string TypeAbbreviate { get; set; }
    //    public virtual CallerType CallerType { get; set; }
    //    public virtual ICollection<Request> Requests { get; set; }
    //}

    //[Table("CallerType")]
    //public class CallerType
    //{
    //    [Key]
    //    public string TypeAbbreviate { get; set; }
    //    public string Name { get; set; }
    //}
}
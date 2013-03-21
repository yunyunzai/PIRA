using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.Data;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MvcApplication2.Models
{
    public class ModelContext : DbContext
    {
        public ModelContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<KeywordModel> Keyword { get; set; }
        public DbSet<Request> Request { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<QuestionKeyword> QuestionKeyword { get; set; }
        public DbSet<Patient> Patient { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    [Table("Keyword")]
    public class KeywordModel
    {
        [Key, Column(Order = 1)]
        [Required]
        [Display(Name = "Keyword")]
        public string Keyword { get; set; }
        [Required]
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }
    }

    [Table("Request")]
    public class Request
    {
        [Key, Column(Order = 1)]        
        [Required(ErrorMessage = "RequestID is required")]        
        public int RequestID { get; set; } 
        [System.ComponentModel.DisplayName("Activation")]        
        public bool IsActive { get; set; } 
        [System.ComponentModel.DisplayName("Time Spent")]        
        public Int64 TotalTimeSpent { get; set; } 
        public string Name { get; set; } 
        public string Phone { get; set; } 
        public int PatientID { get; set; } 
        public virtual Patient Patient { get; set; } 
        public virtual Caller Caller { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }

    [Table("Patient")]
    public class Patient 
    { 
        [Key, Column(Order = 1)]        
        [Required(ErrorMessage = "PaientID is required")]        
        [Editable(true)]        
        public int PatientID { get; set; } 
        public string Name { get; set; } 
        public int AgencyID { get; set; } 
        public int Age { get; set; } 
        public string Gender { get; set; } 
        public virtual ICollection<Request> Requests { get; set; } 
    }

    [Table("Caller")]
    public class Caller 
    { 
        [Key, Column(Order = 1)]        
        [Required(ErrorMessage = "Name of the caller is required")]        
        public string Name { get; set; } [Key, Column(Order = 2)]        
        [Required(ErrorMessage = "Phone number of the caller is required")]        
        public string Phone { get; set; } 
        public string Email { get; set; } 
        public string Region { get; set; } 
        public string TypeAbbreviate { get; set; } 
        public virtual CallerType CallerType { get; set; } 
        public virtual ICollection<Request> Requests { get; set; } 
    }

    [Table("CallerType")]
    public class CallerType { 
        [Key]        
        public string TypeAbbreviate { get; set; } 
        public string Name { get; set; } 
    }

    [Table("Question")]
    public class Question
    {
        [Key, Column(Order = 1)]
        [Required]
        public int QuestionID { get; set; }
        public string QuestionContent { get; set; }
        [Required]
        public string Response { get; set; }
        [Required]
        public int Severity { get; set; }
        [Required]
        public float probability { get; set; }
        public float ComputedImpact { get; set; }
        public DateTime TimeTaken { get; set; }
        [Required]
        public string TumourGroup { get; set; }
        [Required]
        public int RequestID { get; set; }
        [Required]
        public string QuestionType { get; set; }
        public virtual ICollection<QuestionKeyword> Keywords { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }

    [Table("QuestionKeyword")]
    public class QuestionKeyword
    {
        [Key, Column(Order = 1)]
        [Required]
        public int QuestionID { get; set; }
        [Key, Column(Order = 2)]
        [Required]
        public string Keyword { get; set; }
        public virtual KeywordModel Key { get; set; }
    }
}
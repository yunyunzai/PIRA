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
    public class QuestionContext : DbContext
    {
        public QuestionContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Keywords> Keywords { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<QuestionKeyword> QuestionKeywords { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Reference> References { get; set; }
        public DbSet<QuestionReference> QuestionReferences { get; set; }
        public DbSet<TumorGroup> TumorGroups { get; set; }
        public DbSet<Caller> Callers { get; set; }
        public DbSet<CallerType> Callertypes { get; set; }
        public DbSet<UserViewRequest> UserViewRequest { get; set; }
        public DbSet<UserCompleteRequest> UserCompleteRequest { get; set; }
        public DbSet<UserCreateRequest> UserCreateRequest { get; set; }
        public DbSet<UserEditRequest> UserEditRequest { get; set; }




        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    [Table("Keyword")]
    public class Keywords
    {
        [Key, Column(Order = 1)]
        //[Required]
        [Display(Name = "Keyword")]
        public string Keyword { get; set; }
        //[Required]
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }
        public virtual ICollection<QuestionKeyword> Questions { get; set; }
    }

    [Table("Request")]
    public class Request
    {
        [Key, Column(Order = 1)]
        //[Required(ErrorMessage = "RequestID is required")]
        public int RequestId { get; set; }
        [System.ComponentModel.DisplayName("Activation")]
        public bool IsActive { get; set; }
        [System.ComponentModel.DisplayName("Time Spent")]
        public Int64 TotalTimeSpent { get; set; }

        [System.ComponentModel.DisplayName("Caller Name")]
        public string Name { get; set; }
        [System.ComponentModel.DisplayName("Caller Phone")]
        public string Phone { get; set; }
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Caller Caller { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<UserCompleteRequest> UserCompleteRequests { get; set; }
    }

    [Table("Patient")]
    public class Patient
    {
        [Key, Column(Order = 1)]
        //[Required(ErrorMessage = "PaientID is required")]
        [Editable(true)]
        public int PatientId { get; set; }
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
        //[Required(ErrorMessage = "Name of the caller is required")]
        public string Name { get; set; }
        [Key, Column(Order = 2)]
        //[Required(ErrorMessage = "Phone number of the caller is required")]
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Region { get; set; }
        public string TypeAbbreviate { get; set; }
        public virtual CallerType CallerType { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }

    [Table("CallerType")]
    public class CallerType
    {
        [Key]
        public string TypeAbbreviate { get; set; }
        public string Name { get; set; }

        public ICollection<Caller> Callers { get; set; }
    }

    [Table("Question")]
    public class Question
    {
        [Key, Column(Order = 1)]
        //[Required]
        public int QuestionId { get; set; }
        [Column(TypeName = "text")]
        [MaxLength(3000)]
        public string QuestionContent { get; set; }
        // [Required]
        [Column(TypeName = "text")]
        [MaxLength(3000)]
        public string Response { get; set; }
        //[Required]
        public string Severity { get; set; }
        //[Required]
        public string Probability { get; set; }
        public int ComputedImpact { get; set; }
        public Int64 TimeTaken { get; set; }
        //[Required]
        //public string TumourGroup { get; set; }
        public string TumorTypeAbbreviate { get; set; }
        //[Required]
        public int RequestId { get; set; }
        //[Required]
        //public string QuestionType { get; set; }
        public string QuestionTypeAbbreviate { get; set; }
        public virtual ICollection<QuestionKeyword> Keywords { get; set; }
        public virtual ICollection<QuestionReference> References { get; set; }

        public virtual Request Request { get; set; }
        public virtual TumorGroup TumorGroup { get; set; }
        public virtual QuestionType QType { get; set; }
    }

    [Table("QuestionKeyword")]
    public class QuestionKeyword
    {
        [Key, Column(Order = 0)]
        //[Required]
        public int QuestionId { get; set; }
        [Key, Column(Order = 1)]
        //[Required]
        public string Keyword { get; set; }

        public virtual Question Question { get; set; }
        public virtual Keywords Key { get; set; }
    }

    [Table("QuestionReference")]
    public class QuestionReference
    {
        [Key, Column(Order = 1)]
        //[Required]
        public int QuestionId { get; set; }
        [Key, Column(Order = 2)]
        //[Required]
        public int ReferenceId { get; set; }
        public virtual Question Question { get; set; }
        public virtual Reference Reference { get; set; }
    }

    [Table("QuestionType")]
    public class QuestionType
    {
        [Key, Column(Order = 1)]
        // [Required]
        //public string TypeAbbreviate { get; set; }
        public string QuestionTypeAbbreviate { get; set; }
        // [Required]
        public string Name { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }

    [Table("Reference")]
    public class Reference
    {
        [Key, Column(Order = 1)]
        // [Required]
        public int ReferenceId { get; set; }
        // [Required]
        [Column(TypeName = "text")]
        [MaxLength(3000)]
        public string ReferenceContent { get; set; }
        // [Required]
        public string Type { get; set; }

        public virtual ICollection<QuestionReference> QuestionRef { get; set; }
    }

    [Table("TumorGroup")]
    public class TumorGroup
    {
        [Key, Column(Order = 1)]
        // [Required]
        //public string TypeAbbreviate { get; set; }
        public string TumorTypeAbbreviate { get; set; }
        // [Required]
        public string Name { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }




    //[Table("UserCompleteRequest")]
    //public class UserCompleteRequest
    //{
    //    [Key, Column(Order = 1)]
    //    // [Required]
    //    public int UserID { get; set; }
    //    [Key, Column(Order = 2)]
    //    // [Required]
    //    public int RequestID { get; set; }
    //    // [Required]
    //    public DateTime CompletionTime { get; set; }
    //    public virtual UserProfile User { get; set; }
    //    public virtual Request Request { get; set; }
    //}

    //[Table("UserCreateRequest")]
    //public class UserCreateRequest
    //{
    //    [Key, Column(Order = 1)]
    //    //[Required]
    //    public int UserId { get; set; }
    //    [Key, Column(Order = 2)]
    //    // [Required]
    //    public int RequestId { get; set; }
    //    [Key, Column(Order = 3)]
    //    // [Required]
    //    public DateTime TimeCreated { get; set; }
    //    public virtual UserProfile User { get; set; }
    //    public virtual Request Request { get; set; }
    //}

    //[Table("UserCompleteRequest")]
    //public class UserCompleteRequest
    //{
    //    [Key, Column(Order = 1)]
    //    [Required]
    //    public int UserID { get; set; }
    //    [Key, Column(Order = 2)]
    //    [Required]
    //    public int RequestID { get; set; }
    //    [Required]
    //    public DateTime CompletionTime { get; set; }
    //    public virtual UserProfile User { get; set; }
    //    public virtual Request Request { get; set; }
    //}

    //[Table("UserCreateRequest")]
    //public class UserCreateRequest
    //{
    //    [Key, Column(Order = 1)]
    //    [Required]
    //    public int UserId { get; set; }
    //    [Key, Column(Order = 2)]
    //    [Required]
    //    public int RequestId { get; set; }
    //    [Key, Column(Order = 3)]
    //    [Required]
    //    public DateTime TimeCreated { get; set; }
    //    public virtual UserProfile User { get; set; }
    //    public virtual Request Request { get; set; }
    //}

    [Table("UserEditRequest")]
    public class UserEditRequest
    {
        [Key, Column(Order = 1)]
        // [Required]
        public int UserId { get; set; }
        [Key, Column(Order = 2)]
        //[Required]
        public int RequestId { get; set; }
        [Key, Column(Order = 3)]
        //[Required]
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public virtual UserProfile User { get; set; }
        public virtual Request Request { get; set; }
    }

    [Table("UserViewRequest")]
    public class UserViewRequest
    {
        [Key, Column(Order = 1)]
        //[Required]
        public int UserId { get; set; }
        [Key, Column(Order = 2)]
        //[Required]
        public int RequestId { get; set; }
        [Key, Column(Order = 3)]
        // [Required]
        public DateTime ViewTime { get; set; }
        public virtual UserProfile User { get; set; }
        public virtual Request Request { get; set; }
    }

    //public class Keywords
    //{
    //    [Key, Column(Order = 1)]
    //    [Required]
    //    [Display(Name = "Keyword")]
    //    public string Keyword { get; set; }
    //    [Required]
    //    [Display(Name = "IsActive")]
    //    public bool IsActive { get; set; }

    //}


    //public class Request
    //{
    //    [Key, Column(Order = 1)]
    //    [Required(ErrorMessage = "RequestID is required")]
    //    public int RequestId { get; set; }
    //    [System.ComponentModel.DisplayName("Activation")]
    //    public bool IsActive { get; set; }
    //    [System.ComponentModel.DisplayName("Time Spent")]
    //    public DateTime TotalTimeSpent { get; set; }
    //    public string Name { get; set; }
    //    public string Phone { get; set; }
    //    public int PatientId { get; set; }

    //}


    //public class Patient
    //{
    //    [Key, Column(Order = 1)]
    //    [Required(ErrorMessage = "PaientID is required")]
    //    [Editable(true)]
    //    public int PatientId { get; set; }
    //    public string Name { get; set; }
    //    public int AgencyID { get; set; }
    //    public int Age { get; set; }
    //    public string Gender { get; set; }
    //}


    //public class Caller
    //{
    //    [Key, Column(Order = 1)]
    //    [Required(ErrorMessage = "Name of the caller is required")]
    //    public string Name { get; set; }
    //    [Key, Column(Order = 2)]
    //    [Required(ErrorMessage = "Phone number of the caller is required")]
    //    public string Phone { get; set; }
    //    public string Email { get; set; }
    //    public string Region { get; set; }
    //    public string TypeAbbreviate { get; set; }
    //}

    //public class CallerType
    //{
    //    [Key]
    //    public string TypeAbbreviate { get; set; }
    //    public string Name { get; set; }
    //}


    //public class Question
    //{
    //    [Key, Column(Order = 1)]
    //    [Required]
    //    public int QuestionId { get; set; }
    //    [Column(TypeName = "text")]
    //    [MaxLength(3000)]
    //    public string QuestionContent { get; set; }
    //    [Required]
    //    [Column(TypeName = "text")]
    //    [MaxLength(3000)]
    //    public string Response { get; set; }
    //    [Required]
    //    public string Severity { get; set; }
    //    [Required]
    //    public string Probability { get; set; }
    //    public int ComputedImpact { get; set; }
    //    public DateTime TimeTaken { get; set; }
    //    [Required]
    //    //public string TumourGroup { get; set; }
    //    public string TumorTypeAbbreviate { get; set; }
    //    [Required]
    //    public int RequestId { get; set; }
    //    [Required]
    //    //public string QuestionType { get; set; }
    //    public string QuestionTypeAbbreviate { get; set; }

    //}

    //public class QuestionKeyword
    //{
    //    [Key, Column(Order = 1)]
    //    [Required]
    //    public int QuestionId { get; set; }
    //    [Key, Column(Order = 2)]
    //    [Required]
    //    public string Keyword { get; set; }

    //}


    //public class QuestionReference
    //{
    //    [Key, Column(Order = 1)]
    //    [Required]
    //    public int QuestionId { get; set; }
    //    [Key, Column(Order = 2)]
    //    [Required]
    //    public int ReferenceId { get; set; }

    //}


    //public class QuestionType
    //{
    //    [Key, Column(Order = 1)]
    //    [Required]
    //    //public string TypeAbbreviate { get; set; }
    //    public string QuestionTypeAbbreviate { get; set; }
    //    [Required]
    //    public string Name { get; set; }

    //}


    //public class Reference
    //{
    //    [Key, Column(Order = 1)]
    //    [Required]
    //    public int ReferenceId { get; set; }
    //    [Required]
    //    [Column(TypeName = "text")]
    //    [MaxLength(3000)]
    //    public string ReferenceContent { get; set; }
    //    [Required]
    //    public string Type { get; set; }

    //}


    //public class TumorGroup
    //{
    //    [Key, Column(Order = 1)]
    //    [Required]
    //    //public string TypeAbbreviate { get; set; }
    //    public string TumorTypeAbbreviate { get; set; }
    //    [Required]
    //    public string Name { get; set; }

    //}


    //public class UserCompleteRequest
    //{
    //    [Key, Column(Order = 1)]
    //    [Required]
    //    public int UserID { get; set; }
    //    [Key, Column(Order = 2)]
    //    [Required]
    //    public int RequestID { get; set; }
    //    [Required]
    //    public DateTime CompletionTime { get; set; }

    //}


    //public class UserCreateRequest
    //{
    //    [Key, Column(Order = 1)]
    //    [Required]
    //    public int UserId { get; set; }
    //    [Key, Column(Order = 2)]
    //    [Required]
    //    public int RequestId { get; set; }
    //    [Key, Column(Order = 3)]
    //    [Required]
    //    public DateTime TimeCreated { get; set; }
    //}


    //public class UserEditRequest
    //{
    //    [Key, Column(Order = 1)]
    //    [Required]
    //    public int UserId { get; set; }
    //    [Key, Column(Order = 2)]
    //    [Required]
    //    public int RequestId { get; set; }
    //    [Key, Column(Order = 3)]
    //    [Required]
    //    public DateTime StartTime { get; set; }
    //    public DateTime FinishTime { get; set; }

    //}

    ////[Table("UserExportPatient")]
    ////public class UserExportPatient
    ////{
    ////    [Key, Column(Order = 1)]
    ////    [Required]
    ////    public int UserId { get; set; }
    ////    [Key, Column(Order = 2)]
    ////    [Required]
    ////    public int PatientId { get; set; }
    ////    [Key, Column(Order = 3)]
    ////    [Required]
    ////    public DateTime Time { get; set; }
    ////}

    ////[Table("UserExportRequest")]
    ////public class UserExportRequest
    ////{
    ////    [Key, Column(Order = 1)]
    ////    [Required]
    ////    public int UserId { get; set; }
    ////    [Key, Column(Order = 2)]
    ////    [Required]
    ////    public int RequestId { get; set; }
    ////    [Required]
    ////    public DateTime Time { get; set; }
    ////}

    ////[Table("UserGroup")]
    ////public class UserGroup
    ////{
    ////    [Key, Column(Order = 1)]
    ////    [Required]
    ////    public string Abbreviate { get; set; }
    ////    [Required]
    ////    public string Name { get; set; }
    ////}

    ////[Table("UserProfile")]
    ////public class UserProfile
    ////{
    ////    [Key, Column(Order = 1)]
    ////    [Required]
    ////    public int UserId { get; set; }
    ////    [Required]
    ////    public string UserName { get; set; }
    ////    public int IsActivated { get; set; }
    ////}


    //public class UserViewRequest
    //{
    //    [Key, Column(Order = 1)]
    //    [Required]
    //    public int UserId { get; set; }
    //    [Key, Column(Order = 2)]
    //    [Required]
    //    public int RequestId { get; set; }
    //    [Key, Column(Order = 3)]
    //    [Required]
    //    public DateTime ViewTime { get; set; }
    //}
}
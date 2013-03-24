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
    public class DISpecialistContext : DbContext
    {
        public DISpecialistContext()
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
        public DbSet<UserProfile> Users{ get; set; }
        public DbSet<UserExportRequest> UserExportRequests { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    [Table("Keyword")]
    public class Keywords
    {
        [Key, Column(Order = 1)]
        [Required]
        [Display(Name = "Keyword")]
        public string Keyword { get; set; }
        //[Required]
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }
    }


    [Table("Request")]
    public class Request
    {
        [Key, Column(Order = 1)]
        //[Required(ErrorMessage = "RequestID is required")]
        public Int64 RequestId { get; set; }
        [System.ComponentModel.DisplayName("Activation")]
        public bool IsActive { get; set; }
        [System.ComponentModel.DisplayName("Time Spent")]
        public int? TotalTimeSpent { get; set; }
        [System.ComponentModel.DisplayName("Caller Name")]
        public string Name { get; set; }
        [System.ComponentModel.DisplayName("Caller Phone")]
        public string Phone { get; set; }
        [System.ComponentModel.DisplayName("Patient ID")]
        public Int64? PatientId { get; set; }  
    }

    [Table("Patient")]
    public class Patient
    {
        [Key, Column(Order = 1)]
        //[Required(ErrorMessage = "PaientID is required")]
        [Editable(true)]
        public Int64 PatientId { get; set; }
        public string Name { get; set; }
        public int? AgencyID { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        //public virtual ICollection<Request> Requests { get; set; }
    }

    [Table("Caller")]
    public class Caller
    {
        [Key, Column(Order = 1)]
        [Required(ErrorMessage = "Name of the caller is required")]
        [System.ComponentModel.DisplayName("Caller Name")]
        [MaxLength(100, ErrorMessage = "The {0} must be less than 100 characters")]
        public string Name { get; set; }

        [Key, Column(Order = 2)]
        [System.ComponentModel.DisplayName("Caller Phone")]
        [MaxLength(15, ErrorMessage = "The {0} must be less than 15 characters")]
        [Required(ErrorMessage = "Phone number of the caller is required")]
        public string Phone { get; set; }

        [MaxLength(100, ErrorMessage = "The {0} must be less than 100 characters")]
        [System.ComponentModel.DisplayName("Caller Email")]
        public string Email { get; set; }

        [MaxLength(50, ErrorMessage = "The {0} must be less than 50 characters")]
        [System.ComponentModel.DisplayName("Caller Region")]
        public string Region { get; set; }

        // [MaxLength(100, ErrorMessage = "The {0} must be less than 10 characters")]
        [System.ComponentModel.DisplayName("Caller Type")]
        public string TypeAbbreviate { get; set; }
    }

    [Table("CallerType")]
    public class CallerType
    {
        [Key]
        public string TypeAbbreviate { get; set; }
        public string Name { get; set; }
    }

    [Table("Question")]
    public class Question
    {
        [Key, Column(Order = 1)]
        //[Required]
        public Int64 QuestionId { get; set; }
        [Column(TypeName = "text")]
        [MaxLength(3000)]
        [Required]
        public string QuestionContent { get; set; }
        // [Required]
        [Column(TypeName = "text")]
        [MaxLength(3000)]
        public string Response { get; set; }
        [Required]
        public string Severity { get; set; }
        [Required]
        public string Probability { get; set; }
        
        public int? ComputedImpact { get; set; }
        public int? TimeTaken { get; set; }
        //[Required]
        public string TumorTypeAbbreviate { get; set; }
        //[Required]
        public Int64 RequestId { get; set; }
        //[Required]
        public string QuestionTypeAbbreviate { get; set; }
    }

    [Table("QuestionKeyword")]
    public class QuestionKeyword
    {
        [Key, Column(Order = 0)]
        //[Required]
        public Int64 QuestionId { get; set; }
        [Key, Column(Order = 1)]
        //[Required]
        public string Keyword { get; set; }

        
    }

    [Table("QuestionReference")]
    public class QuestionReference
    {
        [Key, Column(Order = 1)]
        //[Required]
        public Int64 QuestionId { get; set; }
        [Key, Column(Order = 2)]
        //[Required]
        public Int64 ReferenceId { get; set; }
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
    }

    [Table("Reference")]
    public class Reference
    {
        [Key, Column(Order = 1)]
        // [Required]
        public Int64 ReferenceId { get; set; }
        // [Required]
        [Column(TypeName = "text")]
        [MaxLength(3000)]
        public string ReferenceContent { get; set; }
        // [Required]
        public string Type { get; set; }

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
    }


    [Table("UserEditRequest")]
    public class UserEditRequest
    {
        [Key, Column(Order = 1)]
        // [Required]
        public Int64 UserId { get; set; }
        [Key, Column(Order = 2)]
        //[Required]
        public Int64 RequestId { get; set; }
        [Key, Column(Order = 3)]
        //[Required]
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
    }

    [Table("UserViewRequest")]
    public class UserViewRequest
    {
        [Key, Column(Order = 1)]
        //[Required]
        public Int64 UserId { get; set; }
        [Key, Column(Order = 2)]
        //[Required]
        public Int64 RequestId { get; set; }
        [Key, Column(Order = 3)]
        // [Required]
        public DateTime ViewTime { get; set; }
    }

    public class DISpecialistModel
    {
        public SearchModel searchModel{ get; set; }
        public List<RequestViewModel> requests { get; set; }
        public RequestViewModel editModel { get; set; }
        public bool isEditorOpen { get; set; }
    }

    public class SearchModel
    {
        public string searchBy { get; set; }
        public string searchKey { get; set; }
    }

    public class RequestViewModel
    {
        public Request request { get; set; }
        public Patient patient { get; set; }
        public UserProfile createrProfile { get; set; }
        public UserProfile completerProfile { get; set; }
        public Caller caller { get; set; }
        public CallerType callerType { get; set; }
        public UserCreateRequest userCreateRequest { get; set; }
        public UserCompleteRequest userCompleteRequest { get; set; }

        public Question newQuestion { get; set; }
        public QuestionType newQuestionType { get; set; }
        //public List<QuestionKeyword> questionKeyword { get; set; }
        public Keywords newKeyword { get; set; }
        //public QuestionReference questionReference { get; set; }
        public Reference newReference { get; set; }
        public TumorGroup tumorGroup { get; set; }
    }
   
}
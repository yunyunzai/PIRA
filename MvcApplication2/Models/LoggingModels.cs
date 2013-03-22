using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web;

namespace MvcApplication2.Models
{

        public class LoggingContext : DbContext
        {
            public LoggingContext()
                : base("DefaultConnection")
            {
            }

            public DbSet<UserExportRequest> UserExportRequests { get; set; }
            public DbSet<UserExportPatient> UserExportPatients { get; set; }
            public DbSet<UserCreateRequest> UserCreateRequest { get; set; }
            public DbSet<UserCompleteRequest> UserCompleteRequest { get; set; }
            public DbSet<UserEditRequest> UserEditRequests { get; set; }
        }

        [Table("UserExportRequest")]
        public class UserExportRequest
        {

            [Key]
            [Column(Order = 0)]
            //[ForeignKey("UserId")]
            [Required]
            public Int64 UserId { get; set; }
            [Key]
            [Column(Order = 1)]
            //[ForeignKey("RoleId")]
            public Int64 RequestId { get; set; }
            public DateTime time { get; set; }
        }

        [Table("UserExportPatient")]
        public class UserExportPatient
        {

            [Key]
            [Column(Order = 0)]
            //[ForeignKey("UserId")]
            [Required]
            public Int64 UserId { get; set; }
            [Key]
            [Column(Order = 1)]
            //[ForeignKey("RoleId")]
            public Int64 PatientId { get; set; }
            public DateTime time { get; set; }
        }

        [Table("UserCreateRequest")]
        public class UserCreateRequest
        {

            [Key]
            [Column(Order = 0)]
            //[ForeignKey("UserId")]
            [Required]
            public Int64 UserId { get; set; }
            [Key]
            [Column(Order = 1)]
            //[ForeignKey("RoleId")]
            public Int64 RequestId { get; set; }
            public DateTime TimeCreated { get; set; }
        }

        [Table("UserCompleteRequest")]
        public class UserCompleteRequest
        {

            [Key]
            [Column(Order = 0)]
            //[ForeignKey("UserId")]
            [Required]
            public Int64 UserID { get; set; }
            [Key]
            [Column(Order = 1)]
            //[ForeignKey("RoleId")]
            public Int64 RequestID { get; set; }
            public DateTime CompletionTime { get; set; }
           // public virtual ICollection<Request> Requests { get; set; }
        }


}

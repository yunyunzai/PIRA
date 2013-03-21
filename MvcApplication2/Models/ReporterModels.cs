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
    public class ReporterContext : DbContext
    {
        public ReporterContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Request> Requests { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Caller> Callers { get; set; }
        //TODO public DbSet<UserInfo> User { get; set; }
        public DbSet<UserCreateRequest> UserCreateRequest { get; set; }
        public DbSet<UserCompleteRequest> UserCompleteRequest { get; set; }
    }

    public class ReporterPanelModel
    {
        [Required]
        [Display(Name = "From:")]
        public DateTime fromDate { get; set; }
        [Required]
        [Display(Name = "To:")]
        public DateTime toDate { get; set; }
        [Display(Name = "Average time per request")]
        public bool averageTimePerRequests { get; set; }
        [Display(Name = "Total number of requests")]
        public bool totalNRequests { get; set; }
        public bool wantCallerTypeChart { get; set; }
        public bool wantCallerRegionChart { get; set; }
        public bool wantTumorTypeChart { get; set; }
        //public string stratify { get; set; }
        
        //public IEnumerable<SelectListItem> stratifys
        //{
        //    get
        //    {
        //        return Enumerable.Range(0, 23).Select(x => new SelectListItem
        //            {
        //                Value = x.ToString("00"),
        //                Text = x.ToString("00")
        //            });
        //    }
        //}

        

    }


    public class Fields
    { 

    }


}

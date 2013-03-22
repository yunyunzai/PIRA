
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Serialization;
using NPOI.HSSF.EventModel;
using OfficeOpenXml;
using MvcApplication2.Models;
using OfficeOpenXml.Drawing.Chart;

namespace MvcApplication2.Controllers
{
    public class ReporterController : Controller
    {
        //
        // GET: /Reporter/




        public ActionResult Reporter()
        {
            
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reporter(ReporterPanelModel m) 
        {
            if (ModelState.IsValid)
            {
                try
                {
                    FileInfo newFile = new FileInfo(Server.MapPath(@"\Content\ExcelPackageNewFile.xlsx"));
                    newFile.Delete();
                    //FileInfo template = new FileInfo(Server.MapPath(@"\Content\ExcelPackageTemplate.xlsx"));
                    ExcelPackage p = new ExcelPackage(newFile);

                    //UsersContext uc = new UsersContext();
                    int reporterID = int.Parse(Membership.GetUser().ProviderUserKey.ToString());
                    System.Diagnostics.Debug.WriteLine(reporterID);
                    //int reporterID = uc.UserProfiles.Where(a => a.UserName == System.Web.HttpContext.Current.User.Identity.Nam).First().UserId;
                    System.Diagnostics.Debug.WriteLine(m.fromDate);

                    generateRequestTable(reporterID, p, m);
                    generatePatientTable(reporterID, p, m);
                    generateRequestStatsChart(reporterID, p, m);

                    p.Save();
                    Response.Clear();
                    System.Diagnostics.Debug.WriteLine("Excel report created successfully!");
                    System.Diagnostics.Process.Start(Server.MapPath(@"\Content\ExcelPackageNewFile.xlsx"));
                    return View(m);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Oops! Something went wrong." + ex.Message);
                    return new EmptyResult();
                }
            }
            else
                // If we got this far, something failed, redisplay form
                return new EmptyResult();
        }

        
        private void generateRequestStatsChart(int reporterID, ExcelPackage p,ReporterPanelModel m)
        {
            p.Workbook.Worksheets.Add("RequestsStats");
            ExcelWorksheet ws =  p.Workbook.Worksheets.Last();
            // preparing data     

            ReporterContext rc = new ReporterContext();
            var info = from table1 in rc.Requests
                       join table2 in rc.UserCreateRequest on table1.RequestId equals table2.RequestId
                       select new
                       {
                           table1.TotalTimeSpent,
                           table2.TimeCreated
                       };

            int totalMonths = Math.Max(m.toDate.Month - m.fromDate.Month + (m.toDate.Year - m.fromDate.Year) * 12+1,0);
            DateTime currentStartDate=new DateTime(m.fromDate.Year,m.fromDate.Month,1);
            DateTime currentEndDate = new DateTime(m.fromDate.Year, m.fromDate.Month, 1);
            currentEndDate = currentEndDate.AddMonths(1);
            ws.Cells[1, 1].Value = "Month";
            ws.Cells[2, 1].Value = "Total Number of Request";
            ws.Cells[3, 1].Value = "Average Time Spent (mins)";
            ws.Cells[1, 1, 3, 1].AutoFitColumns();
            for (int i = 2; i <= totalMonths+1; i++)
            {
                ws.Cells[1, i].Value = currentStartDate.Year + " " + currentStartDate.ToString("MMMM");
                var requestsThisMonth = from r in info
                                        where r.TimeCreated.CompareTo(currentStartDate)>=0 && r.TimeCreated.CompareTo(currentEndDate)<=0
                                        select new { r.TotalTimeSpent, r.TimeCreated };
                
                ws.Cells[2, i].Value = requestsThisMonth.Count();
                ws.Cells[3, i].Value = requestsThisMonth.Count()==0?0:requestsThisMonth.Sum(r => r.TotalTimeSpent) / 1.0 /requestsThisMonth.Count();
                ws.Column(i).Width = 13;
                currentStartDate=currentStartDate.AddMonths(1);
                currentEndDate=currentEndDate.AddMonths(1);
                                       
            }

            var chartType1 = ws.Drawings.AddChart("RequestsStats", eChartType.ColumnStacked) as ExcelChart;

            var ser1 = chartType1.Series.Add(ws.Cells[2, 2, 2, totalMonths + 1], ws.Cells[1, 2, 1, totalMonths + 1]);
            chartType1.SetPosition(3, 0, 0, 0);
            chartType1.Series[0].Header = "total number";
            chartType1.SetSize(Math.Max(totalMonths * 100, 500), 500);
            chartType1.Title.Text = "Total Number of Request for Each Month";

            var chartType2 = chartType1.PlotArea.ChartTypes.Add(eChartType.LineStacked);
            var ser2 = chartType2.Series.Add(ws.Cells[3, 2, 3, totalMonths + 1], ws.Cells[1, 2, 1, totalMonths + 1]);
            ser2.Header = "average time (mins)";
            
            


            //int startDate=form.GetValues("fromDatePicker").Count();
            ////for (int i = 0; i < form.Keys.Count;i++ )
            //System.Diagnostics.Debug.WriteLine(123);
            //System.Diagnostics.Debug.WriteLine(startDate);
            
        }

        private void generateRequestTable(int reporterID, ExcelPackage p, ReporterPanelModel m)
        {
            p.Workbook.Worksheets.Add("Requests"); 
            ExcelWorksheet ws = p.Workbook.Worksheets.Last();

            ws.Name = "Requests"; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

            //Merging cells and create a center heading for out table
            ReporterContext rc = new ReporterContext();
            
            var requests = rc.Requests.Select(a => a);
            Models.Request requestTable = new Models.Request();

            var info = from table1 in rc.Requests
                       join table2 in rc.UserCreateRequest on table1.RequestId equals table2.RequestId
                       join table3 in rc.Callers on new { table1.Name, table1.Phone } equals new { table3.Name, table3.Phone }
                       join table4 in rc.UserCompleteRequest on table1.RequestId equals table4.RequestID into ps
                       from joinedTable in ps.DefaultIfEmpty() // left outer join
                       where table2.TimeCreated.CompareTo(m.fromDate) >= 0 && table2.TimeCreated.CompareTo(m.toDate) <= 0
                       select new
                       {
                           table1.RequestId,
                           table1.IsActive,
                           table1.TotalTimeSpent,
                           table1.Name,
                           table1.Phone,
                           table1.PatientId,
                           table2.TimeCreated,
                           table3.TypeAbbreviate,
                           CompletionTime = (joinedTable == null ? new DateTime() : joinedTable.CompletionTime)
                       };
            
            ws.Cells[1, 1].Value = "Request ID";
            ws.Cells[1, 2].Value = "Still Active?";
            ws.Cells[1, 3].Value = "Total Time Spent";
            ws.Cells[1, 4].Value = "Caller Name";
            ws.Cells[1, 5].Value = "Caller Phone";
            ws.Cells[1, 6].Value = "Caller Type";
            ws.Cells[1, 7].Value = "Patient ID";
            ws.Cells[1, 8].Value = "Time Created";
            ws.Cells[1, 9].Value = "Completed By";
            int row = 2;

            int col = 1;

            foreach (var r in info)
            {
                
                ws.Cells[row, col].Value = r.RequestId;                
                ws.Cells[row, col + 1].Value = r.IsActive;
                ws.Cells[row, col + 2].Value = r.TotalTimeSpent.ToString();
                ws.Cells[row, col + 3].Value = r.Name;
                ws.Cells[row, col + 4].Value = r.Phone;
                ws.Cells[row, col + 5].Value = r.TypeAbbreviate;
                ws.Cells[row, col + 6].Value = r.PatientId;
                ws.Cells[row, col + 7].Value = r.TimeCreated.ToString();
                ws.Cells[row, col + 8].Value = r.CompletionTime==new DateTime()?"In Progress":r.CompletionTime.ToString();
                logRequestExport(reporterID, r.RequestId);
                row++;
            }
            ws.Cells[1, 1, row,9].AutoFitColumns();

            
        }

        private void logRequestExport(int reporterID, long requestID)
        {
            LoggingContext lc = new LoggingContext();
            UserExportRequest r=new UserExportRequest();

            r.UserId = reporterID;
            r.RequestId = requestID;
            r.time = DateTime.Now;
            lc.UserExportRequests.Add(r);
            lc.SaveChanges();

        }
        private void generatePatientTable(int reporterID, ExcelPackage p, ReporterPanelModel m)
        {
            p.Workbook.Worksheets.Add("Patients"); 
            ExcelWorksheet ws = p.Workbook.Worksheets[2];

            ws.Name = "Patients"; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

            //Merging cells and create a center heading for out table
            ReporterContext rc = new ReporterContext();
            var patients = rc.Patients.Select(a => a);

            ws.Cells[1, 1].Value = "PatientID";
            ws.Cells[1, 2].Value = "Name";
            ws.Cells[1, 3].Value = "AgencyID";
            ws.Cells[1, 4].Value = "Age";
            ws.Cells[1, 5].Value = "Gender";
            int row = 2;

            int col = 1;
            foreach (Patient r in patients)
            {
                ws.Cells[row, col].Value = r.PatientId;
                ws.Cells[row, col + 1].Value = r.Name;
                ws.Cells[row, col + 2].Value = r.AgencyID;
                ws.Cells[row, col + 3].Value = r.Age;
                ws.Cells[row, col + 4].Value = r.Gender;
                logPatientExport(reporterID, r.PatientId);
                row++;
            }
        }

        private void logPatientExport(int reporterID, long patientID)
        {
            LoggingContext lc = new LoggingContext();
            UserExportPatient r = new UserExportPatient();

            r.UserId = reporterID;
            r.PatientId = patientID;
            r.time = DateTime.Now;
            lc.UserExportPatients.Add(r);
            lc.SaveChanges();

        }


    }
}


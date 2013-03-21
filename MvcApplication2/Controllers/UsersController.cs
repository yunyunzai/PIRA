using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication2.Models;

namespace MvcApplication2.Controllers
{
    public class UsersController : Controller
    {
        private UsersContext db = new UsersContext();
        
        //
        // GET: /Users/

        public ActionResult Index()
        {
          
            return View(db.UserProfiles.ToList());
        }

        //
        // GET: /Users/Details/5

        public ActionResult Details(int id = 0)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
           
            return View(userprofile);
        }

        //
        // GET: /Users/Create

        public ActionResult Create()
        {
          
            return View();
        }

       
       
        //
        // POST: /Users/Create

        [HttpPost]
        public ActionResult Create(UserProfile userprofile)
        {

            
                if (ModelState.IsValid)
                {


                    
                            db.UserProfiles.Add(userprofile);
                                                  

                            db.SaveChanges();

                            
                          
                          

                            return RedirectToAction("Index");
                                            
                        
                     
                    
                    
                }
            
            
            return View(userprofile);
        }

        
            

        
        //
        // GET: /Users/Edit/5

        public ActionResult Edit(int id = 0)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            ListAllAssignedRoles(id);
            ListAllAssignedGroups(id);
            return View(userprofile);
        }
        private void ListAllAssignedGroups(int UserId)
        {
            var user = db.UserProfiles
                .Include(i => i.UserBelongsGroup)
                .Where(i => i.UserId == UserId)
                .Single();

            var viewModel = new HashSet<string>();
            foreach (var group in user.UserBelongsGroup)
            {
                viewModel.Add(group.Abbreviate);
            }
            var allGroups = db.UserGroup;
            var groupViewModel = new List<ViewModels.UserGroup>();
            foreach (var group in allGroups)
            {
                if (viewModel.Contains(group.Abbreviate))
                {
                    groupViewModel.Add(new ViewModels.UserGroup
                    {
                        Name = group.Name,
                        Abbreviate = group.Abbreviate,
                        IsAssigned = true

                    });


                }
                else
                {
                    groupViewModel.Add(new ViewModels.UserGroup
                    {
                        Name = group.Name,
                        Abbreviate = group.Abbreviate,
                        IsAssigned = false

                    });
                }
                if (groupViewModel != null)
                {
                    ViewBag.AllAssignedGroups = groupViewModel;
                }
            }

        }
        private void ListAllAssignedRoles(int UserId)
        {
            var user = db.UserProfiles
                .Include(i => i.UsersInRoles)
                .Where(i => i.UserId == UserId)
                .Single();
          //  var allRoles = db.Roles;
            var viewModel = new HashSet<int>();
           
            foreach (var role in user.UsersInRoles)
            {
                viewModel.Add(role.RoleId);
            }
            
            var allRoles = db.Roles;
            var roleViewModel = new List<ViewModels.Roles>();
            foreach (var role in allRoles)
            {
                if(viewModel.Contains(role.RoleId)){
                    roleViewModel.Add(new ViewModels.Roles{
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    IsSuperUser = role.IsSuperUser,
                    IsAssigned = true
                    });

                }
                else{
                roleViewModel.Add(new ViewModels.Roles{
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    IsSuperUser = role.IsSuperUser,
                    IsAssigned = false
                });
            }
            }

            if (roleViewModel != null)
            {
                ViewBag.AllAssignedRoles = roleViewModel;
            }
        }


        //
        // POST: /Users/Edit/5

        [HttpPost]
        public ActionResult Edit(int UserId, FormCollection collection, string [] selectedRoles, string [] selectedGroups)
        {
            var userprofile = db.UserProfiles
                .Include(i => i.UsersInRoles)
                .Where(i => i.UserId == UserId)
                .Single();

          if(TryUpdateModel(userprofile))
          { 
            try
            {
                if (ModelState.IsValid)
                {

                              
                    db.Entry(userprofile).State = EntityState.Modified;
                    db.SaveChanges();
                    EditUserRoles(UserId, selectedRoles);
                    EditUserGroups(UserId, selectedGroups);
                    return RedirectToAction("Index");
                }
                
                
            }
           catch (DataException)
            {
                ModelState.AddModelError("", "Couldn't save. Please contact the system admin.");
                return View();
            }
          
          }
            return View(userprofile);
        }

        private void EditUserGroups(int UserId, string[] selectedGroups)
        {
            var user = db.UserProfiles
                .Include(i => i.UserBelongsGroup)
                .Where(i => i.UserId == UserId)
                .Single();
            var groupQuery = db.UserGroup;
            var viewModelGroup = new List<Models.UserGroup>();
            foreach (var group in groupQuery)
            {
                viewModelGroup.Add(new Models.UserGroup
                {
                    Abbreviate = group.Abbreviate,
                    Name = group.Name,
                   // IsAssigned = false
                });
            }

            var sel_groups = new HashSet<string>(selectedGroups);

            var userGroups = new HashSet<string>(user.UserBelongsGroup.Select(c => c.Abbreviate));
            foreach (var g in viewModelGroup)
            {
                if (sel_groups.Contains(g.Abbreviate))
                {
                    if (!userGroups.Contains(g.Abbreviate))
                    {
                        UserBelongsGroup ubg = new UserBelongsGroup
                        {
                            Abbreviate = g.Abbreviate,
                            UserId = user.UserId
                        };
                        db.UserBelongsGroup.Add(ubg);
                        db.SaveChanges();

                    }
                    else
                    {
                        if (userGroups.Contains(g.Abbreviate))
                        {
                            var usersInGroups = from p in db.UserBelongsGroup
                                                where p.UserId == user.UserId
                                                where p.Abbreviate == g.Abbreviate
                                                select p;
                            foreach (var group in usersInGroups)
                            {
                                db.UserBelongsGroup.Attach(group);
                                db.UserBelongsGroup.Remove(group);
                            }
                            db.SaveChanges();

                        }

                    }
                }
            }

        }

        private void EditUserRoles(int UserId, string[] selectedRoles)
        {
            var userprofile = db.UserProfiles
                  .Include(i => i.UsersInRoles)
                  .Where(i => i.UserId == UserId)
                  .Single();
          
            
                var roleQuery = db.Roles;
                var viewModelRole = new List<ViewModels.Roles>();
                foreach (var role in roleQuery)
                {
                    viewModelRole.Add(new ViewModels.Roles
                    {
                        RoleId = role.RoleId,
                        RoleName = role.RoleName,
                        IsSuperUser = role.IsSuperUser,
                        IsAssigned = false
                    });
                }
               
              
                var sel_roles = new HashSet<string>(selectedRoles);
                var userRoles = new HashSet<int>(userprofile.UsersInRoles.Select(c => c.RoleId));
                foreach (var role in viewModelRole)
                {

                    if (sel_roles.Contains(role.RoleId.ToString()))
                    {
                        if (!userRoles.Contains(role.RoleId))
                        {
                            UsersInRoles uir = new UsersInRoles
                            {
                                UserId = userprofile.UserId,
                                RoleId = role.RoleId
                            };
                            db.UsersInRoles.Add(uir);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        if (userRoles.Contains(role.RoleId))
                        {
                            var usersInRoles = from p in db.UsersInRoles
                                               where p.UserId == UserId
                                               where p.RoleId == role.RoleId
                                               select p;
                           
                            foreach (var uir in usersInRoles)
                            {
                                db.UsersInRoles.Attach(uir);
                                db.UsersInRoles.Remove(uir);
                                
                                
                            }
                            db.SaveChanges();
                           
                        }

                    }
                    
                   
                    
                    
                   
                

            }
        }

        //
        // GET: /Users/Delete/5

        public ActionResult Delete(int id = 0)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            return View(userprofile);
        }

        //
        // POST: /Users/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            db.UserProfiles.Remove(userprofile);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
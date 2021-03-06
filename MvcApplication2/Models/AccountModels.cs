﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using MvcApplication2.Controllers;

namespace MvcApplication2.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UsersInRoles> UsersInRoles { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserBelongsGroup> UserBelongsGroup { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<webpages_Membership> webpages_Memberships { get; set; }
    }

    [Table("webpages_UsersInRoles")]
    public class UsersInRoles
    {

        [Key]
        [Column(Order = 0)]
       // [ForeignKey("UserId")]
      //  [Required]
        public Int64 UserId { get; set; }
        [Key][Column(Order = 1)]
      //  [ForeignKey("RoleId")]
        public Int64 RoleId { get; set; }
        
        public virtual Roles Role { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        
    }

    [Table("webpages_Membership")]
    public class webpages_Membership
    {
        [Key]
        public Int64 UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public string ConfirmationToken { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime LastPasswordFailureDate { get; set; }
        public int PasswordFailuresSinceLastSuccess { get; set; }
        public string Password { get; set; }
        public DateTime PasswordChangeDate { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordVerificationToken { get; set; }
        public DateTime PasswordVerificationTokenExpirationDate { get; set; }
    }
    [Table("UserBelongsGroup")]
    public class UserBelongsGroup
    {
        [Key]
        [Column(Order = 0)]
        public Int64 UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string Abbreviate { get; set; }

        public virtual UserGroup UserGroup { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }


    [Table("UserGroup")]
    public class UserGroup
    {
        [Key]
        public string Abbreviate { get; set; }

        public string Name { get; set; }
        public Int16 IsActive { get; set; }


        public virtual ICollection<UserBelongsGroup> UserBelongsGroup { get; set; }
    }

    [Table("webpages_Roles")]
    public class Roles
    {
        [Key]
        public Int64 RoleId { get; set; }
     //   [Required]
        public string RoleName { get; set; }
    /*    [Required]
        public bool CanEdit { get; set; }
        [Required]
        public bool CanView { get; set; }
        [Required]
        public bool CanCreate { get; set; }
        [Required]
        public bool CanExport { get; set; }*/
     //   [Required]
        public bool IsSuperUser { get; set; }

        public virtual ICollection<UsersInRoles> UsersInRoles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        /* TODO LATER!!!! @_@
         * 
         * */

         [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 UserId { get; set; }
       // [Required(ErrorMessage="Please input a username")]
        public string UserName { get; set; }
       // [Required(ErrorMessage="Please specify the user's activation status")]
      
        public bool? IsActivated { get; set; }
       // [Required(ErrorMessage="Please input password")]
  
       // [Required(ErrorMessage="Please input an email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
       // [Required(ErrorMessage="Please input a first-name")]
        public string FirstName { get; set; }
      //  [Required(ErrorMessage="Please input a last-name")]
        public string LastName { get; set; }

     
        public virtual ICollection<UsersInRoles> UsersInRoles { get; set; }
        public virtual ICollection<UserBelongsGroup> UserBelongsGroup { get; set; }
   
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Activation Status")]
        public Boolean IsActivated { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        [Display(Name="First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name="Activation Status")]
        public Boolean IsActivated { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

       
     //   public ICollection<UsersInRoles> UsersInRoles { get; set; }
        public ICollection<UserBelongsGroup> UserBelongsGroup { get; set; }

    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }

    public class ForgotPassword
    {
        public string UserName { get; set; }
    }
}

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mvc5Project.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mvc5Project.Controllers
{
    public class AdminController : Controller
    {

        #region Vars/Props
        public UserManager<ApplicationUser> UserManager { get; set; }
        public ApplicationDbContext context { get; set; }

        public static List<AdminUserViewModel> usrList = new List<AdminUserViewModel>();
        public static List<SelectListItem> roleList = new List<SelectListItem>();
        public static string AdmUsrName { get; set; }
        public static string AdmUsrEmail { get; set; }
        public static string AdmUsrRole { get; set; }
        public static string AdmNameSrch { get; set; }
        public static string AdmRankSrch { get; set; }

        #endregion  Vars/Props

        public AdminController()
        {
            context = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index(AdminUserViewModel model, string sortOrder, string searchString, string searchRank, int? page, ManageMessageId? message = null)
        {
            ViewBag.StatusMessage =
             message == ManageMessageId.UserDeleted ? "User account has successfully been deleted."
             : message == ManageMessageId.UserUpdated ? "User account has been updated."
             : "";

            ViewBag.ErrorMessage =
                message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.HighRankedUser ? "This user account cannot be deleted due to its rank."
                : "";

            await ShowUserDetails(model, sortOrder, searchString, searchRank, page);
            return View();
        }


        [Authorize(Roles = "Admin")]
        [ChildActionOnly]
        public async Task<ActionResult> ShowUserDetails(AdminUserViewModel model, string sortOrder, string searchString, string searchRank, int? page)
        {
            usrList.Clear();
            roleList.Clear();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.RankSortParm = string.IsNullOrEmpty(sortOrder) ? "rank_desc" : "";
            ViewBag.UsernameSortParm = sortOrder == "Username" ? "username_desc" : "Username";
            IList<ApplicationUser> users = context.Users.ToList();

            foreach (var user in users)
            {
                var roles = await UserManager.GetRolesAsync(user.Id);
                model.UserName = user.UserName;
                foreach (var role in roles)
                {
                    model.RankName = role;
                    switch (role)
                    {
                        case "Admin":
                            model.RankId = "1";
                            break;
                        case "Senior":
                            model.RankId = "2";
                            break;
                        case "Moderator":
                            model.RankId = "3";
                            break;
                        case "Member":
                            model.RankId = "4";
                            break;
                        case "Junior":
                            model.RankId = "5";
                            break;
                        case "Candidate":
                            model.RankId = "6";
                            break;
                    }
                }
                model.UserId = user.Id;
                model.UserFullName = user.FirstName + " " + user.LastName;
                usrList.Add(new AdminUserViewModel() { UserName = model.UserName, RankName = model.RankName, UserId = model.UserId, RankId = model.RankId, UserFullName = model.UserFullName });
                //model.RankName = null;
            }

            List<AdminRoleViewModel> rlList = new List<AdminRoleViewModel>();
            rlList.Add(new AdminRoleViewModel() { Role = "All", RoleId = "0", RoleValue = "" });
            rlList.Add(new AdminRoleViewModel() { Role = "Admin", RoleId = "1", RoleValue = "Admin" });
            rlList.Add(new AdminRoleViewModel() { Role = "Senior", RoleId = "2", RoleValue = "Senior" });
            rlList.Add(new AdminRoleViewModel() { Role = "Moderator", RoleId = "3", RoleValue = "Moderator" });
            rlList.Add(new AdminRoleViewModel() { Role = "Member", RoleId = "4", RoleValue = "Member" });
            rlList.Add(new AdminRoleViewModel() { Role = "Junior", RoleId = "5", RoleValue = "Junior" });
            rlList.Add(new AdminRoleViewModel() { Role = "Candidate", RoleId = "6", RoleValue = "Candidate" });
            rlList = rlList.OrderBy(x => x.RoleId).ToList();
            foreach (var role in rlList)
            {
                roleList.Add(new SelectListItem { Text = role.Role, Value = role.RoleValue });
            }


            if (searchString != null)
            {
                usrList = usrList.Where(x => x.UserName.Contains(searchString)).ToList();
                AdmNameSrch = searchString;
            }
            if (AdmNameSrch != null)
            {
                usrList = usrList.Where(x => x.UserName.Contains(AdmNameSrch)).ToList();
            }
            if (searchRank != null)
            {
                usrList = usrList.Where(x => x.RankName.Contains(searchRank)).ToList();
                AdmRankSrch = searchRank;
            }
            if (AdmRankSrch != null)
            {
                usrList = usrList.Where(x => x.RankName.Contains(AdmRankSrch)).ToList();
            }


            switch (sortOrder)
            {
                case "rank_desc":
                    usrList = usrList.OrderByDescending(x => x.RankId).ToList();
                    break;
                case "Username":
                    usrList = usrList.OrderBy(x => x.UserName).ToList();
                    break;
                case "username_desc":
                    usrList = usrList.OrderByDescending(x => x.UserName).ToList();
                    break;
                default:
                    usrList = usrList.OrderBy(x => x.RankId).ToList();
                    break;
            }

            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return PartialView("ShowUserDetails", usrList.ToPagedList(pageNumber, pageSize));

            
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser()
        {
            return View();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(string id, AdminEditViewModel model)
        {
            try
            {
                // TODO: Add update logic here
                var user = UserManager.FindById(id);
                model.Email = user.Email;
                var roles = await UserManager.GetRolesAsync(user.Id);
                model.UserName = user.UserName;
                foreach (var role in roles)
                {
                    model.RankName = role;
                }
                AdmUsrName = model.UserName;
                AdmUsrEmail = model.Email;
                AdmUsrRole = model.RankName;
                return RedirectToAction("EditUser");
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveUser(string id, AdminEditViewModel model)
        {
            try
            {
                // TODO: Add update logic here

                AdmUsrRole = model.RankName;
                AdmUsrName = model.UserName;
                var userid = context.Users.Where(x => x.UserName == AdmUsrName).Select(x => x.Id).FirstOrDefault();
                var user = await UserManager.FindByIdAsync(userid);
                var userRoles = await UserManager.GetRolesAsync(user.Id);
                string[] roles = new string[userRoles.Count];
                userRoles.CopyTo(roles, 0);
                await UserManager.RemoveFromRolesAsync(user.Id, roles);
                await UserManager.AddToRoleAsync(user.Id, AdmUsrRole);

                return RedirectToAction("Index", "Admin", new { Message = ManageMessageId.UserUpdated });
            }
            catch
            {
                return RedirectToAction("Index", "Admin", new { Message = ManageMessageId.Error });
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUser()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteUser(string userid)
        {
            if (AdmUsrRole == "Admin" || AdmUsrRole == "Senior")
            {
                return RedirectToAction("Index", "Admin", new { Message = ManageMessageId.HighRankedUser });
            }
            userid = context.Users.Where(x => x.UserName == AdmUsrName).Select(x => x.Id).FirstOrDefault();
            var user = await UserManager.FindByIdAsync(userid);
            var userClaims = await UserManager.GetClaimsAsync(user.Id);
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            var userLogins = await UserManager.GetLoginsAsync(user.Id);
            foreach (var claim in userClaims)
            {
                await UserManager.RemoveClaimAsync(user.Id, claim);
            }
            string[] roles = new string[userRoles.Count];
            userRoles.CopyTo(roles, 0);
            await UserManager.RemoveFromRolesAsync(user.Id, roles);
            foreach (var log in userLogins)
            {
                await UserManager.RemoveLoginAsync(user.Id, new UserLoginInfo(log.LoginProvider, log.ProviderKey));
            }
            await UserManager.DeleteAsync(user);

            return RedirectToAction("Index", "Admin", new { Message = ManageMessageId.UserDeleted });
        }


        #region Helpers
        public IEnumerable<SelectListItem> GetUserRoles(string usrrole)
        {
            var roles = context.Roles.OrderBy(x => x.Name).ToList();
            List<AdminRoleViewModel> rlList = new List<AdminRoleViewModel>();
            rlList.Add(new AdminRoleViewModel() { Role = "Admin", RoleId = "1" });
            rlList.Add(new AdminRoleViewModel() { Role = "Senior", RoleId = "2" });
            rlList.Add(new AdminRoleViewModel() { Role = "Moderator", RoleId = "3" });
            rlList.Add(new AdminRoleViewModel() { Role = "Member", RoleId = "4" });
            rlList.Add(new AdminRoleViewModel() { Role = "Junior", RoleId = "5" });
            rlList.Add(new AdminRoleViewModel() { Role = "Candidate", RoleId = "6" });
            rlList = rlList.OrderBy(x => x.RoleId).ToList();

            List<SelectListItem> roleNames = new List<SelectListItem>();
            foreach (var role in rlList)
            {
                roleNames.Add(new SelectListItem()
                {
                    Text = role.Role,
                    Value = role.Role
                });
            }
            var selectedRoleName = roleNames.FirstOrDefault(d => d.Value == usrrole);
            if (selectedRoleName != null)
                selectedRoleName.Selected = true;

            return roleNames;
        }

        public enum ManageMessageId
        {
            HighRankedUser,
            Error,
            UserDeleted,
            UserUpdated
        }

        #endregion
    }
}
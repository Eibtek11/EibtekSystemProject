using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BL;
using EibtekSystemProject.Data;
//using EibtekSystemProject.Data;
using EibtekSystemProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EibtekSystemProject.Controllers
{
    public class UserController : Controller
    {
        private readonly EibtekSystemDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(RoleManager<IdentityRole> RoleManager,UserManager<ApplicationUser> userManager, EibtekSystemDbContext db)
        {
            _userManager = userManager;

            _db = db;
            _roleManager = RoleManager;
        }

        [Authorize(Roles ="Admin")]

        public IActionResult Index()
        {
            var userList = _db.Users.ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in userList)
            {
                var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
                if (role == null)
                {
                    user.Role = "None";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
                }
            }
            return View();

        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string userId)
        {
            ViewBag.cities = _roleManager.Roles.ToList();
            var objFromDb = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            var role = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id);
            if (role != null)
            {
                objFromDb.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;
            }
            objFromDb.RoleList = _db.Roles.Select(u => new System.Web.Mvc.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(objFromDb);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            ViewBag.cities = _roleManager.Roles.ToList();
           
                var objFromDb = _db.Users.FirstOrDefault(u => u.Id == user.Id);
                if (objFromDb == null)
                {
                    return NotFound();
                }
                var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == objFromDb.Id);
                if (userRole != null)
                {
                    var previousRoleName = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();
                    //removing the old role
                    await _userManager.RemoveFromRoleAsync(objFromDb, previousRoleName);

                }

                //add new role
                await _userManager.AddToRoleAsync(objFromDb, _db.Roles.FirstOrDefault(u => u.Id == user.RoleId).Name);
                objFromDb.Name = user.Name;
                _db.SaveChanges();
                TempData[SD.Success] = "User has been edited successfully.";
                return RedirectToAction(nameof(Index));
           


            user.RoleList = _db.Roles.Select(u => new System.Web.Mvc.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(user);
        }




        [HttpPost]
        public IActionResult LockUnlock(string userId)
        {
            var objFromDb = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is locked and will remain locked untill lockoutend time
                //clicking on this action will unlock them
                objFromDb.LockoutEnd = DateTime.Now;
                TempData[SD.Success] = "User unlocked successfully.";
            }
            else
            {
                //user is not locked, and we want to lock the user
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
                TempData[SD.Success] = "User locked successfully.";
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public IActionResult Delete(string userId)
        {
            var objFromDb = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }
            _db.Users.Remove(objFromDb);
            _db.SaveChanges();
            TempData[SD.Success] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var existingUserClaims = await _userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel()
            {
                UserId = userId
            };

            foreach (Claim claim in ClaimStore.claimsList)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };
                if (existingUserClaims.Any(c => c.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel userClaimsViewModel)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userClaimsViewModel.UserId);

            if (user == null)
            {
                return NotFound();
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                TempData[SD.Error] = "Error while removing claims";
                return View(userClaimsViewModel);
            }

            result = await _userManager.AddClaimsAsync(user,
                userClaimsViewModel.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.IsSelected.ToString()))
                );

            if (!result.Succeeded)
            {
                TempData[SD.Error] = "Error while adding claims";
                return View(userClaimsViewModel);
            }

            TempData[SD.Success] = "Claims updated successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}

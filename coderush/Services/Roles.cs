using coderush.Models;
using coderush.Pages;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace coderush.Services
{
    public class Roles : IRoles
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public Roles(RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task GenerateRolesFromPagesAsync()
        {
            Type t = typeof(MainMenu);
            foreach (Type item in t.GetNestedTypes())
            {
                foreach (var itm in item.GetFields())
                {
                    if (itm.Name.Contains("RoleName"))
                    {
                        string roleName = (string)itm.GetValue(item);
                        if (!await _roleManager.RoleExistsAsync(roleName))
                            await _roleManager.CreateAsync(new ApplicationRole(roleName));
                    }
                }
            }
        }

        public async Task AddToRoles(int applicationUserId)
        {
            var user = await _userManager.FindByIdAsync(applicationUserId.ToString());
            if (user != null)
            {
                var roles = _roleManager.Roles;
                List<string> listRoles = new List<string>();
                foreach (var item in roles)
                {
                    listRoles.Add(item.Name);
                }
                await _userManager.AddToRolesAsync(user, listRoles);
            }
        }
    }
}

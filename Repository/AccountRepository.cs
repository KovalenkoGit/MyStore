using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MyStore.Data;
using MyStore.Models;

namespace MyStore.Repository
{
	public class AccountRepository : IAccountRepository
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AccountRepository(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		//public async Task<List<ApplicationUser>> GetAllAccountsAsync()
		//{
		//	return await _userManager.Users.ToListAsync();
		//}
		public async Task<List<ApplicationUser>> GetAllAccountsAsync()
		{
			var users = await _userManager.Users.ToListAsync();
			foreach (var user in users) 
			{
				var roles = await _userManager.GetRolesAsync(user);
				user.Roles = roles.ToList();
			}
			return users;
		}
	}
}

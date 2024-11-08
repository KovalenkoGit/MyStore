using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyStore.Models;

namespace MyStore.Repository
{
	public interface IAccountRepository
	{
        Task<List<ApplicationUser>> GetAllAccountsAsync();
	}
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStore.Repository;

namespace MyStore.Areas.Admin.Controllers
{
	[Area("admin")]
	[Route("admin/[controller]/[action]")]
	[Authorize(Roles = "Admin")]

	public class AccountController : Controller
    {

		private readonly IAccountRepository _accountRepository;
		
		public AccountController(IAccountRepository accountRepository)
		{
			_accountRepository = accountRepository;
		}
		[Route("~/all-account")]
		public async Task<IActionResult> GetAllAccount()
		{
			var users = await _accountRepository.GetAllAccountsAsync();
			return View(users);
		}
	}
}

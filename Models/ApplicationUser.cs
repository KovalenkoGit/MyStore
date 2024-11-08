using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStore.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime? DateOfBirth { get; set; }
		//Roles не потрібно додавати б БД
		[NotMapped]
		public List<string> Roles { get; set; } = new List<string>();
	}
}

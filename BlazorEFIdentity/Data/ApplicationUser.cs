using Microsoft.AspNetCore.Identity;
using BlazorEFIdentity.Models;
namespace BlazorEFIdentity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string? Handle { get; set; }
        public string? PersonId { get; set; } 

        public string? Adress { get; set; }
        public List<BankAccount> Account {get; set;} = new List <BankAccount>();

    }
}

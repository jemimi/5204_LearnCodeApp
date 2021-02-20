using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace _5204_LearnCodeApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class LearnCodeAppDbContext : IdentityDbContext<ApplicationUser>
    {
        public LearnCodeAppDbContext()
            : base("name=LearnCodeDataContext", throwIfV1Schema: false)
        {
        }

        public static LearnCodeAppDbContext Create()
        {
            return new LearnCodeAppDbContext();
        }

        //Instruction to set the models as tables in our database.
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Coder> Coders{ get; set; }


        //To Run the database, use code-first migrations
        //https://www.entityframeworktutorial.net/code-first/code-based-migration-in-code-first.aspx

        //Tools > NuGet Package Manager > Package Manager Console
        //enable-migrations (only once)
        //add-migration {migration name}
        //update-database

        //To View the Database Changes sequentially, go to Project/Migrations folder

        //To View Database itself, go to View > SQL Server Object Explorer
        // (localdb)\MSSQLLocalDB
        // Right Click {ProjectName}.Models.DataContext > Tables
        // Can Right Click Tables to View Data
        // Can Right Click Database to Query

        // You will have to add in some example data to the database locally
    }
}
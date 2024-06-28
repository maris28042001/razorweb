using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using razor_web.models;

namespace razor.models
{
    //razor.models.MyBlogContext
    public class MyBlogContext : IdentityDbContext<AppUser> {
        public MyBlogContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            foreach(var entytiTye in modelBuilder.Model.GetEntityTypes()){
                var tableName = entytiTye.GetTableName();
                if(tableName.StartsWith("AspNet")){
                    entytiTye.SetTableName(tableName.Substring(6));
                }
            }
        }

        public DbSet<Article> Articles { get;set; }   
    }
}

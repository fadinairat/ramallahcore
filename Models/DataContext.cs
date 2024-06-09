using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Ramallah.Models;

namespace Ramallah.Models
{
    public class DataContext : IdentityDbContext<IdentityUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //System Shared Controller Models
        public DbSet<Language> Languages { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Villages> Villages { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<GroupPermissions> GroupPermissions { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PageCategory> PagesCategories { get; set; }
        public DbSet<CategoryTypes> Category_Types { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<FilesList> FilesList { get; set; }
        public DbSet<FilesType> FileType { get; set; }
        public DbSet<HtmlTemplate> HtmlTemplates { get; set; }
        public DbSet<HtmlTemplatesType> HtmlTemplatesTypes { get; set; }
        public DbSet<AdminLog> AdminLogs { get; set; }
        public DbSet<AdminLogFor> AdminLogFor { get; set; }
        //public DbSet<AdminLogAction> AdminLogAction { get; set; }
        public DbSet<MenuLocation> MenuLocations { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagRel> TagsRel { get; set; }
        public DbSet<Ramallah.Models.Author> Author { get; set; } = default!;

        //////////////////// Lookup Tables //////////////////////
        ///
     
        public DbSet<LookupCurrencies> LookupCurrencies { get; set; }
        public DbSet<LookupCountries> LookupCountries { get; set; }
        public DbSet<Lookups> Lookups { get; set; }

		public DbSet<Suggesstions> Suggesstions { get; set; }

		//Forms Fields 
		public DbSet<Ramallah.Models.Forms> Forms { get; set; } = default!;
        public DbSet<Ramallah.Models.FormsFields> FormsFields { get; set; } = default!;
        public DbSet<Ramallah.Models.FormsFieldsOptions> FormsFieldsOptions { get; set; } = default!;
        public DbSet<Ramallah.Models.FormsFieldsTypes> FormsFieldsTypes { get; set; } = default!;
        public DbSet<Ramallah.Models.FormsEntries> FormsEntries { get; set; } = default!;
        public DbSet<Ramallah.Models.FormsEntriesFields> FormsEntriesFields { get; set; } = default!;
        public DbSet<Ramallah.Models.Visits> Visits { get; set; } = default!;
        
    }
}

using System.Data.Entity;

namespace Project.Models.Database
{
    public class AdminContext : DbContext
    { 
        public DbSet<Peoples> Peoples { get; set; }
        public DbSet<Agendas> Agendas { get; set; }
        public DbSet<AgendaUsers> AgendaUsers { get; set; }
        public DbSet<Notes> Notes { get; set; }
        public DbSet<NotesRequests> NotesRequests { get; set; }
        public AdminContext() : base("name=AdminContext")  
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }
    }
}
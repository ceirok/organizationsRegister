using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace tthk.ContactsRegistry.Data
{
    public class OrganizationsContext : DbContext
    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public OrganizationsContext(DbContextOptions<OrganizationsContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Organization");
            modelBuilder.Entity<Organization>()
                .HasMany(x => x.Employees).WithOne(x => x.Organization).HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Cascade);
        }
    }    

    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Employee> Employees { get; set; } = new Collection<Employee>();
    }
    
    public class Employee : IEntity
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual Organization Organization { get; set; }
        public TitleType? Title
        {
            get { return Title; }
            set
            {
                if(value == TitleType.Other)
                {
                    Title = value;
                    TitleOther = true;
                } else
                {
                    Title = value;                    
                }
            }            
        }
        public bool TitleOther { get; set; } = false;

        public string GetFullName()
        {
            return string.Join(" ", this.FirstName, this.LastName);
        }
    }

    public enum TitleType
    {
        CEO = 1,
        CTO = 2,
        CFO = 3,
        Developer = 4,
        Tester = 5,
        Other = 6
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace tthk.ContactsRegistry.Data
{
    public class ContactsContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactPhoneNumber> ContactPhoneNumbers { get; set; }
        public DbSet<ContactEmail> ContactEmails { get; set; }

        public ContactsContext(DbContextOptions<ContactsContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Contact");
            modelBuilder.Entity<Contact>()
                .HasMany(x => x.Emails).WithOne(x => x.Contact).HasForeignKey(x => x.ContactId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contact>()
                .HasMany(x => x.PhoneNumbers).WithOne(x => x.Contact).HasForeignKey(x => x.ContactId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class Contact
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
        public virtual ICollection<ContactPhoneNumber> PhoneNumbers { get; set; } = new Collection<ContactPhoneNumber>();
        public virtual ICollection<ContactEmail> Emails { get; set; } = new Collection<ContactEmail>();
    }

    public class ContactPhoneNumber : IEntity, IHasDefault
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public virtual Contact Contact { get; set; }

        public string Number { get; set; }

        public PhoneNumberType? Type { get; set; }
        public bool IsDefault { get; set; }
    }

    public class ContactEmail : IEntity, IHasDefault
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public virtual Contact Contact { get; set; }

        public string Email { get; set; }
        public EmailType? Type { get; set; }
        public bool IsDefault { get; set; }

    }

    public interface IEntity
    {
        Guid Id { get; set; }
    }

    public interface IHasDefault
    {
        bool IsDefault { get; set; }
    }

    public enum PhoneNumberType
    {
        Work = 1,
        Home = 2
    }

    public enum EmailType
    {
        Work = 1,
        Spam = 2,
        Fun = 3
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tthk.ContactsRegistry.Data;

namespace tthk.ContactsRegistry.Controllers
{
    public interface IContactsService
    {

    }

    public class ContactsService : IContactsService
    {

    }

    [Route("api/[controller]")]
    public class ContactsController : Controller
    {
        private readonly ContactsContext _context;

        public ContactsController(ContactsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string term)
        {
            IQueryable<Contact> seed = _context.Contacts;

            if (!string.IsNullOrEmpty(term))
            {
                seed = seed.Where(x => x.Name.Contains(term)
                    || x.Emails.Any(email => email.Email.Contains(term))
                    || x.PhoneNumbers.Any(phone => phone.Number.Contains(term))
                );
            }

            var res = await seed
                .Select(x => new
                {
                    id = x.Id,
                    name = x.Name,
                    initials = x.Initials,
                    defaultPhoneNumber = x.PhoneNumbers.OrderByDescending(phone => phone.IsDefault).FirstOrDefault().Number,
                    defaultEmail = x.Emails.OrderByDescending(email => email.IsDefault).FirstOrDefault().Email
                })
            .ToArrayAsync();

            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContact(Guid id)
        {
            var contact = await _context.Contacts
                .Include(x => x.Emails)
                .Include(x => x.PhoneNumbers)
                .FirstOrDefaultAsync(x => x.Id == id);

            return Ok(new
            {
                id = contact.Id,
                name = contact.Name,
                initials = contact.Initials,
                phoneNumbers = GetDtos(contact.PhoneNumbers),
                emails = GetDtos(contact.Emails)
            });
        }

        [HttpGet("{id}/phoneNumbers")]
        public async Task<IActionResult> PhoneNumbers(Guid id)
        {
            var phoneNumbers = await GetContactPhoneNumbers(id);
            var res = GetDtos(phoneNumbers);
            return Ok(res);
        }

        [HttpPost("{id}/phoneNumbers")]
        public async Task<IActionResult> PhoneNumbers(Guid id, [FromBody] PhoneNumberModel[] updates)
        {
            var existingPhoneNumbers = await GetContactPhoneNumbers(id);
            UpdatePhoneNumbers(id, existingPhoneNumbers, updates);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<ContactPhoneNumber[]> GetContactPhoneNumbers(Guid id)
        {
            return await _context
                .ContactPhoneNumbers
                .Where(x => x.ContactId == id)
                .ToArrayAsync();
        }

        [HttpGet("{id}/emails")]
        public async Task<IActionResult> Emails(Guid id)
        {
            var phoneNumbers = await _context
                .ContactEmails
                .Where(x => x.ContactId == id)
                .ToArrayAsync();

            var res = GetDtos(phoneNumbers);
            return Ok(res);
        }

        private IEnumerable<PhoneNumberDto> GetDtos(IEnumerable<ContactPhoneNumber> phoneNumbers)
        {
            return phoneNumbers
                .Select(x => new PhoneNumberDto
                {
                    Id = x.Id,
                    Number = x.Number,
                    IsDefault = x.IsDefault,
                    Type = x.Type.ToString()
                });
        }

        private IEnumerable<EmailDto> GetDtos(IEnumerable<ContactEmail> emails)
        {
            return emails
                .Select(x => new EmailDto
                {
                    Id = x.Id,
                    Email = x.Email,
                    IsDefault = x.IsDefault,
                    Type = x.Type.ToString()
                });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            _context.Remove(contact);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] ContactModel model)
        {
            var contact = new Contact
            {
                Name = model.Name,
                Initials = model.Initials
            };

            if (model.Emails != null)
            {
                contact.Emails = model.Emails
                    .Select(x => new ContactEmail
                    {
                        Email = x.Email,
                        Type = x.Type,
                        IsDefault = x.IsDefault
                    })
                    .ToArray();
            }

            if (model.PhoneNumbers != null)
            {
                contact.PhoneNumbers = model.PhoneNumbers
                    .Select(x => new ContactPhoneNumber
                    {
                        Number = x.Number,
                        Type = x.Type,
                        IsDefault = x.IsDefault
                    })
                    .ToArray();
            }

            _context.Add(contact);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ContactModel model)
        {
            var contact = await _context.Contacts
                .Include(x => x.Emails)
                .Include(x => x.PhoneNumbers)
                .FirstOrDefaultAsync(x => x.Id == id);

            contact.Name = model.Name;
            contact.Initials = model.Initials;

            UpdateEmails(id, contact.Emails, model.Emails);
            UpdatePhoneNumbers(id, contact.PhoneNumbers, model.PhoneNumbers);
            
            await _context.SaveChangesAsync();

            return Ok();
        }

        private void UpdatePhoneNumbers(Guid contactId, IEnumerable<ContactPhoneNumber> existingPhoneNumbers, PhoneNumberModel[] updates)
        {
            UpdateList(
                existingPhoneNumbers,
                updates,
                () =>
                {
                    return new ContactPhoneNumber { ContactId = contactId };
                },
                (phoneNumber, model) =>
                {
                    phoneNumber.Number = model.Number;
                    phoneNumber.IsDefault = model.IsDefault;
                    phoneNumber.Type = model.Type;
                });
        }

        private void UpdateEmails(Guid contactId, IEnumerable<ContactEmail> existingEmails, EmailModel[] updates)
        {
            UpdateList(
                existingEmails,
                updates,
                () =>
                {
                    return new ContactEmail { ContactId = contactId };
                },
                (contactEmail, emailModel) =>
                {
                    contactEmail.Email = emailModel.Email;
                    contactEmail.IsDefault = emailModel.IsDefault;
                    contactEmail.Type = emailModel.Type;
                });
        }


        private void UpdateList<T, TModel>(IEnumerable<T> list, IEnumerable<TModel> updates, Func<T> newEntity, Action<T, TModel> updateValues) 
            where T : class, IHasDefault, IEntity
            where TModel : IModelWithId
        {
            if (updates == null || !updates.Any()) { 
                _context.RemoveRange(list);
            }

            var existingItems = list.ToDictionary(x => x.Id);
            T defaultItem = null;
            foreach (var model in updates)
            {
                var id = model.Id;
                T item = null;
                if (id == Guid.Empty)
                {
                    item = newEntity();
                    _context.Add(item);

                }
                else if (existingItems.ContainsKey(id))
                {
                    item = existingItems[id];
                    existingItems.Remove(id);
                }

                if (item != null)
                {
                    updateValues(item, model);

                    if (item.IsDefault)
                    {
                        if (defaultItem != null)
                        {
                            defaultItem.IsDefault = false;
                        }
                        defaultItem = item;
                    }
                }
            }

            _context.RemoveRange(existingItems.Values);
        }
    }
 
    public class ContactModel
    {
        public string Name { get; set; }
        public string Initials { get; set; }

        public PhoneNumberModel[] PhoneNumbers { get; set; }
        public EmailModel[] Emails { get; set; }
    }

    public class PhoneNumberModel : IModelWithId
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public PhoneNumberType? Type { get; set; }
        public bool IsDefault { get; set; }
    }

    public class EmailModel : IModelWithId
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public EmailType? Type { get; set; }
        public bool IsDefault { get; set; }
    }

    public interface IModelWithId
    {
        Guid Id { get; set; }
    }

    public class PhoneNumberDto
    {
        public Guid Id { get; internal set; }
        public string Number { get; internal set; }
        public bool IsDefault { get; internal set; }
        public string Type { get; internal set; }
    }

    public class EmailDto
    {
        public Guid Id { get; internal set; }
        public string Email { get; internal set; }
        public bool IsDefault { get; internal set; }
        public string Type { get; internal set; }
    }
}

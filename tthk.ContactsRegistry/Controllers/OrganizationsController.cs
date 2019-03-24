using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tthk.ContactsRegistry.Data;

namespace tthk.ContactsRegistry.Controllers
{
    public interface IOrganizationsService
    {

    }

    public class OrganizationsService : IOrganizationsService
    {

    }

    [Route("api/[controller]")]
    public class OrganizationsController : Controller
    {
        private readonly OrganizationsContext _context;

        public OrganizationsController(OrganizationsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string term)
        {
            IQueryable<Organization> seed = _context.Organizations;

            if (!string.IsNullOrEmpty(term)) {
                seed = seed.Where(x => x.Name.Contains(term)
                || x.Employees.Any(emp => emp.FirstName.Contains(term) || emp.LastName.Contains(term))
                );
            }

            var res = await seed
                .Select(x => new
                {
                    id = x.Id,
                    name = x.Name,
                    ceo = x.Employees.OrderByDescending(emp => emp.Title == TitleType.CEO).FirstOrDefault().GetFullName()
                })
            .ToArrayAsync();

            return Ok(res);
        }        

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var organization = await _context.Organizations.FindAsync(id);
            _context.Remove(organization);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] OrganizationModel model)
        {
            var organization = new Organization
            {
                Name = model.Name
            };

            if (model.Employees != null)
            {
                organization.Employees = model.Employees
                    .Select(x => new Employee
                    {
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Title = x.Title,
                        TitleOther = false
                    })
                    .ToArray();
            }

            _context.Add(organization);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{orgId}/employee/{id}")]
        public async Task<IActionResult> UpdateEmployee(Guid orgId, Guid id, [FromBody] EmployeeModel[] updates)
        {
            var existingEmployees = await GetEmployees(orgId);
            UpdateEmployees(id, existingEmployees, updates);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<Employee[]> GetEmployees(Guid id)
        {
            return await _context
                .Employees
                .Where(x => x.OrganizationId == id)
                .ToArrayAsync();
        }

        private void UpdateEmployees(Guid organizationId, IEnumerable<Employee> existingEmployees, EmployeeModel[] updates)
        {
            UpdateList(
                existingEmployees,
                updates,
                () =>
                {
                    return new Employee { OrganizationId = organizationId };
                },
                (employee, employeeModel) =>
                {
                    employee.FirstName = employeeModel.FirstName;
                    employee.LastName = employeeModel.LastName;
                    employee.Title = employeeModel.Title;
                });
        }

        private void UpdateList<T, TModel>(IEnumerable<T> list, IEnumerable<TModel> updates, Func<T> newEntity, Action<T, TModel> updateValues)
            where T : class, IEntity
            where TModel : IModelWithId
        {
            if (updates == null || !updates.Any())
            {
                _context.RemoveRange(list);
            }

            var existingItems = list.ToDictionary(x => x.Id);
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
                }
            }

            _context.RemoveRange(existingItems.Values);
        }

        private IEnumerable<EmployeeDto> GetDtos(IEnumerable<Employee> employees)
        {
            return employees
                .Select(x => new EmployeeDto
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Title = x.Title.ToString()
                });
        }
    }

    public class OrganizationModel
    {
        public string Name { get; set; }

        public EmployeeModel[] Employees { get; set; }
    }

    public class EmployeeModel : IModelWithId
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public TitleType? Title { get; set; }
        public bool TitleOther { get; set; }
    }

    public class EmployeeDto
    {
        public Guid Id { get; internal set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
    }
}
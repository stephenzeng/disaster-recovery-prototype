using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StephenZeng.Prototypes.DisasterRecovery.Dal;

namespace StephenZeng.Prototypes.DisasterRecovery.Api.Controllers
{
    [ApiController]
    [Route("")]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomersController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("/api/customers")]
        public async Task<dynamic> Get()
        {
            var list = await _dbContext.Customers.Select(c => new
            {
                c.Id,
                c.FriendlyId,
                c.FirstName,
                c.Surname,
                c.Email
            }).ToListAsync();

            return list;
        }

        [HttpGet("/api/customers/{id}")]
        public async Task<dynamic> Get(Guid id)
        {
            var customer = await _dbContext.Customers.Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.FriendlyId,
                    c.FirstName,
                    c.Surname,
                    c.Email
                }).FirstOrDefaultAsync();

            return customer;
        }
    }
}
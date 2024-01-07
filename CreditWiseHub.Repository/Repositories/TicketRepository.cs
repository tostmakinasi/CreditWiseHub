using CreditWiseHub.Core.Abstractions.Repositories;
using CreditWiseHub.Core.Models;
using CreditWiseHub.Repository.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Repository.Repositories
{
    public class TicketRepository : GenericRepository<CustomerTicket, long>, ITicketRepository
    {
        public TicketRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<CustomerTicket>> GetTicketsByUsername(string username)
        {
            var tickets = await _dbSet.Include(x => x.UserApp).Where(x => x.UserApp.UserName == username).ToListAsync();
            
            return tickets;
        }

        public async Task<CustomerTicket> GetTicketsById(long ticketNumber)
        {
            var ticket = await _dbSet.Include(x => x.UserApp).Where(x => x.Id == ticketNumber).FirstOrDefaultAsync();

            return ticket;
        }
        public override async Task<IEnumerable<CustomerTicket>> GetAllAsync(Expression<Func<CustomerTicket, bool>> expression)
        {
            var tickets = await _dbSet.AsNoTracking().Where(expression).Include(x=> x.UserApp).OrderBy(x=> x.CreatedDate).ThenBy(x=> x.Priority).ToListAsync();
            return tickets;
        }

        
    }
}

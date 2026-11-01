using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payment.Grpc.Models;

namespace Payment.Grpc.Data

{
    public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
    {
        public DbSet<PayModel> Pagos => Set<PayModel>();
        //TODO terminar de definir el DB context para pagos, con sus indices y demas

    }
}

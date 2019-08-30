using AspNetCoreWebSocket.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreWebSocket.Common.DataContext
{
    public class SocketContext : DbContext
    {
        public SocketContext(DbContextOptions<SocketContext> options)
            : base(options)
        { }

        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }
    }
}
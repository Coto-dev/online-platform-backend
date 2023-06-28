using HW.Account.DAL.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HW.Account.DAL.Data; 

/// <summary>
/// Auth database context
/// </summary>
public class AccountDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>> {
    
    /// <summary>
    /// Users table
    /// </summary>
    public new DbSet<User> Users { get; set; } = null!; 
    
    /// <summary>
    /// Devices table
    /// </summary>
    public DbSet<Device> Devices { get; set; } = null!; 

    /// <inheritdoc />
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) {
    }
}
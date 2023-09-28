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
    public DbSet<EducationInfo> EducationInfos { get; set; }
    public DbSet<WorkExperienceInfo> WorkExperienceInfos { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<PhotoId> PhotoIds { get; set; }
    public DbSet<BirthDate> BirthDate { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>()
            .HasOne(u => u.BirthDate)
            .WithOne(c => c.User)
            .HasForeignKey<BirthDate>();
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.Location)
            .WithOne(c => c.User)
            .HasForeignKey<Location>();
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.WorkExperience)
            .WithOne(c => c.User)
            .HasForeignKey<WorkExperience>();
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.Education)
            .WithOne(c => c.User)
            .HasForeignKey<Education>();
        
        modelBuilder.Entity<Role>(o => {
            o.ToTable("Roles");
        });
        modelBuilder.Entity<UserRole>(o => {
            o.ToTable("UserRoles");
            o.HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            o.HasOne(x => x.User)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Devices table
    /// </summary>
    public DbSet<Device> Devices { get; set; } = null!; 

    /// <inheritdoc />
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) {
    }
}
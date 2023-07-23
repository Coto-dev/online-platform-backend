using HW.Backend.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HW.Backend.DAL.Data; 

public class BackendDbContext : DbContext {
    
    
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<ChapterComment> ChapterComments { get; set; }
    public DbSet<CorrectSequenceAnswer> CorrectSequenceAnswers { get; set; }
    public DbSet<CorrectSequenceTest> CorrectSequenceTest { get; set; }
    public DbSet<CorrectSequenceUserAnswer> CorrectSequenceUserAnswers { get; set; }
    public DbSet<DetailedAnswer> DetailedAnswers { get; set; }
    public DbSet<EducationalProgram> EducationalPrograms { get; set; }
    public DbSet<Learned> Learned { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<ModuleComment> ModuleComments { get; set; }
    public DbSet<ModuleInEducationalProgram> ModuleInEducationalPrograms { get; set; }
    public DbSet<SimpleAnswer> SimpleAnswers { get; set; }
    public DbSet<SimpleAnswerTest> SimpleAnswerTests { get; set; }
    public DbSet<SimpleUserAnswer> SimpleUserAnswers { get; set; }
    public DbSet<StreamingModule> StreamingModules { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<SubModule> SubModules { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<UserAction> UserActions { get; set; }
    public DbSet<UserAnswer> UserAnswers { get; set; }
    public DbSet<UserAnswerTest> UserAnswerTests { get; set; }
    public DbSet<UserBackend> UserBackends { get; set; }
    public DbSet<StudentModule> UserModules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Module>()
            .ToTable("ModuleCreators")
            .HasMany(m => m.Creators)
            .WithMany(t => t.CreatedModules);
        modelBuilder.Entity<Module>()
            .ToTable("ModuleTeachers")
            .HasMany(m => m.Teachers)
            .WithMany(t => t.ControlledModules);
    }

    public BackendDbContext(DbContextOptions<BackendDbContext> options) : base(options) {
    }
}
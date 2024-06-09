using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webapiemp.Models.Seeding;

namespace webapiemp.Models;

public class ApplicationDbContext : IdentityDbContext<User,Role, int>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Card>()
            .HasOne(c => c.Author)
            .WithMany(e => e.CardsAuthored)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Card>()
            .HasOne(c => c.Responder)
            .WithMany(e => e.CardsRespondedTo)
            .HasForeignKey(c => c.ResponderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Card>()
            .HasOne<Group>(c => c.Group)
            .WithMany(g => g.Cards)
            .HasForeignKey(c => c.GroupId)
            .OnDelete(DeleteBehavior.Cascade);


        ///////////////////////////////////////////


        modelBuilder.Entity<GroupMembership>()
            .HasOne<User>(g => g.User)
            .WithMany(u => u.GroupMemberships)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GroupMembership>()
            .HasOne<Group>(g => g.Group)
            .WithMany(g => g.GroupMemberships)
            .HasForeignKey(g => g.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GroupMembership>()
            .HasKey(u => new { u.UserId, u.GroupId });

        ///////////////////////////////////////////
        
        modelBuilder.Entity<Group>()
            .HasOne<User>(g => g.Author)
            .WithMany(u => u.GroupsAuthored)
            .HasForeignKey(g => g.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);


        Module3Seeding.Seed(modelBuilder);
    }

    public DbSet<Card> Cards { get; set; }
    public override DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMembership> GroupMemberships { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }


}

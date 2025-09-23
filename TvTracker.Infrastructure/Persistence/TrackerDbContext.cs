using Microsoft.EntityFrameworkCore;
using TvTracker.Domain.Entities;

namespace TvTracker.Infrastructure.Persistence;

public class TrackerDbContext : DbContext
{
    public TrackerDbContext(DbContextOptions<TrackerDbContext> options) : base(options) { }

    public DbSet<TvShow> TvShows => Set<TvShow>();
    public DbSet<Episode> Episodes => Set<Episode>();
    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<CastMember> CastMembers => Set<CastMember>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // Episode: 1-n TvShow
        b.Entity<Episode>()
            .HasOne(e => e.TvShow)
            .WithMany(s => s.Episodes)
            .HasForeignKey(e => e.TvShowId)
            .OnDelete(DeleteBehavior.Cascade);

        // CastMember: chave composta + relações
        b.Entity<CastMember>()
            .HasKey(c => new { c.TvShowId, c.ActorId });

        b.Entity<CastMember>()
            .HasOne(c => c.TvShow)
            .WithMany(s => s.Cast)
            .HasForeignKey(c => c.TvShowId);

        b.Entity<CastMember>()
            .HasOne(c => c.Actor)
            .WithMany(a => a.CastIn)
            .HasForeignKey(c => c.ActorId);

        // Favorite: chave composta + relações
        b.Entity<Favorite>()
            .HasKey(f => new { f.UserId, f.TvShowId });

        b.Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId);

        b.Entity<Favorite>()
            .HasOne(f => f.TvShow)
            .WithMany(s => s.Favorites)
            .HasForeignKey(f => f.TvShowId);

        // TvShow <-> Genre (m:n) sem entidade: EF cria join table automaticamente
        b.Entity<TvShow>()
            .HasMany(s => s.Genres)
            .WithMany(g => g.TvShows)
            .UsingEntity(j => j.ToTable("TvShowGenres"));

        // Pequenas restrições úteis
        b.Entity<TvShow>().Property(s => s.Name).HasMaxLength(200).IsRequired();
        b.Entity<Actor>().Property(a => a.Name).HasMaxLength(150).IsRequired();
        b.Entity<Genre>().Property(g => g.Name).HasMaxLength(60).IsRequired();
        b.Entity<User>().HasIndex(u => u.Email).IsUnique();


            b.Entity<TvShow>().HasIndex(s => s.ExternalId).IsUnique().HasFilter("\"ExternalId\" IS NOT NULL");
    b.Entity<Episode>().HasIndex(e => new { e.TvShowId, e.ExternalId }).IsUnique().HasFilter("\"ExternalId\" IS NOT NULL");


    }
    


}

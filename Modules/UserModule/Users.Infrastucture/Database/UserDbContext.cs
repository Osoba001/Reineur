using AuthUser.Application.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Scrypt;

namespace AuthUser.Infrastucture.Database
{
    internal class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options, DeploymentConfiguration deploymentConfiguration) : base(options)
        {
            var dbCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (!dbCreator.CanConnect())
                dbCreator.Create();

            if (!dbCreator.HasTables())
            {
                var users = new List<UserModel>
                {
                    new() { Email = deploymentConfiguration.SupperAdminEmail, Role = Role.SuperAdmin, PasswordHash = new ScryptEncoder().Encode("admin") }
                };
                dbCreator.CreateTables();


                UserTb.AddRange(users);
                SaveChangesAsync();
            }
        }

        public DbSet<UserModel> UserTb { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new UserEntityConfig().Configure(modelBuilder.Entity<UserModel>());
        }

        public async Task<ActionResponse> CompleteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                return SuccessResult();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException)
                {
                    return sqlException.Number switch
                    {
                        2601 or 2627 => BadRequestResult($"The record already exists in the database."),
                        547 => BadRequestResult("Operation failed due to a foreign key constraint violation."),
                        _ => ServerExceptionError(ex),
                    };
                }

                return ServerExceptionError(ex);
            }
            
            catch (Exception ex)
            {
                return ServerExceptionError(ex);
            }
        }

    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using AuthUser.Application.DTOs;
using AuthUser.Application.Constants;

namespace AuthUser.Infrastucture.Database
{
    [Table("users")]
    internal class UserModel
    {
        public virtual Guid Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public required Role Role { get; set; }
        public required string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime AllowSetNewPassword { get; set; }
        public DateTime RefreshTokenExpireTime { get; set; }
        public int PasswordRecoveryPin { get; set; }
        public DateTime RecoveryPinCreationTime { get; set; }

        public byte Attent { get; set; } = 0;
        public DateTime WhenToUnlock { get; set; } = DateTime.UtcNow;
        public bool IsLock { get; set; }
        public string? LockMessage { get; set; }

        public static implicit operator UserResponse(UserModel model)
        {
            return new UserResponse() { Email = model.Email, Id = model.Id, Role = model.Role.ToString() };
        }

        public static List<UserResponse> ConvertUserList(List<UserModel> list)
        {
            var resp = new List<UserResponse>();
            foreach (var user in list)
            {
                resp.Add(user);
            }
            return resp;
        }
    }

    internal class UserEntityConfig : IEntityTypeConfiguration<UserModel>
    {
        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(225);
            builder.Property(x => x.Role).IsRequired();
            builder.HasIndex(x => x.Email).IsUnique();
            builder.Property(x => x.PasswordHash).IsRequired();
        }
    }
}

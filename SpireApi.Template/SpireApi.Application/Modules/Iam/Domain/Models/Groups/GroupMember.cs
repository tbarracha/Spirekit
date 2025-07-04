using SpireApi.Application.Modules.Iam.Domain.Models.Roles;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Groups
{
    public class GroupMember : GuidEntityBy
    {
        public Guid GroupId { get; set; }
        public Group Group { get; set; } = default!;

        public Guid UserId { get; set; }
        // Optionally: public User User { get; set; } = default!;

        // Reference the Role entity
        public Guid? RoleId { get; set; }      // Nullable to allow "role-less" members if needed
        public Role? Role { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}

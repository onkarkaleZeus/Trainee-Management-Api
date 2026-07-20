using System.Collections.Frozen;

namespace TraineeManagement.Api.Utils
{
    public static class AccessDefination
    {
        public static readonly FrozenSet<string> AllowedRolesToDownload = new[] { "Admin", "Mentor"}.ToFrozenSet();

        public const string Admin = "Admin";
        public const string Mentor = "Mentor";
    }
}
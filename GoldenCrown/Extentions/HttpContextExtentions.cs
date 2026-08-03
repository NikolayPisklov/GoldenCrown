using System.Runtime.CompilerServices;

namespace GoldenCrown.Extentions
{
    public static class HttpContextExtentions
    {
        private const string UserIdKey = nameof(UserIdKey);

        public static void SetUserId(this HttpContext context, int userId)
        {
            context.Items[UserIdKey] = userId;
        }
        public static int GetUserId(this HttpContext context)
        {
            return (int)context.Items[UserIdKey]!;
        }
    }
}

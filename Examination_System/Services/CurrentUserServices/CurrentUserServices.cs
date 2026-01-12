namespace Examination_System.Services.CurrentUserServices
{
    using System.Security.Claims;

    public class CurrentUserServices : ICurrentUserServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserServices(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserIdGuid
        {
            get
            {
                var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(userIdString, out var userId) ? userId : null;
            }
        }
        public Guid UserId => UserIdGuid.Value;


        public string? UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        public bool IsInRole(string roleName) => _httpContextAccessor.HttpContext?.User?.IsInRole(roleName) ?? false;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}

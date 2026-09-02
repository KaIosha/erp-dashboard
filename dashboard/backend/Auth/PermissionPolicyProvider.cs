using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace backend.Auth
{
    public class PermissionPolicyProvider: IAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _options = options.Value;
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (string.IsNullOrWhiteSpace(policyName))
            {
                return Task.FromResult(_options.DefaultPolicy);
            }

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            return Task.FromResult(policy);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => Task.FromResult(_options.DefaultPolicy);

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => Task.FromResult(_options.FallbackPolicy);
    }
}

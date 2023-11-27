namespace WhiteEagles.WebApi.Common
{
    using System.Linq;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Mvc;

    using Data.DomainModels;

    public class CustomControllerBase : ControllerBase
    {
        protected IActionResult JsonText(string resultText)
        {
            return null;
        }

        protected UserInfo GetUserInfo()
        {
            var claims = HttpContext.User.Claims;

            var enumerable = claims as Claim[] ?? claims.ToArray();

            return new UserInfo
            {
                Code = enumerable.FirstOrDefault(x => x.Type == "code")?.Value,
                Id = enumerable.FirstOrDefault(x => x.Type == "id")?.Value,
                EMail = enumerable.FirstOrDefault(x
                    => x.Type == ClaimTypes.Email)?.Value,
                RoleCode = enumerable.FirstOrDefault(x
                    => x.Type == ClaimTypes.Role)?.Value
            };
        }
    }
}

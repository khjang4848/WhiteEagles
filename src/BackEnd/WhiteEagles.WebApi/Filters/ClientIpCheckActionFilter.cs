namespace WhiteEagles.WebApi.Filters
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    using Data.ViewModels;

    public class ClientIpCheckActionFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly string _safeList;

        public ClientIpCheckActionFilter(string safeList, ILogger logger)
        {
            _safeList = safeList;
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            _logger.LogDebug("Remote IpAddress: {RemoteIp}", remoteIp);
            var ip = _safeList.Split(';');
            var badIp = true;

            if (remoteIp.IsIPv4MappedToIPv6)
            {
                remoteIp = remoteIp.MapToIPv4();
            }

            foreach (var address in ip)
            {
                var testIp = IPAddress.Parse(address);

                if (testIp.Equals(remoteIp))
                {
                    badIp = false;
                    break;
                }
            }

            if (badIp)
            {
                _logger.LogWarning("Forbidden Request from IP: {RemoteIp}",
                    remoteIp);
                context.Result = new JsonResult(new ResponseBaseViewModel()
                {
                    ResultCode = "404",
                    ResultMessage = $"Forbidden Request from IP: {remoteIp}"

                })
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                }; 

                return;
            }

            base.OnActionExecuting(context);
        }
    }
}

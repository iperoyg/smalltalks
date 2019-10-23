using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace SmallTalks.Api.Filters
{
    public class CustomAuthenticationFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;

        public CustomAuthenticationFilter(ILogger logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authentication))
            {
                try
                {
                    byte[] data = Convert.FromBase64String(authentication.ToString());
                    string decodedString = Encoding.UTF8.GetString(data);
                    var bot = decodedString.Split(':')[0];
                    var accesskey = decodedString.Split(':')[1];
                }
                catch (Exception ex)
                {
                    //context.Result = new UnauthorizedResult();
                    _logger.Warning(ex, "[AUTH]Request with invalid {@AuthorizationHeader}!", authentication);
                }
            }
            else
            {
                _logger.Warning("[AUTH]Request without authorization header!");
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after the action executes
        }
    }
}

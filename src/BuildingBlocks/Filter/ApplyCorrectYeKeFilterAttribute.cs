using BuildingBlocks.Service;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuildingBlocks.Filter;
public sealed class ApplyCorrectYeKeFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        context?.CleanupActionStringValues(data => data.ApplyCorrectYeKe());
    }
}

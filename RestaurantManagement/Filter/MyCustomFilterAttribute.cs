using Microsoft.AspNetCore.Mvc.Filters;

namespace RestaurantManagement.Filter
{
    public class MyCustomFilterAttribute:  Attribute,IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Logic before the action executes
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Logic after the action executes
        }
    }
}

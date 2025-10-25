namespace RestaurantManagement.MiddleWares
{
    public class BusinessHoursMiddleware
    {
        private readonly RequestDelegate _next;

        public BusinessHoursMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true && context.User.IsInRole("Admin"))
            {
                await _next(context);
                return;
            }

            var currentTime = DateTime.Now.TimeOfDay;
            var start = new TimeSpan(20, 0, 0); 
            var end = new TimeSpan(8, 0, 0);  

            if (currentTime > start || currentTime < end)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("The application is only available during business hours (8 PM - 8 AM).");
            }
            else
            {
                await _next(context);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestaurantManagement.Filter
{
    public class HandleErrorAttribute: Attribute,IExceptionFilter      //IExceptionFilter emplement fron ifiltermetadata
    {
        public void OnException(ExceptionContext context)
        {
            // Handle the exception
            //دي ول طريقة بسيطة للتعامل مع الاخطاء
            //ContentResult result = new ContentResult();
            //result.Content = "An error occurred: " + context.Exception.Message;
            //context.Result = result;
            //  دي طريقة تانية ارجع فيو للتعامل مع الاخطاء
            ViewResult result = new ViewResult();
            result.ViewName = "Error"; // Name of the error view
            context.Result = result;

        }
    }
}

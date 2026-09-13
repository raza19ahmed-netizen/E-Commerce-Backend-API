using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualBasic;
using System.Net;        // isse hme Http Status code milata hai ,Not found =404, InnternalServererrror=505//
using System.Text.Json;   // ye C# object ko JSON me convert karne ke liye use ho rha hai//


namespace E_commerce.API.Helpers
{
    public class GlobalExceptionMiddleware          //API ki error ko globali handle karne wala Middlewaer//
    {
        private readonly RequestDelegate _next;     // request ko pipeline me agle component ke pass bhejo//

        public GlobalExceptionMiddleware(RequestDelegate next) 
        {
            _next = next;   // Application middleware bnate waqt .NET next deta hai hum isko store kradete hai //
        }

        public async Task InvokeAsync(HttpContext context) // ye middleware ka main method hai jab request middle tak aati hai to ye execute hota hai//
        {                                                   // context ke andar current HTTP request aur response ki information hoti hai//
            try
            {
                await _next(context); // request ko aage bhejo agar aage kahin  exception aaye to mjhe btadena //
            }                         // matlab request ko Controller/Service/Repository ki taraf bhej deta hai//
            catch (KeyNotFoundException ex) // ye KeyNotFoundException pakad raha hai//
            {
                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.NotFound, // 404 error//
                    ex.Message);             // product not found//
            }

            catch (InvalidOperationException ex)
            {
                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.Conflict,
                    ex.Message);
            }

            catch (UnauthorizedAccessException ex)
            {
                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.Unauthorized,
                    ex.Message);
            }
            catch (Exception)                     // baki unexpected exception ke liye
            {
                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.InternalServerError,  //505 error//
                    "An unexpected error occurred.");
            }

            
        }

        private static async Task HandleExceptionAsync(     // actual response banane ka kaam yaha hota hai//
            HttpContext context,
            HttpStatusCode statusCode,  // 404/500 error //
            string message)            //  product not found//
        {
            context.Response.ContentType = "application/json";  // ye browser/swagger ko btata hai "mai response JSON format me bhej rha hu"//
            context.Response.StatusCode = (int)statusCode;     // statusCode ek enum hai HttpStatusCode.NotFound iski valu 404 hai  isliye (int) set kiya//

            var response = new ApiErrorResponse((int)statusCode, message);  // yahan temporary object ban raha hai Agar 404 error hai//
            

            await context.Response.WriteAsJsonAsync(response); // ab ye C# object ko JSON me bnayega//
        }
    }
}
//  I have implement global exception handling using middleware. KeyNotFoundException is mapped to 404,duplicate/Conflict situation are mapped to 409, and unexpected exception are mapped to 500.succesful request return appropriate 200 or 201 responses//

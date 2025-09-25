using System.Text.Json;

namespace WebAPI.Middleware
{
    public class ValidateBookJsonFieldsMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidateBookJsonFieldsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Chỉ áp dụng cho POST/PUT JSON
            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
            {
                if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
                {
                    context.Request.EnableBuffering();

                    using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
                    {
                        var body = await reader.ReadToEndAsync();
                        context.Request.Body.Position = 0;

                        if (!string.IsNullOrWhiteSpace(body))
                        {
                            try
                            {
                                var jsonDoc = JsonDocument.Parse(body);
                                var root = jsonDoc.RootElement;

                                var missingFields = new List<string>();

                                // Danh sách các trường bắt buộc
                                string[] requiredFields = new string[]
                                {
                                "Title",
                                "Description",
                                "IsRead",
                                "DateRead",
                                "Rate",
                                "Genre",
                                "CoverUrl",
                                "DateAdded",
                                "PublisherID"
                                };

                                foreach (var field in requiredFields)
                                {
                                    if (!root.TryGetProperty(field, out _))
                                    {
                                        missingFields.Add(field);
                                    }
                                }

                                if (missingFields.Count > 0)
                                {
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                    await context.Response.WriteAsJsonAsync(new
                                    {
                                        message = "Missing required fields",
                                        missingFields
                                    });
                                    return; // Ngừng pipeline
                                }
                            }
                            catch (JsonException)
                            {
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                await context.Response.WriteAsJsonAsync(new
                                {
                                    message = "Invalid JSON format"
                                });
                                return;
                            }
                        }
                    }
                }
            }

            await _next(context); // Tiếp tục pipeline nếu hợp lệ
        }
    }
}

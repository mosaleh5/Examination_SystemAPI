using Examination_System.Data;

namespace Examination_System.Middleware
{
    public class TransactionMiddleware : IMiddleware
    {
        private readonly Context _context;  

        public TransactionMiddleware(Context context)
        {
            _context = context;
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            
            
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await next(httpContext);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

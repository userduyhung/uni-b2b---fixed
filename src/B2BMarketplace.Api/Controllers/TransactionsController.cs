using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        [HttpGet("history")]
        public IActionResult GetTransactionHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                // Mock transaction history
                var transactions = new List<object>
                {
                    new
                    {
                        id = Guid.NewGuid(),
                        type = "Payment",
                        amount = 150.00m,
                        description = "Order payment",
                        date = DateTime.UtcNow.AddDays(-5),
                        status = "Completed"
                    },
                    new
                    {
                        id = Guid.NewGuid(),
                        type = "Refund",
                        amount = -25.00m,
                        description = "Product return",
                        date = DateTime.UtcNow.AddDays(-2),
                        status = "Completed"
                    }
                };

                return Ok(new
                {
                    success = true,
                    message = "Transaction history retrieved successfully",
                    data = new
                    {
                        items = transactions,
                        currentPage = page,
                        pageSize = pageSize,
                        totalItems = transactions.Count,
                        totalPages = 1
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, timestamp = DateTime.UtcNow });
            }
        }
    }
}

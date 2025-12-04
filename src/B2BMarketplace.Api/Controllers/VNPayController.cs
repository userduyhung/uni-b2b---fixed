using B2BMarketplace.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace B2BMarketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public VNPayController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("create-payment")]
        public IActionResult CreatePayment([FromBody] VNPayPaymentRequest request)
        {
            try
            {
                string vnp_Returnurl = _configuration["VNPay:ReturnUrl"];
                string vnp_Url = _configuration["VNPay:Url"];
                string vnp_TmnCode = _configuration["VNPay:TmnCode"];
                string vnp_HashSecret = _configuration["VNPay:HashSecret"];

                if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
                {
                    return BadRequest(new { success = false, error = "VNPay configuration not found" });
                }

                VnPayLibrary vnpay = new VnPayLibrary();

                vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", (request.Amount * 100).ToString());

                if (!string.IsNullOrEmpty(request.BankCode))
                {
                    vnpay.AddRequestData("vnp_BankCode", request.BankCode);
                }

                vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", GetIpAddress());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", request.OrderInfo ?? $"Thanh toan don hang {request.OrderId}");
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", request.OrderId);

                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

                return Ok(new
                {
                    success = true,
                    paymentUrl = paymentUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpGet("callback")]
        public IActionResult PaymentCallback()
        {
            try
            {
                string vnp_HashSecret = _configuration["VNPay:HashSecret"];
                var vnpayData = Request.Query;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (var key in vnpayData.Keys)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, vnpayData[key]);
                    }
                }

                string vnp_SecureHash = Request.Query["vnp_SecureHash"];
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

                if (checkSignature)
                {
                    string orderId = vnpay.GetResponseData("vnp_TxnRef");
                    string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                    string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                    long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                    string vnp_TransactionNo = vnpay.GetResponseData("vnp_TransactionNo");

                    return Ok(new
                    {
                        success = true,
                        orderId = orderId,
                        responseCode = vnp_ResponseCode,
                        transactionStatus = vnp_TransactionStatus,
                        amount = vnp_Amount,
                        transactionNo = vnp_TransactionNo
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Invalid signature"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        private string GetIpAddress()
        {
            try
            {
                var ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }
                return ipAddress ?? "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }
    }

    public class VNPayPaymentRequest
    {
        public string OrderId { get; set; }
        public long Amount { get; set; }
        public string OrderInfo { get; set; }
        public string BankCode { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Paymob.Models;
using Paymob.Services;

namespace Paymob.Controllers
{
	public class HomeController : Controller
	{
		private readonly PaymentService _paymentService;
		private readonly PaymentConfiguration _options;
		public HomeController(PaymentService paymentService, IOptions<PaymentConfiguration> options)
		{
			_paymentService = paymentService;
			_options = options.Value;
		}

		public IActionResult Index()
		{
			
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> PayAction([FromBody] Payload payload)
		{

			var auth_token = await _paymentService.Pay<TokenResponse>(new
			{
				_options.api_key 
			},
			"https://accept.paymob.com/api/auth/tokens");

			var orderResponse = await _paymentService.Pay<OrderResponse>(new
			{
				auth_token = auth_token.Token,
				payload.delivery_needed,
				payload.amount_cents,
				payload.currency,
				payload.items
			}, 
			"https://accept.paymob.com/api/ecommerce/orders");

			var pageToken = await _paymentService.Pay<TokenResponse>(new
			{
				auth_token = auth_token.Token,
				payload.amount_cents,
				payload.expiration,
				_options.integration_id,
				payload.billing_data,
				order_id = orderResponse.Id,
				payload.currency
			}, 
			"https://accept.paymob.com/api/acceptance/payment_keys");

			var page = $"https://accept.paymob.com/api/acceptance/iframes/808928?payment_token={pageToken.Token}";

			return Ok(page);
		} 
	}
}
using API.Errors;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<IPaymentService> _logger;
        // This ensures that the event was genuinely sent by Stripe and was not tampered with.
        private const string WhSecret= "whsec_18a9e9f0aa002a190380db17918bd83fe3b308b4761eba33be727fae2acfb382";
        public PaymentsController(IPaymentService paymentService, ILogger<IPaymentService> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null) return BadRequest(new ApiResponse(400, "Problem with your basket"));

            return basket;
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            // Reads the entire body of the incoming HTTP request as a string
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            // Constructs a Stripe event object from the JSON and verifies the signature with the secret
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WhSecret);

            PaymentIntent intent;
            Order order;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    // This line extracts the PaymentIntent object from the event data
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    // This logs the payment intent ID, indicating that the payment has succeeded.
                    _logger.LogInformation("Payment Succeeded: ", intent.Id);
                    // Updates the order status in the database to reflect the successful payment
                    order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    _logger.LogInformation("Order updated to payment received: ", order.Id);
                    break;

                case "payment_intent.payment_failed":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Failed: ", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                    _logger.LogInformation("Payment Failed: ", order.Id);
                    break;
            }

            // Returns an empty result, indicating the webhook was successfully processed
            return new EmptyResult();
        }

    }
}

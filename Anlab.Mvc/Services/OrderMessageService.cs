using System.Threading.Tasks;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Anlab.Core.Services;
using Microsoft.Extensions.Options;

namespace AnlabMvc.Services
{
    public interface IOrderMessageService
    {
        Task EnqueueCreatedMessage(Order order);
        Task EnqueueReceivedMessage(Order order, bool bypass = false);
        Task EnqueueFinalizedMessage(Order order, bool bypass = false);
        Task EnqueuePaidMessage(Order order);
        Task EnqueueBillingMessage(Order order, string subject = "Anlab Work Order Billing Info");

    }

    public class OrderMessageService : IOrderMessageService
    {
        private readonly ViewRenderService _viewRenderService;
        private readonly IMailService _mailService;
        private readonly AppSettings _appSettings;
        private readonly EmailSettings _emailSettings;


        public OrderMessageService(ViewRenderService viewRenderService, IMailService mailService, IOptions<AppSettings> appSettings, IOptions<EmailSettings> emailSettings)
        {
            _viewRenderService = viewRenderService;
            _mailService = mailService;
            _appSettings = appSettings.Value;
            _emailSettings = emailSettings.Value;
        }
        public async Task EnqueueCreatedMessage(Order order)
        {
            var body = await _viewRenderService.RenderViewToStringAsync("Templates/_OrderCreated", order);

            var message = new MailMessage
            {
                Subject = "Work Order Confirmation",
                Body = body,
                SendTo = GetSendTo(order),
                Order = order,
                User = order.Creator,
            };

            _mailService.EnqueueMessage(message);
        }

        private string GetSendTo(Order order)
        {
            var sendTo = order.Creator.Email;
            if (!string.IsNullOrWhiteSpace(order.AdditionalEmails))
            {
                sendTo = $"{sendTo};{order.AdditionalEmails}";
            }

            return sendTo;
        }

        public async Task EnqueueReceivedMessage(Order order, bool bypass = false)
        {
            //TODO: change body of email, right now it is the same as OrderCreated
            var body = await _viewRenderService.RenderViewToStringAsync("Templates/_OrderReceived", order);

            if (bypass)
            {
                body = $"Email not sent to clients. </br> {GetSendTo(order)} </br></br></br> {body}";
            }

            var message = new MailMessage
            {
                Subject = "Work Request Confirmation",
                Body = body,
                SendTo = GetSendTo(order),
                Order = order,
                User = order.Creator,
            };

            if (bypass)
            {
                message.Subject = $"{message.Subject} -- Bypass Client";
                message.SendTo = _emailSettings.AnlabAddress;
            }

            _mailService.EnqueueMessage(message);
        }

        public async Task EnqueueFinalizedMessage(Order order, bool bypass = false)
        {
            var orderDetails = order.GetOrderDetails();
            var subject = "Work Request Finalized - Payment Pending";
            //TODO: change body of email, right now it is the same as OrderCreated
            var body = await _viewRenderService.RenderViewToStringAsync("Templates/_OrderFinalized", order);

            if (bypass)
            {
                body = $"Email not sent to clients. </br> {GetSendTo(order)} </br></br></br> {body}";
            }

            var message = new MailMessage
            {
                Subject = subject,
                Body = body,
                SendTo = GetSendTo(order),
                Order = order,
                User = order.Creator,
            };

            if (bypass)
            {
                message.Subject = $"{message.Subject} -- Bypass Client";
                message.SendTo = _emailSettings.AnlabAddress;
            }

            _mailService.EnqueueMessage(message);
        }

        public async Task EnqueuePaidMessage(Order order)
        {
            var orderDetails = order.GetOrderDetails();
            var subject = "Work Request Payment Complete";
            //TODO: change body of email, right now it is the same as OrderCreated
            var body = await _viewRenderService.RenderViewToStringAsync("Templates/_PaymentReceived", order);

            var message = new MailMessage
            {
                Subject = subject,
                Body = body,
                SendTo = GetSendTo(order),
                Order = order,
                User = order.Creator,
            };

            _mailService.EnqueueMessage(message);
        }

        public async Task EnqueueBillingMessage(Order order, string subject = "Anlab Work Request Billing Info")
        {
            var orderDetails = order.GetOrderDetails();
            //TODO: change wording, change SendTo to billing email
            var body = await _viewRenderService.RenderViewToStringAsync("Templates/_BillingInformation", order);


            var message = new MailMessage
            {
                Subject = subject,
                Body = body,
                SendTo = _appSettings.AccountsEmail,
                Order = order,
                User = order.Creator,
            };

            _mailService.EnqueueMessage(message);
        }

    }
}

using System;
using Moneybox.App.Domain.Services;

namespace Moneybox.App
{
    public class User
    {
        private INotificationService notificationService;

        public User(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public void NotifyApproachingPayInLimit()
        {
            notificationService.NotifyApproachingPayInLimit(this.Email);
        }

        public void NotifyFundsLow()
        {
            notificationService.NotifyFundsLow(this.Email);
        }
    }
}

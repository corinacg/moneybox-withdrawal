using System;
using Xunit;
using Moneybox.App;
using Moneybox.App.Domain.Services;
using Moq;

namespace Moneybox.Tests.Domain
{
    public class AccountTests
    {
        [Fact]
        public void Withdraw_BalanceLessThanAmount_InvalidOperationExceptionThrown()
        {
            Account testAccount = new Account()
            {
                Balance = 10m
            };

            Assert.Throws<InvalidOperationException>(() => testAccount.Withdraw(20m));
        }

        [Fact]
        public void Withdraw_LowFunds_NotificationServiceCalledWithCorrectEmail()
        {
            var emailAddress = "test@test.com";
            var notificationServiceMock = CreateNotificationServiceMock(emailAddress);
            Account testAccount = CreateAccount(emailAddress, 40m, notificationServiceMock.Object);

            testAccount.Withdraw(20m);

            notificationServiceMock.Verify(s => s.NotifyFundsLow(It.Is<string>(e => e == emailAddress)), Times.Once);
        }

        [Fact]
        public void Withdraw_OperationCanExecute_BalanceCorrectlyUpdated()
        {
            var emailAddress = "test@test.com";
            var testAccount = CreateAccount(emailAddress, 40m);

            testAccount.Withdraw(20m);

            Assert.Equal(20m, testAccount.Balance);
        }

        [Fact]
        public void Withdraw_OperationCanExecute_WithdrawnCorrectlyUpdated()
        {
            var emailAddress = "test@test.com";
            var testAccount = CreateAccount(emailAddress, 40m);
            testAccount.Withdrawn = -20m;

            testAccount.Withdraw(20m);

            Assert.Equal(-40m, testAccount.Withdrawn);
        }

        [Fact]
        public void PayIn_PaidInLimitExceeded_InvalidOperationExceptionThrown()
        {
            Account testAccount = new Account()
            {
                PaidIn = 3800m
            };

            Assert.Throws<InvalidOperationException>(() => testAccount.PayIn(500m));
        }

        [Fact]
        public void PayIn_PaidInLimitProximityReached_NotificationServiceCalledWithCorrectEmail()
        {
            var emailAddress = "test@test.com";
            var notificationServiceMock = CreateNotificationServiceMock(emailAddress);
            Account testAccount = CreateAccount(emailAddress, 40m, notificationServiceMock.Object);
            testAccount.PaidIn = 3300m;

            testAccount.PayIn(400m);

            notificationServiceMock.Verify(s => s.NotifyApproachingPayInLimit(It.Is<string>(e => e == emailAddress)), Times.Once);
        }

        [Fact]
        public void PayIn_OperationCanExecute_BalanceCorrectlyUpdated()
        {
            var emailAddress = "test@test.com";
            var testAccount = CreateAccount(emailAddress, 900m);

            testAccount.PayIn(20m);

            Assert.Equal(920m, testAccount.Balance);
        }

        [Fact]
        public void PayIn_OperationCanExecute_PaidInCorrectlyUpdated()
        {
            var emailAddress = "test@test.com";
            var testAccount = CreateAccount(emailAddress, 900m);
            testAccount.PaidIn = 200m;

            testAccount.PayIn(20m);

            Assert.Equal(220m, testAccount.PaidIn);
        }
        private Account CreateAccount(string userEmail, decimal balanceValue, INotificationService notificationService = null)
        {
            return new Account()
            {
                Balance = balanceValue,
                User = new User(notificationService?? CreateNotificationServiceMock(userEmail).Object)
                {
                    Email = userEmail
                }
            };
        }

        private Mock<INotificationService> CreateNotificationServiceMock(string emailAddress)
        {
            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(s => s.NotifyFundsLow(It.Is<string>(e => e == emailAddress)));
            notificationServiceMock.Setup(s => s.NotifyApproachingPayInLimit(It.Is<string>(e => e == emailAddress)));

            return notificationServiceMock;
        }
    }
}

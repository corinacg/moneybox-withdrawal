using System;
using Xunit;
using Moq;
using Moneybox.App;
using Moneybox.App.Features;

namespace Moneybox.Tests.Features
{
    public class TransferMoneyTests
    {
        [Fact]
        public void Execute_OperationIsSuccessful_FromAccountCorrectlyUpdated()
        {
            Account fromAccount = new Account()
            {
                Balance = 1000m,
                Withdrawn = -100m,
                Id = Guid.NewGuid()
            };

            Account toAccount = new Account()
            {
                Balance = 1000m,
                Id = Guid.NewGuid()
            };

            var repositoryMock = TestHelpers.CreateRepositoryMock(new[] { fromAccount, toAccount });
            var transferMoneyService = new TransferMoney(repositoryMock.Object);

            transferMoneyService.Execute(fromAccount.Id, toAccount.Id, 20m);

            repositoryMock.Verify(r => r.Update(
                It.Is<Account>(a => a.Id == fromAccount.Id
                               && a.Balance == 980m
                               && a.Withdrawn == -120m)), Times.Once);

        }

        [Fact]
        public void Execute_OperationIsSuccessful_ToAccountCorrectlyUpdated()
        {
            Account fromAccount = new Account()
            {
                Balance = 1000m,
                Id = Guid.NewGuid()
            };

            Account toAccount = new Account()
            {
                Balance = 1000m,
                PaidIn = 200m,
                Id = Guid.NewGuid()
            };

            var repositoryMock = TestHelpers.CreateRepositoryMock(new[] { fromAccount, toAccount });
            var transferMoneyService = new TransferMoney(repositoryMock.Object);

            transferMoneyService.Execute(fromAccount.Id, toAccount.Id, 20m);

            repositoryMock.Verify(r => r.Update(
                It.Is<Account>(a => a.Id == toAccount.Id
                               && a.Balance == 1020m
                               && a.PaidIn == 220m)), Times.Once);

        }

        [Fact]
        public void Execute_OperationIsNotSuccessful_ExceptionThrownAccountNotUpdated()
        {
            Account fromAccount = new Account()
            {
                Balance = 1000m,
                Withdrawn = -100m,
                Id = Guid.NewGuid()
            };

            Account toAccount = new Account()
            {
                Balance = 1000m,
                Id = Guid.NewGuid()
            };

            var repositoryMock = TestHelpers.CreateRepositoryMock(new[] { fromAccount, toAccount });
            var transferMoneyService = new TransferMoney(repositoryMock.Object);

            Assert.Throws<InvalidOperationException>(() => transferMoneyService.Execute(fromAccount.Id, toAccount.Id, 2000m));

            repositoryMock.Verify(r => r.Update(It.Is<Account>(a => a.Id == fromAccount.Id)), Times.Never);
            repositoryMock.Verify(r => r.Update(It.Is<Account>(a => a.Id == toAccount.Id)), Times.Never);

        }
    }
}

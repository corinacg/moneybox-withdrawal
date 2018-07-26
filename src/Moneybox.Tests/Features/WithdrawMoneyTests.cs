using System;
using Xunit;
using Moq;
using Moneybox.App;
using Moneybox.App.Features;

namespace Moneybox.Tests.Features
{
    public class WithdrawMoneyTests
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

            var repositoryMock = TestHelpers.CreateRepositoryMock(new[] { fromAccount });
            var withdrawMoneyService = new WithdrawMoney(repositoryMock.Object);

            withdrawMoneyService.Execute(fromAccount.Id, 20m);

            repositoryMock.Verify(r => r.Update(
                It.Is<Account>(a => a.Id == fromAccount.Id
                               && a.Balance == 980m
                               && a.Withdrawn == -120m)), Times.Once);

        }

        [Fact]
        public void Execute_OperationIsNotSuccessful_ExceptionThrownAccountNotUpdated()
        {
            Account fromAccount = new Account()
            {
                Balance = 100,
                Withdrawn = -100m,
                Id = Guid.NewGuid()
            };

            var repositoryMock = TestHelpers.CreateRepositoryMock(new[] { fromAccount });
            var withdrawMoneyService = new WithdrawMoney(repositoryMock.Object);

            Assert.Throws<InvalidOperationException>(() => withdrawMoneyService.Execute(fromAccount.Id, 200m));

            repositoryMock.Verify(r => r.Update(It.Is<Account>(a => a.Id == fromAccount.Id)), Times.Never);

        }
    }
}

using System;
using System.Collections.Generic;
using Moq;
using Moneybox.App.DataAccess;
using Moneybox.App;

namespace Moneybox.Tests.Features
{
    public static class TestHelpers
    {
        public static Mock<IAccountRepository> CreateRepositoryMock(IEnumerable<Account> accounts)
        {
            var repositoryMock = new Mock<IAccountRepository>();

            foreach (var item in accounts)
            {
                repositoryMock.Setup(r => r.GetAccountById(It.Is<Guid>(id => id == item.Id))).Returns(item);
            }

            return repositoryMock;
        }
    }
}

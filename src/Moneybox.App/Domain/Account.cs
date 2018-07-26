using System;

namespace Moneybox.App
{
    public class Account
    {
        private const decimal payInLimit = 4000m;
        private const decimal withdrawLimit = 0m;
        private const decimal lowFundsLimit = 500m;
        private const decimal payInProximity = 500m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public void Withdraw(decimal amount)
        {
            var fromBalance = Balance - amount;

            if (fromBalance < withdrawLimit)
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }

            if(fromBalance < lowFundsLimit)
            {
                User.NotifyFundsLow();
            }

            this.Balance = fromBalance;
            this.Withdrawn -= amount;
        }

        public void PayIn(decimal amount)
        {
            var paidIn = PaidIn + amount;

            if (paidIn > payInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            if (payInLimit - paidIn < payInProximity)
            {
               User.NotifyApproachingPayInLimit();
            }

            Balance += amount;
            PaidIn = paidIn;
        }
    }
}

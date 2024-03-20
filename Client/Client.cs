using System;
using System.Collections.Generic;
using System.Text;

namespace bank
{
    public class Client
    {
        private string name;
        private string surname;
        private Account clientAccount;

        public Client()
        {
        }

        public Client(string n, string s, Account a)
        {
            this.name = n;
            this.surname = s;
            this.clientAccount = a;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public string Surname
        {
            get
            {
                return this.surname;
            }
            set
            {
                this.surname = value;
            }
        }

        public Account ClientAccount
        {
            get
            {
                return this.clientAccount;
            }
            set
            {
                this.clientAccount = value;
            }
        }

        public void TransferFundsTo(Client receiver, float amount)
        {
            this.ClientAccount.TransferFunds(receiver.ClientAccount, amount);
        }
    }
}

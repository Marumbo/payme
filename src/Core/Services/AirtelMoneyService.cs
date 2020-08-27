using System;
using System.Text.RegularExpressions;
using Core.Entities;

namespace Core.Services
{
    public class AirtelMoneyService : IPaymentService
    {
        public readonly string _message;
        public readonly string _phoneNumber;

        public AirtelMoneyService(string message, string phoneNumber)
        {
            _message = IPaymentService.SanitizeMessage(message);
            _phoneNumber = phoneNumber;
        }

        public Payment GeneratePayment()
        {
            var amountRegex = new Regex("((?<=(Amount: )|(Amt: ))(.*?)(?=M))|((?<=(recieved ))(.*?)(?=M))");
            var referenceRegex = new Regex(@"(?<=Ref: )(.*?)(?=\s)");
            var bankNameRegex = new Regex("(?<=from )(.*?)(?= on)");
            var fromAgentRegex = new Regex("(Cash In)");
            var amount = Decimal.Parse(amountRegex.Match(_message).ToString().Trim());
            var reference = referenceRegex.Match(_message).ToString();
            var fromAgent = fromAgentRegex.IsMatch(_message);
            var BankName = bankNameRegex.Match(_message).ToString();

            return new Payment()
            {
                Amount = amount,
                PhoneNumber = _phoneNumber,
                FromAgent = fromAgent,
                Reference = reference,
                BankName = IPaymentService.GetBankNameFromString(BankName),
                ProviderName = Provider.Mpamba
            };
        }

        public bool HasInvalidReference()
        {
            throw new System.NotImplementedException();
        }

        public bool IsDeposit()
        {
                      /* sample messages for the regex below
                        var message = @" Trans ID: CI200808.0942.H11201: you have received MK20000.00 from BT59687, CHIMWEMWE MUGHOGHO. your new balance is MK20020.41.Note that Cash In is free of charge";
                        var message2 = "trans.ID :  PP200705.1009.H42097. Dear customer, you have received MK 6000.00 from 999025907,PRINCE CHIKWEBA . Your available balance is MK 6020.41.Trans id";
                        var message3 = "Dear Customer, money transfer to 999057130, ELIZABETH JERE is successful, Trans Id: PP200815.0822.G60250,Trans Amt: MK2000.00. Your available balance MK110.41";

                        */
                        
                        var findFirst = new Regex(@"(^( [tT]rans ID: |[tT]rans ID: |[Tt]rans.ID|Trans Id:|Dear Customer))");
                        var airtimeRegex = new Regex(@"[aA]irtime");
                        var findReceived = new Regex(@"(\b(money transfer)|(you have received)\b)");
                        var transIdRegex = new Regex(@"(?<=([tT]rans.ID :)|([tT]rans ID:|Trans Id:))(.*?)(?=([dD]|[yY]|:|,))");
                        var amountRegex = new Regex(@"((?<=(Amt: MK |received MK|Trans Amt: MK))(.*?)(?=\.|[yY]|f))");
                        var senderRegex = new Regex(@"(?<=(from |to ))(.*?)(?=\.|is)");
                       

                        var firstWord = findFirst.Match(_message).ToString().ToLower();
                        var receivedMessage = findReceived.Match(_message).ToString();
                        var airtime = airtimeRegex.Match(_message).ToString();
                        var transId = transIdRegex.Match(_message).ToString().Trim();
                        var amount = amountRegex.Match(_message).ToString().Trim();
                        var sender = senderRegex.Match(_message).ToString().Trim();
                        var numberName = sender.Split(",");

                        var number = numberName[0];
                        var name = numberName[1].Trim();
                        
                        /* simple deposit for airtel check for trans at begining or dear customer and you have received or monrey transfer to Dali's or airtel money account number
                        other checks for amount and sender not necessary at this stage, 
                        */


                        /*test for airtel trans id and dear customer only in airtel messages and no airtime */
                         
                        if(firstWord != "" && receivedMessage != "" && airtime == "")
                        {
                            return true;
                        }
                        else 
                            return false;

        }
    }
}
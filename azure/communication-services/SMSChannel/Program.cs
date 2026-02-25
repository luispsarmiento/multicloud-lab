using System;
using System.Collections.Generic;

using Azure;
using Azure.Communication;
using Azure.Communication.Sms;

string connectionString = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING");
string toPhoneNumber = Environment.GetEnvironmentVariable("TO_PHONE_NUMBER");
string partnerApiKey = Environment.GetEnvironmentVariable("PARTNER_API_KEY");

SmsClient smsClient = new SmsClient(connectionString);
// Send SMS with Messaging Connect
SmsSendResult sendResult = smsClient.Send(
    from: "ATSMSTest",
    to: toPhoneNumber,
    message: "Hello World via SMS",
    options: new SmsSendOptions(enableDeliveryReport: true)
    {
        Tag = "OTP",
        MessagingConnect = new MessagingConnectOptions(partnerApiKey, "infobip")
    });

Console.WriteLine($"Sms id: {sendResult.MessageId}");
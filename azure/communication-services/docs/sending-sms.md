# Sending SMS with Azure Communication Services

This guide explains how to send SMS messages using the `SMSChannel` console application, which leverages the Azure Communication Services SDK and Infobip as a messaging provider.

## Prerequisites

Before running the application, ensure you have:
1. An **Azure Communication Services** resource.
2. An active **Phone Number** associated with your resource.
3. A **Partner API Key** (e.g., from Infobip).

## Configuration

The application requires the following environment variables to be set:

| Variable | Description |
| :--- | :--- |
| `COMMUNICATION_SERVICES_CONNECTION_STRING` | The connection string for your Azure Communication Services resource. |
| `TO_PHONE_NUMBER` | The recipient's phone number in E.164 format. |
| `PARTNER_API_KEY` | The API key provided by the messaging partner. |

## How to Run

To execute the console application with the required configurations, use the following PowerShell command:

```powershell
. {
dotnet run -e "COMMUNICATION_SERVICES_CONNECTION_STRING=<connection-string-value>" `
    -e "TO_PHONE_NUMBER=<to-phone-number-value>" `
    -e "PARTNER_API_KEY=<partner-api-key-value>"
}
```

## Implementation Details

The core logic uses the `SmsClient` from the `Azure.Communication.Sms` package. It specifies `MessagingConnectOptions` to route the message through the specified partner.

```csharp
SmsClient smsClient = new SmsClient(connectionString);
SmsSendResult sendResult = smsClient.Send(
    from: "ATSMSTest",
    to: toPhoneNumber,
    message: "Hello World via SMS",
    options: new SmsSendOptions(enableDeliveryReport: true)
    {
        Tag = "OTP",
        MessagingConnect = new MessagingConnectOptions(partnerApiKey, "infobip")
    });
```

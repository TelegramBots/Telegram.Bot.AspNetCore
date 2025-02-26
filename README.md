# Telegram.Bot support for ASP.NET Core

When using Telegram.Bot v22.5 or later, you need to add this package to your project in order to be able to call:
```csharp
builder.Services.ConfigureTelegramBotMvc();
````

This above line configures your ASP.NET Core WebAPI project to be able to receive Telegram updates.

It ensures the best compatibility with your other Controllers endpoints if they need different JsonNamingPolicy than the SnakeCaseLower used by `JsonBotAPI`.

> Starting with Telegram.Bot v22.5, we moved method `ConfigureTelegramBotMvc` into this package so as to avoid Telegram.Bot dependency on Microsoft.AspNetCore framework

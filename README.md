# Azure OpenAI SSE Audit/Log Proxy

A small demo to show inline logging of request & response for both streaming and non-streaming chat completion endpoints.

## Getting Started:
1. Update the `AOAI_BASEADDRESS` value in [appsettings.json](appsettings.json) to point to your Azure OpenAI endpoint. Eg: `https://example123.openai.azure.com/`
2. In your consumer code, point it towards this instead of directly to Azure OpenAI. Eg:
```
var client = new OpenAIClient(new Uri("https://localhost:7027/"), new AzureKeyCredential("..."));
```
3. Run the console app, and try calling both `GetChatCompletionsAsync` and `GetChatCompletionsStreamingAsync`, you should see logs appear for both including request and response. (For streamed responses you will see the individual chunks)
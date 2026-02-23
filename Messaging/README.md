# Messaging Project

A .NET Standard library providing RabbitMQ messaging capabilities for Fusion P8 custom activities and integrations.

## Overview

The `Messaging` project provides abstracted message publishing and consumption patterns for RabbitMQ-based event driven workflows. It handles connection management, queue configuration, and automatic retry/dead-letter queue setup.

## Target Framework

- **.NET Standard 2.0** - Compatible with .NET Framework 4.7.2+ and .NET Core/.NET 5+

## Dependencies

- **RabbitMQ.Client** (v6.8.1) - Official RabbitMQ .NET client library
- **System.Configuration.ConfigurationManager** (v10.0.1) - Configuration support
- **Logging** - Internal logging library

## Core Classes

### RabbitMQMessageSubmission

Publishes messages to RabbitMQ queues with built-in retry and dead-letter queue handling.

```csharp
// Constructor
var submission = new RabbitMQMessageSubmission(
    rabbitMQHost: "localhost",
    rabbitMQPort: 5672,
    rabbitMQUsername: "guest",
    rabbitMQPassword: "guest",
    rabbitMQVirtualHost: "/",
    queueName: "fusion.doc.events"  // optional, defaults to "fusion.doc.events"
);

// Publish a message
submission.PostMessage("fusion.doc.events", "my-message-content");
```

**Features:**
- Publisher confirms enabled for message acknowledgment
- Automatic queue declaration with durability
- Dead-letter exchange configured for failed messages
- Retry queue with 30-second TTL

**Queue Structure:**
- `{queueName}` - Main queue
- `{queueName}.retry` - Retry queue (30 second TTL)
- `{queueName}.dlq` - Dead-letter queue

### RabbitMQMessageReceipt

Consumes messages from RabbitMQ queues with event-driven processing.

```csharp
// Constructor - reads config from app.config
var receipt = new RabbitMQMessageReceipt(queueName: "fusion.doc.events");

// Set up event handler
receipt.MessageReceived += (message) =>
{
    Console.WriteLine($"Received: {message}");
};

// Start listening
receipt.StartConsumer();

// Stop when done
receipt.StopConsumer();
```

**Features:**
- Configuration-driven (reads from `app.config` or `web.config`)
- Event-based message processing
- Automatic message acknowledgment
- Single message dispatch (QoS = 1)
- Exception handling with dead-letter queueing on failure

## Configuration

### Configuration Settings (app.config / web.config)

```xml
<configuration>
  <appSettings>
    <add key="RabbitMQ:Host" value="localhost" />
    <add key="RabbitMQ:Port" value="5672" />
    <add key="RabbitMQ:Username" value="guest" />
    <add key="RabbitMQ:Password" value="guest" />
    <add key="RabbitMQ:VirtualHost" value="/" />
  </appSettings>
</configuration>
```

### For Fusion P8 Custom Activities

Activity parameters:
- `RabbitMQ:Host` - RabbitMQ server hostname
- `RabbitMQ:Port` - RabbitMQ port (typically 5672)
- `RabbitMQ:Username` - RabbitMQ username
- `RabbitMQ:Password` - RabbitMQ password
- `RabbitMQ:VirtualHost` - RabbitMQ virtual host (e.g., `/`)
- `QueueName` - Target queue name

## Usage Examples

### Example 1: Publishing Messages in a Custom Activity

```csharp
using Messaging;

public class DocumentEventActivity : IActivity
{
    public void Do(ActivityExecutionContext context)
    {
        var submission = new RabbitMQMessageSubmission(
            (string)context.Parameters["RabbitMQ:Host"],
            (int)context.Parameters["RabbitMQ:Port"],
            (string)context.Parameters["RabbitMQ:Username"],
            (string)context.Parameters["RabbitMQ:Password"],
            (string)context.Parameters["RabbitMQ:VirtualHost"],
            (string)context.Parameters["QueueName"]
        );

        var document = context.LifecycleObject as IDocument;
        var eventPayload = $"{{\"action\": \"processed\", \"docId\": \"{document.Id}\"}}";
        
        submission.PostMessage((string)context.Parameters["QueueName"], eventPayload);
    }
}
```

### Example 2: Consuming Messages in a Service

```csharp
using Messaging;

public class DocumentProcessor
{
    private RabbitMQMessageReceipt _consumer;

    public void StartProcessing()
    {
        _consumer = new RabbitMQMessageReceipt("fusion.doc.events");
        _consumer.MessageReceived += OnMessageReceived;
        _consumer.StartConsumer();
    }

    private void OnMessageReceived(string message)
    {
        try
        {
            // Parse and process the message
            var docEvent = JsonConvert.DeserializeObject<DocumentEvent>(message);
            ProcessDocument(docEvent);
        }
        catch (Exception ex)
        {
            // Exception logged, message sent to dead-letter queue
        }
    }

    public void StopProcessing()
    {
        _consumer?.StopConsumer();
    }
}
```

## Testing

Unit tests are located in `RabbitMQTests` project.

```bash
dotnet test RabbitMQTests
```

## Queue Architecture

```
Main Queue (fusion.doc.events)
    ↓
    ├─→ [Successfully Processed] ✓
    └─→ [Failed/Nack]
         ↓
    Retry Queue (fusion.doc.events.retry)
         ├─→ [30 second TTL expires]
         ├─→ [Redelivered to Main Queue]
         │    ├─→ [Successfully Processed] ✓
         │    └─→ [Failed Again]
         │         ↓
         └─→ Dead Letter Queue (fusion.doc.events.dlq)
              [Manual review required]
```

## Related Projects

- **PushToRabbitMQActivity** - Fusion P8 custom activity that publishes document events
- **RabbitMQTests** - Unit tests for messaging functionality
- **Logging** - Logging abstraction used by messaging

## Troubleshooting

### Connection Issues

- Verify RabbitMQ server is running
- Check hostname, port, and credentials
- Ensure firewall allows port 5672 (or your custom port)

### Messages Not Being Consumed

- Verify `app.config` contains correct RabbitMQ settings
- Check that `StartConsumer()` is called
- Review Logging output for exceptions

### Messages in Dead-Letter Queue

- Check exception logs in Logging
- Verify message format matches expected schema
- Review `OnMessageReceived` event handler logic

## License

See repository LICENSE file.

## Support

For issues or questions, contact the development team or create an issue in the repository.

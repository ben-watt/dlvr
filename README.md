# Delivery Boy

Send and receive asynchronous messages without the fuss 📬🚴📭

## Why?

Messaging can be a pain, it adds additional cognitive load to the already labored mind of developers. Delivery Boy attempts to make things a little easier by providing a simple HTTP abstraction for asynchronous messaging.

Also, it provides a couple of additional benefits:

- ⚡ Handles all retry logic
- 📤 Implements the Outbox pattern
- 🧪 Improves application testability

## Usage

The easiest way to get started is to create a `config.yaml` file which has a connection string for a given provider

```yaml
message_providers:
  - type: service_bus
    name: sb
    connection_string: <connection-string>
```

Then fire it up 🚀

```sh
docker run -p 8080:80 -v ./config.yaml:/app/messaging_config/config.yaml -t wattcode/message-sidecar:latest

# You can now hit the service and publish messages to any topic (as long as it actually exists)
# curl http://localhost:8080/{message_provider_name}/{topic_name}
curl http://localhost:8080/sb/some-topic
```

### Subscriptions

You can also tell Delivery Boy that you want to listen to messages on a given subscription by tweaking the config file slightly, to something like this:

```yaml
message_providers:
  - type: service_bus
    name: sb
    connection_string: <connection_string>
    subscriptions:
      - name: hello
        topic_name: hello-topic
        handler_name: default
        handler_args:
          endpoint: /test

handlers:
  - type: http
    name: default
    base_uri: https://localhost/my-service
    port: 8000
    retry_policy: exponential
```

Which states that your application wants to listen to a subscription named `hello` on the topic `hello-topic` and with a `http` handler named `default` and publish the messages to `https://localhost/my-service/test`

## Providers

| name | type | status |
|---|---|---|
| Service Bus Topics | `service_bus` | ✅ |

## To Do

- Create other persistence providers
- Support other message providers
- More test coverage
  - Service Bus provider
  - Background service tests
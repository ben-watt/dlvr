This service is designed to be injected into a kubernetes service and handle all messaging concerns. Allowing the core application and business logic to remain a simple Http or GRPC application.

To Do:

- Integration tests to ensure routing is correct.
- End to end test with k6 using service bus + in memory
- Load config from an environment
  - Subscriptions
  - Connection String
- Create terraform scripts to create and tear down azure service bus
- Example small application
- Deploy to container registry
- How to get config into the application?
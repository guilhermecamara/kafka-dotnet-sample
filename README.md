# Kafka .NET Sample

This project is a sample implementation of Kafka consumer and publisher using .NET Minimal API. It demonstrates how to:
- Publish events to Kafka topics
- Consume events from Kafka topics
- Track processed events in-memory
- Expose endpoints for login, logout, purchase, and event retrieval
- Integrate health checks for both the webapp and Kafka

## Monorepo Structure
This is an Nx monorepo, organizing multiple .NET and JavaScript projects for scalable development and testing.

## Build & Test Instructions

1. **Install dependencies:**
   ```sh
   pnpm install
   ```
2. **Build all projects:**
   ```sh
   pnpm build
   ```
3. **Run tests:**
   ```sh
   pnpm test
   ```

## Kafka Configuration
Kafka is an external resource and must be available for the application to connect and function correctly. Set the following environment variable before running the webapp:

```
KAFKA_BOOTSTRAP_SERVERS=<your-kafka-broker:9092>
```

Replace `<your-kafka-broker:9092>` with the address of your Kafka broker.

## Endpoints
- `/login` - User login (publishes to Kafka)
- `/logout` - User logout (publishes to Kafka)
- `/purchase` - User purchase (publishes to Kafka)
- `/events` - Returns all processed Kafka events so far
- `/health` - Health check for webapp and Kafka connectivity

## Notes
- The project uses .NET Minimal API for simplicity and performance.
- Kafka integration is handled via Confluent.Kafka.
- All event processing is tracked in-memory for demonstration purposes.
- Health checks ensure both the webapp and Kafka are reachable.

---
MIT License


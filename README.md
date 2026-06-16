# EFlow-Commerce

EventFlow Commerce is a production-inspired, event-driven order processing platform built with .NET.

The project demonstrates backend engineering practices such as distributed messaging, transactional outbox, optimistic concurrency, integration testing, observability, and containerized local development.

## Current status

Milestone 1 completed:
- Orders API
- Create order
- Get order by id
- PostgreSQL persistence
- EF Core migrations
- Vertical Slice structure
- FluentValidation via decorator pipeline
- Integration tests with Testcontainers
- DB cleanup with Respawn

Milestone 2 completed:
- Catalog API
- Product model
- Product lookup
- Validate products before order creation

## Roadmap

Milestone 3:
- RabbitMQ.
- MassTransit.
- Integration events.
- Payments.Worker.
- OrderCreated event.
- PaymentSucceeded event.
- PaymentFailed event.
- Orders.API reacts to payment result events.

Milestone 4:
- OutboxMessages table.
- Save integration events in DB transaction.
- Background outbox processor.
- Reliable event publishing.
- Retry behavior.
- Idempotent event handling.

Milestone 5:
- Structured logging.
- Correlation IDs.
- OpenTelemetry traces.
- Health checks.
- Metrics.
- Docker Compose improvements.
- GitHub Actions CI.

## Docker Run

Build and start all services (`docker-compose.yml` in "deploy" directory):

```bash
docker compose up -d --build
```

### Open:

Order.API (on `5001` port by default):

```text
http://localhost:5001
```

Open Swagger page (if `ASPNETCORE_ENVIRONMENT` is "Development"):

```text
http://localhost:5001/swagger
```

Catalog.API (on `5002` port by default)

```text
http://localhost:5002
```

Open Swagger page (also like Order.API available if `ASPNETCORE_ENVIRONMENT` is "Development":

```text
http://localhost:5002/swagger
```

# EFlow-Commerce

EventFlow Commerce is a production-inspired, event-driven order processing platform built with .NET.

The project demonstrates backend engineering practices such as distributed messaging, transactional outbox, optimistic concurrency, integration testing, observability, and containerized local development.

## Current status

### Completed: Milestone 1 — Orders API foundation

* Orders API
* Create order endpoint
* Get order by id endpoint
* PostgreSQL persistence
* EF Core migrations
* Vertical Slice structure
* FluentValidation via decorator pipeline
* Integration tests with Testcontainers
* Database cleanup with Respawn

### Completed: Milestone 2 — Catalog API and product validation

* Catalog API
* Product model
* Create product endpoint
* Get product by id endpoint
* Separate Catalog database
* Orders API validates product existence through Catalog API
* Orders API uses product price from Catalog instead of trusting client input
* Integration tests for order creation scenarios

### Completed: Milestone 3 — Asynchronous payments

* RabbitMQ message broker
* MassTransit messaging
* Shared integration contracts project
* `OrderCreatedIntegrationEvent`
* `PaymentSucceededIntegrationEvent`
* `PaymentFailedIntegrationEvent`
* `Payments.Worker`
* Orders API publishes `OrderCreatedIntegrationEvent`
* Payments Worker consumes order-created events
* Payments Worker simulates payment processing
* Payments Worker publishes payment success/failure events
* Orders API consumes payment result events
* Order status flow:

  * `Pending`
  * `PaymentProcessing`
  * `Paid`
  * `PaymentFailed`
  * `Cancelled`
* Payment failure reason is stored on failed orders
* Consumer tests with MassTransit TestHarness
* Integration tests for payment result handling

## Architecture overview

The solution currently contains three runtime services and one shared contracts project:

```text
src/
├── Orders.API
├── Catalog.API
├── Payment.Worker
└── Eflow.Contracts
```

### Orders.API

Responsible for order management.

Main responsibilities:

* Create orders
* Get order details
* Validate products through Catalog API
* Persist orders in PostgreSQL
* Publish `OrderCreatedIntegrationEvent`
* Consume payment result events
* Update order status after payment processing

### Catalog.API

Responsible for product data.

Main responsibilities:

* Create products
* Get product by id
* Store product price and stock data
* Provide product information to Orders API

### Payment.Worker

Background worker responsible for asynchronous payment simulation.

Main responsibilities:

* Consume `OrderCreatedIntegrationEvent`
* Simulate payment processing
* Publish `PaymentSucceededIntegrationEvent`
* Publish `PaymentFailedIntegrationEvent`

### Eflow.Contracts

Shared contracts project for integration events.

It contains message contracts used between services. The project does not contain infrastructure-specific code and does not depend on MassTransit.

## Event flow

Current Milestone 3 flow:

```text
Client
  ↓
Orders.API
  ↓
Create order in PostgreSQL
  ↓
Publish OrderCreatedIntegrationEvent
  ↓
RabbitMQ
  ↓
Payment.Worker
  ↓
Simulate payment
  ↓
Publish PaymentSucceededIntegrationEvent or PaymentFailedIntegrationEvent
  ↓
RabbitMQ
  ↓
Orders.API payment consumers
  ↓
Update order status in PostgreSQL
```

Successful payment flow:

```text
Pending
  → PaymentProcessing
  → Paid
```

Failed payment flow:

```text
Pending
  → PaymentProcessing
  → PaymentFailed
```

When payment fails, the failure reason is stored on the order.

## Tech stack

* .NET
* ASP.NET Core Minimal APIs
* Entity Framework Core
* PostgreSQL
* RabbitMQ
* MassTransit
* FluentValidation
* Docker
* Docker Compose
* Testcontainers
* Respawn
* xUnit

## Roadmap

### Milestone 4 — Reliability

Planned:

* Transactional outbox
* Outbox messages table
* Save integration events in the same database transaction as business changes
* Background outbox processor
* Reliable event publishing
* Idempotent consumers
* Inbox table
* Retry behavior
* Dead-letter queue handling

### Milestone 5 — Observability and operations

Planned:

* Structured logging improvements
* Correlation IDs
* OpenTelemetry traces
* Metrics
* Health checks
* Docker Compose improvements

### Milestone 6 — CI/CD and portfolio polish

Planned:

* GitHub Actions CI
* Automated test execution
* Architecture tests
* Improved README documentation
* Example API requests
* Final portfolio-ready cleanup

## Project goal

The goal of EFlow-Commerce is not to build a complete e-commerce product.

The goal is to demonstrate backend engineering skills through a realistic service-based system with:

* Clean service boundaries
* Event-driven communication
* PostgreSQL persistence
* Message broker integration
* Integration testing
* Incremental reliability improvements
* Production-inspired architecture decisions

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

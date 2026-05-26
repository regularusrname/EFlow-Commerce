# EFlow-Commerce

EventFlow Commerce is a production-inspired, event-driven order processing platform built with .NET.

The project demonstrates backend engineering practices such as distributed messaging, transactional outbox, optimistic concurrency, integration testing, observability, and containerized local development.

## Architecture

The system consists of several independently deployable services:

- Catalog API
- Orders API
- Payments Worker
- API Gateway

Services communicate asynchronously through RabbitMQ using integration events.

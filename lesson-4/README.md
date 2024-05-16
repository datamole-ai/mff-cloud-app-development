# Lesson 4

## Introduction to Observability

The presentation on Observability is available [here](Observability.pdf).

## Instrumenting the Case Study Solution

![Design](./imgs/diagram_3.png)

## Ideas

- **Events Processing**
  - Durations of events processing
  - Lag - Time & Count
  - What about traces?
- **APIs**
  - Durations of requests
  - Status codes
  - Cache Hits/Misses

### Implementation

See the [OpenTelemetry documentation](https://opentelemetry.io/docs/languages/) for guide on how to instrument an application in your programming language.

You can find a sample implementation in the `sln` directory.

For the observability back-end, you can use a free accout of [Grafana Cloud](https://grafana.com/). You'll need to provide three parameters to the ARM template:

1. `otelProtocol` - "http/protobuf"
2. `otelEndpoint`
3. `otelHeaders`

You can find more information in the [OpenTelemetry documentation](https://grafana.com/docs/grafana-cloud/send-data/otlp/send-data-otlp/).



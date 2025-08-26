# Willow

<p align="center">
<img src="https://github.com/MatheusSw/willow/actions/workflows/dotnet-test.yml/badge.svg?branch=main" alt="Dotnet Test">
<img src="https://github.com/MatheusSw/willow/actions/workflows/dotnet-build.yml/badge.svg?branch=main" alt="Dotnet Build Status">
<img src="https://github.com/MatheusSw/willow/actions/workflows/quality.yml/badge.svg?branch=main" alt="SonarQube Status">
<a href="./LICENSE"><img src="https://img.shields.io/badge/license-apache_2.0-orange" alt="License"></a>
</p>

A self-hosted, open source **feature flag service** built with **.NET 9**, designed for teams who want full control over their release toggles without relying on SaaS vendors. Deployable on **AWS** via Terraform and Kubernetes Helm charts.

## About Willow

The Feature Toggle Service provides:

- **Remote evaluation API** (`/v1/evaluate`) for applications to check if a feature is enabled.
- **Admin API** for managing organizations, projects, environments, features, and rules.
- **SignalR WebSocket hub** for real-time notifications when toggles change.
- **PostgreSQL** as the source of truth.
- **Redis** for low-latency caching and pub/sub.
- **Terraform module + Helm chart** for easy self-hosting on AWS (EKS, RDS, ElastiCache).

This project is designed to be:
- **Modular**: separate Admin API, Evaluation API, and Notifier services.
- **Event-oriented**: consumers can subscribe to changes over WebSockets.
- **Enterprise-friendly**: self-hosted, API-first, Apache-licensed.

---

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- [Terraform](https://developer.hashicorp.com/terraform)
- [Helm](https://helm.sh/)
- AWS account (for production deployment)

---

## Installation

Clone the repository:

```bash
git clone https://github.com/your-org/feature-toggle-service.git
cd feature-toggle-service
```

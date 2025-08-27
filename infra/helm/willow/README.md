# Willow Helm Chart

A comprehensive Helm chart for deploying the Willow feature flag system, including PostgreSQL, Redis, Evaluation API, and Admin API components.

## Components

- **PostgreSQL**: Database for storing feature flag configurations and metadata
- **Redis**: Cache and pub/sub for real-time feature flag updates
- **Evaluation API**: High-performance API for feature flag evaluation
- **Admin API**: Management API for feature flag administration

## Prerequisites

- Kubernetes 1.19+
- Helm 3.2.0+
- Ingress controller (e.g., NGINX Ingress Controller)

## Installation

### Development Environment

```bash
helm install willow ./infra/helm/willow -f ./infra/helm/willow/values-dev.yaml
```

### Staging Environment

```bash
helm install willow ./infra/helm/willow -f ./infra/helm/willow/values-staging.yaml
```

### Production Environment

```bash
helm install willow ./infra/helm/willow -f ./infra/helm/willow/values-prod.yaml
```

## Configuration

### Environment-Specific Values

The chart includes three environment-specific values files:

- `values-dev.yaml`: Development environment with minimal resources
- `values-staging.yaml`: Staging environment with moderate resources and TLS
- `values-prod.yaml`: Production environment with high availability and security

### Key Configuration Options

#### Global Settings

```yaml
global:
  environment: dev|staging|production
  imageRegistry: "your-registry.com/"
  imagePullSecrets: []
```

#### PostgreSQL Configuration

```yaml
postgresql:
  enabled: true
  replicaCount: 1
  persistence:
    enabled: true
    size: 10Gi
    storageClass: ""
  resources:
    requests:
      memory: "256Mi"
      cpu: "250m"
    limits:
      memory: "512Mi"
      cpu: "500m"
```

#### Redis Configuration

```yaml
redis:
  enabled: true
  replicaCount: 1
  cluster:
    enabled: false  # Set to true for Redis cluster mode
  persistence:
    enabled: true
    size: 5Gi
```

#### API Configuration

Both Evaluation API and Admin API support:

```yaml
evaluationApi:
  enabled: true
  replicaCount: 2
  image:
    repository: willow/evaluation-api
    tag: "latest"
  resources:
    requests:
      memory: "128Mi"
      cpu: "100m"
    limits:
      memory: "512Mi"
      cpu: "500m"
  ingress:
    enabled: true
    hosts:
      - host: evaluation-api.dummy.com
        paths:
          - path: /
            pathType: Prefix
```

### Secrets Management

#### Development

For development, secrets are defined directly in the values files. This is acceptable for local development but should never be used in production.

#### Production

For production environments, use external secret management systems:

1. **Kubernetes Secrets**: Create secrets manually or via CI/CD
2. **External Secrets Operator**: Integrate with AWS Secrets Manager, HashiCorp Vault, etc.
3. **Sealed Secrets**: Encrypt secrets that can be safely stored in Git

Example using kubectl:

```bash
kubectl create secret generic willow-postgres-secret \
  --from-literal=postgres-password=your-secure-password

kubectl create secret generic willow-evaluation-api-secret \
  --from-literal=API_KEY=your-evaluation-api-key

kubectl create secret generic willow-admin-api-secret \
  --from-literal=API_KEY=your-admin-api-key \
  --from-literal=ConnectionStrings__Postgres="Host=willow-postgresql;Port=5432;Database=feature_toggle;Username=postgres;Password=your-secure-password;"
```

## Health Checks

All services include comprehensive health checks:

- **Liveness Probes**: Ensure containers are running and responsive
- **Readiness Probes**: Ensure services are ready to accept traffic

## Persistence

### PostgreSQL

- Uses PersistentVolumeClaims for data persistence
- Configurable storage class and size
- Default: 10Gi with default storage class

### Redis

- Uses PersistentVolumeClaims for data persistence
- Configurable storage class and size
- Default: 5Gi with default storage class

## Networking

### Services

- **PostgreSQL**: ClusterIP service on port 5432
- **Redis**: ClusterIP service on port 6379
- **Evaluation API**: ClusterIP service on port 8080
- **Admin API**: ClusterIP service on port 8081

### Ingress

Both APIs support configurable ingress with:
- Custom hostnames per environment
- TLS termination
- Annotations for rate limiting, IP whitelisting, etc.

## Monitoring and Observability

The chart includes:
- Proper labeling for service discovery
- Health check endpoints
- Resource limits and requests for proper scheduling

## Scaling

### Horizontal Scaling

Configure replica counts per environment:

```yaml
evaluationApi:
  replicaCount: 3  # Scale based on load

adminApi:
  replicaCount: 2  # Admin API typically needs fewer replicas
```

### Vertical Scaling

Adjust resource requests and limits:

```yaml
resources:
  requests:
    memory: "512Mi"
    cpu: "250m"
  limits:
    memory: "1Gi"
    cpu: "500m"
```

## Upgrading

```bash
helm upgrade willow ./infra/helm/willow -f ./infra/helm/willow/values-prod.yaml
```

## Uninstalling

```bash
helm uninstall willow
```

**Note**: PersistentVolumeClaims are not automatically deleted. Delete them manually if needed:

```bash
kubectl delete pvc willow-postgresql-pvc willow-redis-pvc
```

## Troubleshooting

### Check Pod Status

```bash
kubectl get pods -l app.kubernetes.io/instance=willow
```

### View Logs

```bash
kubectl logs -l app.kubernetes.io/component=evaluation-api
kubectl logs -l app.kubernetes.io/component=admin-api
kubectl logs -l app.kubernetes.io/component=postgresql
kubectl logs -l app.kubernetes.io/component=redis
```

### Check Services

```bash
kubectl get services -l app.kubernetes.io/instance=willow
```

### Check Ingress

```bash
kubectl get ingress -l app.kubernetes.io/instance=willow
```

## Security Considerations

1. **Secrets**: Use external secret management for production
2. **Network Policies**: Consider implementing network policies to restrict traffic
3. **RBAC**: Implement proper RBAC for service accounts
4. **Image Security**: Use specific image tags and scan images for vulnerabilities
5. **TLS**: Enable TLS for all external communications
6. **Rate Limiting**: Configure rate limiting on ingress controllers

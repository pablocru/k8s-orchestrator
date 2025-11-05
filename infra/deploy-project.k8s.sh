#!/bin/bash

set -e

# Variables
IMAGE_NAME=k8s-orch
APP_LABEL="app=k8s-orchestrator"
DEPLOYMENT_FILE="k8s/deployment.yml"
SERVICE_FILE="k8s/service.yml"

echo
echo "Checking Minikube status..."
if ! minikube status --format '{{.Host}}' 2>/dev/null | grep -q "Running"; then
  echo "Minikube is not running. Starting Minikube..."
  minikube start
else
  echo "Minikube is already running."
fi

echo
echo "Building Docker Image..."
eval $(minikube docker-env)
docker build -f ../src/orchestrator/Dockerfile -t $IMAGE_NAME ../src/

echo
echo "Applying k8s infra..."
kubectl apply -f "$DEPLOYMENT_FILE"
kubectl apply -f "$SERVICE_FILE"

echo
echo "Waiting for Pods to be ready..."
kubectl wait --for=condition=ready pod -l "$APP_LABEL" --timeout=120s

echo
echo "Deployment complete!"
echo "To delete all resources with label '$APP_LABEL', run:"
echo "kubectl delete all -l $APP_LABEL"

echo
echo "Exposing service in Minikube..."
SERVICE_NAME=$(kubectl get svc -l "$APP_LABEL" -o jsonpath="{.items[0].metadata.name}")
minikube service "$SERVICE_NAME"

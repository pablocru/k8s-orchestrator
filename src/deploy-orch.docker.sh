#!/bin/bash

set -e

# Variables
IMAGE_NAME=k8s-orch
ORCH_DOCKERFILE="./Orchestrator/Dockerfile"

# Program
echo
echo "Building Docker Image..."
docker build -f $ORCH_DOCKERFILE -t $IMAGE_NAME .

echo
echo "Running a development container..."
docker run --rm --name $IMAGE_NAME-container -p 80:8080 $IMAGE_NAME

#!/bin/bash

# Variables
IMAGE_NAME=k8s-orch

# Program
echo
echo "Building Docker Image..."
docker build -f ./orchestrator/Dockerfile -t $IMAGE_NAME .

echo
echo "Running a development container..."
docker run --rm --name $IMAGE_NAME-container -p 80:8080 $IMAGE_NAME

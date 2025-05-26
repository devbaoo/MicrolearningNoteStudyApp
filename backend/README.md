# Microlearning Note Study App - Backend

This project contains serverless AWS Lambda functions for the Microlearning Note Study App backend.

## Architecture
- .NET 8.0 AWS Lambda Functions
- Amazon API Gateway v2
- Amazon DynamoDB

## Project Structure
- **Common**: Shared models, interfaces, and repositories used across all Lambda functions
- **AuthenticationFunction**: User authentication and account management
- **UserManagementFunction**: User profile and settings management
- **NoteManagementFunction**: Note creation, updating, and management
- **AtomManagementFunction**: Atom (microlearning unit) management
- **KnowledgeGraphFunction**: Knowledge graph generation and analysis
- **ReviewSystemFunction**: Spaced repetition review system
- **LearningAnalyticsFunction**: Learning performance analytics
- **SearchDiscoveryFunction**: Search and content discovery
- **IntegrationFunction**: External integrations
- **NotificationFunction**: Notification system

## Setup and Deployment
1. Install the AWS CLI and configure credentials
2. Install the AWS Lambda .NET Core Global Tool
3. Build and deploy each Lambda function 
# Diabits Web App

Blazor WebAssembly app for visualizing and analyzing health data in the Diabits system.

## Purpose
Provides a unified overview of health data to support analysis of blood glucose patterns and their relation to other factors.

- Visualizes glucose alongside contextual health data
- Enables comparison across days and periods
- Highlights patterns and potential correlations

## Tech Stack
- Blazor WebAssembly
- MudBlazor
- ApexCharts
- REST API (Diabits API)

## Responsibilities
- Fetch aggregated data from backend
- Render interactive charts and dashboards
- Enable filtering and comparison of data
- Provide admin functionality for user access

## Key Concepts

### Pre-Aggregated Data
- Backend provides chart-ready data
- Minimizes frontend processing
- Ensures consistent calculations

### Timeline Visualization
- Displays multiple data types in aligned time buckets
- Supports shared tooltips and comparisons

### Comparative Analysis
- Compare days and periods
- Identify patterns in glucose variability

## Features
- Daily overview dashboard
- Timeline with multiple data series
- Summary insights and trends
- Admin invite page for generating user invite codes

## Notes
- Primarily read-only (except admin features)
- Optimized for clarity and fast insights
- Part of a larger system with mobile app and API

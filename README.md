# Environmental Monitor Backend

Backend service for collecting, storing, and analyzing environmental sensor data such as temperature and humidity.

This project contacts the OpenMeteo API for real time outside weather conditions.

## Overview

By interacting with a Raspberry Pi and various sensors, this backend collects data on indoor environmental conditions.

It also integrates with the OpenMeteo API to fetch real-time outdoor weather data.

The collected data is stored in human readable reports and JSON files for further analysis and visualization.

## Key Features

- Indoor Sensor Data Collection — Collects data from connected sensors for temperature and humidity.
- Outdoor Weather Integration — Fetches real-time weather data from the OpenMeteo API.
- Generates Human Readable Reports — Creates reports summarizing the collected data for easy interpretation.
- JSON Data Storage — Stores raw sensor data and API responses in JSON format for further analysis and visualization.
- Tracks historical data to identify trends and patterns in environmental conditions over time.

## Requirements

- Windows 10 or later (64-bit)

## Quick Start

### Option 1 - Download the release

1. Download the latest release from the [Releases]() section of the repository.
2. Extract the downloaded archive to a desired location.
3. Navigate to the extracted folder and run the executable file (e.g., `EnvironmentalMonitorBackend.exe`).

### Option 2 - Clone and run

1. Clone the repository

```bash
git clone https://github.com/alex-jns/environmental-monitor-backend.git
cd environmental-monitor-backend
```

2. Restore and build

```bash
dotnet restore
dotnet build
```

3. Run (replace `<path-to-project>` with the actual project file or solution)

```bash
dotnet run --project "Environmental Monitor.csproj"
```

## Usage

1. Ensure that the Raspberry Pi is properly connected to the sensors and configured to send data to the backend.
2. Start the backend service using one of the methods described in the Quick Start section.
3. Upon selecting the "Start Monitoring" option, select a time interval for data collection (e.g., every 10 minutes).
4. The backend will begin collecting data from the sensors and fetching outdoor weather data at the specified intervals.
5. The collected data will be stored in JSON files and human-readable reports will be generated for analysis.
6. To create a report to compare long-term trends, select the "Generate Report" option and specify the desired time range for the report.

## License

Specify the license for the project (e.g., `MIT`, `Apache-2.0`, or a custom license).

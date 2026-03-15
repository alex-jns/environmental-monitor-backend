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

1. Download the latest release from the [Releases](https://github.com/alex-jns/environmental-monitor-backend/releases) section of the repository.
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

3. Run

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


### Expected Inputs

1. Time interval for data collection (e.g., every 10 minutes).
	a. Must be a positive integer representing the number of minutes between each data collection cycle.
2. Time range for report generation (e.g., last 24 hours, last week).
	a. Must be a valid time range that the backend can use to filter the collected data for report generation.
		i. Any date range in the format `YYYY-MM-DD to YYYY-MM-DD` (e.g., `2024-01-01 to 2024-01-31`).
	b. Any date before or after the earliest or latest report will default to the earliest or latest report, respectively.
3. Sensor data must communicate on port 11000 and be in a valid JSON format containing temperature and humidity readings.
	a. JSON format example: { temperatureF: 72.5, "temperatureC": 22.5, "humidity": 45.0 }
4. OpenMeteo API response is configured for latitude 36.5951 and longitude -82.1887, with time zone set to "America/New_York"

### Expected Outputs

1. JSON files containing raw sensor data and API responses.
2. Human-readable reports summarizing the collected data, including trends and patterns in environmental conditions over time.
3. Console output indicating the status of data collection and report generation processes.
4. Error messages for any issues encountered during data collection, API integration, or report generation.
5. Logs for debugging and monitoring the backend service.

### Expected Errors

1. Invalid time interval input (e.g., negative numbers, non-integer values).
2. Invalid time range input for report generation (e.g., incorrect date format).
3. Failure to connect to the Raspberry Pi or sensors (e.g., network issues, incorrect port).
4. Failure to fetch data from the OpenMeteo API (e.g., network issues, API downtime).
5. Invalid JSON format from sensor data or API responses.
6. File I/O errors when writing JSON files or generating reports (e.g., permission issues, disk space).
7. Unhandled exceptions during data collection, API integration, or report generation processes.
8. Data validation errors (e.g., missing temperature or humidity values in the sensor data).
9. API rate limit errors when fetching data from the OpenMeteo API.
10. Incorrect configuration for API parameters (e.g., invalid latitude, longitude, or time zone).

### Troubleshooting

1. Ensure that the Raspberry Pi and sensors are properly connected and configured to send data to the backend.
2. Check the console output and logs for any error messages or exceptions that may indicate issues with data collection, API integration, or report generation.
3. Verify that the OpenMeteo API is accessible and that the API parameters (latitude, longitude, time zone) are correctly configured.
4. Ensure that the time interval and time range inputs are valid and in the correct format.
5. Check for any file I/O issues when writing JSON files or generating reports, such as permission issues or insufficient disk space.

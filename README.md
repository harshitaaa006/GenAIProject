# IS7024 - SafeStreet

## Introduction

Discover the safety of your next home or investment with SafeStreet! This platform provides easy-to-understand crime data and visualizations, helping you assess neighborhood safety at a glance. Whether you're a realtor, renter, homebuyer, or investor, SafeStreet empowers you to make informed decisions about where you live and invest. Stay safe and informed with just a tap!

---

## Logo
![SafeStreet (1) (1)](https://github.com/Josh-Pai/IS7024/blob/main/logo.jpeg)

---

## Storyboard

The storyboard for SafeStreet outlines the main user screens and features:

- **Search Page**: Introduction to SafeStreet with Search feature that takes you to crime data breakdowns based on city or zip code.
- **Crime Data Map**: An interactive map where users can view crime markers, filter by crime type, and adjust date ranges.
- **Neighborhood Summary**: A summary page for each neighborhood with crime stats, recent incidents, and a simple safety score.
- **Safety Score**: A visual indicator (e.g., green, yellow, red) based on recent crime levels in the neighborhood.
- **Crime Trend Chart**: Monthly, yearly, or weekly line/bar chart showing trends in neighborhood crime levels over time.

---
## GitHub Projects
Use GitHub Projects to manage tasks and track progress. The project boards should include the following sections:

- **To-Do**: Tasks yet to be started.
- **In Progress**: Tasks currently being worked on.
- **Completed**: Tasks that are done.

---

## Functional Requirements

### Requirement 1: Crime Data Map with Filters
- **Scenario**: As a user, I want to see recent crime incidents on a map and filter by crime type and date range to assess crime patterns in an area.
- **Dependencies**: Google Maps API, public crime data APIs or sample CSV data.
- **Assumptions**: Crime data includes date, type, and location.
  
**Examples**:
- **Given**: Public crime data is available  
  **When**: I select the crime type “theft”  
  **Then**: The map displays only incidents of theft.

- **Given**: Public crime data is available  
  **When**: I select a date range from January to March  
  **Then**: The map shows only crimes that occurred during that timeframe.

---

### Requirement 2: Neighborhood Summary Report
- **Scenario**: As a user, I want a quick summary of crime stats in each neighborhood to gauge the overall safety.
- **Dependencies**: SQL database for storing incident data, Razor Pages for displaying neighborhood summaries.
- **Assumptions**: Data includes crime type, date, and neighborhood location.

**Examples**:
- **Given**: Crime data is updated regularly  
  **When**: I open a neighborhood summary page  
  **Then**: I should see the total number of recent crimes and the most common crime types.

---

### Requirement 3: Simple Safety Score
- **Scenario**: As a user, I want a simple safety score for each neighborhood to quickly assess overall safety.
- **Dependencies**: SQL database to calculate and store scores, simple scoring algorithm based on recent crime data.
- **Assumptions**: Safety scores are calculated weekly based on crime levels.

**Examples**:
- **Given**: Recent crime data is available  
  **When**: I view a neighborhood  
  **Then**: I should see a safety score based on crime frequency and severity.

---

## Data Sources

- [Crime Data from 2020 to Present](https://catalog.data.gov/dataset/crime-data-from-2020-to-present)
- [Google Maps Geocoding API](https://developers.google.com/maps/documentation/geocoding/start)
- [Data.gov](https://data.gov/)
- [ArcGIS Crime Data](https://www.arcgis.com/home/item.html?id=56b89613f9f7450fb44e857691a244e7)

---

## Team Composition

- **Amy Chen**
- **Archana Kasturi**
- **Ariela Kurtzer**
- **Harshita Patel**
- **Josh Pai**

### Suggested Tech Stack

- **DevOps/Product Owner/Scrum Master**: Archana Kasturi
- **Frontend**: HTML, CSS, JavaScript with Razor Pages (ASP.NET Core): Harshita Patel
- **Data Visualization**: Google Maps API or Leaflet.js for mapping, Chart.js for trend charts: Ariela Kurtzer
- **Backend**: ASP.NET Core, SQL database for storing data: Josh Pai
- **Deployment**: Azure or GitHub Pages for deployment: Amy Chen

### Weekly Meeting
- **When**: Sunday at 7 PM
- **Platform**: Zoom

---



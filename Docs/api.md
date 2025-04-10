# Donors Data Feed API
Allows to submit donors data to the repository.

> [!Note]
> API is accessible for authorized users only and requires `JWT` token as `Authorization` header (read more about [Identity Service](https://github.com/dkfz-unite/unite-identity)).

API is **proxied** to main API and can be accessed at [[host]/api/donors-feed](http://localhost/api/donors-feed) (**without** `api` prefix).

All data submision request implement **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated
- Redundand data will be removed


## Overview
- get:[api](#get-api) - health check.
- get:[api/entries/{id}](#get-apientriesid) - get patients data submission document. 
- post:[api/entries/{type?}](#post-apientriestype) - submit patients and their clinical data in given type.
- get:[api/treatments/{id}](#get-apitreatmentsid) - get patients treatments data submission document.
- post:[api/treatments/{type?}](#post-apitreatmentstype) - submit patients treatments data in given type.
- delete:[api/entry/{id}](#delete-apientryid) - delete donor data.

> [!Note]
> **Json** is default data type for all requests and will be used if no data type is specified.


## GET: [api](http://localhost:5100/api)
Health check.

### Responses
`"2022-03-17T09:45:10.9359202Z"` - Current UTC date and time in JSON format, if service is up and running


## GET: [api/entries/{id}](http://localhost:5100/api/entries/1)
Get patients data (including clinical and treatments data) submission document.

### Parameters
- `id` - submission ID.

### Responses
- `200` - submission document in JSON format
- `401` - missing JWT token
- `403` - missing required permissions
- `404` - submission with given ID was not found


## POST: [api/entries/{type?}](http://localhost:5100/api/entries)
Submit patients data (including clinical and treatments data).

### Parameters
- `review` - indicates if submitted data requires reviewal. Default is `true`.

### Body
Supported formats are:
- `json` (**empty**) - application/json
- `tsv` - text/tab-separated-values

##### json - application/json
```json
[
  {
    "id": "Donor1",
    "mta": true,
    "projects": [ "Project1" ],
    "studies": [ "Study1" ],
    "clinical_data": {
      "sex": "Female",
      "enrollment_date": "2020-01-01",
      "enrollment_age": 45,
      "diagnosis": "Diagnosis1",
      "primary_site": "Brain",
      "localization": "Left",
      "vital_status": true,
      "vital_status_change_date": "2021-01-01",
      "vital_status_change_day": null,
      "progression_status": false,
      "progression_status_change_date": "2021-01-01",
      "progression_status_change_day": null,
      "kps_baseline": 90,
      "steroids_baseline": true
    }
  },
  {
    "id": "Donor2",
    "mta": true,
    "projects": [ "Project1" ],
    "studies": [ "Study1" ],
    "clinical_data": {
      "sex": "Female",
      "enrollment_date": "2020-01-01",
      "enrollment_age": 75,
      "diagnosis": "Diagnosis1",
      "primary_site": "Brain",
      "localization": "Right",
      "vital_status": false,
      "vital_status_change_date": "2021-01-01",
      "vital_status_change_day": null,
      "progression_status": true,
      "progression_status_change_date": "2020-03-01",
      "progression_status_change_day": null,
      "kps_baseline": 50,
      "steroids_baseline": false
    }
  }
]
```

##### tsv - text/tab-separated-values
```tsv
id	mta	projects	studies	sex	enrollment_date	enrollment_age	diagnosis	primary_site	localization	vital_status	vital_status_change_date	vital_status_change_day	progression_status	progression_status_change_date	progression_status_change_day	kps_baseline	steroids_baseline
Donor1	true	Project1	Study1	Female	2020-01-01	45	Diagnosis1	Brain	Left	true	2021-01-01		false	2021-01-01		90	true
Donor2	true	Project1	Study1	Female	2020-01-01	75	Diagnosis1	Brain	Right	false	2021-01-01		true	2020-03-01		50	false
```

Fields description can be found [here](./api-models-donors.md).

### Responses
- `200` - submission ID (can be used to track submission status)
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## GET: [api/treatments/{id}](http://localhost:5100/api/treatments/1)
Get patients treatments data submission document.

### Parameters
- `id` - submission ID.

### Responses
- `200` - submission document in JSON format
- `401` - missing JWT token
- `403` - missing required permissions
- `404` - submission with given ID was not found


## POST: [api/treatments/{type?}](http://localhost:5100/api/treatments)
Submit patients treatment data. Patients should exist in the system.

### Parameters
- `review` - indicates if submitted data requires reviewal. Default is `true`.

### Body
Supported formats are:
- `json` (**empty**) - application/json
- `tsv` - text/tab-separated-values

##### json - application/json
```json
[
  {
    "donor_id": "Donor1",
    "entries": [
      {
        "therapy": "Therapy1",
        "details": "Patient specific therapy details.",
        "start_date": "2020-01-15",
        "end_date": "2020-01-15",
        "results": "Patient specific therapy results."
      },
      {
        "therapy": "Therapy2",
        "details": "Patient specific therapy details.",
        "start_date": "2020-02-01",
        "end_date": "2020-03-01",
        "results": "Patient specific therapy results."
      }
    ]
  },
  {
    "donor_id": "Donor2",
    "entries": [
      {
        "therapy": "Therapy1",
        "details": "Patient specific therapy details.",
        "start_date": "2020-01-15",
        "end_date": "2020-01-15",
        "results": "Patient specific therapy results."
      },
      {
        "therapy": "Therapy2",
        "details": "Patient specific therapy details.",
        "start_date": "2020-02-01",
        "end_date": "2020-03-01",
        "results": "Patient specific therapy results."
      }
    ]
  }
]
```

##### tsv - text/tab-separated-values
```tsv
donor_id	therapy	details	start_date	start_day	end_date	duration_days	results
Donor1	Therapy1	Patient specific therapy details.	2020-01-15		2020-01-15		Patient specific therapy results.
Donor1	Therapy2	Patient specific therapy details.	2020-02-01		2020-03-01		Patient specific therapy results.
Donor2	Therapy1	Patient specific therapy details.	2020-01-15		2020-01-15		Patient specific therapy results.
Donor2	Therapy2	Patient specific therapy details.	2020-02-01		2020-03-01		Patient specific therapy results.
```

Fields description can be found [here](api-models-treatments.md).

### Responses
- `200` - submission ID (can be used to track submission status)
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## DELETE: [api/entry/{id}](http://localhost:5100/api/entry/1)
Delete donor data.

### Parameters
- `id` - ID of the donor to delete.

### Responses
- `200` - request was processed successfully
- `401` - missing JWT token
- `403` - missing required permissions
- `404` - image with given ID was not found
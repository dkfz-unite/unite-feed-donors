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
- post:[api/donors/{type?}](#post-apidonorstype) - submit all patients data in given type.
- post:[api/treatments/{type?}](#post-apitreatmentstype) - submit patients treatments data in given type.

> [!Note]
> **Json** is default data type for all requests and will be used if no data type is specified.


## GET: [api](http://localhost:5100/api)
Health check.

### Responses
`"2022-03-17T09:45:10.9359202Z"` - Current UTC date and time in JSON format, if service is up and running


## POST: [api/donors/{type?}](http://localhost:5100/api/donors)
Submit patients data (including clinical and treatments data).

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
      "age": 45,
      "diagnosis": "Diagnosis1",
      "diagnosis_date": "2020-01-01",
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
      "age": 75,
      "diagnosis": "Diagnosis1",
      "diagnosis_date": "2020-01-01",
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
id	mta	projects	studies	sex	age	diagnosis	diagnosis_date	primary_site	localization	vital_status	vital_status_change_date	vital_status_change_day	progression_status	progression_status_change_date	progression_status_change_day	kps_baseline	steroids_baseline
Donor1	true	Project1	Study1	Female	45	Diagnosis1	2020-01-01	Brain	Left	true	2021-01-01		false	2021-01-01		90	true
Donor2	true	Project1	Study1	Female	75	Diagnosis1	2020-01-01	Brain	Right	false	2021-01-01		true	2020-03-01		50	false
```

Fields description can be found [here](./api-models-donors.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions



## POST: [api/treatments/{type?}](http://localhost:5100/api/treatments)
Submit patients treatment data. Patients should exist in the system.

### Body
Supported formats are:
- `json` (**empty**) - application/json
- `tsv` - text/tab-separated-values

##### json - application/json
```json
[
  {
    "donor_id": "Donor1",
    "data": [
      {
        "therapy": "Therapy1",
        "details": "Patient specific therapy details.",
        "start_date": "2020-01-15",
        "start_day": null,
        "end_date": "2020-01-15",
        "duration_days": null,
        "results": "Patient specific therapy results."
      },
      {
        "therapy": "Therapy2",
        "details": "Patient specific therapy details.",
        "start_date": "2020-02-01",
        "start_day": null,
        "end_date": "2020-03-01",
        "duration_days": null,
        "results": "Patient specific therapy results."
      }
    ]
  },
  {
    "donor_id": "Donor2",
    "data": [
      {
        "therapy": "Therapy1",
        "details": "Patient specific therapy details.",
        "start_date": "2020-01-15",
        "start_day": null,
        "end_date": "2020-01-15",
        "duration_days": null,
        "results": "Patient specific therapy results."
      },
      {
        "therapy": "Therapy2",
        "details": "Patient specific therapy details.",
        "start_date": "2020-02-01",
        "start_day": null,
        "end_date": "2020-03-01",
        "duration_days": null,
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
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions

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
        "id": "DO1",
        "mta": true,
        "projects": [
            "WP1"
        ],
        "studies": [
            "ST1"
        ],
        "clinical_data": {
            "sex": "Other",
            "age": 56,
            "diagnosis": "Glioblastoma",
            "diagnosis_date": "2020-01-01",
            "primary_site": "Brain",
            "localization": "Left",
            "vital_status": true,
            "vital_status_change_date": "2021-01-01",
            "vital_status_change_day": 365,
            "progression_status": false,
            "progression_status_change_date": "2020-02-12",
            "progression_status_change_day": 37,
            "kps_baseline": 90,
            "steroids_baseline": false
        },
        "treatments": [
            {
                "therapy": "Radiotherapy",
                "details": "Patient specific therapy details.",
                "start_date": "2020-01-07",
                "start_day": 7,
                "end_date": "2020-01-27",
                "duration_days": 20,
                "results": "Patient specific therapy results."
            }
        ]
    }
]
```

##### tsv - text/tab-separated-values
```tsv
id	mta	projects	studies	sex	age	diagnosis	diagnosis_date	primary_site	localization	vital_status	vital_status_change_date	vital_status_change_day	progression_status	progression_status_change_date	progression_status_change_day	kps_baseline	steroids_baseline
DO1	true	WP1	ST1	sex	56	Glioblastoma	2020-01-01	Brain	Left	true	2021-01-01	365	false	2020-02-12	37	90	false

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
{
    "donor_id": "DO1",
    "data": [
        {
            "therapy": "Radiotherapy",
            "details": "Patient specific therapy details.",
            "start_date": "2020-01-07",
            "start_day": 7,
            "end_date": "2020-01-27",
            "duration_days": 20,
            "results": "Patient specific therapy results."
        }
    ]
}
```

##### tsv - text/tab-separated-values
```tsv
donor_id	therapy	details	start_date	start_day	end_date	duration_days	results
DO1	Radiotherapy	Patient specific therapy details.	2020-01-07	7	2020-01-27	20	Patient specific therapy results.

```

Fields description can be found [here](api-models-treatments.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions

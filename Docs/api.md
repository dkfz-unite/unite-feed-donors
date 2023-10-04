# Donors Data Feed API

## GET: [api](http://localhost:5100/api) - [api/donors-feed](https://localhost/api/donors-feed)
Health check.

### Responses
`"2022-03-17T09:45:10.9359202Z"` - Current UTC date and time in JSON format, if service is up and running


## POST: [api/donors](http://localhost:5100/api/donors) - [api/donors-feed/donors/json](https://localhost/api/donors-feed/donors)
Submit donors data (including clinical and treatment data).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Headers
- `Authorization: Bearer [token]` - JWT token with `Data.Write` permission.

### Body - application/json
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
Fields description can be found [here](api-donors-models.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/donors/tsv](http://localhost:5100/api/donors/tsv) - [api/donors-feed/donors/tsv](https://localhost/api/donors-feed/donors/tsv)
Submit donors data (including clinical and excluding treatment data).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Headers
- `Authorization: Bearer [token]` - JWT token with `Data.Write` permission.

### Body - application/json
```tsv
id	mta	projects	studies	sex	age	diagnosis	diagnosis_date	primary_site	localization	vital_status	vital_status_change_date	vital_status_change_day	progression_status	progression_status_change_date	progression_status_change_day	kps_baseline	steroids_baseline
DO1	true	WP1	ST1	sex	56	Glioblastoma	2020-01-01	Brain	Left	true	2021-01-01	365	false	2020-02-12	37	90	false

```
Fields description can be found [here](api-donors-models.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/treatments](http://localhost:5100/api/treatments) - [api/donors-feed/treatments](https://localhost/api/donors-feed/treatments)
Submit treatment data (including a donor id for reference whom to add this to).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Headers
- `Authorization: Bearer [token]` - JWT token with `Data.Write` permission.

### Body - application/json
```json
[
    {
        "donor_id": "DO1",
        "therapy": "Radiotherapy",
        "details": "Patient specific therapy details.",
        "start_date": "2020-01-07",
        "start_day": 7,
        "end_date": "2020-01-27",
        "duration_days": 20,
        "results": "Patient specific therapy results."
    }
]
```
Fields description can be found [here](api-treatments-models.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/treatments/tsv](http://localhost:5100/api/treatments/tsv) - [api/donors-feed/treatments/tsv](https://localhost/api/donors-feed/treatments/tsv)
Submit treatment data (including a donor id for reference whom to add this to).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Headers
- `Authorization: Bearer [token]` - JWT token with `Data.Write` permission.

### Body - application/json
```tsv
donor_id	therapy	details	start_date	start_day	end_date	duration_days	results
DO1	Radiotherapy	Patient specific therapy details.	2020-01-07	7	2020-01-27	20	Patient specific therapy results.

```
Fields description can be found [here](api-treatments-models.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions

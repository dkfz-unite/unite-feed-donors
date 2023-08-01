# Donors Data Feed API

## GET: [api](http://localhost:5100/api)
Health check.

### Responses
`"2022-03-17T09:45:10.9359202Z"` - Current UTC date and time in JSON format, if service is up and running


## POST: [api/donors](http://localhost:5100/api/donors)
Submit donors data (including clinical and treatment data).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Boby - application/json
```json
[
    {
        "Id": "DO1",
        "MtaProtected": true,
        "Projects": [
            "WP1"
        ],
        "Studies": [
            "ST1"
        ],
        "ClinicalData": {
            "Sex": "Male",
            "Age": 56,
            "Diagnosis": "Glioblastoma",
            "DiagnosisDate": "2020-01-01T00:00:00",
            "PrimarySite": "Brain",
            "Localization": "Left",
            "VitalStatus": true,
            "VitalStatusChangeDate": "2021-01-01T00:00:00",
            "VitalStatusChangeDay": 365,
            "ProgressionStatus": false,
            "ProgressionStatusChangeDate": "2020-02-12T00:00:00",
            "ProgressionStatusChangeDay": 37,
            "KpsBaseline": 90,
            "SteroidsBaseline": false
        },
        "Treatments": [
            {
                "Therapy": "Radiotherapy",
                "Details": "Patient specific therapy details.",
                "StartDate": "2020-01-07T00:00:00",
                "StartDay": 7,
                "EndDate": "2020-01-27T00:00:00",
                "DurationDays": 20,
                "Results": "Patient specific therapy results."
            }
        ]
    }
]
```
Fields description can be found [here](api-donors-models.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation

# Donor Data Models

## Donor
Includes general data about the patient

**`id`*** - Donor pseudonymized identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"DO1"`

**`mta`** - Indicates whether donor data is MTA protected.
- Type: _Boolean_
- Example: `true`

**`projects`** - Projects list, which own donor data.
- Type: _Array_
- Elemet type: _String_
- Element limitations: Maximum length 100
- Example: `["WP1", "WP2"]`

**`studies`** - Studies list, which include donor data.
- Type: _Array_
- Elemet type: _String_
- Element limitations: Maximum length 100
- Example: `["ST1", "ST2"]`

**`clinical_data`** - Donor clinical data.
- Type: _Object([clinical_data](https://github.com/dkfz-unite/unite-donors-feed/blob/main/Docs/api-donors-models.md#clinical-data))_
- Limitations - If set, at least one field has to be set
- Example: `{...}`

**`treatments`** - Donor treatments data.
- Type: _Array_
- Element type: _Object([treatment](https://github.com/dkfz-unite/unite-donors-feed/blob/main/Docs/api-donors-models.md#treatment))_
- Example: `[{...}, {...}]`


## Clinical Data
Includes patient clinical data.

**`sex`** - Donor sex.
- Type: _String_
- Possible values: `"Female"`, `"Male"`, `"Other"`
- Example: `"Other"`

**`age`** - Donor age at diagnosis.
- Type: _Number_
- Limitations: Integer, greater or equal to 0
- Example: `56`

**`diagnosis`** - Donor diagnosis.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"Glioblastoma"`

**`diagnosis_date`** - Date, when diagnosis was stated.
- Note: Serves as anchor date for calculation of all other relative dates. It's hidden and protected. Without this date, no other relative dates will be calculated and shown.
- Type: _String_
- Format: "YYYY-MM-DD"
- Example: `"2020-01-01"`

**`primary_site`** - Primary site of disease.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"Brain"`

**`localization`** - Tumor localization relative to primary site.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"Left"`

**`vital_status`** - Indicates whether patient is still alive.
- Type: _Boolean_
- Example: `true`

**`vital_status_change_date`** - Date, when vital status was last revised.
- Note: It's hidden and protected. Relative date is shown instead, if calculation was possible.
- Type: _String_
- Format: "YYYY-MM-DD"
- Limitations: Only either 'VitalStatusChangeDate' or 'VitalStatusChangeDay' can be set at once, not both
- Example: `"2021-01-01"`

**`vital_status_change_day`** - Relative number of days since diagnosis statement, when vital status was last revised.
- Type: _Number_
- Limitations: Integer, greater or equal to 0, only either 'VitalStatusChangeDate' or 'VitalStatusChangeDay' can be set at once, not both
- Example: `367`

**`progression_status`** - Indicates whether disease was progressing after treatment or not.
- Type: _Boolean_
- Example: `false`

**`progression_status_change_date`** - Date, when progression status was last revised.
- Note: It's hidden and protected. Relative date is shown instead, if calculation was possible.
- Type: _String_
- Format: "YYYY-MM-DD"
- Limitations: Only either 'ProgressionStatusChangeDate' or 'ProgressionStatusChangeDay' can be set at once, not both
- Example: `"2020-02-12"`

**`progression_status_change_day`** - Relative number of days since treatment start, when progression status was last revised.
- Type: _Number_
- Limitations: Integer, greater or equal to 0, only either 'ProgressionStatusChangeDate' or 'ProgressionStatusChangeDay' can be set at once, not both
- Example: `37`

**`kps_baseline`** - KPS baseline.
- Type: _Number_
- Limitations: Integer, greater or equal to 0, less or equal to 100
- Example: `90`

**`steroids_baseline`** - Steroids baseline.
- Type: _Boolean_
- Example: `false`

## Treatment
Includes patient treatment data.

**`therapy`*** - Therapy name.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"Radiotherapy"`

**`details`** - Patient specific therapy details.
- Type: _String_
- Eample: `"Patient specific therapy details."`

**`start_date`** - Date, when treatment has started.
- Note: It's hidden and protected. Relative date is shown instead, if calculation was possible.
- Type: _String_
- Format: "YYYY-MM-DD"
- Limitations: Only either 'StartDateDate' or 'StartDay' can be set at once, not both
- Example: `"2020-01-07"`

**`start_day`** - Relative number of days since diagnosis statement, when treatment has started.
- Type: _Number_
- Limitations: Integer, greater or equal to 0, only either 'StartDate' or 'StartDay' can be set at once, not both
- Example: `7`

**`end_date`** - Date, when treatment has ended.
- Note: It's hidden and protected. Relative date is shown instead, if calculation was possible.
- Type: _String_
- Format: "YYYY-MM-DD"
- Limitations: Only either 'EndDateDate' or 'DurationDays' can be set at once, not both
- Example: `"2020-01-27"`

**`duration_days`** - Treatment duration in days.
- Type: _Number_
- Limitations: Integer, greater or equal to 0, only either 'EndDate' or 'DurationDays' can be set at once, not both
- Example: `20`

**`results`** - Patient specific therapy results.
- Type: _String_
- Example: `"Patient specific therapy results."`

##
**`*`** - Required fields

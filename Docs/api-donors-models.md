# Donor Data Models

## Donor
Includes general data about the patient

**`Id`*** - Donor pseudonymized identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"DO1"`

**`MtaProtected`** - Indicates whether donor data is MTA protected.
- Type: _Boolean_
- Example: `true`

**`WorkPackages`** - Work packages list, which own donor data.
- Type: _Array_
- Elemet type: _String_
- Element limitations: Maximum length 100
- Example: `["WP1", "WP2"]`

**`Studies`** - Studies list, which include donor data.
- Type: _Array_
- Elemet type: _String_
- Element limitations: Maximum length 100
- Example: `["ST1", "ST2"]`

**`ClinicalData`** - Donor clinical data.
- Type: _Object([ClinicalData](https://github.com/dkfz-unite/unite-donors-feed/blob/main/Docs/api-donors-models.md#clinical-data))_
- Limitations - If set, at least one field has to be set
- Example: `{...}`

**`Treatments`** - Donor treatments data.
- Type: _Array_
- Element type: _Object([Treatment](https://github.com/dkfz-unite/unite-donors-feed/blob/main/Docs/api-donors-models.md#treatment))_
- Example: `[{...}, {...}]`


## Clinical Data
Includes patient clinical data.

**`Gender`** - Donor Gender.
- Type: _String_
- Possible values: `"Female"`, `"Male"`, `"Other"`
- Example: `"Male"`

**`Age`** - Donor age at diagnosis.
- Type: _Number_
- Limitations: Integer, greater or equal to 0
- Example: `56`

**`Diagnosis`** - Donor diagnosis.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"Glioblastoma"`

**`DiagnosisDate`** - Date, when diagnosis was stated.
- Note: Serves as anchor date for calculation of all other relative dates. It's hidden and protected. Without this date, no other relative dates will be calculated and shown.
- Type: _String_
- Format: "YYYY-MM-DDTHH-MM-SS"
- Example: `"2020-01-01T00:00:00"`

**`PrimarySite`** - Primary site of disease.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"Head"`

**`Localization`** - Tumor localization relative to primary site.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"Left"`

**`VitalStatus`** - Indicates whether patient is still alive.
- Type: _Boolean_
- Example: `true`

**`VitalStatusChangeDate`** - Date, when vital status was last revised.
- Note: It's hidden and protected. Relative date is shown instead, if calculation was possible.
- Type: _String_
- Format: "YYYY-MM-DDTHH-MM-SS"
- Limitations: Only either 'VitalStatusChangeDate' or 'VitalStatusChangeDay' can be set at once, not both
- Example: `"2021-01-01T00:00:00"`

**`VitalStatusChangeDay`** - Relative number of days since diagnosis statement, when vital status was last revised.
- Type: _Number_
- Limitations: Integer, greater or equal to 0, only either 'VitalStatusChangeDate' or 'VitalStatusChangeDay' can be set at once, not both
- Example: `365`

## Treatment
Includes patient treatment data.

**`Therapy`*** - Therapy name.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"Radiotherapy"`

**`Details`*** - Patient specific therapy details.
- Type: _String_
- Example: `"Patient specific therapy details."`

**`StartDate`** - Date, when treatment has started.
- Note: It's hidden and protected. Relative date is shown instead, if calculation was possible.
- Type: _String_
- Format: "YYYY-MM-DDTHH-MM-SS"
- Limitations: Only either 'StartDateDate' or 'StartDay' can be set at once, not both
- Example: `"2020-01-07T00:00:00"`

**`StartDay`** - Relative number of days since diagnosis statement, when treatment has started.
- Type: _Number_
- Limitations: Integer, greater or equal to 0, only either 'StartDate' or 'StartDay' can be set at once, not both
- Example: `7`

**`EndDate`** - Date, when treatment has ended.
- Note: It's hidden and protected. Relative date is shown instead, if calculation was possible.
- Type: _String_
- Format: "YYYY-MM-DDTHH-MM-SS"
- Limitations: Only either 'EndDateDate' or 'DurationDays' can be set at once, not both
- Example: `"2020-01-27T00:00:00"`

**`DurationDays`** - Treatment duration in days.
- Type: _Number_
- Limitations: Integer, greater or equal to 0, only either 'EndDate' or 'DurationDays' can be set at once, not both
- Example: `20`

**`ProgressionStatus`** - Indicates whether disease was progressing after treatment or not.
- Type: _Boolean_
- Example: `false`

**`ProgressionStatusChangeDate`** - Date, when progression status was last revised.
- Note: It's hidden and protected. Relative date is shown instead, if calculation was possible.
- Type: _String_
- Format: "YYYY-MM-DDTHH-MM-SS"
- Limitations: Only either 'ProgressionStatusChangeDate' or 'ProgressionStatusChangeDay' can be set at once, not both
- Example: `"2020-02-12T00:00:00"`

**`ProgressionStatusChangeDay`** - Relative number of days since treatment start, when progression status was last revised.
- Type: _Number_
- Limitations: Integer, greater or equal to 0, only either 'ProgressionStatusChangeDate' or 'ProgressionStatusChangeDay' can be set at once, not both
- Example: `37`

**`Results`*** - Patient specific therapy results.
- Type: _String_
- Example: `"Patient specific therapy results."`

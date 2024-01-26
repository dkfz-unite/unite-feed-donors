# Clinical Data Model
Includes patient clinical data.

>[!NOTE]
> All exact dates are hiddent and protected. Relative dates are shown instead, if calculation was possible.

**`sex`** - Donor sex.
- Type: _String_
- Possible values: `"Female"`, `"Male"`, `"Other"`
- Example: `"Other"`

**`age`** - Donor age at diagnosis.
- Type: _Number_
- Limitations: Integer, greater or equal to 0
- Example: `56`

**`diagnosis`*** - Donor diagnosis.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"Glioblastoma"`

**`diagnosis_date`** - Date, when diagnosis was stated.
- Note: **Serves as anchor date for calculation of all other relative dates. Without this date, no other relative dates will be calculated and shown.**
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
- Type: _String_
- Format: "YYYY-MM-DD"
- Limitations: Only either 'vital_status_change_date' or 'vital_status_change_day' can be set at once, not both
- Example: `"2021-01-01"`

**`vital_status_change_day`** - Relative number of days since diagnosis statement, when vital status was last revised.
- Type: _Number_
- Limitations: Integer, greater or equal to 1, only either 'vital_status_change_date' or 'vital_status_change_day' can be set at once, not both
- Example: `367`

**`progression_status`** - Indicates whether disease was progressing after treatment or not.
- Type: _Boolean_
- Example: `false`

**`progression_status_change_date`** - Date, when progression status was last revised.
- Type: _String_
- Format: "YYYY-MM-DD"
- Limitations: Only either 'progression_status_change_date' or 'progression_status_change_day' can be set at once, not both
- Example: `"2020-02-12"`

**`progression_status_change_day`** - Relative number of days since treatment start, when progression status was last revised.
- Type: _Number_
- Limitations: Integer, greater or equal to 1, only either 'progression_status_change_date' or 'progression_status_change_day' can be set at once, not both
- Example: `37`

**`kps_baseline`** - KPS baseline.
- Type: _Number_
- Limitations: Integer, greater or equal to 0, less or equal to 100
- Example: `90`

**`steroids_baseline`** - Steroids baseline.
- Type: _Boolean_
- Example: `false`

##
**`*`** - Required fields

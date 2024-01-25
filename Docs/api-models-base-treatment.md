# Treatment Model
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

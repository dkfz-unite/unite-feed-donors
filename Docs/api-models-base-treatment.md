# Treatment Model
Includes patient treatment data.

>[!NOTE]
> All exact dates are hiddent and protected. Relative dates are shown instead, if calculation was possible.

**`therapy`*** - Therapy name.
- Type: _String_
- Limitations: Maximum length 100
- Example: `"Radiotherapy"`

**`details`** - Patient specific therapy details.
- Type: _String_
- Eample: `"Patient specific therapy details."`

**`start_date`** - Date, when treatment has started.
- Type: _String_
- Format: "YYYY-MM-DD"
- Limitations: Only either 'start_date' or 'start_day' can be set at once, not both
- Example: `"2020-01-07"`

**`start_day`** - Relative number of days since diagnosis statement, when treatment has started.
- Type: _Number_
- Limitations: Integer, greater or equal to 1, only either 'start_date' or 'start_day' can be set at once, not both
- Example: `7`

**`end_date`** - Date, when treatment has ended.
- Type: _String_
- Format: "YYYY-MM-DD"
- Limitations: Can not be set, if 'start_day' was set, only either 'end_date' or 'duration_days' can be set at once, not both
- Example: `"2020-01-27"`

**`duration_days`** - Treatment duration in days.
- Type: _Number_
- Limitations: Integer, greater or equal to 1, only either 'end_date' or 'duration_days' can be set at once, not both
- Example: `20`

**`results`** - Patient specific therapy results.
- Type: _String_
- Example: `"Patient specific therapy results."`

##
**`*`** - Required fields

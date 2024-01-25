# Treatmen Upload Data Model
Treatment upload data model.

**`donor_id`*** - Donor pseudonymized identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"DO1"`

**`data`*** - Treatments data.
- Type: _Array_
- Element type: _Object([Treatment](api-models-base-treatment.md))_
- Example: `[{...}, {...}]`

##
**`*`** - Required fields

# Donor Data Model
Includes information about patient.

**`id`*** - Donor identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"Donor1"`

**`mta`** - Indicates whether donor data is MTA protected.
- Type: _Boolean_
- Example: `true`

**`projects`** - Projects list, which own donor data.
- Type: _Array_
- Elemet type: _String_
- Element limitations: Maximum length 100
- Limitations: Should contain at least one element
- Example: `["Project1", "Project2"]`

**`studies`** - Studies list, which include donor data.
- Type: _Array_
- Elemet type: _String_
- Element limitations: Maximum length 100
- Limitations: Should contain at least one element
- Example: `["Study1", "Study2"]`

**`clinical_data`** - Donor clinical data.
- Type: _Object([clinical_data](./api-models-base-clinical.md))_
- Example: `{...}`

**`treatments`** - Donor treatments data.
- Type: _Array_
- Element type: _Object([treatment](./api-models-base-treatment.md))_
- Limitations: Should contain at least one element
- Example: `[{...}, {...}]`

##
**`*`** - Required fields

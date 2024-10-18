# Treatmen Data Model
Includes information about patient treatments.

**`donor_id`*** - Donor pseudonymized identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"Donor1"`

**`entries`*** - Treatment entries.
- Type: _Array_
- Element type: _Object([Treatment](api-models-base-treatment.md))_
- Limitations: Should contain at least one element
- Example: `[{...}, {...}]`

##
**`*`** - Required fields

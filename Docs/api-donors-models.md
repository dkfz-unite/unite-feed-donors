# Donor Data Models

## Donor
Includes general data about the patient

**`Id`*** - Donor pseudonymized identifier.

_Type_: String  
_Limitations_: Maximum length 255  
_Example_: `"DO1"`

**`MtaProtected`** - Indicates whether donor data is MTA protected.

**Type**: _Boolean_  
**Example**: `true`


|Field|Required|Type|Limitations|Example|Description|
|:---|:---|:---|:---|:---|:---|
|`Id`|**Yes**|String|MaxLength(255)|`"DO1"`|Donor pseudonymized identifier|
|`MtaProtected`|No|Boolean||`true`|Whether donor data is MTA protected or not|
|`WorkPackages`|No|String[]|MaxLength(100)|`["WP1", "WP2"]`|Work packages list, which own donor data|
|`Studies`|No|String[]|MaxLength(100)|`["ST1", "ST2"]`|Studies list, which include donor data|
|`ClinicalData`|No|[ClinicalData](https://github.com/dkfz-unite/unite-donors-feed/new/main/Docs#clinical-data)||`["{...}"]`|Donor clinical data|
|`Treatments`|No|Treatment[]||`["[{...}]"]`|Donor treatments data|

## Clinical Data
Includes patient clinical data.

_At least one field has to be set._

Field|Required|Type|Limitations|Example|Description|
|:---|:---|:---|:---|:---|:---|
|`Gender`|No|String|Gender|`"Male"`|Donor gender|
|`Age`|No|Int|0 or higher|`56`|Age at diagnosis|
|`Diagnosis`|No|String|MaxlLength(255)|`"Glioblastoma"`|Donor diagnosis|
|`DiagnosisDate`|No|DateTime|"YYYY-MM-DDTHH:MM:SS"|`"2020-01-01T00:00:00"`|Diagnosis statement date|
|`PrimarySite`|No|String|MaxlLength(100)|`"Head"`|Primary site of disease|
|`Localization`|No|String|MaxlLength(100)|`"Left"`|Disease relative location|
|`VitalStatus`|No|Boolean||`true`|Whether donor is still alive|
|`VitalStatusChangeDate`|No|DateTime|"YYYY-MM-DDTHH:MM:SS"|`"2021-01-01T00:00:00"`|Date when vital status was last revised|
|`VitalStatusChangeDay`|No|Int|0 or higher|`365`|Day relative to diagnosis date when vital status was last revised|

- `DiagnosisDate` - serves as anchor date for calculation of all other relative dates, it's hidden and protected.

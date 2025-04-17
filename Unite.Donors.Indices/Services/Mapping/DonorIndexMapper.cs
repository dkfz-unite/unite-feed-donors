using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Donors;
using Unite.Indices.Entities.Basic.Projects;

namespace Unite.Donors.Indices.Services.Mapping;

public class DonorIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Donor entity) where T : DonorIndex, new()
    {
        if (entity == null)
        {
            return null;
        }

        var index = new T();

        Map(entity, index);

        return index;
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    public static void Map(in Donor entity, DonorIndex index)
    {
        if (entity == null || index == null)
        {
            return;
        }

        var enrollmentDate = entity.ClinicalData?.EnrollmentDate;

        index.Id = entity.Id;
        index.ReferenceId = entity.ReferenceId;
        index.MtaProtected = entity.MtaProtected;

        index.ClinicalData = CreateFrom(entity.ClinicalData, enrollmentDate);
        index.Treatments = CreateFrom(entity.Treatments, enrollmentDate);
        index.Projects = CreateFrom(entity.DonorProjects);
        index.Studies = CreateFrom(entity.DonorStudies);
    }


    private static ClinicalDataIndex CreateFrom(in ClinicalData entity, DateOnly? enrollmentDate)
    {
        if (entity == null)
        {
            return null;
        }

        return new ClinicalDataIndex
        {
            Sex = entity.SexId?.ToDefinitionString(),
            Age = entity.EnrollmentAge,
            Diagnosis = entity.Diagnosis,
            PrimarySite = entity.PrimarySite?.Value,
            Localization = entity.Localization?.Value,
            VitalStatus = entity.VitalStatus,
            VitalStatusChangeDay = entity.VitalStatusChangeDay ?? entity.VitalStatusChangeDate?.RelativeFrom(enrollmentDate),
            ProgressionStatus = entity.ProgressionStatus,
            ProgressionStatusChangeDay = entity.ProgressionStatusChangeDay ?? entity.ProgressionStatusChangeDate?.RelativeFrom(enrollmentDate),
            SteroidsReactive = entity.SteroidsReactive,
            Kps = entity.Kps
        };
    }

    private static TreatmentIndex[] CreateFrom(in IEnumerable<Treatment> entities, DateOnly? enrollmentDate)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            var index = new TreatmentIndex
            {
                Therapy = entity.Therapy.Name,
                Details = entity.Details,
                StartDay = entity.StartDay ?? entity.StartDate?.RelativeFrom(enrollmentDate),
                DurationDays = entity.DurationDays ?? entity.EndDate?.RelativeFrom(entity.StartDate),
                Results = entity.Results
            };

            return index;

        }).ToArray();
    }

    private static ProjectIndex[] CreateFrom(in IEnumerable<ProjectDonor> entities)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            var index = new ProjectIndex
            {
                Id = entity.Project.Id,
                Name = entity.Project.Name
            };

            return index;

        }).ToArray();
    }

    private static StudyIndex[] CreateFrom(in IEnumerable<StudyDonor> entities)
    {
        if (entities.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            var index = new StudyIndex
            {
                Id = entity.Study.Id,
                Name = entity.Study.Name
            };

            return index;

        }).ToArray();
    }
}

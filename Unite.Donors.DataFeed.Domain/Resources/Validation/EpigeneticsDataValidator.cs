using FluentValidation;

namespace Unite.Donors.DataFeed.Domain.Resources.Validation
{
    public class EpigeneticsDataValidator : AbstractValidator<EpigeneticsData>
    {
        public EpigeneticsDataValidator()
        {
            RuleFor(epigeneticsData => epigeneticsData.IdhMutationId)
                .Empty()
                .When(epigeneticsData => epigeneticsData.IdhStatusId != Data.Entities.Epigenetics.Enums.IDHStatus.Mutant)
                .WithMessage("IDH mutation can be set only if IDH status is 'Mutant'");

            RuleFor(epigeneticsData => epigeneticsData.MethylationSubtypeId)
                .Empty()
                .When(epigeneticsData => epigeneticsData.MethylationStatusId != Data.Entities.Epigenetics.Enums.MethylationStatus.Methylated)
                .WithMessage("Methylation type can be set only if methylation status is 'Methylated'");

            RuleFor(epigeneticsData => epigeneticsData)
                .Must(AtLeastOneFiledIsSet)
                .WithMessage("At least one field has to be set.");
        }

        private bool AtLeastOneFiledIsSet(EpigeneticsData epigeneticsData)
        {
            return epigeneticsData.GeneExpressionSubtypeId != null ||
                   epigeneticsData.IdhStatusId != null ||
                   epigeneticsData.MethylationStatusId != null ||
                   epigeneticsData.GcimpMethylation != null;
        }
    }
}

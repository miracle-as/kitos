using System.Data.Entity;
using Core.DomainModel;
using Core.DomainModel.ItContract;
using Core.DomainModel.ItProject;
using Core.DomainModel.ItSystem;
using Infrastructure.DataAccess.Mapping;

namespace Infrastructure.DataAccess
{
    public class KitosContext : DbContext
    {
        static KitosContext()
        {

        }

        public KitosContext()
            : base("Name=KitosContext")
        {
        }

        public DbSet<AdminRight> AdminRights { get; set; }
        public DbSet<AdminRole> AdminRoles { get; set; }
        public DbSet<Advice> Advices { get; set; }
        public DbSet<AgreementElement> AgreementElements { get; set; }
        public DbSet<AppType> AppTypes { get; set; }
        public DbSet<ArchiveType> ArchiveTypes { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<BusinessType> BusinessTypes { get; set; }
        public DbSet<Communication> Communications { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<ContractTemplate> ContractTemplates { get; set; }
        public DbSet<ContractType> ContractTypes { get; set; }
        public DbSet<CustomAgreementElement> CustomAgreementElements { get; set; }
        public DbSet<DataType> DataTypes { get; set; }
        public DbSet<DataRow> DataRows { get; set; }
        public DbSet<DataRowUsage> DataRowUsages { get; set; }
        public DbSet<Economy> Economies { get; set; }
        public DbSet<EconomyYear> EconomyYears { get; set; }
        public DbSet<EconomyStream> EconomyStrams { get; set; }
        public DbSet<ExtReference> ExtReferences { get; set; }
        public DbSet<ExtReferenceType> ExtReferenceTypes { get; set; }
        public DbSet<ExtRefTypeLocale> ExtRefTypeLocales { get; set; }
        public DbSet<Frequency> Frequencies { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<GoalStatus> GoalStatus { get; set; }
        public DbSet<GoalType> GoalTypes { get; set; }
        public DbSet<Handover> Handovers { get; set; }
        public DbSet<HandoverTrial> HandoverTrials { get; set; }
        public DbSet<Hierarchy> Hierarchies { get; set; }
        public DbSet<Interface> Interfaces { get; set; }
        public DbSet<InterfaceUsage> InterfaceUsages { get; set; }
        public DbSet<InterfaceExposure> InterfaceExposure { get; set; }
        public DbSet<InterfaceType> InterfaceTypes { get; set; }
        public DbSet<InterfaceCategory> InterfaceCategories { get; set; }
        public DbSet<ItContractModuleName> ItContractModuleNames { get; set; }
        public DbSet<ItContract> ItContracts { get; set; }
        public DbSet<ItContractRight> ItContractRights { get; set; }
        public DbSet<ItContractRole> ItContractRoles { get; set; }
        public DbSet<ItProjectModuleName> ItProjectModuleNames { get; set; }
        public DbSet<ItProject> ItProjects { get; set; }
        public DbSet<ItProjectRight> ItProjectRights { get; set; }
        public DbSet<ItProjectRole> ItProjectRoles { get; set; }
        public DbSet<ItSupportModuleName> ItSupportModuleNames { get; set; }
        public DbSet<ItSystemModuleName> ItSystemModuleNames { get; set; }
        public DbSet<ItSystem> ItSystems { get; set; }
        public DbSet<ItSystemUsage> ItSystemUsages { get; set; }
        public DbSet<ItSystemRight> ItSystemRights { get; set; }
        public DbSet<ItSystemRole> ItSystemRoles { get; set; }
        public DbSet<Localization> Localizations { get; set; }
        public DbSet<Method> Methods { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<OptionExtend> OptionExtention { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrgTab> OrgTabs { get; set; }
        public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
        public DbSet<OrganizationRight> OrganizationRights { get; set; }
        public DbSet<OrganizationRole> OrganizationRoles { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
        public DbSet<PaymentFreqency> PaymentFreqencies { get; set; }
        public DbSet<PaymentMilestone> PaymentMilestones { get; set; }
        public DbSet<PaymentModel> PaymentModels { get; set; }
        public DbSet<PriceRegulation> PriceRegulations { get; set; }
        public DbSet<ProcurementStrategy> ProcurementStrategies { get; set; }
        public DbSet<ItProjectCategory> ProjectCategories { get; set; }
        public DbSet<ProjectPhase> ProjectPhases { get; set; }
        public DbSet<ProjPhaseLocale> ProjectPhaseLocales { get; set; }
        public DbSet<ProjectStatus> ProjectStatus { get; set; }
        public DbSet<ItProjectType> ProjectTypes { get; set; }
        public DbSet<PurchaseForm> PurchaseForms { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Risk> Risks { get; set; }
        public DbSet<SensitiveDataType> SensitiveDataTypes { get; set; }
        public DbSet<Stakeholder> Stakeholders { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<TerminationDeadline> TerminationDeadlines { get; set; }
        public DbSet<TaskRef> TaskRefs { get; set; }
        public DbSet<TaskUsage> TaskUsages { get; set; }
        public DbSet<Text> Texts { get; set; }
        public DbSet<Tsa> Tsas { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Wish> Wishes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AdminRightMap());
            modelBuilder.Configurations.Add(new AdminRoleMap());
            modelBuilder.Configurations.Add(new AdviceMap());
            modelBuilder.Configurations.Add(new AgreementElementMap());
            modelBuilder.Configurations.Add(new AppTypeMap());
            modelBuilder.Configurations.Add(new ActivityMap());
            modelBuilder.Configurations.Add(new ArchiveTypeMap());
            modelBuilder.Configurations.Add(new BusinessTypeMap());
            modelBuilder.Configurations.Add(new CommunicationMap());
            modelBuilder.Configurations.Add(new ConfigMap());
            modelBuilder.Configurations.Add(new ContractTemplateMap());
            modelBuilder.Configurations.Add(new ContractTypeMap());
            modelBuilder.Configurations.Add(new DataTypeMap());
            modelBuilder.Configurations.Add(new DataRowMap());
            modelBuilder.Configurations.Add(new DataRowUsageMap());
            modelBuilder.Configurations.Add(new EconomyMap());
            modelBuilder.Configurations.Add(new EconomyStreamMap());
            modelBuilder.Configurations.Add(new EconomyYearMap());
            modelBuilder.Configurations.Add(new ExtReferenceMap());
            modelBuilder.Configurations.Add(new ExtReferenceTypeMap());
            modelBuilder.Configurations.Add(new ExtRefTypeLocaleMap());
            modelBuilder.Configurations.Add(new FrequencyMap());
            modelBuilder.Configurations.Add(new GoalMap());
            modelBuilder.Configurations.Add(new GoalStatusMap());
            modelBuilder.Configurations.Add(new GoalTypeMap());
            modelBuilder.Configurations.Add(new HandoverMap());
            modelBuilder.Configurations.Add(new HandoverTrialMap());
            modelBuilder.Configurations.Add(new HierarchyMap());
            modelBuilder.Configurations.Add(new InterfaceMap());
            modelBuilder.Configurations.Add(new InterfaceUsageMap());
            modelBuilder.Configurations.Add(new InterfaceExposureMap());
            modelBuilder.Configurations.Add(new InterfaceTypeMap());
            modelBuilder.Configurations.Add(new InterfaceCategoryMap());
            modelBuilder.Configurations.Add(new ItContractMap());
            modelBuilder.Configurations.Add(new ItContractRightMap());
            modelBuilder.Configurations.Add(new ItContractRoleMap());
            modelBuilder.Configurations.Add(new ItContractModuleNameMap());
            modelBuilder.Configurations.Add(new ItProjectMap());
            modelBuilder.Configurations.Add(new ItProjectRightMap());
            modelBuilder.Configurations.Add(new ItProjectRoleMap());
            modelBuilder.Configurations.Add(new ItProjectModuleNameMap());
            modelBuilder.Configurations.Add(new ItSupportModuleNameMap());
            modelBuilder.Configurations.Add(new ItSystemMap());
            modelBuilder.Configurations.Add(new ItSystemUsageMap());
            modelBuilder.Configurations.Add(new ItSystemRightMap());
            modelBuilder.Configurations.Add(new ItSystemRoleMap());
            modelBuilder.Configurations.Add(new ItSystemModuleNameMap());
            modelBuilder.Configurations.Add(new LocalizationMap());
            modelBuilder.Configurations.Add(new MethodMap());
            modelBuilder.Configurations.Add(new MilestoneMap());
            modelBuilder.Configurations.Add(new OrganizationMap());
            modelBuilder.Configurations.Add(new OrgTabMap());
            modelBuilder.Configurations.Add(new OrganizationUnitMap());
            modelBuilder.Configurations.Add(new OrganizationRightMap());
            modelBuilder.Configurations.Add(new OrganizationRoleMap());
            modelBuilder.Configurations.Add(new PasswordResetRequestMap());
            modelBuilder.Configurations.Add(new ItProjectCategoryMap());
            modelBuilder.Configurations.Add(new ProcurementStrategyMap());
            modelBuilder.Configurations.Add(new ProjectPhaseMap());
            modelBuilder.Configurations.Add(new ProjectPhaseLocaleMap());
            modelBuilder.Configurations.Add(new ProjectStatusMap());
            modelBuilder.Configurations.Add(new ItProjectTypeMap());
            modelBuilder.Configurations.Add(new PurchaseFormMap());
            modelBuilder.Configurations.Add(new ResourcesMap());
            modelBuilder.Configurations.Add(new RiskMap());
            modelBuilder.Configurations.Add(new SensitiveDataTypeMap());
            modelBuilder.Configurations.Add(new StakeholderMap());
            modelBuilder.Configurations.Add(new StateMap());
            modelBuilder.Configurations.Add(new TaskRefMap());
            modelBuilder.Configurations.Add(new TaskUsageMap());
            modelBuilder.Configurations.Add(new TextMap());
            modelBuilder.Configurations.Add(new TsaMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new WishMap());
        }
    }
}

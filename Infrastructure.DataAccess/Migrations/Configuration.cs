using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainModel.ItContract;
using Core.DomainModel.ItProject;
using Core.DomainModel.ItSystem;

namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Infrastructure.DataAccess.KitosContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());

            //Use a smaller key size for our migration history table. See MySqlHistoryContext.cs
            SetHistoryContextFactory("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));
        }

        protected override void Seed(Infrastructure.DataAccess.KitosContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var cryptoService = new CryptoService();

            #region AdminRoles
            var globalAdmin = new Role { Name = "GlobalAdmin" };
            var localAdmin = new Role { Name = "LocalAdmin" };

            context.Roles.AddOrUpdate(x => x.Name, globalAdmin, localAdmin);
            
            #endregion

            #region Drop Down Data

            context.ProjectCategories.AddOrUpdate(x => x.Name,
                                             new ProjectCategory() { IsActive = true, Note = "...", Name = "Light" },
                                             new ProjectCategory() { IsActive = true, Note = "...", Name = "Lokal" },
                                             new ProjectCategory() { IsActive = true, Note = "...", Name = "Tv�rkommunalt" },
                                             new ProjectCategory() { IsActive = true, Note = "...", Name = "SKAL" });

            context.ProjectTypes.AddOrUpdate(x => x.Name,
                                             new ProjectType() { IsActive = true, Note = "...", Name = "IT Projekt" },
                                             new ProjectType() { IsActive = true, Note = "...", Name = "IT Program" },
                                             new ProjectType() { IsActive = true, Note = "En samlebetegnelse for projekter, som ikke er et IT Program", Name = "Indsatsomr�de" });

            context.SystemTypes.AddOrUpdate(x => x.Name,
                                            new SystemType() { IsActive = true, Note = "...", Name = "Fag" },
                                            new SystemType() { IsActive = true, Note = "...", Name = "ESDH" },
                                            new SystemType() { IsActive = true, Note = "...", Name = "St�ttesystemer" });

            context.InterfaceTypes.AddOrUpdate(x => x.Name,
                                               new InterfaceType() { IsActive = true, Note = "...", Name = "WS" });

            context.ProtocolTypes.AddOrUpdate(x => x.Name,
                                              new ProtocolType() { IsActive = true, Note = "...", Name = "OIORES" },
                                              new ProtocolType() { IsActive = true, Note = "...", Name = "WS SOAP" });

            context.Methods.AddOrUpdate(x => x.Name,
                                        new Method() { IsActive = true, Note = "...", Name = "Batch" },
                                        new Method() { IsActive = true, Note = "...", Name = "Request-Response" });

            context.DatabaseTypes.AddOrUpdate(x => x.Name,
                                              new DatabaseType() { IsActive = true, Note = "...", Name = "MSSQL" },
                                              new DatabaseType() { IsActive = true, Note = "...", Name = "MySQL" });

            context.Environments.AddOrUpdate(x => x.Name,
                                             new Core.DomainModel.ItSystem.Environment()
                                             {
                                                 IsActive = true,
                                                 Note = "...",
                                                 Name = "Citrix"
                                             });

            context.ContractTypes.AddOrUpdate(x => x.Name,
                                              new ContractType() { IsActive = true, Note = "...", Name = "Hovedkontrakt" },
                                              new ContractType()
                                              {
                                                  IsActive = true,
                                                  Note = "...",
                                                  Name = "Til�gskontrakt"
                                              },
                                              new ContractType() { IsActive = true, Note = "...", Name = "Snitflade" },
                                              new ContractType() { IsActive = false, Note = "...", Name = "Tidligere aktiv kontrakttype" },
                                              new ContractType() { IsSuggestion = true, Note = "...", Name = "Forslag1" },
                                              new ContractType() { IsSuggestion = true, Note = "...", Name = "Forslag2" });

            context.ContractTemplates.AddOrUpdate(x => x.Name,
                                                  new ContractTemplate() { IsActive = true, Note = "...", Name = "K01" },
                                                  new ContractTemplate() { IsActive = true, Note = "...", Name = "K02" },
                                                  new ContractTemplate() { IsActive = true, Note = "...", Name = "K03" });

            context.PurchaseForms.AddOrUpdate(x => x.Name,
                                              new PurchaseForm() { IsActive = true, Note = "...", Name = "SKI" },
                                              new PurchaseForm() { IsActive = true, Note = "...", Name = "SKI 02.19" },
                                              new PurchaseForm() { IsActive = true, Note = "...", Name = "Udbud" });

            context.PaymentModels.AddOrUpdate(x => x.Name,
                                              new PaymentModel() { IsActive = true, Note = "...", Name = "Licens" });

            context.SaveChanges();

            #endregion

            #region Global municipality

            var globalMunicipality = new Municipality()
                {
                    Name = "F�lleskommune"
                };

            context.Municipalities.AddOrUpdate(x => x.Name, globalMunicipality);

            var itSupportName = new ItSupportModuleName() {Name = "IT Underst�ttelse"};
            context.ItSupportModuleNames.AddOrUpdate(x => x.Name,
                new ItSupportModuleName() {Name = "IT Underst�ttelse af organisation"},
                itSupportName,
                new ItSupportModuleName() {Name = "Organisation"});

            var itProjectName = new ItProjectModuleName() {Name = "IT Projekter"};
            context.ItProjectModuleNames.AddOrUpdate(x => x.Name, 
                itProjectName,
                new ItProjectModuleName() {Name = "Projekter"});

            var itSystemName = new ItSystemModuleName() {Name = "IT Systemer"};
            context.ItSystemModuleNames.AddOrUpdate(x => x.Name,
                itSystemName,
                new ItSystemModuleName() {Name = "Systemer"});

            var itContractName = new ItContractModuleName() {Name = "IT Kontrakter"};
            context.ItContractModuleNames.AddOrUpdate(x => x.Name,
                itContractName,
                new ItContractModuleName() {Name = "Kontrakter"});

            context.Configs.AddOrUpdate(x => x.Id,
                new Core.DomainModel.Config() {
                    Municipality = globalMunicipality,
                    ItContractModuleName = itContractName,
                    ItProjectModuleName = itProjectName,
                    ItSupportModuleName = itSupportName,
                    ItSystemModuleName = itSystemName,
                    ShowItContractModule = true,
                    ShowItProjectModule = true,
                    ShowItSystemModule = true,
                    ItSupportGuide = ".../itunderst�ttelsesvejledning",
                    ItProjectGuide = ".../itprojektvejledning",
                    ItSystemGuide = ".../itsystemvejledning",
                    ItContractGuide = ".../itkontraktvejledning"
                });

            context.SaveChanges();

            #endregion

            var department = new Department()
                {
                    Name = "Test organisation"
                };

            context.Departments.AddOrUpdate(d => d.Id, department);

            #region Department roles

            var boss = new DepartmentRole()
                {
                    IsActive = true,
                    Name = "Chef",
                    Note = "Lederen af en organisationsenhed"
                };

            var resourcePerson = new DepartmentRole()
                {
                    IsActive = true,
                    Name = "Ressourceperson",
                    Note = "..."
                };

            context.DepartmentRoles.AddOrUpdate(role => role.Id,
                                                boss, resourcePerson,
                                                new DepartmentRole() {IsActive = false, Name = "Inaktiv Org rolle"},
                                                new DepartmentRole()
                                                    {
                                                        IsActive = false,
                                                        IsSuggestion = true,
                                                        Name = "Forslag til org rolle"
                                                    }
                );

            #endregion
            
            #region Project roles
            
            context.ItProjectRoles.AddOrUpdate(r => r.Id,
                                               new ItProjectRole() {IsActive = true, Name = "Projektejer"},
                                               new ItProjectRole() {IsActive = true, Name = "Projektleder"},
                                               new ItProjectRole() {IsActive = true, Name = "Delprojektleder"},
                                               new ItProjectRole() {IsActive = true, Name = "Projektdeltager"},
                                               new ItProjectRole() {IsActive = false, Name = "Inaktiv projektrolle"},
                                               new ItProjectRole()
                                                   {
                                                       IsActive = false,
                                                       IsSuggestion = true,
                                                       Name = "Foresl�et projektrolle"
                                                   }
                );

            #endregion
            
            #region Users

            var simonSalt = cryptoService.Encrypt("simonsalt");
            var simon = new User
            {
                Name = "Simon Lynn-Pedersen",
                Email = "slp@it-minds.dk",
                Salt = simonSalt,
                Password = cryptoService.Encrypt("slp123" + simonSalt),
                Role = globalAdmin,
                Municipality = globalMunicipality
            };

            var eskildSalt = cryptoService.Encrypt("eskildsalt");
            var eskild = new User()
                {
                    Name = "Eskild",
                    Email = "esd@it-minds.dk",
                    Salt = eskildSalt,
                    Password = cryptoService.Encrypt("arne123" + eskildSalt),
                    Role = localAdmin,
                    Municipality = globalMunicipality
                };

            context.Users.AddOrUpdate(x => x.Email, simon);

            context.SaveChanges();

            #endregion

            var simonDepartmentRight = new DepartmentRight()
                {
                    Object = department,
                    Role = boss,
                    User = simon
                };

            var eskildDepartmentRight = new DepartmentRight()
                {
                    Object = department,
                    Role = resourcePerson,
                    User = eskild
                };

            context.DepartmentRights.AddOrUpdate(r => new {r.Object_Id, r.Role_Id, r.User_Id}, simonDepartmentRight, eskildDepartmentRight);

            #region Password Reset Requests

            /*
            var simonId = context.Users.Single(x => x.Email == "slp@it-minds.dk").Id;

            context.PasswordResetRequests.AddOrUpdate(x => x.Id,
                                                      new PasswordResetRequest
                                                      {
                                                          //This reset request is fine
                                                          Id = "workingRequest", //ofcourse, this should be a hashed string or something obscure
                                                          Time = DateTime.Now.AddYears(+20), //.MaxValue also seems to be out-of-range, but this should hopefully be good enough
                                                          User_Id = simonId
                                                      }
                );

             */
            #endregion

            #region Texts

            context.Texts.AddOrUpdate(x => x.Id,
                                      new Text() {Id = "intro-head", Value = "Head"},
                                      new Text() {Id = "intro-body", Value = "Body"});

            #endregion

            var extRef1 = new ExtReferenceType()
                {
                    IsActive = true,
                    Name = "ESDH Ref",
                    Note = "Ref. til ESDH system, hvor der er projektdokumenter"
                };
            var extRef2 = new ExtReferenceType()
                {
                    IsActive = true,
                    Name = "CMDB Ref",
                    Note = "Ref. til CMDB o.l system, hvor der er projektdokumenter"
                };
            var extRef3 = new ExtReferenceType()
                {
                    IsActive = true,
                    Name = "Mappe Ref",
                    Note = "Ref. til andre steder, hvor der er projektdokumenter"
                };
            context.ExtReferenceTypes.AddOrUpdate(x => x.Name, extRef1, extRef2, extRef3);

            var projPhase1 = new ProjectPhase()
                {
                    IsActive = true,
                    Name = "Afventer",
                    Note = "..."
                };

            context.ProjectPhases.AddOrUpdate(x => x.Name, projPhase1,
                new ProjectPhase()
                    {
                        IsActive = true,
                        Name = "Foranalyse",
                        Note = "..."
                    },
                new ProjectPhase()
                    {
                        IsActive = true,
                        Name = "Gennemf�rsel",
                        Note = "..."
                    },
                new ProjectPhase()
                    {
                        IsActive = true,
                        Name = "Overlevering",
                        Note = "..."
                    },
                new ProjectPhase()
                    {
                        IsActive = true,
                        Name = "Drift",
                        Note = "..."
                    }
                );

            context.ProjectPhaseLocales.AddOrUpdate(x => new {x.Municipality_Id, x.Original_Id},
                                                    new ProjPhaseLocale()
                                                        {
                                                            Municipality = globalMunicipality,
                                                            Original = projPhase1,
                                                            Name = "Pending"
                                                        }
                );

            base.Seed(context);
        }
    }
}
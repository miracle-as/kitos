﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainModel.ItSystem;
using Core.DomainServices;
using Newtonsoft.Json.Linq;
using UI.MVC4.Models;

namespace UI.MVC4.Controllers.API
{
    public class ItInterfaceController : GenericApiController<ItInterface, ItInterfaceDTO>
    {
        public ItInterfaceController(IGenericRepository<ItInterface> repository) 
            : base(repository)
        {
        }

        public HttpResponseMessage GetSearch(string q, int orgId)
        {
            try
            {
                var interfaces = Repository.Get(
                    s =>
                        // filter by name
                        s.Name.Contains(q) &&
                        // global admin sees all within the context 
                        (KitosUser.IsGlobalAdmin &&
                         s.OrganizationId == orgId ||
                         // object owner sees his own objects     
                         s.ObjectOwnerId == KitosUser.Id ||
                         // it's public everyone can see it
                         s.AccessModifier == AccessModifier.Public ||
                         // everyone in the same organization can see normal objects
                         s.AccessModifier == AccessModifier.Normal &&
                         s.OrganizationId == orgId)
                        // it systems doesn't have roles so private doesn't make sense
                        // only object owners will be albe to see private objects
                    );
                var dtos = Map(interfaces);
                return Ok(dtos);
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        public HttpResponseMessage GetCatalog(string q, int organizationId, [FromUri] PagingModel<ItInterface> pagingModel)
        {
            try
            {
                pagingModel.Where(s =>
                    // global admin sees all within the context 
                    KitosUser.IsGlobalAdmin &&
                    s.OrganizationId == organizationId ||
                    // object owner sees his own objects     
                    s.ObjectOwnerId == KitosUser.Id ||
                    // it's public everyone can see it
                    s.AccessModifier == AccessModifier.Public ||
                    // everyone in the same organization can see normal objects
                    s.AccessModifier == AccessModifier.Normal &&
                    s.OrganizationId == organizationId
                    // it systems doesn't have roles so private doesn't make sense
                    // only object owners will be albe to see private objects
                    );

                if (!string.IsNullOrEmpty(q)) pagingModel.Where(s => s.Name.Contains(q));

                var interfaces = Page(Repository.AsQueryable(), pagingModel);

                var dtos = Map(interfaces);
                return Ok(dtos);
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        public HttpResponseMessage GetExcel(bool? csv, int organizationId)
        {
            try
            {
                var interfaces = Repository.Get(
                    x =>
                        // global admin sees all within the context 
                        KitosUser.IsGlobalAdmin &&
                        x.OrganizationId == organizationId ||
                        // object owner sees his own objects     
                        x.ObjectOwnerId == KitosUser.Id ||
                        // it's public everyone can see it
                        x.AccessModifier == AccessModifier.Public ||
                        // everyone in the same organization can see normal objects
                        x.AccessModifier == AccessModifier.Normal &&
                        x.OrganizationId == organizationId
                        // it systems doesn't have roles so private doesn't make sense
                        // only object owners will be albe to see private objects
                    );
                var dtos = Map(interfaces);
                
                var list = new List<dynamic>();
                var header = new ExpandoObject() as IDictionary<string, Object>;
                header.Add("Snitflade", "Snitflade");
                header.Add("Public", "(P)");
                header.Add("Snitfladetype", "Snitfladetype");
                header.Add("Interface", "Grænseflade");
                header.Add("Metode", "Metode");
                header.Add("TSA", "TSA");
                header.Add("Udstillet af", "Udstillet af");
                header.Add("Rettighedshaver", "Rettighedshaver");
                header.Add("Oprettet af", "Oprettet af");
                list.Add(header);
                foreach (var itInterface in dtos)
                {
                    var obj = new ExpandoObject() as IDictionary<string, Object>;
                    obj.Add("Snitflade", itInterface.Name);
                    obj.Add("Public", itInterface.AccessModifier == AccessModifier.Public ? "(P)" : "");
                    obj.Add("Snitfladetype", itInterface.InterfaceTypeName);
                    obj.Add("Interface", itInterface.InterfaceName);
                    obj.Add("Metode", itInterface.MethodName);
                    obj.Add("TSA", itInterface.TsaName);
                    obj.Add("Udstillet af", itInterface.ExhibitedByItSystemName);
                    obj.Add("Rettighedshaver", itInterface.BelongsToName);
                    obj.Add("Oprettet af", itInterface.OrganizationName);
                    list.Add(obj);
                }
                var s = list.ToCsv();
                var bytes = Encoding.Unicode.GetBytes(s);
                var stream = new MemoryStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);

                var result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "snitfladekatalog.csv" };
                return result;
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        /// <summary>
        /// Get interfaces by name that aren't already used by the system in question
        /// </summary>
        /// <param name="q"></param>
        /// <param name="orgId"></param>
        /// <param name="sysId"></param>
        /// <returns>Available interfaces</returns>
        public HttpResponseMessage GetSearchExclude(string q, int orgId, int sysId)
        {
            try
            {
                var interfaces = Repository.Get(
                    s =>
                        // filter by name
                        s.Name.Contains(q) &&
                        // filter (remove) interfaces already used by the system
                        s.CanBeUsedBy.Count(x => x.ItSystemId == sysId) == 0 &&
                        // global admin sees all within the context 
                        (KitosUser.IsGlobalAdmin &&
                         s.OrganizationId == orgId ||
                         // object owner sees his own objects     
                         s.ObjectOwnerId == KitosUser.Id ||
                         // it's public everyone can see it
                         s.AccessModifier == AccessModifier.Public ||
                         // everyone in the same organization can see normal objects
                         s.AccessModifier == AccessModifier.Normal &&
                         s.OrganizationId == orgId)
                        // it systems doesn't have roles so private doesn't make sense
                        // only object owners will be albe to see private objects
                    );
                var dtos = Map(interfaces);
                return Ok(dtos);
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        public override HttpResponseMessage Post(ItInterfaceDTO dto)
        {
            try
            {
                if (!IsAvailable(dto.Name, dto.OrganizationId))
                    return Conflict("Name is already taken!");

                var item = Map(dto);

                item.ObjectOwner = KitosUser;
                item.LastChangedByUser = KitosUser;

                foreach (var dataRow in item.DataRows)
                {
                    dataRow.ObjectOwner = KitosUser;
                    dataRow.LastChangedByUser = KitosUser;
                }

                PostQuery(item);

                return Created(Map(item), new Uri(Request.RequestUri + "/" + item.Id));
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        public override HttpResponseMessage Patch(int id, JObject obj)
        {
            // try get name value
            JToken nameToken;
            obj.TryGetValue("name", out nameToken);
            if (nameToken != null)
            {
                var orgId = Repository.GetByKey(id).OrganizationId;
                if (!IsAvailable(nameToken.Value<string>(), orgId))
                    return Conflict("Name is already taken!");
            }

            return base.Patch(id, obj);
        }

        public HttpResponseMessage GetNameAvailable(string checkname, int orgId)
        {
            try
            {
                return IsAvailable(checkname, orgId) ? Ok() : Conflict("Name is already taken!");
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        private bool IsAvailable(string name, int orgId)
        {
            var system = Repository.Get(x => x.Name == name && x.OrganizationId == orgId);
            return !system.Any();
        }
    }
}

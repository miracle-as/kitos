﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AutoMapper;
using Core.DomainModel.ItSystem;
using Core.DomainServices;
using UI.MVC4.Models;

namespace UI.MVC4.Controllers.API
{
    public class ExhibitController : GenericApiController<ItInterfaceExhibit, ItInterfaceExhibitDTO>
    {
        private readonly IGenericRepository<ItInterfaceExhibit> _repository;

        public ExhibitController(IGenericRepository<ItInterfaceExhibit> repository) 
            : base(repository)
        {
            _repository = repository;
        }

        public HttpResponseMessage GetInterfacesBySystem(int sysId, bool? interfaces)
        {
            try
            {
                var exhibits = _repository.Get(x => x.ItSystemId == sysId);
                var intfs = exhibits.Select(x => x.ItInterface);
                var dtos = Mapper.Map<IEnumerable<ItInterfaceDTO>>(intfs);
                return Ok(dtos);
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        public HttpResponseMessage GetBySystem(int sysId, string q)
        {
            try
            {
                var exhibit = _repository.Get(x => x.ItSystemId == sysId && x.ItSystem.Name.Contains(q));
                var dtos = Map(exhibit);
                return Ok(dtos);
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }
    }
}

﻿using Core.DomainModel.Advice;
using Core.DomainServices;
using Presentation.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Web.Controllers.API
{
    public class AdviceUserRelationController : GenericApiController<AdviceUserRelation, AdviceUserRelationDTO>
    {
        IGenericRepository<AdviceUserRelation> _repository;
        public AdviceUserRelationController(IGenericRepository<AdviceUserRelation> repository) : base(repository)
        {
            _repository = repository;
        }

        [HttpDelete]
        public virtual HttpResponseMessage DeleteByAdviceId(int adviceId)
        {
            try
            {
                foreach (var d in _repository.AsQueryable().Where(d => d.AdviceId == adviceId)) {
                    _repository.Delete(d);
                }
                _repository.Save();
                return Ok();
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }


    }
}
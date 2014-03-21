﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http;
using Core.DomainModel;
using Core.DomainServices;
using UI.MVC4.Models;

namespace UI.MVC4.Controllers.API
{
    public abstract class GenericOptionApiController<TModel, TReference> : GenericApiController<TModel, int, OptionDTO>
        where TModel : class, IOptionEntity<TReference>
    {
        protected GenericOptionApiController(IGenericRepository<TModel> repository) : base(repository)
        {
        }

        protected override IEnumerable<TModel> GetAllQuery()
        {
            return this.Repository.Get(t => t.IsActive && !t.IsSuggestion);
        }

        public HttpResponseMessage GetAllSuggestions(bool? suggestions)
        {
            var items = this.Repository.Get(t => t.IsSuggestion);
            
            return Ok(Map<IEnumerable<TModel>, IEnumerable<OptionDTO>>(items));
        }


        public HttpResponseMessage GetAllNonSuggestions(bool? nonsuggestions)
        {
            var items = this.Repository.Get(t => !t.IsSuggestion);

            return Ok(Map<IEnumerable<TModel>, IEnumerable<OptionDTO>>(items));
        }
        
        protected override TModel PostQuery(TModel item)
        {
            if(!item.IsSuggestion && !User.IsInRole("GlobalAdmin"))
                throw new SecurityException();

            return base.PostQuery(item);
        }

        protected override TModel PutQuery(TModel item)
        {
            if (!item.IsSuggestion && !User.IsInRole("GlobalAdmin"))
                throw new SecurityException();

            return base.PutQuery(item);
        }

        [Authorize(Roles = "LocalAdmin, GlobalAdmin")]
        public override HttpResponseMessage Post(OptionDTO dto)
        {
            return base.Post(dto);
        }

        [Authorize(Roles = "LocalAdmin, GlobalAdmin")]
        public override HttpResponseMessage Put(int id, OptionDTO dto)
        {
            return base.Put(id, dto);
        }
    }
}
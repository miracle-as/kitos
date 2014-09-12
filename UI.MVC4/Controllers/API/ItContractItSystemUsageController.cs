﻿using System.Net.Http;
using Core.DomainModel.ItContract;
using Core.DomainModel.ItSystem;
using Core.DomainServices;

namespace UI.MVC4.Controllers.API
{
    public class ItContractItSystemUsageController : BaseApiController
    {
        private readonly IGenericRepository<ItContractItSystemUsage> _repository;
        private readonly IGenericRepository<ItSystemUsage> _usageRepository;

        public ItContractItSystemUsageController(IGenericRepository<ItContractItSystemUsage> repository, IGenericRepository<ItSystemUsage> usageRepository)
        {
            _repository = repository;
            _usageRepository = usageRepository;
        }

        public HttpResponseMessage PostMainContract(int contractId, int usageId)
        {
            var item = _repository.GetByKey(new object[] {contractId, usageId});
            if (item == null)
                return NotFound();

            item.ItSystemUsage.MainContract = item;
            _repository.Save();
            return Ok();
        }

        public HttpResponseMessage DeleteMainContract(int usageId)
        {
            var usage = _usageRepository.GetByKey(usageId);
            if (usage == null)
                return NotFound();
            
            // WARNING: force loading so setting it to null will be tracked
            var forceLoad = usage.MainContract;
            usage.MainContract = null;

            _usageRepository.Save();
            return Ok();
        }
    }
}
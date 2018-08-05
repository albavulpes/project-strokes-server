﻿using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AlbaVulpes.API.Base;
using AlbaVulpes.API.Interfaces;
using AlbaVulpes.API.Models.Resource;
using AlbaVulpes.API.Repositories;

namespace AlbaVulpes.API.Controllers
{
    [Route("arcs")]
    [Produces("application/json")]
    public class ArcController : ApiController<Arc>
    {
        public ArcController(IUnitOfWork unitOfWork, IValidatorService validator) : base(unitOfWork, validator)
        {
        }

        public override async Task<IActionResult> Get(Guid id)
        {
            var arc = await UnitOfWork.GetRepository<Arc>().Get(id);

            if (arc == null)
            {
                return NotFound();
            }

            return Ok(arc);
        }

        public override async Task<IActionResult> Create([FromBody] Arc arc)
        {
            if (arc == null)
            {
                return BadRequest();
            }

            var savedArc = await UnitOfWork.GetRepository<Arc, ArcRepository>().Create(arc);

            if (savedArc == null)
            {
                return BadRequest();
            }

            return CreatedAtAction("Get", new { id = savedArc.Id }, savedArc);
        }

        public override async Task<IActionResult> Update(Guid id, [FromBody] Arc arc)
        {
            if (arc == null)
            {
                return BadRequest();
            }

            var updatedArc = await UnitOfWork.GetRepository<Arc>().Update(id, arc);

            if (updatedArc == null)
            {
                return NotFound();
            }

            return Ok(updatedArc);
        }

        public override async Task<IActionResult> Delete(Guid id)
        {
            var deletedArc = await UnitOfWork.GetRepository<Arc>().Delete(id);

            if (deletedArc == null)
            {
                return NotFound();
            }

            return Ok(deletedArc);
        }
    }
}
﻿using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using AlbaVulpes.API.Base;
using AlbaVulpes.API.Interfaces;
using AlbaVulpes.API.Models.Resource;

namespace AlbaVulpes.API.Controllers
{
    [Route("chapters")]
    [Produces("application/json")]
    public class ChapterController : ApiController<Chapter>
    {
        public ChapterController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override IActionResult Create([FromBody] Chapter chapter)
        {
            if (chapter == null)
            {
                return BadRequest();
            }

            var newChapter = new Chapter
            {
                ChapterNumber = chapter.ChapterNumber,
                Title = chapter.Title,
                Pages = chapter.Pages,
                ArcId = chapter.ArcId
            };

            UnitOfWork.GetRepository<Chapter>().Create(newChapter);

            Response.Headers["ETag"] = newChapter.Hash;

            return CreatedAtAction("Read", new { id = newChapter.Id }, newChapter);
        }

        public override IActionResult Read(Guid id)
        {
            var chapter = UnitOfWork.GetRepository<Chapter>().GetSingle(id);

            if (chapter == null)
            {
                return NotFound();
            }

            var requestHash = Request.Headers["If-None-Match"];

            if (!string.IsNullOrEmpty(requestHash))
            {
                if (requestHash == chapter.Hash)
                {
                    return StatusCode((int)HttpStatusCode.NotModified);
                }
            }

            Response.Headers["ETag"] = chapter.Hash;

            return Ok(chapter);
        }


        public override IActionResult Update(Guid id, [FromBody] Chapter chapter)
        {
            if (chapter == null)
            {
                return BadRequest();
            }

            var updatedChapter = UnitOfWork.GetRepository<Chapter>().Update(id, chapter);

            if (updatedChapter == null)
            {
                return NotFound();
            }

            Response.Headers["ETag"] = updatedChapter.Hash;

            return Ok(updatedChapter);
        }

        public override IActionResult Delete(Guid id)
        {
            var chapterToDelete = UnitOfWork.GetRepository<Chapter>().RemoveSingle(id);

            if (chapterToDelete == null)
            {
                return NotFound();
            }

            return Ok(chapterToDelete);
        }
    }
}


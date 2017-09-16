﻿using System;
using AlbaVulpes.API.Base;
using AlbaVulpes.API.Models.App;

namespace AlbaVulpes.API.Models.Resource
{
    public class Page : ApiModel
    {
        public Guid ChapterId { get; set; }

        public int PageNumber { get; set; }
        public ImageSet Image { get; set; }
    }
}

﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Interface
{
    public interface ICloudinaryProxy
    {
        Task<string> UploadImageAsync(IFormFile file);
       void DeleteImage(string imageUrl);
    }
}

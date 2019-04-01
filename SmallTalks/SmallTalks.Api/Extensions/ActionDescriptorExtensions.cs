using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Versioning;
using System;
using System.Linq;

namespace SmallTalks.Api.Extensions
{
    public static class ActionDescriptorExtensions
    {
        public static ApiVersionModel GetApiVersion(this ActionDescriptor actionDescriptor)
             => actionDescriptor?.Properties
               .Where((kvp) => ((Type)kvp.Key).Equals(typeof(ApiVersionModel)))
               .Select(kvp => kvp.Value as ApiVersionModel).FirstOrDefault();
    }
}
﻿#pragma warning disable 1998

using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Eff.Web.Demo.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly IEffectHandler handler;
        public ValuesController()
        {
            handler = new DefaultEffectHandler();
        }

        // GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        public Task<IEnumerable<string>> Get()
        {
            async Eff<IEnumerable<string>> Get()
            {
                return new string[] { "value1", "value2" };
            }

            return Get().Run(handler);
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}

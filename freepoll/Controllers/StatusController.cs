using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using freepoll.Common;
using freepoll.Models;
using Microsoft.AspNetCore.Mvc;

namespace freepoll.Controllers
{
    [Route("api/status")]
    public class StatusController : Controller
    {
        private readonly FreePollDBContext _dBContext;

        public StatusController(FreePollDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        [HttpGet]
        public List<Status> GetStatus()
        {
            return _dBContext.Status.ToList();
        }

        [HttpGet]
        [Route("{id}")]
        public Status GetStatusById(int id)
        {
            return _dBContext.Status.FirstOrDefault(x => x.Statusid == id);
        }

        [HttpPut]
        public Status PutStatus(Status status)
        {
            _dBContext.Status.Add(status);
            _dBContext.SaveChanges();
            return status;
        }

        [HttpPost]
        public Status UpdateStatus(Status status)
        {
            _dBContext.Status.Update(status);
            _dBContext.SaveChanges();
            return status;
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteStatus(int id)
        {
            try
            {
                Status s = _dBContext.Status.Where(x => x.Statusid == id).FirstOrDefault();
                _dBContext.Remove(s);
                _dBContext.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

        [Route("encrypttest")]
        [HttpGet]
        public IActionResult TestEncrypt(string abc)
        {
            string temp1 = Security.Encrypt(abc);
            return Ok(temp1);
        }

        [Route("decrypttest")]
        [HttpGet]
        public IActionResult TestDecrypt(string abc)
        {
            string temp2 = Security.Decrypt(abc);
            return Ok(temp2);
        }
    }
}
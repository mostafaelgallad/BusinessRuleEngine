using AutoMapper;
using BusinessRuleEngine.Entities;
using BusinessRuleEngine.Helpers;
using FastMember;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessRuleEngine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly TestContext _context;
        private readonly IMapper _mapper;
        public BusinessController(TestContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet("GetExpression")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public ActionResult<string> GetExpression()
        {
            Employee khaled = new Employee()
            {
                Salary = 2000,
                Name = "khaled"
            };
            Employee Mohamed = new Employee()
            {
                Salary = 3000,
                Name = "Mohamed"
            };
            var rules = _context.RuleSetRules.ToList();
            Expression<Func<Employee, bool>> predicate = null;
            if (rules.Count > 0)
            {

                foreach (var rule in rules)
                {
                    var expression = new ExpressionBuilder<Employee>().GenerateExpression(rule);
                    predicate = new ExpressionBuilder<Employee>().CombineExpression(expression, predicate, "AND");
                }
            }
            var d = predicate.Compile()(khaled);
            var s = predicate.Compile()(Mohamed);
            return predicate.ToString();
        }

        [HttpGet("GetListOFContries")]
        [ProducesResponseType(typeof(ContriesFromTestDB), StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<List<ContriesFromTestDB>>> GetListOFContries()
        {
            return await Test.GetListOFContriesFromSql(_context);
        }

        [HttpGet("GetListOFContries2")]
        [ProducesResponseType(typeof(ContriesFromTestDB), StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<List<ContriesFromTestDB>>> GetListOFContries2()
        {
            return await Test.GetListOFContriesFromSql2(_context);
        }
    }
}

using BusinessRuleEngine.Entities;
using BusinessRuleEngine.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public BusinessController(TestContext context)
        {
            _context = context;
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
    }
}

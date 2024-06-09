using webapiemp.Models;
using webapiemp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace webapiemp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<ActionResult<List<User>>> GetAll() => await EmployeeServices.GetUsers(_context);

        //[HttpGet("{id}")]
        //public async Task<ActionResult<User>> GetEmployee(int id)
        //{
        //    var employee = await EmployeeServices.GetUser(id, _context);
        //    if (employee == null)
        //        return NotFound();
        //    return employee;
        //}

       
        //[HttpPost]
        //public async Task<ActionResult> Create(User user)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException(nameof(user));
        //    await EmployeeServices.AddUser(user, _context);
        //    return CreatedAtAction(nameof(Create), new { id = user.Id }, user);
        //}

        //[HttpPut]
        //public async Task<ActionResult> Update(int id, User user)
        //{
        //    if (id != user.Id) return BadRequest();
        //    var existingEmployee = await EmployeeServices.GetUser(id, _context);
        //    if (existingEmployee == null) return NotFound();

        //    await EmployeeServices.UpdateUser(user, _context);
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    var existingEmployee = await EmployeeServices.GetUser(id, _context);
        //    if (existingEmployee == null)
        //        return NotFound();
        //    await EmployeeServices.DeleteUser(id, _context);
        //    return NoContent();
        //}


        
    }
}

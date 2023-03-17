using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;

namespace RKM_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserInterface _userInterface;
        private readonly IMapper _mapper;
        public UserController(UserInterface userInterface, IMapper mapper)
        {
            _userInterface = userInterface;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public IActionResult GetUsers()
        {
            var users = _mapper.Map<List<UserDto>>(_userInterface.GetUsers());
            if(!ModelState.IsValid)
                return BadRequest(ModelState);           
            return Ok(users);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        public IActionResult GetUser(int id)
        {
            if (!_userInterface.UserExist(id))
                return NotFound();

            var user = _mapper.Map<UserDto>( _userInterface.GetUser(id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateUser([FromBody] UserDto userCreate)
        {
            if (userCreate == null)
                return BadRequest(ModelState);

            var user = _userInterface.GetUsers()
                .Where(c => c.UserName.Trim().ToUpper() == userCreate.UserName.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (user != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userMap = _mapper.Map<User>(userCreate);


            if (!_userInterface.CreateUser(userMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }
        [HttpPut("{userId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateUser(int userId, [FromBody] UserDto updatedUser)
        {
            if (updatedUser == null)
                return BadRequest(ModelState);

            if (!_userInterface.UserExist(userId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var userMap = _mapper.Map<User>(updatedUser);

            if (!_userInterface.UpdateUser(userMap))
            {
                ModelState.AddModelError("", "Update Error");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{userId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteUser(int userId)
        {
            if (!_userInterface.UserExist(userId))
            {
                return NotFound();
            }

            var userToDelete = _userInterface.GetUser(userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userInterface.DeleteUser(userToDelete))
            {
                ModelState.AddModelError("", "Delete Error");
            }

            return NoContent();
        }

        [Route("login")]
        [HttpGet]
        public IActionResult LogIn(String userName, String password)
        {               
            var user = _mapper.Map<UserDto>(_userInterface.GetUser(userName));
            if (user == null)
                    return NotFound();

                if (user.Password == password)
                    return Ok(user);
                else
                    return BadRequest();
            
        }


    }

}

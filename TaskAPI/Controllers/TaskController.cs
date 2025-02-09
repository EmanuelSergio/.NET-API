using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskApi.Data;

namespace TaskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController: ControllerBase
    {
        private readonly TaskContext _context;

        public TaskController(TaskContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Task>>> GetTasks(){
            return await _context.Tasks.ToListAsync();
        }

        [HttpGet("hello")]
        public  String Hello(){
            return "Hello";
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Task>> GetTask(int id){
            var task = await _context.Tasks.FindAsync(id);

            if(task == null){
                return NotFound();
            }

            return task;
        }

        [HttpPost] 
        public async Task<ActionResult<Task>> createTask(Task task){

            task.CreatedAt = DateTime.UtcNow;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new {id = task.Id}, task);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Task>> deleteTask(int id ){
            var  task = await _context.FindAsync<Task>(id);
            if(task == null){
                return NotFound();
            }
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Task>> updateTask(int id, Task task){
            
            if(id != task.Id){
                return BadRequest();
            }

            var exisingTask = await _context.Tasks.FindAsync(id);
            if(exisingTask == null){
                return NotFound();
            }

            exisingTask.Title = task.Title;
            exisingTask.Description = task.Description;
            exisingTask.IsCompleted = task.IsCompleted;
        
            if(exisingTask.IsCompleted){
                exisingTask.CompletedAt = DateTime.UtcNow;
            }

            try{
                await _context.SaveChangesAsync();
            }catch(DbUpdateConcurrencyException){
                if(_context.Tasks.Find(id) == null){
                    return NotFound();
                }else{
                    throw;
                }
            }
            
            return NoContent();
        }

    }
}
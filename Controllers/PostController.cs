using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Delicious.Models;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;

namespace Delicious.Controllers;    
public class PostController : Controller
{    
    private readonly ILogger<PostController> _logger;
    // Add a private variable of type MyContext (or whatever you named your context file)
    private MyContext _context;         
    // Here we can "inject" our context service into the constructor 
    // The "logger" was something that was already in our code, we're just adding around it   
    public PostController(ILogger<PostController> logger, MyContext context)    
    {        
        _logger = logger;

        _context = context;    
    }

    [HttpGet("dishes/new")]
    public ViewResult NewDish()
    {
        return View();
    }

    [HttpPost("dish/creat")]
    public IActionResult CreateDish(Dish newDish)
    {
        if(!ModelState.IsValid)
        {
            return View("NewDish");
        }
        _context.Add(newDish);
        _context.SaveChanges();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("dishes")]
    public ViewResult ViewDishes()
    {
        List<Dish> Dishes = _context.Dishes.ToList();
        return View(Dishes);
    }

    [HttpGet("dish/{DishId}")]
    public IActionResult OneDish(int DishId)
    {
        Dish? SingleDish = _context.Dishes.FirstOrDefault(p => p.DishId == DishId);
        if(SingleDish == null)
        {
            return RedirectToAction("ViewDishes");
        }
        return View(SingleDish);
    }

    [HttpPost("dish/{DishId}/delete")]
    public IActionResult DeleteDish(int DishId)
    {
        Dish? deleted = _context.Dishes.SingleOrDefault(d => d.DishId == DishId);
        if(deleted != null)
        {
            _context.Remove(deleted);
            _context.SaveChanges();
        }
        return RedirectToAction("ViewDishes");
    }

    [HttpGet("dish/{DishId}/edit")]
    public IActionResult EditDish(int DishId)
    {
        Dish? DishEdit = _context.Dishes.FirstOrDefault(p => p.DishId == DishId);
        if(DishEdit == null)
        {
            return RedirectToAction("ViewDishes");
        }
        return View(DishEdit);
    }

    [HttpPost("dish/{DishId}/update")]
    public IActionResult UpdateDish(int DishId, Dish editedDish)
    {
        Dish? DishUpdate = _context.Dishes.FirstOrDefault(p => p.DishId == DishId);
        if(!ModelState.IsValid || DishUpdate == null)
        {
            return View("EditDish", editedDish);
        }
        DishUpdate.Name = editedDish.Name;
        DishUpdate.Calories = editedDish.Calories;
        DishUpdate.Chef = editedDish.Chef;
        DishUpdate.Description = editedDish.Description;
        DishUpdate.Tastiness = editedDish.Tastiness;
        DishUpdate.UpdatedAt = DateTime.Now;
        _context.SaveChanges();
        return RedirectToAction("OneDish", new{DishId});
    }          

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    } 
}
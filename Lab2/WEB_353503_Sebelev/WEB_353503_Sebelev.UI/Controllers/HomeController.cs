using System.Reactive.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_353503_Sebelev.UI.Models;

namespace WEB_353503_Sebelev.UI.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        List<ListDemo> demos = new();
        demos.Add(new ListDemo { Id = 1, Name = "Item 1"});
        demos.Add(new ListDemo { Id = 2, Name = "Item 2"});
        demos.Add(new ListDemo { Id = 3, Name = "Item 3"});
        

        ViewData["LabTitle"] = "Лабораторная работа №2";
        
        return View(new ListFormViewModel{ List=new SelectList(demos,"Id","Name") });
    }
}

public class ListDemo
{
    public int Id { get; set; }
    public string Name { get; set; }
}
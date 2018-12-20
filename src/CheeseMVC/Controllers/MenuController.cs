using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseMVC.Data;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<Menu> menus = context.Menus.ToList();
            return View(menus);
        }

        public IActionResult ViewMenu(int id)
        {
            Menu menu = context.Menus.SingleOrDefault(m => m.ID == id);
            List<CheeseMenu> items = context
                .CheeseMenus
                .Include(item => item.Cheese)
                .Where(cm => cm.MenuID == id)
                .ToList();
            ViewMenuViewModel model = new ViewMenuViewModel()
            {
                Menu = menu,
                Items = items
            };

            return View(model);
        }

        public IActionResult Add()
        {
            AddMenuViewModel model = new AddMenuViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                Menu newMenu = new Menu { Name = model.Name };
                context.Menus.Add(newMenu);
                context.SaveChanges();
                return Redirect("/Menu/ViewMenu/" + newMenu.ID.ToString());
            }

        }

        public IActionResult AddItem(int id)
        {
            Menu menu = context.Menus.SingleOrDefault(m => m.ID == id);
            AddMenuItemViewModel model = new AddMenuItemViewModel(menu, context.Cheeses.ToList());
            return View(model);
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                IList<CheeseMenu> existingItems = context.CheeseMenus
                    .Where(cm => cm.CheeseID == model.cheeseID)
                    .Where(cm => cm.MenuID == model.menuID).ToList();

                if (existingItems.Count == 0)
                {
                    CheeseMenu cm = new CheeseMenu();
                    cm.CheeseID = model.cheeseID;
                    cm.MenuID = model.menuID;
                    context.CheeseMenus.Add(cm);
                    context.SaveChanges();
                    return Redirect("/Menu/ViewMenu/" + model.menuID.ToString());
                }
            }
            return View();
        }
    }
}

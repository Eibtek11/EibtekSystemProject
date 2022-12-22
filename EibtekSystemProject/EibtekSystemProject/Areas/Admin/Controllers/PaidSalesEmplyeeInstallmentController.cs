using BL;
using Domains;
using EibtekSystemProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EibtekSystemProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaidSalesEmplyeeInstallmentController : Controller
    {
        PaidSalesEmplyeeInstallmentService paidSalesEmplyeeInstallmentService;
        PaidProjectInstallmentService paidProjectInstallmentService;
        EmployeeService employeeService;
        ClientService clientService;
        ProjectService projectService;
        EibtekSystemDbContext ctx;
        UserManager<ApplicationUser> Usermanager;
        public PaidSalesEmplyeeInstallmentController(PaidSalesEmplyeeInstallmentService PaidSalesEmplyeeInstallmentService,PaidProjectInstallmentService PaidProjectInstallmentService, EmployeeService EmployeeService, ProjectService ProjectService, ClientService ClientService, UserManager<ApplicationUser> usermanager, EibtekSystemDbContext context)
        {

            ctx = context;
            projectService = ProjectService;
            Usermanager = usermanager;
            clientService = ClientService;
            employeeService = EmployeeService;
            paidProjectInstallmentService = PaidProjectInstallmentService;
            paidSalesEmplyeeInstallmentService = PaidSalesEmplyeeInstallmentService;
        }
        public IActionResult Index()
        {
            HomePageModel model = new HomePageModel();
            model.lsClients = clientService.getAll();
            model.lstPaidProjectInstallments = paidProjectInstallmentService.getAll();
            model.lstPaidSalesEmplyeeInstallments = paidSalesEmplyeeInstallmentService.getAll();
            return View(model);


        }


        public async Task<IActionResult> Save(TbPaidSalesEmplyeeInstallment ITEM, List<IFormFile> files)
        {


            if (ITEM.PaidSalesEmplyeeInstallmentId == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string ImageName = Guid.NewGuid().ToString() + ".pdf";
                        var filePaths = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Uploads", ImageName);
                        using (var stream = System.IO.File.Create(filePaths))
                        {
                            await file.CopyToAsync(stream);
                        }
                        ITEM.PaidSalesInstallmentValueDocument = ImageName;
                    }
                }


                var result = paidSalesEmplyeeInstallmentService.Add(ITEM);
                if (result == true)
                {
                    TempData[SD.Success] = "Paid Sales Emplyee Installment successfully Created.";
                }
                else
                {
                    TempData[SD.Error] = "Error in Paid Sales Emplyee Installment  Creating.";
                }


            }
            else
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string ImageName = Guid.NewGuid().ToString() + ".jpg";
                        var filePaths = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Uploads", ImageName);
                        using (var stream = System.IO.File.Create(filePaths))
                        {
                            await file.CopyToAsync(stream);
                        }
                        ITEM.PaidSalesInstallmentValueDocument = ImageName;
                    }
                }




                var result = paidSalesEmplyeeInstallmentService.Edit(ITEM);
                if (result == true)
                {
                    TempData[SD.Success] = "Paid Sales Emplyee Installment successfully Updated.";
                }
                else
                {
                    TempData[SD.Error] = "Error in Paid Sales Emplyee Installment  Updating.";
                }

            }


            HomePageModel model = new HomePageModel();
            model.lsClients = clientService.getAll();
            model.lstPaidProjectInstallments = paidProjectInstallmentService.getAll();
            model.lstPaidSalesEmplyeeInstallments = paidSalesEmplyeeInstallmentService.getAll();
            return View("Index", model);
        }





        public IActionResult Delete(Guid id)
        {

            TbPaidSalesEmplyeeInstallment oldItem = ctx.TbPaidSalesEmplyeeInstallments.Where(a => a.PaidSalesEmplyeeInstallmentId == id).FirstOrDefault();

            var result = paidSalesEmplyeeInstallmentService.Delete(oldItem);
            if (result == true)
            {
                TempData[SD.Success] = "Paid Sales Emplyee Installment successfully Removed.";
            }
            else
            {
                TempData[SD.Error] = "Error in Paid Sales Emplyee Installment  Removing.";
            }

            HomePageModel model = new HomePageModel();
            model.lsClients = clientService.getAll();
            model.lstPaidProjectInstallments = paidProjectInstallmentService.getAll();
            model.lstPaidSalesEmplyeeInstallments = paidSalesEmplyeeInstallmentService.getAll();
            return View("Index", model);



        }




        public IActionResult Form(Guid? id)
        {
            TbPaidSalesEmplyeeInstallment oldItem = ctx.TbPaidSalesEmplyeeInstallments.Where(a => a.PaidSalesEmplyeeInstallmentId == id).FirstOrDefault();

            ViewBag.Employees = employeeService.getAll().Where(a => a.EmployeeCategoryId == Guid.Parse("935b10a7-b48d-4ae6-a7a7-df2630dbb7ef"));
            return View(oldItem);
        }
    }
}

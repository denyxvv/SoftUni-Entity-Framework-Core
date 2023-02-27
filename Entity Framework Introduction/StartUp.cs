﻿using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using System.Text;
using SoftUni.Models;

namespace SoftUni;

public class StartUp
{
    static void Main(string[] args)
    {
        SoftUniContext dbContext = new SoftUniContext();

        string output = GetEmployeesFullInformation(dbContext);

        Console.WriteLine(output);
    }
    // 03
    public static string GetEmployeesFullInformation(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var employees = context.Employees
            .OrderBy(e => e.EmployeeId)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.MiddleName,
                e.JobTitle,
                e.Salary
            })
            .ToArray();

        foreach (var employee in employees)
        {
            sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
        }

        return sb.ToString().TrimEnd();
    }
    // 04
    public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();
        var employees = context.Employees
            .Where(e => e.Salary > 50000)
            .OrderBy(e => e.FirstName)
            .Select(e => new
            {
                e.FirstName,
                e.Salary
            })
            .ToArray();

        foreach ( var e in employees)
        {
            sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
        }

        return sb.ToString().TrimEnd();
    }
    // 05
    public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var employees = context.Employees
            .Where(e => e.Department.Name == "Research and Development")
            .OrderBy(e => e.Salary) .ThenByDescending(e => e.FirstName)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                departmentName = e.Department.Name,
                e.Salary
            })
            .ToArray();

        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} from {e.departmentName} - ${e.Salary:f2}");
        }

        return sb.ToString().TrimEnd();
    }
    // 06
    public static string AddNewAddressToEmployee(SoftUniContext context)
    {
        Address newAddy = new Address()
        {
            AddressText = "Vitoshka 15",
            TownId = 4
        };

        Employee? employee = context.Employees.FirstOrDefault(e => e.LastName == "Nakov");
         employee!.Address = newAddy;

         context.SaveChanges();

         var employees = context.Employees
             .OrderByDescending(e => e.AddressId)
             .Take(10)
             .Select(e => e.Address!.AddressText);

         return string.Join(Environment.NewLine, employees);
    }
    // 07
    public static string GetEmployeesInPeriod(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var employeesWithProjects = context.Employees
            .Take(10)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                managerFirstName = e.Manager!.FirstName,
                managerLastName = e.Manager!.LastName,
                Projects = e.EmployeesProjects
                    .Where(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003)
                    .Select(ep => new
                {
                    ProjectName = ep.Project.Name
                    ,StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                    ,EndDate = ep.Project.EndDate.HasValue ? 
                        ep.Project.EndDate.Value.ToString("M / d / yyyy h: mm: ss tt", CultureInfo.InvariantCulture) : "not finished"
                })
                    .ToArray()
            }).ToArray();

        foreach (var e in employeesWithProjects)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.managerFirstName} {e.managerLastName}");
            foreach (var p in e.Projects)
            {
                sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");
            }
        }
        return sb.ToString().TrimEnd( );
    }

    // 08
    public static string GetAddressesByTown(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var addresses = context.Addresses
            .OrderByDescending(a => a.Employees.Count)
            .ThenBy(a => a.Town!.Name)
            .ThenBy(a => a.AddressText)
            .Take(10)
            .Select(a => new
            {
                a.AddressText,
                TownName = a.Town!.Name,
                EmployeesCount = a.Employees.Count
            })
            .ToArray();

        foreach (var a in addresses)
        {
            sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeesCount} employees");
        }

        return sb.ToString().TrimEnd();
    }
    //09
    public static string GetEmployee147(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var employee = context.Employees
            .Where(e => e.EmployeeId == 147)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                projectName = e.EmployeesProjects.OrderBy(ep => ep.Project.Name).Select(ep => new
                {
                    ep.Project.Name
                }).ToArray()
            })
            .FirstOrDefault();

        sb.AppendLine($"{employee!.FirstName} {employee!.LastName} - {employee.JobTitle}");
        sb.AppendLine(string.Join(Environment.NewLine, employee.projectName.Select(p => p.Name)));

        return sb.ToString().TrimEnd();
    }
    //10
    public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var departments = context.Departments
            .Where(d => d.Employees.Count > 5)
            .OrderBy(d => d.Employees.Count)
            .ThenBy(d => d.Name)
            .Select(d => new
            {
                d.Name,
                managerFirstName = d.Manager.FirstName,
                managerLastName = d.Manager.LastName,
                Employees = d.Employees
                    .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle
                })
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToArray()

            }).ToArray();

        foreach (var d in departments)
        {
            sb.AppendLine($"{d.Name} – {d.managerFirstName} {d.managerLastName}");

            foreach (var e in d.Employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
            }
        }
        return sb.ToString().TrimEnd();
    }
    //11
    public static string GetLatestProjects(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var projects = context.Projects
            .OrderByDescending(p => p.StartDate)
            .Take(10)
            .Select(p => new
            {
                p.Name,
                p.Description,
                StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
            })
            .OrderBy(p => p.Name)
            .ToArray();

        foreach (var p in projects)
        {
            sb.AppendLine(p.Name);
            sb.AppendLine(p.Description);
            sb.AppendLine(p.StartDate);
        }

        return sb.ToString().TrimEnd();
    }
    // 12
    public static string IncreaseSalaries(SoftUniContext context)
    {

        decimal salaryModifier = 0.12m;

        int[] deparments = context.Departments
            .Where(d => d.Name == "Engineering" || d.Name == "Tool Design" || d.Name == "Marketing" || d.Name == "Information Services")
            .Select(d => d.DepartmentId)
            .ToArray();

        foreach (var employee in context.Employees)
        {
            foreach (var department in deparments)
            {
                if (employee.DepartmentId == department)
                {
                    employee.Salary += employee.Salary * salaryModifier;
                }
            }
        }

        context.SaveChanges();

        var employees = context.Employees
            .Where(e => deparments.Contains(e.DepartmentId))
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.Salary
            })
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
        }

        return sb.ToString().TrimEnd();
    }
    //13
    public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
    {
        string nameStartWith = "sa";

        StringBuilder sb = new StringBuilder();

        var employees = context.Employees
            .Where(e => e.FirstName.Substring(0, 2).ToLower() == "sa")
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                e.Salary

            }).OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToArray();

        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
        }
        return sb.ToString().TrimEnd();
    }
    //14
    public static string DeleteProjectById(SoftUniContext context)
    {
        var employeeProjecxt = context.EmployeesProjects
            .Where(p => p.ProjectId == 2);

        Project? project = context.Projects
            .FirstOrDefault(p => p.ProjectId == 2);

        context.EmployeesProjects.RemoveRange(employeeProjecxt!);
        context.Projects.Remove(project!);

        context.SaveChanges();

        string[] projects = context.Projects
            .Take(10)
            .Select(p => p.Name)
            .ToArray();

        return string.Join(Environment.NewLine, projects);
    }
    //15
    public static string RemoveTown(SoftUniContext context)
    {
        string townNameForDelete = "Seattle";
        int? townIdForDelete = context.Towns
            .FirstOrDefault(t => t.Name == townNameForDelete)?
            .TownId;

        Address[] addressesForDelete = context.Addresses
            .Where(a => a.TownId == townIdForDelete)
            .ToArray();

        Town? townForDelete = context.Towns
            .FirstOrDefault(t => t.TownId == townIdForDelete);

        Employee[] employeesWithThatAddress = context.Employees
            .Where(e => addressesForDelete.Contains(e.Address))
            .ToArray();


        foreach (var e in employeesWithThatAddress)
        {
            e.AddressId = null;
        }


        int countDeletedTowns = context.Addresses
            .Count(a => a.TownId == townIdForDelete);


        context.Addresses.RemoveRange(addressesForDelete);
        context.Towns.Remove(townForDelete!);
        context.SaveChanges();

        return $"{countDeletedTowns} addresses in Seattle were deleted";
    }
}
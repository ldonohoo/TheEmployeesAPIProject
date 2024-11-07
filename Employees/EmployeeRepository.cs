using System;
using System.ComponentModel.Design;
using TheEmployeeAPI.Abstractions;

namespace TheEmployeeAPI;

// entity refers to a generic object/model managed by a
// respository (non-type specific)
// 
// entity in this repository is obviously an employee...
public class EmployeeRepository : IRepository<Employee>
{
    // this can also be = []; instead of new();
    private readonly List<Employee> _employees = new();

    public Employee? GetById(int id)
    {
        return _employees.FirstOrDefault(e => e.Id == id);
    }

    public IEnumerable<Employee> GetAll()
    {
        return _employees;
    }


    // entity refers to employee passed in to create
    public void Create(Employee entity)
    {
        if (entity == null)
        {
            // nameof just prints the variable name
            // 
            // for example, this error would be printed:
            // Value cannot be null. (Parameter 'entity')
            //
            // better than hardcoding "entity" as a string...
            throw new ArgumentNullException(nameof(entity));
        }
        //we snuck this in because we're no longer providing default employees!
        entity.Id = _employees.Select(e => e.Id).DefaultIfEmpty(0).Max() + 1;
        _employees.Add(entity);
    }

    public void Update(Employee entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        var existingEmployee = GetById(entity.Id);
        if (existingEmployee != null)
        {
            // existing employee points to _employee????
            existingEmployee.FirstName = entity.FirstName;
            existingEmployee.LastName = entity.LastName;
        }
    }
    public void Delete(Employee entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        _employees.Remove(entity);
    }
}
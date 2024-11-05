using System;

namespace TheEmployeeAPI.Abstractions;

// this is a generic repository!

// the repository is of type T
public interface IRepository<T>
{
    // this method gets an item from the repository
    // given an integer id
    // and will return an item of type T
    //
    // if the objectdoesn't exist by that id, this will
    // return a null, hence the type T is nullable!
    T? GetById(int id);

    // this method will get all items
    // returning an enumerable of type T 
    IEnumerable<T> GetAll();

    // this will create an entity of type T
    void Create(T entity);

    // this will update an entity of type T
    void Update(T entity);

    // this will delete an entity of type T
    void Delete(T entity);
}

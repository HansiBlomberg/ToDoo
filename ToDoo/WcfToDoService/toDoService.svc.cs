﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Data;
using ToDoBase;
using ToDoDAL;
using System.Web.Configuration;

namespace WcfToDoService
{
    
    public class ToDoService : IToDoService
    {

        // using no path will pick the configuration file from the applications running directory(/bin/debug while we are coding)
        /* Configuration file example, 
    
            <?xml version="1.0" encoding="utf-8" ?>
            <Configuration>
                <ConnectionString = 'my connection string'>
            </Configuration>
    
        */

        static readonly string configurationFileName = "ToDoServiceConfig.XML";


        // This instance and only this instance of the Data Access Layer will be used
        // The readonly keyword is a modifier that you can use on fields. When a field declaration includes a readonly modifier,
        // assignments to the fields introduced by the declaration can only occur as part of the declaration or in a constructor in the same class. 
        // We will assign it in the constructor.
        private readonly DAL ourDataAccessLayer;




        // This is the constructor, code that runs when an instance
        // of the toDoService svc is created.     
        // We are using it to get the connection string that the DAL expects
        // by reading it from a file, then we create the one and only DAL instance that
        // will be used by our methods. 
        public ToDoService()
        {

            string connectionString = getConnectionStringFromWebConfig();
            if (connectionString == null)
                connectionString = getConnectionStringFromXML(configurationFileName);
             ourDataAccessLayer = new DAL(connectionString);
            
        }


        // This method is used by the static constructor to get the connection string from the configuration file
        private static string getConnectionStringFromXML(string cfgFileName)
        {

            // Throwing an unhandled exception. We cant do much about a missing config file and this code does not have any user interface.
            if (!File.Exists(cfgFileName)) throw new FileNotFoundException($"Cant find the configuration file at {Directory.GetCurrentDirectory()}", cfgFileName);
            
            XmlDocument configuration = new XmlDocument();
            configuration.Load(cfgFileName);
            
            return configuration.SelectSingleNode("/Configuration/ConnectionString").InnerText;


        }


        private static string getConnectionStringFromWebConfig()
        {

            try
            {
                return ConfigurationManager.ConnectionStrings["ToDooConnection"].ConnectionString;
            }

            catch
            {
                return null;
            }
            
            
        }
        
        
        // This is just for play and debug
        // give the correct password and receive the connection string
        //public string RevealAllMySecrets(string password)
        //{
        //    if (SecurePasswordHasher.Verify(password, "$MYHASH$V1$10000$lMUQ80G3AJNALrr0PHROwncegDhn8zIWgHdLweAJO7p92ieA"))
        //    {
        //        return getConnectionStringFromXML(configurationFileName);
        //    }
        //    else return "No way!";
        //}
        

        public List<ToDo> GetToDo(string name)
        {
            // Get the ToDo-list with the name name from the database via our data access layer (DAL)
            return ourDataAccessLayer.GetToDoListByName(name).Where(t=>t.Name.ToLower() == name.ToLower()).ToList();

                      
        }

       
        public List<ToDo> GetToDoImportant(string name)
        {

            return GetToDo(name).Where(t => t.Name.ToLower() == name.ToLower()).Where(t => t.Description.Last() == '!').ToList();

        }

        public List<ToDo> GetToDoPriority(string name)
        {

            return GetToDo(name).Where(t => t.Name.ToLower() == name.ToLower()).OrderBy(t => t.DeadLine).ToList();

        }

        public List<ToDo> GetToDoPriorityImportant(string name)
        {

            return GetToDoImportant(name).Where(t => t.Name.ToLower() == name.ToLower()).OrderBy(t => t.DeadLine).ToList();

        }


        public int CountToDoImportant(string name)
        {
            return GetToDoImportant(name).Count();
        }

        public int CountDoneImportant(string name)
        {
            return GetDoneImportant(name).Count();
        }

        public int CountNotDoneImportant(string name)
        {
            return GetNotDoneImportant(name).Count();
        }

        public List<ToDo> GetDone(string name)
        {

            return ourDataAccessLayer.GetToDoListByName(name).Where(t => t.Name.ToLower() == name.ToLower()).Where(t => t.Finnished).ToList();            
        }

        public List<ToDo> GetNotDone(string name)
        {

            return ourDataAccessLayer.GetToDoListByName(name).Where(t => t.Name.ToLower() == name.ToLower()).Where(t => t.Finnished == false).ToList();
        }

        public List<ToDo> GetDoneImportant(string name)
        {

            return GetDone(name).Where(t => t.Name.ToLower() == name.ToLower()).Where(t => t.Description.Last() == '!').ToList();
        }
        public List<ToDo> GetNotDoneImportant(string name)
        {

            return GetNotDone(name).Where(t => t.Name.ToLower() == name.ToLower()).Where(t => t.Description.Last() == '!').ToList();
        }


        public ToDo CreateToDo(string name, ToDo todo)
        {

            // We are about to create a ToDo-item in the database
            // Maybe some validation is in order?
            // Such as checking if name == ToDo.Name?
                                    
            // Because we really really want the todo to have the same Name property as specified by the name parameter to this method.
            if (name != todo.Name) return null;

            // We dont care what CreatedDate came with the todo parameter. We will use the current Date and Time!
            todo.CreatedDate = DateTime.Now;


            // ourDataAccessLayer is our DAL
            // This is where the ToDo-item is sent to the database!
            
            var tempToDoList = ourDataAccessLayer.GetToDoListByName(todo.Name); // Get the existing todo list
            foreach(var t in tempToDoList)
            {
                if (t == todo) return null; // If there is an existing todo list with same information, dont insert the new one :-)
            }

            ourDataAccessLayer.AddToDo(todo);  // (try to...) Save the ToDo-item to the database!!

            tempToDoList = ourDataAccessLayer.GetToDoListByName(todo.Name).Where(t => t.Name.ToLower() == name.ToLower()).ToList(); // Get the existing todo list again!
            foreach (var t in tempToDoList)
            {
                if (t == todo) return t; // This time, we DO want to find our todo list in the database, if so return its Id
            }

            return null;  // Something went wrong, the todo list did never make it to the database
        }


        


        // This method will accept a comma separated list of todo descriptions and create
        // multiple todo items in the database.
        // We will use the same deadline and estimationtime for every item.
        // We will use the existing method CreateToDo(string name, ToDo todo) to insert the items
        // so this method will pretty much only parse the CSV and build the ToDo objects.

        public bool CreateToDoCSV(string name, string toDoItemDescriptionsCSV, string deadLine, string estimationTime)
        {

            // Some validation may be in order
            if (toDoItemDescriptionsCSV == "" || 
                toDoItemDescriptionsCSV == null ||
                name == "" ||
                name == null) return false;

            int _estimationTime;
            if (!int.TryParse(estimationTime, out _estimationTime)) return false;
            if (_estimationTime < 0) return false;

            DateTime _deadLine;
            if (!DateTime.TryParse(deadLine, out _deadLine)) return false;
            
            DateTime createdDate = DateTime.Now;
            var toDoItemDescriptions = toDoItemDescriptionsCSV.Split(',');

            var toDoList = new List<ToDo>();
            foreach (var toDoItemDescription in toDoItemDescriptions)
            {
                toDoList.Add(new ToDo { Name = name,
                    CreatedDate = createdDate,
                    DeadLine = _deadLine,
                    Description = toDoItemDescription,
                    EstimationTime = _estimationTime,
                    Finnished = false });
            }

            bool didEverythingGetAdded = true;
            foreach (var toDo in toDoList)
            {
                if (CreateToDo(name, toDo) == null) didEverythingGetAdded = false;
            }

            return didEverythingGetAdded;
        }

        // This method will remove all todo items with the given name
        // use with caution :-)
        public bool DeleteToDoByName(string name)
        {

            if (name == "" || name == null) return false;

            var toDoList = ourDataAccessLayer.GetToDoListByName(name);
            if (toDoList.Count == 0) return false;

            foreach (var toDo in toDoList)
            {
                ourDataAccessLayer.DeleteToDo(toDo.Id);
            }

            toDoList = ourDataAccessLayer.GetToDoListByName(name).Where(t => t.Name.ToLower() == name.ToLower()).ToList();
            if (toDoList.Count > 0) return false;
            return true;

        }


        // Remove todo with the given ID
        public bool DeleteToDoByID(string _name, string _id)
        {
            if (_name == "" || _name == null || _id == "" || _id == null) return false;

            int id;
            bool result = int.TryParse(_id, out id);

            if (result)
            {
                try
                {
                    var toDo = ourDataAccessLayer.GetToDoById(id);
                    if (toDo.Name.ToLower() == _name.ToLower())
                    {
                        ourDataAccessLayer.DeleteToDo(id);
                        return true;
                    }
                }
                catch { return false; }
            }
            return false;
        }

        // Set todo with the given ID to Finished (done).
        public bool UpdateToDoByID(string name, string id, ToDo todo)
        {
            if (name == "" || name == null || id == "" || id == null) return false;

            try
            {
                todo.Finnished = true;
                ourDataAccessLayer.UpdateToDo(todo);
                return true;
            }
            catch { return false; }
        }



        public Estimate GetEstimate(string name)
        {
            var totalTime = GetToDo(name).Sum(t => t.EstimationTime);
            return new Estimate { TotalTime = totalTime, CompletedAt = DateTime.Now.AddMinutes(totalTime)};

        }

        public Estimate GetEstimateNotDone(string name)
        {
            var totalTime = GetNotDone(name).Sum(t => t.EstimationTime);
            return new Estimate { TotalTime = totalTime, CompletedAt = DateTime.Now.AddMinutes(totalTime) };

        }

        public Estimate GetEstimateImportant(string name)
        {
            var totalTime = GetToDoImportant(name).Sum(t => t.EstimationTime);
            return new Estimate { TotalTime = totalTime, CompletedAt = DateTime.Now.AddMinutes(totalTime) };
        }

        public Estimate GetEstimateNotDoneImportant(string name)
        {
            var totalTime = GetNotDoneImportant(name).Sum(t => t.EstimationTime);
            return new Estimate { TotalTime = totalTime, CompletedAt = DateTime.Now.AddMinutes(totalTime) };
        }

        
        public ToDo MarkToDoDone(string name, string id)
        {
            int _id;
            if ( int.TryParse(id, out _id))
            {
                var aToDo = ourDataAccessLayer.GetToDoById(_id);
                if (aToDo == null) return null;
                if (aToDo.Name == null) return null;
                if (aToDo.Name.ToLower() == name.ToLower())
                {
                    aToDo.Finnished = true;
                    ourDataAccessLayer.UpdateToDo(aToDo);
                    aToDo = ourDataAccessLayer.GetToDoById(_id);
                    return aToDo;
                }
            }
            return null;
        }

        public bool IsToDoDone(string name, string id)
        {
            int _id;
            if( int.TryParse(id, out _id))
            {
                var aToDo = ourDataAccessLayer.GetToDoById(_id);
                if (aToDo == null) return false; // I hate to do this
                if (aToDo.Name == null) return false; // but REST dont like nullable types at all :-(
                if (aToDo.Name.ToLower() == name.ToLower())
                    return aToDo.Finnished;
            }
            return false;
            
        }

        public ToDo MarkToDoNotDone(string name, string id)
        {
            int _id;
            if (int.TryParse(id, out _id))
            {
                var aToDo = ourDataAccessLayer.GetToDoById(_id);
                if (aToDo == null) return null;
                if (aToDo.Name == null) return null;
                if (aToDo.Name.ToLower() == name.ToLower())
                {
                    aToDo.Finnished = false;
                    ourDataAccessLayer.UpdateToDo(aToDo);
                    aToDo = ourDataAccessLayer.GetToDoById(_id);
                    return aToDo;
                }
            }
            return null;
        }


        public bool IsToDoNotDone(string name, string id)
        {

            return !IsToDoDone(name, id);
                                    
        }








        public bool EditToDo(string id,
                               string description,
                               string name,
                               string deadLine,
                               string estimationTime,
                               string finnished)
        {
            int _id;
            ToDo toDoOld, toDoNew;


            if (Int32.TryParse(id, out _id))
            {
                toDoOld = ourDataAccessLayer.GetToDoById(_id);

                if (toDoOld != null)
                {
                    toDoNew = toDoOld;
                    if (description != "__default__")
                    {
                        toDoNew.Description = description;
                    }
                    else
                    {
                        toDoNew.Description = toDoOld.Description;
                    }

                    if (name != "__default__")
                    {
                        toDoNew.Name = name;
                    }
                    else
                    {
                        toDoNew.Name = toDoOld.Name;
                    }

                    DateTime _deadLine;
                    if (DateTime.TryParse(deadLine, out _deadLine))
                    {
                        toDoNew.DeadLine = _deadLine;
                    }
                    else
                    {
                        toDoNew.DeadLine = toDoOld.DeadLine;
                    }

                    int _estimationTime;
                    if (Int32.TryParse(estimationTime, out _estimationTime))
                    {
                        toDoNew.EstimationTime = _estimationTime;
                    }
                    else
                    {
                        toDoNew.EstimationTime = toDoOld.EstimationTime;
                    }

                    finnished = finnished.ToLower();
                    if (finnished == "true" || finnished == "false")
                    {
                        if (finnished == "true")
                        {
                            toDoNew.Finnished = true;
                        }
                        else
                        {
                            toDoNew.Finnished = false;
                        }
                    }
                    else
                    {
                        toDoNew.Finnished = toDoOld.Finnished;
                    }


                    ourDataAccessLayer.UpdateToDo(toDoNew);
                    if (toDoNew == ourDataAccessLayer.GetToDoById(_id)) // Kontrollera om det blivit förändringar
                    {
                        return true; // Svara true om det blivit förändringar
                    }
                    else
                    {
                        return false; // Svara false om det INTE blivit förändringar
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } // EditToDo slutar här



        public ToDo ChangeToDoDescription(string id,
                        string description,
                        string name)
        {

            int _id;
            if (!int.TryParse(id, out _id)) return null;

            var aToDo = ourDataAccessLayer.GetToDoById(_id);
            if (aToDo == null) return null;
            if (aToDo.Name.ToLower() != name.ToLower()) return null;

            aToDo.Description = description;
            ourDataAccessLayer.UpdateToDo(aToDo);
            aToDo = ourDataAccessLayer.GetToDoById(_id);
            return aToDo;

        }


        // This is just a sample method, that shows how to return a "composite" type
        // composite just means it is an object with several properties and not
        // just a regular datatype. "CompositeType" is just a class like any other.
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }


        public int GetNumberOfToDoos(string name)
        {
            if(name == null)
            {
                throw new ArgumentNullException("name");
            }
            // Get the number of ToDoos with a specific name
            List<ToDo> toDoos = ourDataAccessLayer.GetToDoListByName(name).Where(t => t.Name.ToLower() == name.ToLower()).ToList();
            return toDoos.Count;
        }

        public int GetNumberOfMarkedToDoos(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            // Get the number of ToDoos with a specific name
            List<ToDo> toDoos = ourDataAccessLayer.GetToDoListByName(name).Where(t => t.Name.ToLower() == name.ToLower()).ToList();
            int _count = 0;

            var markedTodoos = from todo in toDoos where todo.Finnished.Equals(true) select todo;
            foreach (ToDo markedTodo in markedTodoos)
            {
                _count++;
            }


            return _count;
        }


    }
}




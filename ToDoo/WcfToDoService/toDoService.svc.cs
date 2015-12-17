using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using ToDoBase;
using ToDoDAL;


namespace WcfToDoService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
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
            
             ourDataAccessLayer = new DAL(getConnectionStringFromXML(configurationFileName));
            
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

        // This is just for play and debug
        // give the correct password and receive the connection string
        public string RevealAllMySecrets(string password)
        {
            if (SecurePasswordHasher.Verify(password, "$MYHASH$V1$10000$lMUQ80G3AJNALrr0PHROwncegDhn8zIWgHdLweAJO7p92ieA"))
            {
                return getConnectionStringFromXML(configurationFileName);
            }
            else return "No way!";
        }
        

        public List<ToDo> GetToDo(string name)
        {
            // Get the ToDo-list with the name name from the database via our data access layer (DAL)
            return ourDataAccessLayer.GetToDoListByName(name);

                      
        }

        public bool CreateToDo(string name, ToDo todo)
        {

            // Nu skall vi skapa ett ToDo item till databasen
            // Skall vi validera lite saker?
            // Skall vi kolla om name == ToDo.Name?

            // Vi vill att vi får in en todo med samma namn som den url som vi tog emot den från
            if (name != todo.Name) return false;

            // Vi ändrar/ställer CreatedDate till NU!
            todo.CreatedDate = DateTime.Now;
            

            // ourDataAccessLayer är vår DAL
            ourDataAccessLayer.AddToDo(todo);

            return true;
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
    }
}




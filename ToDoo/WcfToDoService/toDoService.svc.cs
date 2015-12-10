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
        static readonly DAL ourDataAccessLayer;




        // This is a static constructor. This code will be executed only once.
        
        // We are using it to get the connection string that the DAL expects
        // by reading it from a file, then we create the one and only DAL instance that
        // will be used by our methods. 
        static ToDoService()
        {
            
             ourDataAccessLayer = new DAL(getConnectionStringFromXML(configurationFileName));
            
        }


        // This method is used by the static constructor to get the connection string from the configuration file
        private static string getConnectionStringFromXML(string cfgFileName)
        {

            // Throwing an unhandled exception. We cant do much about a missing config file and this code does not have any user interface.
            if (!File.Exists(cfgFileName)) throw new FileNotFoundException("Cant find the configuration file", cfgFileName);
            
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




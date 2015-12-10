using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace WcfToDoService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class ToDoService : IToDoService
    {
        static readonly string connectionString;

        /* Configuration file example, using no path will pick the file from the applications running directory (/bin/)
        <?xml version="1.0" encoding="utf-8" ?>
        <Configuration>
            <ConnectionString = 'my connection string'>
	    </Configuration>
        */

        static readonly string configurationFileName = "ToDoServiceConfig.XML";
        // Hansi: This is a static constructor.
        // We are using it to get the connection string that the DAL expects
        // by reading it from a file.
        static ToDoService()
        {
            // connectionString = "test";
            connectionString = getConnectionStringFromXML(configurationFileName);
            
        }


        // This method is used by the static constructor to get the connection string from the configuration file
        private static string getConnectionStringFromXML(string cfgFileName)
        {

            // Throwing an unhandled exception. We cant do much about a missing config file and this code does not have any user interface.
            if (!File.Exists(cfgFileName)) throw new FileNotFoundException("Cant find the configuration file", cfgFileName);
            // if (!File.Exists(cfgFileName)) return $"file not found: {Directory.GetCurrentDirectory()} {cfgFileName}";
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
                return connectionString;
            }
            else return "No way!";
        }

        public string GetToDo(string name)
        {
            return ($"You entered: {name}");
        }

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




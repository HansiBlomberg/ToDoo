using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ToDoBase;
using ToDoDAL;

namespace WcfToDoService
{
    
    [ServiceContract]
    public interface IToDoService
    {

        // Vi är i klassen som definierar INTERFACET IToDoService. I:et i början av namnet är konvention för Interface.
        // Här deklarerar vi alla metoder som måste implementeras i toDoService.cs.
        // WebGet "pyntningen" beskriver hur webservern skall tolka inkommande anrop där parametrarna anges som om dom vore underkataloger
        // T.ex. "GetToDo/{name}" för så att servern kan anropas med http://www.todoservern.com/GetToDo/ettnamn där ettnamn är namnet på
        // ToDo-listan vi vill få från servern.

        // This is the class that define the INTERFACE IToDoService. The I in the beginning of the name is convention for Interface.
        // Here is the declaration for all methods that need to be implemented in toDoService.cs
        // The WebGet decoration describe how the webserver is going to understand incomming calls where parameters are entered as subdirectories.

        [OperationContract]
        [WebGet(UriTemplate = "GetToDo/{name}")]
        List<ToDo> GetToDo(string name);

        [OperationContract]
        [WebGet(UriTemplate = "RevealAllMySecrets/{password}")]
        string RevealAllMySecrets(string password);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "CreateToDo/{name}")]
        bool CreateToDo(string name, ToDo todo);




        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // Här är det bara fortsätta definiera våra metoder som skall in i WCF
        // Keep on defining the WCF methods we need
    }

    // The class below is just an example from the tutorial, not used by our application.
    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}

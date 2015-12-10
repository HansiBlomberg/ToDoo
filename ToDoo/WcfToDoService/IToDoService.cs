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

        [OperationContract]
        [WebGet(UriTemplate = "GetToDo/{name}")]
        List<ToDo> GetToDo(string name);

        [OperationContract]
        [WebGet(UriTemplate = "RevealAllMySecrets/{password}")]
        string RevealAllMySecrets(string password);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // Här är det bara fortsätta definiera våra metoder som skall in i WCF
    }


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

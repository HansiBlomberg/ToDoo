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

        // GET is for retrieving data
        // POST is for updating existing data
        // PUT is for inserting new data
        // ResponseFormat = WebMessageFormat.Json will make the method return data i JSON format
        // RequestFormat = WebMessageFormat.Json will make the method want input data in JSON format


        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "todo/{name}")]
        List<ToDo> GetToDo(string name);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "todo/{name}/priority")]
        List<ToDo> GetToDoPriority(string name);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "todo/{name}/priority/important")]
        List<ToDo> GetToDoPriorityImportant(string name);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "todo/{name}/important")]
        List<ToDo> GetToDoImportant(string name);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "todo/{name}/count/important")]
        int CountToDoImportant(string name);

        
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "getdone/{name}/count/important")]
        int CountDoneImportant(string name);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "getnotdone/{name}/count/important")]
        int CountNotDoneImportant(string name);




        //[OperationContract]
        //[WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "RevealAllMySecrets/{password}")]
        //string RevealAllMySecrets(string password);

        //  Bare
        //  Both Request and Reponse are not Wrapped
        //  Wrapped
        //  Both requests and responses are wrapped.
        //  WrappedRequest
        //  Requests are wrapped, responses are not wrapped.
        //  WrappedResponse
        //  Responses are wrapped, requests are not wrapped.




        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json,
                    RequestFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.WrappedRequest,
                    Method = "POST", UriTemplate = "todo/{name}/new")]
        ToDo CreateToDo(string name, ToDo todo);



        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json,
                    RequestFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare,
                    Method = "PUT", UriTemplate = "todo/{name}/{id}/done")]
        ToDo MarkToDoDone(string name, string id);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
                UriTemplate = "todo/{name}/{id}/done")]
        bool IsToDoDone(string name, string id);


        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json,
                    RequestFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare,
                    Method = "PUT", UriTemplate = "todo/{name}/{id}/notdone")]
        ToDo MarkToDoNotDone(string name, string id);


        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
                UriTemplate = "todo/{name}/{id}/notdone")]
        bool IsToDoNotDone(string name, string id);

        





        // Som utvecklare av frontend vill jag att det skapas flera punkter om jag
        // skickar en kommaseparerad lista som innehåll på en punkt.Ex: Äpple,
        // Banan, Päron ska bli tre punkter i listan.
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json,
                            RequestFormat = WebMessageFormat.Json,
                            Method = "POST", UriTemplate = "CreateToDoCSV/{name}/{toDoItemDescriptionsCSV}/{deadLine}/{estimationTime}")]
        bool CreateToDoCSV(string name, string toDoItemDescriptionsCSV, string deadLine, string estimationTime);

        
        // We need a quick way to clear out a TODO list while testing...
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json,
                            RequestFormat = WebMessageFormat.Json,
                            Method = "DELETE", UriTemplate = "DeleteToDoByName/{name}")]
        bool DeleteToDoByName(string name);






        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // Här är det bara fortsätta definiera våra metoder som skall in i WCF
        // Keep on defining the WCF methods we need


        //Som utvecklare av en frontend vill jag kunna få ut alla avklarade punkter i en given todo-lista (Lista med avklarade)
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetDone/{name}")]
        List<ToDo> GetDone(string name);

        //Som utvecklare av en frontend vill jag kunna få ut alla EJ avklarade punkter i en given todo-lista (Lista med avklarade)
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetNotDone/{name}")]
        List<ToDo> GetNotDone(string name);

        //Som utvecklare av en frontend vill jag kunna få ut alla VIKTIGA avklarade punkter i en given todo-lista
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetDone/{name}/important")]
        List<ToDo> GetDoneImportant(string name);

        //Som utvecklare av en frontend vill jag kunna få ut alla VIKTIGA EJ avklarade punkter i en given todo-lista
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetNotDone/{name}/important")]
        List<ToDo> GetNotDoneImportant(string name);



        // Som utvecklare av en frontend vill jag ha en endpoint för att kunna
        //markera en punkt i en todo-lista som klar eller 
        // för att ta bort den helt från systemet
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json,
                      RequestFormat = WebMessageFormat.Json,
                      Method = "DELETE", UriTemplate = "todo/{name}/{id}")]
        bool DeleteToDoByID(string name, string id);


        //Som utvecklare av en frontend vill jag kunna få ut ett estimat på tidsåtgång
        //samt tidpunkt för att klara av alla punkter beräknat från tiden anropet gjordes
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "todo/{name}/estimate")]
        Estimate GetEstimate(string name);

        //Som utvecklare av en frontend vill jag kunna få ut ett estimat på tidsåtgång
        //samt tidpunkt för att klara av alla EJ REDAN AVKLARADE punkter beräknat från tiden anropet gjordes
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "getnotdone/{name}/estimate")]
        Estimate GetEstimateNotDone(string name);

        //Som utvecklare av en frontend vill jag kunna få ut ett estimat på tidsåtgång
        //samt tidpunkt för att klara av alla VIKTIGA punkter beräknat från tiden anropet gjordes
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "todo/{name}/estimate/important")]
        Estimate GetEstimateImportant(string name);

        //Som utvecklare av en frontend vill jag kunna få ut ett estimat på tidsåtgång
        //samt tidpunkt för att klara av alla EJ AVKLARADE VIKTIGA punkter beräknat från tiden anropet gjordes
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "getnotdone/{name}/estimate/important")]
        Estimate GetEstimateNotDoneImportant(string name);



        //Som utvecklare av frontend vill 
        //jag kunna redigera punkter som redan
        //finns i min todo
        [OperationContract]
        /*[WebInvoke(ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                   Method = "PUT", UriTemplate = "EditToDo/{id}/{description}")]*/
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "EditToDo/{id=__default__}/{description=__default__}/{name=__default__}/{deadLine=__default__}/{estimationTime=__default__}/{finnished=__default__}")]
        bool EditToDo(string id,
                        string description,
                        string name,
                        string deadLine,
                        string estimationTime,
                        string finnished);



        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare,
                   Method = "PUT", UriTemplate = "todo/{name}/{id}/description/{description}")]        
        ToDo ChangeToDoDescription(string id,
                        string description,
                        string name);




    }


    // ¨This class is used by the Estimate method
    [DataContract]
    public class Estimate
    {
        
        [DataMember]
         public int TotalTime { get; set; }
        
        [DataMember]
        public DateTime CompletedAt { get; set; }
        
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

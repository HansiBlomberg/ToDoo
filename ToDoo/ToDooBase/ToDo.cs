using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoBase
{
    public class ToDo : IComparable
    {
        public ToDo() { }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Finnished { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DeadLine { get; set; }
        public int EstimationTime { get; set; }


        // This method is implementing the IComparable interface
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            ToDo otherToDo = obj as ToDo;
            if(otherToDo != null)
            {
                // Check if all important properties are equal
                // NOTE! By design choice, the Finnished (misspelled I know!) prooperty is not compared!
                if (this.Name == otherToDo.Name &&
                     this.Description == otherToDo.Description &&
                     this.DeadLine == otherToDo.DeadLine &&
                     this.Finnished == otherToDo.Finnished &&
                     this.EstimationTime == otherToDo.EstimationTime) return 0;

                // If not, check if it is the same Name, if so compare on Description
                else if (this.Name == otherToDo.Name) return String.Compare(this.Description, otherToDo.Description);

                // And if the name was not the same, compare on Name
                else return  String.Compare(this.Name, otherToDo.Name);
            }
            else
                // I can only be compared to my own kind of object, sorry!
                throw new ArgumentException("Object is not a ToDO");

        }

        // This take care of comparing two ToDo objects with ==, != etc.


        public bool Equals(ToDo other)
        {
            if (ReferenceEquals(null, other))  return false;
         

            if (ReferenceEquals(this, other)) return true;


            return Equals(other.DeadLine, DeadLine) &&
                   Equals(other.Description, Description) &&
                   Equals(other.EstimationTime, EstimationTime) &&
                   Equals(other.Name, Name);
        }




        public override bool Equals(object obj)
        {
            
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ToDo)) return false;

            return Equals((ToDo)obj);


        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DeadLine != null ? DeadLine.GetHashCode() : 0) * 397)
                    ^ (Description != null ? Description.GetHashCode() : 0)
                    ^ EstimationTime.GetHashCode()
                     ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ToDo left, ToDo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ToDo left, ToDo right)
        {
            return !Equals(left, right);
        }

    }


    

}

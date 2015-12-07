using System;
using ToDoList;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DAL
{
    public class DAL
    {
        private string ErrorMessage { get; set; }
        private SqlConnection conn;
        private static string connString;
        private SqlCommand command;       
        private static List<ToDo> toDoList;

        public DAL(string _connString)
        {
            connString = _connString;            
        }
        /// <summary>
        /// Add an ToDo
        /// </summary>
        /// <param name="toDo"></param>
        public void AddToDo(ToDo toDo)
        {
            try
            {
                using (conn)
                {
                    //using parametirized query
                    string sqlInserString =
                    "INSERT INTO ToDoList (Description, Name, CreatedDate, DeadLine, EstimationTime, Finnished) VALUES ( @description, @name, @CreatedDate, @deadLine, @estimationTime, @finnished)";
                   
                    conn = new SqlConnection(connString);
                    
                    command = new SqlCommand();
                    command.Connection = conn;
                    command.Connection.Open();
                    command.CommandText = sqlInserString;

                    SqlParameter descriptionParam = new SqlParameter("@description", toDo.Description);
                    SqlParameter userParam = new SqlParameter("@name", toDo.Name);
                    SqlParameter createdParam = new SqlParameter("@createdDate", toDo.CreatedDate);
                    SqlParameter deadLineParam = new SqlParameter("@deadLine", toDo.DeadLine);
                    SqlParameter estimateParam = new SqlParameter("@estimationTime", toDo.EstimationTime);
                    SqlParameter flagParam = new SqlParameter("@finnished", toDo.Finnished ? 1:0);


                    command.Parameters.AddRange(new SqlParameter[]{ descriptionParam, userParam, createdParam, deadLineParam, estimateParam, flagParam });
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                    
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
        /// <summary>
        /// Update ToDo
        /// </summary>
        /// <param name="toDo"></param>
        public void UpdateToDoList(ToDo toDo)
        {
            try
            {
                using (conn)
                {
                    string sqlUpdateString =
                    "UPDATE ToDoList SET Description=@description, Name=@name, CreatedDate=@createdDate, DeadLine=@deadLine, EstimationTime=@estimationTime, Finnished=@finnished WHERE ID=" + toDo.Id;

                    conn = new SqlConnection(connString);

                    command = new SqlCommand();
                    command.Connection = conn;
                    command.Connection.Open();
                    command.CommandText = sqlUpdateString;

                    SqlParameter descriptionParam = new SqlParameter("@Description", toDo.Description);
                    SqlParameter userParam = new SqlParameter("@Name", toDo.Name);
                    SqlParameter createdParam = new SqlParameter("@createdDate", toDo.CreatedDate);
                    SqlParameter deadLineParam = new SqlParameter("@deadLine", toDo.DeadLine);
                    SqlParameter estimateParam = new SqlParameter("@EstimationTime", toDo.EstimationTime);
                    SqlParameter flagParam = new SqlParameter("@finnished", toDo.Finnished ? 1 : 0);

                    command.Parameters.AddRange(new SqlParameter[] { descriptionParam, userParam, createdParam, deadLineParam, estimateParam, flagParam });
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                }
            }
            catch (Exception ex)
            {
               ErrorMessage = ex.Message;
            }
        }
        /// <summary>
        /// Delete ToDo
        /// </summary>
        /// <param name="ID"></param>
        public void DeleteToDoList(int ID)
        {
            try
            {
                using (conn)
                {
                    string sqlDeleteString = "DELETE FROM ToDoLIst WHERE ID=@ID ";

                    conn = new SqlConnection(connString);

                    command = new SqlCommand();
                    command.Connection = conn;
                    command.Connection.Open();
                    command.CommandText = sqlDeleteString;

                    SqlParameter IdParam = new SqlParameter("@ID", ID);
                    command.Parameters.Add(IdParam);
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Get ToDo list
        /// </summary>
        /// <returns></returns>
        public List<ToDo> GetToDoList()
        {
            try
            {
                using (conn)
                {
                    toDoList = new List<ToDo>();

                    conn = new SqlConnection(connString);

                    string sqlSelectString = "SELECT * FROM ToDoList";
                    command = new SqlCommand(sqlSelectString, conn);
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ToDo toDo = new ToDo();
                        toDo.Id = reader.GetInt32(reader.GetOrdinal("ID"));
                        toDo.Description = reader.GetString(reader.GetOrdinal("Description"));
                        toDo.Name = reader.GetString(reader.GetOrdinal("Name"));
                        toDo.DeadLine = reader.GetDateTime(reader.GetOrdinal("DeadLine"));
                        toDo.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                        toDo.EstimationTime = reader.GetInt32(reader.GetOrdinal("EstimationTime"));
                        toDo.Finnished = reader.GetBoolean(reader.GetOrdinal("Finnished"));
                        toDoList.Add(toDo);
                    }
                    command.Connection.Close();
                    return toDoList;
                }
                
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return null;


        }

        /// <summary>
        /// Get ToDo list
        /// </summary>
        /// <returns></returns>
        public List<ToDo> GetToDoListById(int id)
        {
            try
            {
                using (conn)
                {
                    toDoList = new List<ToDo>();

                    conn = new SqlConnection(connString);

                    string sqlSelectString = "SELECT * FROM ToDoLIst WHERE ID=" + id;
                    command = new SqlCommand(sqlSelectString, conn);
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        ToDo toDo = new ToDo();
                        toDo.Id = reader.GetInt32(reader.GetOrdinal("ID"));
                        toDo.Description = reader.GetString(reader.GetOrdinal("Description"));
                        toDo.Name = reader.GetString(reader.GetOrdinal("Name"));
                        toDo.DeadLine = reader.GetDateTime(reader.GetOrdinal("DeadLine"));
                        toDo.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                        toDo.EstimationTime = reader.GetInt32(reader.GetOrdinal("EstimationTime"));
                        toDo.Finnished = reader.GetBoolean(reader.GetOrdinal("Finnished"));
                        toDoList.Add(toDo);
                    }
                    command.Connection.Close();
                    return toDoList;
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return null;


        }

        public List<ToDo> GetToDoListByName(string name)
        {
            try
            {
                using (conn)
                {
                    toDoList = new List<ToDo>();

                    conn = new SqlConnection(connString);

                    string sqlSelectString = "select * from ToDoList where Name like '%" + name + "%'";
                    command = new SqlCommand(sqlSelectString, conn);
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ToDo toDo = new ToDo();
                        toDo.Id = reader.GetInt32(reader.GetOrdinal("ID"));
                        toDo.Description = reader.GetString(reader.GetOrdinal("Description"));
                        toDo.Name = reader.GetString(reader.GetOrdinal("Name"));
                        toDo.DeadLine = reader.GetDateTime(reader.GetOrdinal("DeadLine"));
                        toDo.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                        toDo.EstimationTime = reader.GetInt32(reader.GetOrdinal("EstimationTime"));
                        toDo.Finnished = reader.GetBoolean(reader.GetOrdinal("Finnished"));
                        toDoList.Add(toDo);
                    }
                    command.Connection.Close();
                    return toDoList;
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return null;


        }

        public String GetErrorMessage()
        {
            return ErrorMessage;
        }


    }
}

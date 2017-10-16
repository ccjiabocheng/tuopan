using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;

namespace DataObject.Generator.Core.Comm
{
    ///  
    /// SqlServer���ݷ��ʰ����� 
    ///  
    public sealed class SqlHelper
    {
        #region ˽�й��캯���ͷ���

        private SqlHelper() { }

        ///  
        /// ��SqlParameter��������(����ֵ)�����SqlCommand����. 
        /// ������������κ�һ����������DBNull.Value; 
        /// �ò�������ֹĬ��ֵ��ʹ��. 
        ///  
        /// ������ 
        /// SqlParameters���� 
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // ���δ����ֵ���������,���������DBNull.Value. 
                        if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        ///  
        /// ��DataRow���͵���ֵ���䵽SqlParameter��������. 
        ///  
        /// Ҫ����ֵ��SqlParameter�������� 
        /// ��Ҫ������洢���̲�����DataRow 
        private static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters == null) || (dataRow == null))
            {
                return;
            }

            int i = 0;
            // ���ò���ֵ 
            foreach (SqlParameter commandParameter in commandParameters)
            {
                // ������������,���������,ֻ�׳�һ���쳣. 
                if (commandParameter.ParameterName == null ||
                    commandParameter.ParameterName.Length <= 1)
                    throw new Exception(
                        string.Format("���ṩ����{0}һ����Ч������{1}.", i, commandParameter.ParameterName));
                // ��dataRow�ı��л�ȡΪ�����������������Ƶ��е�����. 
                // ������ںͲ���������ͬ����,����ֵ������ǰ���ƵĲ���. 
                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                i++;
            }
        }

        ///  
        /// ��һ��������������SqlParameter��������. 
        ///  
        /// Ҫ����ֵ��SqlParameter�������� 
        /// ��Ҫ������洢���̲����Ķ������� 
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                return;
            }

            // ȷ����������������������ƥ��,�����ƥ��,�׳�һ���쳣. 
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("����ֵ�����������ƥ��.");
            }

            // ��������ֵ 
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                // If the current array value derives from IDbDataParameter, then assign its Value property 
                if (parameterValues[i] is IDbDataParameter)
                {
                    IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                    if (paramInstance.Value == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = paramInstance.Value;
                    }
                }
                else if (parameterValues[i] == null)
                {
                    commandParameters[i].Value = DBNull.Value;
                }
                else
                {
                    commandParameters[i].Value = parameterValues[i];
                }
            }
        }

        ///  
        /// Ԥ�����û��ṩ������,���ݿ�����/����/��������/���� 
        ///  
        /// Ҫ�����SqlCommand 
        /// ���ݿ����� 
        /// һ����Ч�����������nullֵ 
        /// �������� (�洢����,�����ı�, ����.) 
        /// �洢��������T-SQL�����ı� 
        /// �������������SqlParameter��������,���û�в���Ϊ'null' 
        /// true ��������Ǵ򿪵�,��Ϊtrue,���������Ϊfalse. 
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it 
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // ���������һ�����ݿ�����. 
            command.Connection = connection;

            // ���������ı�(�洢��������SQL���) 
            command.CommandText = commandText;

            // �������� 
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // ������������. 
            command.CommandType = commandType;

            // ����������� 
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }

        #endregion ˽�й��캯���ͷ�������

        #region ���ݿ����� 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        ///  
        ///  
        public static string GetConnSting()
        {
            return ConfigurationManager.ConnectionStrings["connectionStr"].ConnectionString;
        }
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        ///  
        ///  
        public static SqlConnection GetConnection()
        {
            SqlConnection Connection = new SqlConnection(SqlHelper.GetConnSting());
            return Connection;
        }
        #endregion

        #region ExecuteNonQuery����

        ///  
        /// ִ��ָ�������ַ���,���͵�SqlCommand. 
        ///  
        ///  
        /// ʾ��:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders"); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �������� (�洢����,�����ı�, ����.) 
        /// �洢�������ƻ�SQL��� 
        /// ��������Ӱ������� 
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// ִ��ָ�������ַ���,���͵�SqlCommand.���û���ṩ����,�����ؽ��. 
        ///  
        ///  
        /// ʾ��:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �������� (�洢����,�����ı�, ����.) 
        /// �洢�������ƻ�SQL��� 
        /// SqlParameter�������� 
        /// ��������Ӱ������� 
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }

        ///  
        /// ִ��ָ�������ַ����Ĵ洢����,�����������ֵ�����洢���̲���, 
        /// �˷�����Ҫ�ڲ������淽����̽�����������ɲ���. 
        ///  
        ///  
        /// �������û���ṩ������������ͷ���ֵ. 
        /// ʾ��:  
        ///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36); 
        ///  
        /// һ����Ч�����ݿ������ַ���/param> 
        /// �洢�������� 
        /// ���䵽�洢������������Ķ������� 
        /// ������Ӱ������� 
        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ������ڲ���ֵ 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // ��̽���洢���̲���(���ص�����)��������洢���̲�������. 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // ���洢���̲�����ֵ 
                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // û�в�������� 
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ�������� 
        ///  
        ///  
        /// ʾ��:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders"); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// ��������(�洢����,�����ı�������.) 
        /// �洢�������ƻ�T-SQL��� 
        /// ����Ӱ������� 
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ�������� 
        ///  
        ///  
        /// ʾ��:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// ��������(�洢����,�����ı�������.) 
        /// T�洢�������ƻ�T-SQL��� 
        /// SqlParamter�������� 
        /// ����Ӱ������� 
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // ����SqlCommand����,������Ԥ���� 
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Finally, execute the command 
            int retval = cmd.ExecuteNonQuery();

            // �������,�Ա��ٴ�ʹ��. 
            cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retval;
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ��������,�����������ֵ�����洢���̲���. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ 
        /// ʾ��:  
        ///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢������ 
        /// ������洢������������Ķ������� 
        /// ����Ӱ������� 
        public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ����в���ֵ 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // �ӻ����м��ش洢���̲��� 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // ���洢���̷������ֵ 
                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ�д������SqlCommand. 
        ///  
        ///  
        /// ʾ��.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders"); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// ��������(�洢����,�����ı�������.) 
        /// �洢�������ƻ�T-SQL��� 
        /// ����Ӱ�������/returns> 
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// ִ�д������SqlCommand(ָ������). 
        ///  
        ///  
        /// ʾ��:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// ��������(�洢����,�����ı�������.) 
        /// �洢�������ƻ�T-SQL��� 
        /// SqlParamter�������� 
        /// ����Ӱ������� 
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Ԥ���� 
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // ִ�� 
            int retval = cmd.ExecuteNonQuery();

            // ���������,�Ա��ٴ�ʹ��. 
            cmd.Parameters.Clear();
            return retval;
        }

        ///  
        /// ִ�д������SqlCommand(ָ������ֵ). 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ 
        /// ʾ��:  
        ///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢������ 
        /// ������洢������������Ķ������� 
        /// ������Ӱ������� 
        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ����в���ֵ 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // ���洢���̲�����ֵ 
                AssignParameterValues(commandParameters, parameterValues);

                // �������ط��� 
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // û�в���ֵ 
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteNonQuery��������

        #region ExecuteDataset����

        ///  
        /// ִ��ָ�����ݿ������ַ���������,����DataSet. 
        ///  
        ///  
        /// ʾ��:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders"); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// ����һ�������������DataSet 
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// ִ��ָ�����ݿ������ַ���������,����DataSet. 
        ///  
        ///  
        /// ʾ��: 
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// SqlParamters�������� 
        /// ����һ�������������DataSet 
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

            // �����������ݿ����Ӷ���,��������ͷŶ���. 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // ����ָ�����ݿ������ַ������ط���. 
                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
        }

        ///  
        /// ִ��ָ�����ݿ������ַ���������,ֱ���ṩ����ֵ,����DataSet. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ. 
        /// ʾ��: 
        ///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �洢������ 
        /// ������洢������������Ķ������� 
        /// ����һ�������������DataSet 
        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // �ӻ����м����洢���̲��� 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // ���洢���̲�������ֵ 
                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ��������,����DataSet. 
        ///  
        ///  
        /// ʾ��:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders"); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢��������T-SQL��� 
        /// ����һ�������������DataSet 
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ��������,ָ���洢���̲���,����DataSet. 
        ///  
        ///  
        /// ʾ��:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢��������T-SQL��� 
        /// SqlParamter�������� 
        /// ����һ�������������DataSet 
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Ԥ���� 
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // ����SqlDataAdapter��DataSet. 
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();

                // ���DataSet. 
                da.Fill(ds);

                cmd.Parameters.Clear();

                if (mustCloseConnection)
                    connection.Close();

                return ds;
            }
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ��������,ָ������ֵ,����DataSet. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ. 
        /// ʾ��.:  
        ///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢������ 
        /// ������洢������������Ķ������� 
        /// ����һ�������������DataSet 
        public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // �Ȼ����м��ش洢���̲��� 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // ���洢���̲�������ֵ 
                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ�����������,����DataSet. 
        ///  
        ///  
        /// ʾ��:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders"); 
        ///  
        /// ���� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢��������T-SQL��� 
        /// ����һ�������������DataSet 
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// ִ��ָ�����������,ָ������,����DataSet. 
        ///  
        ///  
        /// ʾ��:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// ���� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢��������T-SQL��� 
        /// SqlParamter�������� 
        /// ����һ�������������DataSet 
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Ԥ���� 
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // ���� DataAdapter & DataSet 
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }

        ///  
        /// ִ��ָ�����������,ָ������ֵ,����DataSet. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ. 
        /// ʾ��.:  
        ///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36); 
        ///  
        /// ���� 
        /// �洢������ 
        /// ������洢������������Ķ������� 
        /// ����һ�������������DataSet 
        public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // �ӻ����м��ش洢���̲��� 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // ���洢���̲�������ֵ 
                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteDataset���ݼ��������

        #region ExecuteReader �����Ķ���

        ///  
        /// ö��,��ʶ���ݿ���������SqlHelper�ṩ�����ɵ������ṩ 
        ///  
        private enum SqlConnectionOwnership
        {
            /// ��SqlHelper�ṩ���� 
            Internal,
            /// �ɵ������ṩ���� 
            External
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ���������Ķ���. 
        ///  
        ///  
        /// �����SqlHelper������,�����ӹر�DataReaderҲ���ر�. 
        /// ����ǵ��ö�������,DataReader�ɵ��ö�����. 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// һ����Ч������,����Ϊ 'null' 
        /// �������� (�洢����,�����ı�������) 
        /// �洢��������T-SQL��� 
        /// SqlParameters��������,���û�в�����Ϊ'null' 
        /// ��ʶ���ݿ����Ӷ������ɵ������ṩ������SqlHelper�ṩ 
        /// ���ذ����������SqlDataReader 
        private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            bool mustCloseConnection = false;
            // �������� 
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

                // ���������Ķ��� 
                SqlDataReader dataReader;

                if (connectionOwnership == SqlConnectionOwnership.External)
                {
                    dataReader = cmd.ExecuteReader();
                }
                else
                {
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }

                // �������,�Ա��ٴ�ʹ��.. 
                // HACK: There is a problem here, the output parameter values are fletched 
                // when the reader is closed, so if the parameters are detached from the command 
                // then the SqlReader can�t set its values. 
                // When this happen, the parameters can�t be used again in other command. 
                bool canClear = true;
                foreach (SqlParameter commandParameter in cmd.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                        canClear = false;
                }

                if (canClear)
                {
                    cmd.Parameters.Clear();
                }

                return dataReader;
            }
            catch
            {
                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }

        ///  
        /// ִ��ָ�����ݿ������ַ����������Ķ���. 
        ///  
        ///  
        /// ʾ��:  
        ///  SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders"); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢��������T-SQL��� 
        /// ���ذ����������SqlDataReader 
        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteReader(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// ִ��ָ�����ݿ������ַ����������Ķ���,ָ������. 
        ///  
        ///  
        /// ʾ��:  
        ///  SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢��������T-SQL��� 
        /// SqlParamter��������(new SqlParameter("@prodid", 24)) 
        /// ���ذ����������SqlDataReader 
        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                return ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
            }
            catch
            {
                // If we fail to return the SqlDatReader, we need to close the connection ourselves 
                if (connection != null) connection.Close();
                throw;
            }

        }

        ///  
        /// ִ��ָ�����ݿ������ַ����������Ķ���,ָ������ֵ. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ����. 
        /// ʾ��:  
        ///  SqlDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �洢������ 
        /// ������洢������������Ķ������� 
        /// ���ذ����������SqlDataReader 
        public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ���������Ķ���. 
        ///  
        ///  
        /// ʾ��:  
        ///  SqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders"); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢��������T-SQL��� 
        /// ���ذ����������SqlDataReader 
        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// [�����߷�ʽ]ִ��ָ�����ݿ����Ӷ���������Ķ���,ָ������. 
        ///  
        ///  
        /// ʾ��:  
        ///  SqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �������� (�洢����,�����ı�������) 
        /// SqlParamter�������� 
        /// ���ذ����������SqlDataReader 
        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        ///  
        /// [�����߷�ʽ]ִ��ָ�����ݿ����Ӷ���������Ķ���,ָ������ֵ. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ����. 
        /// ʾ��:  
        ///  SqlDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// T�洢������ 
        /// ������洢������������Ķ������� 
        /// ���ذ����������SqlDataReader 
        public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// [�����߷�ʽ]ִ��ָ�����ݿ�����������Ķ���,ָ������ֵ. 
        ///  
        ///  
        /// ʾ��:  
        ///  SqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders"); 
        ///  
        /// һ����Ч���������� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// ���ذ����������SqlDataReader 
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteReader(transaction, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// [�����߷�ʽ]ִ��ָ�����ݿ�����������Ķ���,ָ������. 
        ///  
        ///  
        /// ʾ��:  
        ///   SqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч���������� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// ����������SqlParamter�������� 
        /// ���ذ����������SqlDataReader 
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        ///  
        /// [�����߷�ʽ]ִ��ָ�����ݿ�����������Ķ���,ָ������ֵ. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ����. 
        /// 
        /// ʾ��:  
        ///  SqlDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36); 
        ///  
        /// һ����Ч���������� 
        /// �洢�������� 
        /// ������洢������������Ķ������� 
        /// ���ذ����������SqlDataReader 
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ����в���ֵ 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // û�в���ֵ 
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteReader�����Ķ���

        #region ExecuteScalar ���ؽ�����еĵ�һ�е�һ��

        ///  
        /// ִ��ָ�����ݿ������ַ���������,���ؽ�����еĵ�һ�е�һ��. 
        ///  
        ///  
        /// ʾ��:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount"); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// ���ؽ�����еĵ�һ�е�һ�� 
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            // ִ�в���Ϊ�յķ��� 
            return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// ִ��ָ�����ݿ������ַ���������,ָ������,���ؽ�����еĵ�һ�е�һ��. 
        ///  
        ///  
        /// ʾ��:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// ����������SqlParamter�������� 
        /// ���ؽ�����еĵ�һ�е�һ�� 
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            // �����������ݿ����Ӷ���,��������ͷŶ���. 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // ����ָ�����ݿ������ַ������ط���. 
                return ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
        }

        ///  
        /// ִ��ָ�����ݿ������ַ���������,ָ������ֵ,���ؽ�����еĵ�һ�е�һ��. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ����. 
        /// 
        /// ʾ��:  
        ///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �洢�������� 
        /// ������洢������������Ķ������� 
        /// ���ؽ�����еĵ�һ�е�һ�� 
        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ����в���ֵ 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // ���洢���̲�����ֵ 
                AssignParameterValues(commandParameters, parameterValues);

                // �������ط��� 
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // û�в���ֵ 
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ��������,���ؽ�����еĵ�һ�е�һ��. 
        ///  
        ///  
        /// ʾ��:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount"); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// ���ؽ�����еĵ�һ�е�һ�� 
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            // ִ�в���Ϊ�յķ��� 
            return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ��������,ָ������,���ؽ�����еĵ�һ�е�һ��. 
        ///  
        ///  
        /// ʾ��:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// ����������SqlParamter�������� 
        /// ���ؽ�����еĵ�һ�е�һ�� 
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // ����SqlCommand����,������Ԥ���� 
            SqlCommand cmd = new SqlCommand();

            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // ִ��SqlCommand����,�����ؽ��. 
            object retval = cmd.ExecuteScalar();

            // �������,�Ա��ٴ�ʹ��. 
            cmd.Parameters.Clear();

            if (mustCloseConnection)
                connection.Close();

            return retval;
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ��������,ָ������ֵ,���ؽ�����еĵ�һ�е�һ��. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ����. 
        /// 
        /// ʾ��:  
        ///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢�������� 
        /// ������洢������������Ķ������� 
        /// ���ؽ�����еĵ�һ�е�һ�� 
        public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ����в���ֵ 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // ���洢���̲�����ֵ 
                AssignParameterValues(commandParameters, parameterValues);

                // �������ط��� 
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // û�в���ֵ 
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ�����ݿ����������,���ؽ�����еĵ�һ�е�һ��. 
        ///  
        ///  
        /// ʾ��:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount"); 
        ///  
        /// һ����Ч���������� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// ���ؽ�����еĵ�һ�е�һ�� 
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // ִ�в���Ϊ�յķ��� 
            return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// ִ��ָ�����ݿ����������,ָ������,���ؽ�����еĵ�һ�е�һ��. 
        ///  
        ///  
        /// ʾ��:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч���������� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// ����������SqlParamter�������� 
        /// ���ؽ�����еĵ�һ�е�һ�� 
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // ����SqlCommand����,������Ԥ���� 
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // ִ��SqlCommand����,�����ؽ��. 
            object retval = cmd.ExecuteScalar();

            // �������,�Ա��ٴ�ʹ��. 
            cmd.Parameters.Clear();
            return retval;
        }

        ///  
        /// ִ��ָ�����ݿ����������,ָ������ֵ,���ؽ�����еĵ�һ�е�һ��. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ����. 
        /// 
        /// ʾ��:  
        ///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36); 
        ///  
        /// һ����Ч���������� 
        /// �洢�������� 
        /// ������洢������������Ķ������� 
        /// ���ؽ�����еĵ�һ�е�һ�� 
        public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ����в���ֵ 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // PPull the parameters for this stored procedure from the parameter cache () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // ���洢���̲�����ֵ 
                AssignParameterValues(commandParameters, parameterValues);

                // �������ط��� 
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // û�в���ֵ 
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteScalar

        #region ExecuteXmlReader XML�Ķ��� 
        ///  
        /// ִ��ָ�����ݿ����Ӷ����SqlCommand����,������һ��XmlReader������Ϊ���������. 
        ///  
        ///  
        /// ʾ��:  
        ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders"); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� using "FOR XML AUTO" 
        /// ����XmlReader���������. 
        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            // ִ�в���Ϊ�յķ��� 
            return ExecuteXmlReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ����SqlCommand����,������һ��XmlReader������Ϊ���������,ָ������. 
        ///  
        ///  
        /// ʾ��:  
        ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� using "FOR XML AUTO" 
        /// ����������SqlParamter�������� 
        /// ����XmlReader���������. 
        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            bool mustCloseConnection = false;
            // ����SqlCommand����,������Ԥ���� 
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

                // ִ������ 
                XmlReader retval = cmd.ExecuteXmlReader();

                // �������,�Ա��ٴ�ʹ��. 
                cmd.Parameters.Clear();

                return retval;
            }
            catch
            {
                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ����SqlCommand����,������һ��XmlReader������Ϊ���������,ָ������ֵ. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ����. 
        /// 
        /// ʾ��:  
        ///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢�������� using "FOR XML AUTO" 
        /// ������洢������������Ķ������� 
        /// ����XmlReader���������. 
        public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ����в���ֵ 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // ���洢���̲�����ֵ 
                AssignParameterValues(commandParameters, parameterValues);

                // �������ط��� 
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // û�в���ֵ 
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ�����ݿ������SqlCommand����,������һ��XmlReader������Ϊ���������. 
        ///  
        ///  
        /// ʾ��:  
        ///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders"); 
        ///  
        /// һ����Ч���������� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� using "FOR XML AUTO" 
        /// ����XmlReader���������. 
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // ִ�в���Ϊ�յķ��� 
            return ExecuteXmlReader(transaction, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// ִ��ָ�����ݿ������SqlCommand����,������һ��XmlReader������Ϊ���������,ָ������. 
        ///  
        ///  
        /// ʾ��:  
        ///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч���������� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� using "FOR XML AUTO" 
        /// ����������SqlParamter�������� 
        /// ����XmlReader���������. 
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // ����SqlCommand����,������Ԥ���� 
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // ִ������ 
            XmlReader retval = cmd.ExecuteXmlReader();

            // �������,�Ա��ٴ�ʹ��. 
            cmd.Parameters.Clear();
            return retval;
        }

        ///  
        /// ִ��ָ�����ݿ������SqlCommand����,������һ��XmlReader������Ϊ���������,ָ������ֵ. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ����. 
        /// 
        /// ʾ��:  
        ///  XmlReader r = ExecuteXmlReader(trans, "GetOrders", 24, 36); 
        ///  
        /// һ����Ч���������� 
        /// �洢�������� 
        /// ������洢������������Ķ������� 
        /// ����һ�������������DataSet. 
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ����в���ֵ 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // ���洢���̲�����ֵ 
                AssignParameterValues(commandParameters, parameterValues);

                // �������ط��� 
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // û�в���ֵ 
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteXmlReader �Ķ�������

        #region FillDataset ������ݼ� 
        ///  
        /// ִ��ָ�����ݿ������ַ���������,ӳ�����ݱ�������ݼ�. 
        ///  
        ///  
        /// ʾ��:  
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// Ҫ���������DataSetʵ�� 
        /// ��ӳ������ݱ����� 
        /// �û�����ı��� (������ʵ�ʵı���.) 
        public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (dataSet == null) throw new ArgumentNullException("dataSet");

            // �����������ݿ����Ӷ���,��������ͷŶ���. 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // ����ָ�����ݿ������ַ������ط���. 
                FillDataset(connection, commandType, commandText, dataSet, tableNames);
            }
        }

        ///  
        /// ִ��ָ�����ݿ������ַ���������,ӳ�����ݱ�������ݼ�.ָ���������. 
        ///  
        ///  
        /// ʾ��:  
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// ����������SqlParamter�������� 
        /// Ҫ���������DataSetʵ�� 
        /// ��ӳ������ݱ����� 
        /// �û�����ı��� (������ʵ�ʵı���.) 
        ///  
        public static void FillDataset(string connectionString, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (dataSet == null) throw new ArgumentNullException("dataSet");
            // �����������ݿ����Ӷ���,��������ͷŶ���. 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // ����ָ�����ݿ������ַ������ط���. 
                FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
            }
        }

        ///  
        /// ִ��ָ�����ݿ������ַ���������,ӳ�����ݱ�������ݼ�,ָ���洢���̲���ֵ. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ����. 
        /// 
        /// ʾ��:  
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, 24); 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �洢�������� 
        /// Ҫ���������DataSetʵ�� 
        /// ��ӳ������ݱ����� 
        /// �û�����ı��� (������ʵ�ʵı���.) 
        ///     
        /// ������洢������������Ķ������� 
        public static void FillDataset(string connectionString, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (dataSet == null) throw new ArgumentNullException("dataSet");
            // �����������ݿ����Ӷ���,��������ͷŶ���. 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // ����ָ�����ݿ������ַ������ط���. 
                FillDataset(connection, spName, dataSet, tableNames, parameterValues);
            }
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ��������,ӳ�����ݱ�������ݼ�. 
        ///  
        ///  
        /// ʾ��:  
        ///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// Ҫ���������DataSetʵ�� 
        /// ��ӳ������ݱ����� 
        /// �û�����ı��� (������ʵ�ʵı���.) 
        ///     
        public static void FillDataset(SqlConnection connection, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames)
        {
            FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ��������,ӳ�����ݱ�������ݼ�,ָ������. 
        ///  
        ///  
        /// ʾ��:  
        ///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// Ҫ���������DataSetʵ�� 
        /// ��ӳ������ݱ����� 
        /// �û�����ı��� (������ʵ�ʵı���.) 
        ///  
        /// ����������SqlParamter�������� 
        public static void FillDataset(SqlConnection connection, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params SqlParameter[] commandParameters)
        {
            FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        ///  
        /// ִ��ָ�����ݿ����Ӷ��������,ӳ�����ݱ�������ݼ�,ָ���洢���̲���ֵ. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ����. 
        /// 
        /// ʾ��:  
        ///  FillDataset(conn, "GetOrders", ds, new string[] {"orders"}, 24, 36); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢�������� 
        /// Ҫ���������DataSetʵ�� 
        /// ��ӳ������ݱ����� 
        /// �û�����ı��� (������ʵ�ʵı���.) 
        ///  
        /// ������洢������������Ķ������� 
        public static void FillDataset(SqlConnection connection, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (dataSet == null) throw new ArgumentNullException("dataSet");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ����в���ֵ 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // ���洢���̲�����ֵ 
                AssignParameterValues(commandParameters, parameterValues);

                // �������ط��� 
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
            {
                // û�в���ֵ 
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        ///  
        /// ִ��ָ�����ݿ����������,ӳ�����ݱ�������ݼ�. 
        ///  
        ///  
        /// ʾ��:  
        ///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}); 
        ///  
        /// һ����Ч���������� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// Ҫ���������DataSetʵ�� 
        /// ��ӳ������ݱ����� 
        /// �û�����ı��� (������ʵ�ʵı���.) 
        ///  
        public static void FillDataset(SqlTransaction transaction, CommandType commandType,
            string commandText,
            DataSet dataSet, string[] tableNames)
        {
            FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
        }

        ///  
        /// ִ��ָ�����ݿ����������,ӳ�����ݱ�������ݼ�,ָ������. 
        ///  
        ///  
        /// ʾ��:  
        ///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч���������� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// Ҫ���������DataSetʵ�� 
        /// ��ӳ������ݱ����� 
        /// �û�����ı��� (������ʵ�ʵı���.) 
        ///  
        /// ����������SqlParamter�������� 
        public static void FillDataset(SqlTransaction transaction, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params SqlParameter[] commandParameters)
        {
            FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        ///  
        /// ִ��ָ�����ݿ����������,ӳ�����ݱ�������ݼ�,ָ���洢���̲���ֵ. 
        ///  
        ///  
        /// �˷������ṩ���ʴ洢������������ͷ���ֵ����. 
        /// 
        /// ʾ��:  
        ///  FillDataset(trans, "GetOrders", ds, new string[]{"orders"}, 24, 36); 
        ///  
        /// һ����Ч���������� 
        /// �洢�������� 
        /// Ҫ���������DataSetʵ�� 
        /// ��ӳ������ݱ����� 
        /// �û�����ı��� (������ʵ�ʵı���.) 
        ///  
        /// ������洢������������Ķ������� 
        public static void FillDataset(SqlTransaction transaction, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (dataSet == null) throw new ArgumentNullException("dataSet");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ����в���ֵ 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // ���洢���̲�����ֵ 
                AssignParameterValues(commandParameters, parameterValues);

                // �������ط��� 
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
            {
                // û�в���ֵ 
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        ///  
        /// [˽�з���][�ڲ�����]ִ��ָ�����ݿ����Ӷ���/���������,ӳ�����ݱ�������ݼ�,DataSet/TableNames/SqlParameters. 
        ///  
        ///  
        /// ʾ��:  
        ///  FillDataset(conn, trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new SqlParameter("@prodid", 24)); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// һ����Ч���������� 
        /// �������� (�洢����,�����ı�������) 
        /// �洢�������ƻ�T-SQL��� 
        /// Ҫ���������DataSetʵ�� 
        /// ��ӳ������ݱ����� 
        /// �û�����ı��� (������ʵ�ʵı���.) 
        ///  
        /// ����������SqlParamter�������� 
        private static void FillDataset(SqlConnection connection, SqlTransaction transaction, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (dataSet == null) throw new ArgumentNullException("dataSet");

            // ����SqlCommand����,������Ԥ���� 
            SqlCommand command = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // ִ������ 
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
            {

                // ׷�ӱ�ӳ�� 
                if (tableNames != null && tableNames.Length > 0)
                {
                    string tableName = "Table";
                    for (int index = 0; index < tableNames.Length; index++)
                    {
                        if (tableNames[index] == null || tableNames[index].Length == 0) throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
                        dataAdapter.TableMappings.Add(tableName, tableNames[index]);
                        tableName += (index + 1).ToString();
                    }
                }

                // ������ݼ�ʹ��Ĭ�ϱ����� 
                dataAdapter.Fill(dataSet);

                // �������,�Ա��ٴ�ʹ��. 
                command.Parameters.Clear();
            }

            if (mustCloseConnection)
                connection.Close();
        }
        #endregion

        #region UpdateDataset �������ݼ� 
        ///  
        /// ִ�����ݼ����µ����ݿ�,ָ��inserted, updated, or deleted����. 
        ///  
        ///  
        /// ʾ��:  
        ///  UpdateDataset(conn, insertCommand, deleteCommand, updateCommand, dataSet, "Order"); 
        ///  
        /// [׷�Ӽ�¼]һ����Ч��T-SQL����洢���� 
        /// [ɾ����¼]һ����Ч��T-SQL����洢���� 
        /// [���¼�¼]һ����Ч��T-SQL����洢���� 
        /// Ҫ���µ����ݿ��DataSet 
        /// Ҫ���µ����ݿ��DataTable 
        public static void UpdateDataset(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand, DataSet dataSet, string tableName)
        {
            if (insertCommand == null) throw new ArgumentNullException("insertCommand");
            if (deleteCommand == null) throw new ArgumentNullException("deleteCommand");
            if (updateCommand == null) throw new ArgumentNullException("updateCommand");
            if (tableName == null || tableName.Length == 0) throw new ArgumentNullException("tableName");

            // ����SqlDataAdapter,��������ɺ��ͷ�. 
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
            {
                // ������������������ 
                dataAdapter.UpdateCommand = updateCommand;
                dataAdapter.InsertCommand = insertCommand;
                dataAdapter.DeleteCommand = deleteCommand;

                // �������ݼ��ı䵽���ݿ� 
                dataAdapter.Update(dataSet, tableName);

                // �ύ���иı䵽���ݼ�. 
                dataSet.AcceptChanges();
            }
        }
        #endregion

        #region CreateCommand ����һ��SqlCommand���� 
        ///  
        /// ����SqlCommand����,ָ�����ݿ����Ӷ���,�洢�������Ͳ���. 
        ///  
        ///  
        /// ʾ��:  
        ///  SqlCommand command = CreateCommand(conn, "AddCustomer", "CustomerID", "CustomerName"); 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢�������� 
        /// Դ������������� 
        /// ����SqlCommand���� 
        public static SqlCommand CreateCommand(SqlConnection connection, string spName, params string[] sourceColumns)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // �������� 
            SqlCommand cmd = new SqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // ����в���ֵ 
            if ((sourceColumns != null) && (sourceColumns.Length > 0))
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // ��Դ����е�ӳ�䵽DataSet������. 
                for (int index = 0; index < sourceColumns.Length; index++)
                    commandParameters[index].SourceColumn = sourceColumns[index];

                // Attach the discovered parameters to the SqlCommand object 
                AttachParameters(cmd, commandParameters);
            }

            return cmd;
        }
        #endregion

        #region ExecuteNonQueryTypedParams ���ͻ�����(DataRow) 
        ///  
        /// ִ��ָ���������ݿ������ַ����Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,������Ӱ�������. 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ����Ӱ������� 
        public static int ExecuteNonQueryTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ���row��ֵ,�洢���̱����ʼ��. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ���������ݿ����Ӷ���Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,������Ӱ�������. 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ����Ӱ������� 
        public static int ExecuteNonQueryTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ���row��ֵ,�洢���̱����ʼ��. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ���������ݿ�����Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,������Ӱ�������. 
        ///  
        /// һ����Ч���������� object 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ����Ӱ������� 
        public static int ExecuteNonQueryTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // Sf the row has values, the store procedure parameters must be initialized 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteDatasetTypedParams ���ͻ�����(DataRow) 
        ///  
        /// ִ��ָ���������ݿ������ַ����Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,����DataSet. 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ����һ�������������DataSet. 
        public static DataSet ExecuteDatasetTypedParams(string connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            //���row��ֵ,�洢���̱����ʼ��. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ���������ݿ����Ӷ���Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,����DataSet. 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ����һ�������������DataSet. 
        /// 
        public static DataSet ExecuteDatasetTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ���row��ֵ,�洢���̱����ʼ��. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ���������ݿ�����Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,����DataSet. 
        ///  
        /// һ����Ч���������� object 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ����һ�������������DataSet. 
        public static DataSet ExecuteDatasetTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ���row��ֵ,�洢���̱����ʼ��. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion

        #region ExecuteReaderTypedParams ���ͻ�����(DataRow) 
        ///  
        /// ִ��ָ���������ݿ������ַ����Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,����DataReader. 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ���ذ����������SqlDataReader 
        public static SqlDataReader ExecuteReaderTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ���row��ֵ,�洢���̱����ʼ��. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }


        ///  
        /// ִ��ָ���������ݿ����Ӷ���Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,����DataReader. 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ���ذ����������SqlDataReader 
        public static SqlDataReader ExecuteReaderTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ���row��ֵ,�洢���̱����ʼ��. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ���������ݿ�����Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,����DataReader. 
        ///  
        /// һ����Ч���������� object 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ���ذ����������SqlDataReader 
        public static SqlDataReader ExecuteReaderTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ���row��ֵ,�洢���̱����ʼ��. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteScalarTypedParams ���ͻ�����(DataRow) 
        ///  
        /// ִ��ָ���������ݿ������ַ����Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,���ؽ�����еĵ�һ�е�һ��. 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ���ؽ�����еĵ�һ�е�һ�� 
        public static object ExecuteScalarTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ���row��ֵ,�洢���̱����ʼ��. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ���������ݿ����Ӷ���Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,���ؽ�����еĵ�һ�е�һ��. 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ���ؽ�����еĵ�һ�е�һ�� 
        public static object ExecuteScalarTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ���row��ֵ,�洢���̱����ʼ��. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ���������ݿ�����Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,���ؽ�����еĵ�һ�е�һ��. 
        ///  
        /// һ����Ч���������� object 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ���ؽ�����еĵ�һ�е�һ�� 
        public static object ExecuteScalarTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ���row��ֵ,�洢���̱����ʼ��. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteXmlReaderTypedParams ���ͻ�����(DataRow) 
        ///  
        /// ִ��ָ���������ݿ����Ӷ���Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,����XmlReader���͵Ľ����. 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ����XmlReader���������. 
        public static XmlReader ExecuteXmlReaderTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ���row��ֵ,�洢���̱����ʼ��. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// ִ��ָ���������ݿ�����Ĵ洢����,ʹ��DataRow��Ϊ����ֵ,����XmlReader���͵Ľ����. 
        ///  
        /// һ����Ч���������� object 
        /// �洢�������� 
        /// ʹ��DataRow��Ϊ����ֵ 
        /// ����XmlReader���������. 
        public static XmlReader ExecuteXmlReaderTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // ���row��ֵ,�洢���̱����ʼ��. 
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // �ӻ����м��ش洢���̲���,��������в�����������ݿ��м���������Ϣ�����ص�������. () 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // �������ֵ 
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

    }

    ///  
    /// SqlHelperParameterCache�ṩ����洢���̲���,���ܹ�������ʱ�Ӵ洢������̽������. 
    ///  
    public sealed class SqlHelperParameterCache
    {
        #region ˽�з���,�ֶ�,���캯�� 
        // ˽�й��캯��,��ֹ�౻ʵ����. 
        private SqlHelperParameterCache() { }

        // �������Ҫע�� 
        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        ///  
        /// ̽������ʱ�Ĵ洢����,����SqlParameter��������. 
        /// ��ʼ������ֵΪ DBNull.Value. 
        ///  
        /// һ����Ч�����ݿ����� 
        /// �洢�������� 
        /// �Ƿ��������ֵ���� 
        /// ����SqlParameter�������� 
        private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            SqlCommand cmd = new SqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            // ����cmdָ���Ĵ洢���̵Ĳ�����Ϣ,����䵽cmd��Parameters��������. 
            SqlCommandBuilder.DeriveParameters(cmd);
            connection.Close();
            // �������������ֵ����,���������е�ÿһ������ɾ��. 
            if (!includeReturnValueParameter)
            {
                cmd.Parameters.RemoveAt(0);
            }

            // ������������ 
            SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];
            // ��cmd��Parameters���������Ƶ�discoveredParameters����. 
            cmd.Parameters.CopyTo(discoveredParameters, 0);

            // ��ʼ������ֵΪ DBNull.Value. 
            foreach (SqlParameter discoveredParameter in discoveredParameters)
            {
                discoveredParameter.Value = DBNull.Value;
            }
            return discoveredParameters;
        }

        ///  
        /// SqlParameter�����������㿽��. 
        ///  
        /// ԭʼ�������� 
        /// ����һ��ͬ���Ĳ������� 
        private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion ˽�з���,�ֶ�,���캯������

        #region ���淽��

        ///  
        /// ׷�Ӳ������鵽����. 
        ///  
        /// һ����Ч�����ݿ������ַ��� 
        /// �洢��������SQL��� 
        /// Ҫ����Ĳ������� 
        public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        ///  
        /// �ӻ����л�ȡ��������. 
        ///  
        /// һ����Ч�����ݿ������ַ� 
        /// �洢��������SQL��� 
        /// �������� 
        public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            SqlParameter[] cachedParameters = paramCache[hashKey] as SqlParameter[];
            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        #endregion ���淽������

        #region ����ָ���Ĵ洢���̵Ĳ�����

        ///  
        /// ����ָ���Ĵ洢���̵Ĳ����� 
        ///  
        ///  
        /// �����������ѯ���ݿ�,������Ϣ�洢������. 
        ///  
        /// һ����Ч�����ݿ������ַ� 
        /// �洢������ 
        /// ����SqlParameter�������� 
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        ///  
        /// ����ָ���Ĵ洢���̵Ĳ����� 
        ///  
        ///  
        /// �����������ѯ���ݿ�,������Ϣ�洢������. 
        ///  
        /// һ����Ч�����ݿ������ַ�. 
        /// �洢������ 
        /// �Ƿ��������ֵ���� 
        /// ����SqlParameter�������� 
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
        }

        ///  
        /// [�ڲ�]����ָ���Ĵ洢���̵Ĳ�����(ʹ�����Ӷ���). 
        ///  
        ///  
        /// �����������ѯ���ݿ�,������Ϣ�洢������. 
        ///  
        /// һ����Ч�����ݿ������ַ� 
        /// �洢������ 
        /// ����SqlParameter�������� 
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
        {
            return GetSpParameterSet(connection, spName, false);
        }

        ///  
        /// [�ڲ�]����ָ���Ĵ洢���̵Ĳ�����(ʹ�����Ӷ���) 
        ///  
        ///  
        /// �����������ѯ���ݿ�,������Ϣ�洢������. 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢������ 
        ///  
        /// �Ƿ��������ֵ���� 
        ///  
        /// ����SqlParameter�������� 
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            using (SqlConnection clonedConnection = (SqlConnection)((ICloneable)connection).Clone())
            {
                return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
            }
        }

        ///  
        /// [˽��]����ָ���Ĵ洢���̵Ĳ�����(ʹ�����Ӷ���) 
        ///  
        /// һ����Ч�����ݿ����Ӷ��� 
        /// �洢������ 
        /// �Ƿ��������ֵ���� 
        /// ����SqlParameter�������� 
        private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            SqlParameter[] cachedParameters;

            cachedParameters = paramCache[hashKey] as SqlParameter[];
            if (cachedParameters == null)
            {
                SqlParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                paramCache[hashKey] = spParameters;
                cachedParameters = spParameters;
            }

            return CloneParameters(cachedParameters);
        }

        #endregion ��������������

    }
}
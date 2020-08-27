// Decompiled with JetBrains decompiler
// Type: Microsoft.Practices.EnterpriseLibrary.Data.TransactionScopeConnections
// Assembly: Microsoft.Practices.EnterpriseLibrary.Data, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A6DC4A7-AC53-43D6-B92A-C1956B8EB36E
// Assembly location: D:\source-code\Binaries-sb2\Microsoft.Practices.EnterpriseLibrary.Data.dll

using System.Collections.Generic;
using System.Transactions;

namespace Microsoft.Practices.EnterpriseLibrary.Data
{
  public static class TransactionScopeConnections
  {
    private static readonly Dictionary<Transaction, Dictionary<string, DatabaseConnectionWrapper>> transactionConnections = new Dictionary<Transaction, Dictionary<string, DatabaseConnectionWrapper>>();

    public static DatabaseConnectionWrapper GetConnection(Database db)
    {
      Transaction current = Transaction.Current;
      if (current == (Transaction) null)
        return (DatabaseConnectionWrapper) null;
      Dictionary<string, DatabaseConnectionWrapper> dictionary;
      lock (TransactionScopeConnections.transactionConnections)
      {
        if (!TransactionScopeConnections.transactionConnections.TryGetValue(current, out dictionary))
        {
          dictionary = new Dictionary<string, DatabaseConnectionWrapper>();
          TransactionScopeConnections.transactionConnections.Add(current, dictionary);
          current.TransactionCompleted += new TransactionCompletedEventHandler(TransactionScopeConnections.OnTransactionCompleted);
        }
      }
      DatabaseConnectionWrapper connectionWrapper;
      lock (dictionary)
      {
        if (!dictionary.TryGetValue(db.ConnectionString, out connectionWrapper))
        {
          connectionWrapper = new DatabaseConnectionWrapper(db.GetNewOpenConnection());
          dictionary.Add(db.ConnectionString, connectionWrapper);
        }
        connectionWrapper.AddRef();
      }
      return connectionWrapper;
    }

    private static void OnTransactionCompleted(object sender, TransactionEventArgs e)
    {
      Dictionary<string, DatabaseConnectionWrapper> dictionary;
      lock (TransactionScopeConnections.transactionConnections)
      {
        if (!TransactionScopeConnections.transactionConnections.TryGetValue(e.Transaction, out dictionary))
          return;
        TransactionScopeConnections.transactionConnections.Remove(e.Transaction);
      }
      lock (dictionary)
      {
        foreach (DatabaseConnectionWrapper connectionWrapper in dictionary.Values)
          connectionWrapper.Dispose();
      }
    }
  }
}

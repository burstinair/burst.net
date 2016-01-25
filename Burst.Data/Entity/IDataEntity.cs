using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Burst.Json;
using Burst.Data.CommandBuilder;

namespace Burst.Data.Entity
{
    public interface IDataEntity : ICloneable, IJsonObject, IFieldReadableAndWritable, IFieldViewableAndReadable, IFieldAccessable
    {
        //void InitializeOwner(Transaction trans, params string[] fields);
        bool Top { get; }
        Command GetDeleteCommand();
        int Delete(Transaction trans);
        Command GetInsertCommand(InsertType iops);
        int Insert(InsertType iops, Transaction trans);
        Command GetUpdateCommand(params string[] fields);
        int Update(Transaction trans, params string[] fields);
        Command GetReplaceCommand();
        int Replace(Transaction trans);
        /// <summary>
        /// 若存在IndexField，则根据IndexField上移一位。
        /// </summary>
        void MoveUp(Transaction trans);
        /// <summary>
        /// 若存在IndexField，则根据IndexField下移一位。
        /// </summary>
        void MoveDown(Transaction trans);
        /// <summary>
        /// 若存在IndexField，则与指定目标交换位置。
        /// </summary>
        void SetTop(bool IsTop, Transaction trans);
        object Key { get; set; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PrefabDocumenter.Unity
{
	public class SqlDbDao : IDocumentDao<DbDocumentVo>
	{
		private SqliteDatabase m_SqlDb;
		
		public SqlDbDao(string DbPath)
		{
			try
			{
				m_SqlDb = new SqliteDatabase(DbPath);

			}
			catch
			{
				throw new Exception("Dbが存在しません。");
			}
		}

		public IEnumerable<DbDocumentVo> GetAll()
		{
			var dataTable = m_SqlDb.ExecuteQuery(DocumentSQLQuery.GetAll);

			try
			{
				return dataTable.Rows.Select(data =>
					new DbDocumentVo((string)data[DocumentColumn.Guid],
						(string)data[DocumentColumn.FileName],
						(string)data[DocumentColumn.FilePath],
						(string)data[DocumentColumn.IndentLevel],
						(string)data[DocumentColumn.Description])
				);
			}
			catch
			{
				throw new Exception("形式が整合しません。");
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrefabDocumenter.Unity
{
	public class SqlDbDao
	{
		private SqliteDatabase m_SqlDb;
		
		public SqlDbDao(string DbPath)
		{
			try
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
			catch 
			{
				return;
			}
		}

		public void Insert()
		{
			
		}

		public List<DocumentVo> GetAll()
		{
			var dataTable = m_SqlDb.ExecuteQuery(DocumentSQLQuery.GET_ALL);

			try
			{
				return dataTable.Rows.Select(data =>
					new DocumentVo((string)data[DocumentColumnName.GUID],
						(string)data[DocumentColumnName.FILENAME],
						(string)data[DocumentColumnName.FILEPATH],
						(string)data[DocumentColumnName.INDENTLEVEL],
						(string)data[DocumentColumnName.DESCRIPTION])
				).ToList();
			}
			catch
			{
				throw new Exception("データベースが整合しません。");
			}

		}
	}
}
